using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Modbus
{
	/// <summary>
	/// Base abstract class for Modbus slave instances
	/// </summary>
	public abstract class ModbusSlave : ModbusBase
	{
		#region Fields

		/// <summary>
		/// Execution status of thread that manage calls
		/// </summary>
		protected volatile bool _run = false;

		/// <summary>
		/// Timeout between two chars in modbus RTU
		/// </summary>
		private long _charTimeout;

		/// <summary>
		/// Incoming calls management thread
		/// </summary>
		protected Thread _guestRequest;

		/// <summary>
		/// Interchar timeout timer
		/// </summary>
		Stopwatch _timeoutStopwatch;

		#endregion

		#region Properties

		/// <summary>
		/// Database Modbus
		/// </summary>
		public Datastore[] ModbusDatabase { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="datastore">Database Modbus</param>
		public ModbusSlave(Datastore[] datastore)
		{
			// Device status
			_deviceType = DeviceType.SLAVE;
			// Assign modbus database
			ModbusDatabase = datastore;
			// Initialize timer
			_timeoutStopwatch = new Stopwatch();
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Incoming calls management thread
		/// </summary>
		protected abstract void GuestRequests();

		/// <summary>
		/// Function prototype for start listening messages        
		/// </summary>
		public abstract void StartListening();

		/// <summary>
		/// Function prototype for stop listening messages
		/// </summary>
		public abstract void StopListening();

		#endregion

		#region Protocol Methods

		/// <summary>
		/// Check if all registers from (starting_address + quantity_of_registers) are present in device database
		/// </summary>
		/// <param name="unitID">Unit ID</param>
		/// <param name="table">Tabella del database modbus</param>
		/// <param name="startingAddress">Starting address (offset) in database</param>
		/// <param name="numRegisters">Quantity of registers to read/write</param>
		/// <returns>True if register are present, otherwhise False</returns>
		bool IsAllRegistersPresent(byte unitID, ModbusDBTables table, ushort startingAddress, ushort numRegisters)
		{
			bool result = true;

			switch (table)
			{
				case ModbusDBTables.DISCRETE_INPUTS_REGISTERS:
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unitID).DiscreteInputs.ToList().GetRange(startingAddress, numRegisters);
					}
					catch
					{
						result = false;
					}
					break;

				case ModbusDBTables.COIL_REGISTERS:
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unitID).Coils.ToList().GetRange(startingAddress, numRegisters);
					}
					catch
					{
						result = false;
					}
					break;

				case ModbusDBTables.INPUT_REGISTERS:
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unitID).InputRegisters.ToList().GetRange(startingAddress, numRegisters);
					}
					catch
					{
						result = false;
					}
					break;

				case ModbusDBTables.HOLDING_REGISTERS:
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unitID).HoldingRegisters.ToList().GetRange(startingAddress, numRegisters);
					}
					catch
					{
						result = false;
					}
					break;

				default:
					result = false;
					break;
			}

			return result;
		}

		/// <summary>
		/// Build an exception message
		/// </summary>
		/// <param name="send_buffer">Send buffer</param>
		/// <param name="code">Modbus operation code</param>
		/// <param name="error">Error code</param>
		void BuildExceptionMessage(List<byte> send_buffer, ModbusCodes code, Errors error)
		{
			byte exception_code = 0;

			send_buffer.Clear();
			switch (error)
			{
				case Errors.EXCEPTION_ILLEGAL_FUNCTION:
					exception_code = 1;
					break;

				case Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS:
					exception_code = 2;
					break;

				case Errors.EXCEPTION_ILLEGAL_DATA_VALUE:
					exception_code = 3;
					break;

				case Errors.EXCEPTION_SLAVE_DEVICE_FAILURE:
					exception_code = 4;
					break;
			}
			// Add exception identifier
			send_buffer.Add((byte)(0x80 + (byte)code));
			// Add exception code
			send_buffer.Add(exception_code);
		}

		/// <summary>
		/// Read a byte from a specified flux
		/// </summary>
		/// <param name="flux">Flux of reading bytes</param>
		/// <returns>Readed byte or -1 if no data is present</returns>
		int ReadByte(object flux)
		{
			int ret = -1;

			switch (_connectionType)
			{
				case ConnectionType.UDP_IP:
					UDPData udpd = (UDPData)flux;
					if (udpd.Length > 0)
						ret = udpd.ReadByte();
					break;

				case ConnectionType.TCP_IP:
					NetworkStream ns = (NetworkStream)flux;
					if (ns.DataAvailable)
						ret = ns.ReadByte();
					break;

				case ConnectionType.SERIAL_ASCII:
				case ConnectionType.SERIAL_RTU:
					SerialPort sp = (SerialPort)flux;
					bool done = false;

					// Await 1 char...
					if (!_timeoutStopwatch.IsRunning)
						_timeoutStopwatch.Start();

					do
					{
						if (sp.BytesToRead > 0)
						{
							ret = sp.ReadByte();
							done = true;
						}
						else
							ret = -1;
					} while ((!done) && ((_timeoutStopwatch.ElapsedMilliseconds - _charTimeout) < _intercharDelay));

					if (done)
						_charTimeout = _timeoutStopwatch.ElapsedMilliseconds;   // Char received with no errors...reset timeout counter for next char!

					break;
			}

			return ret;
		}

		/// <summary>
		/// Write a byte on a specified flux
		/// </summary>
		/// <param name="flux">Writing byte flux</param>
		/// <param name="buffer">Buffer of bytes</param>
		/// <param name="offset">Buffer starting offset</param>
		/// <param name="size">Number of bytes to write</param>
		void WriteBuffer(object flux, byte[] buffer, int offset, int size)
		{
			switch (_connectionType)
			{
				case ConnectionType.UDP_IP:
					UDPData udpd = (UDPData)flux;
					udpd.WriteResp(buffer, offset, size);
					break;

				case ConnectionType.TCP_IP:
					NetworkStream ns = (NetworkStream)flux;
					ns.Write(buffer, offset, size);
					break;

				case ConnectionType.SERIAL_ASCII:
				case ConnectionType.SERIAL_RTU:
					SerialPort sp = (SerialPort)flux;
					sp.Write(buffer, offset, size);
					break;
			}
		}

		/// <summary>
		/// Modbus slave response manager
		/// </summary>
		/// <param name="send_buffer">Send buffer</param>
		/// <param name="receive_buffer">Receive buffer</param>
		/// <param name="flux">Object for flux manager</param>
		protected void IncomingMessagePolling(List<byte> send_buffer, List<byte> receive_buffer, object flux)
		{
			ushort transaction_id = 0;
			long elapsed_time;
			byte unit_id = 0;
			Stopwatch sw = new Stopwatch();

			// Set errors at 0
			Error = Errors.NO_ERROR;
			// Empting reception buffer
			receive_buffer.Clear();
			// Start reception loop
			int data = -1;
			sw.Start();
			bool in_ric = false;
			bool done = false;
			_charTimeout = 0;
			do
			{
				data = ReadByte(flux);
				if (data != -1)
				{
					if (!in_ric)
						in_ric = true;
					if (_connectionType == ConnectionType.SERIAL_ASCII)
					{
						if ((byte)data == Encoding.ASCII.GetBytes(new char[] { ASCII_START_FRAME }).First())
							receive_buffer.Clear();
					}
					receive_buffer.Add((byte)data);
				}
				else if ((data == -1) && in_ric)
					done = true;
				else
					Thread.Sleep(1);
				// Calc elapsed time since reception start
				elapsed_time = sw.ElapsedMilliseconds;
			} while ((elapsed_time < RxTimeout) && _run && (!done));
			_timeoutStopwatch.Stop();
			sw.Stop();
			// Check for a stop request
			if (!_run)
			{
				Error = Errors.THREAD_BLOCK_REQUEST;
				return;
			}
			// Check for timeout error
			if (elapsed_time >= RxTimeout)
			{
				Error = Errors.RX_TIMEOUT;
				return;
			}
			// Check message length
			if (receive_buffer.Count < 3)
			{
				Error = Errors.WRONG_MESSAGE_LEN;
				return;
			}
			// Message received, start decoding...
			switch (_connectionType)
			{
				case ConnectionType.SERIAL_ASCII:
					// Check and delete start char
					if (Encoding.ASCII.GetChars(receive_buffer.ToArray()).FirstOrDefault() != ASCII_START_FRAME)
					{
						Error = Errors.START_CHAR_NOT_FOUND;
						return;
					}
					receive_buffer.RemoveAt(0);
					// Check and delete stop chars
					char[] end_chars = new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND };
					char[] last_two = Encoding.ASCII.GetChars(receive_buffer.GetRange(receive_buffer.Count - 2, 2).ToArray());
					if (!end_chars.SequenceEqual(last_two))
					{
						Error = Errors.END_CHARS_NOT_FOUND;
						return;
					}
					receive_buffer.RemoveRange(receive_buffer.Count - 2, 2);
					// Recode message in binary
					receive_buffer = GetBinaryBufferFromASCIIBytes(receive_buffer.ToArray()).ToList();
					// Calc and remove LRC
					byte msg_lrc = receive_buffer[receive_buffer.Count - 1];
					byte calc_lrc = LRC.CalcLRC(receive_buffer.ToArray(), 0, receive_buffer.Count - 1);
					if (msg_lrc != calc_lrc)
					{
						Error = Errors.WRONG_LRC;
						return;
					}
					receive_buffer.RemoveAt(receive_buffer.Count - 1);
					// Analize destination address, if not present in database, discard message and continue
					unit_id = receive_buffer[0];
					if (!ModbusDatabase.Any(x => x.UnitID == unit_id))
						return;
					receive_buffer.RemoveAt(0);
					break;

				case ConnectionType.SERIAL_RTU:
					// Check CRC
					ushort msg_crc = BitConverter.ToUInt16(receive_buffer.ToArray(), receive_buffer.Count - 2);
					ushort calc_crc = CRC16.CalcCRC16(receive_buffer.ToArray(), 0, receive_buffer.Count - 2);
					if (msg_crc != calc_crc)
					{
						Error = Errors.WRONG_CRC;
						return;
					}
					// Analize destination address, if not present in database, discard message and continue
					unit_id = receive_buffer[0];
					if (!ModbusDatabase.Any(x => x.UnitID == unit_id))
						return;
					// Message is ok, remove unit_id and CRC                    
					receive_buffer.RemoveRange(0, 1);
					receive_buffer.RemoveRange(receive_buffer.Count - 2, 2);
					break;

				case ConnectionType.UDP_IP:
				case ConnectionType.TCP_IP:
					// Decode MBAP Header
					transaction_id = ToUInt16(receive_buffer.ToArray(), 0);
					// Check protocol ID
					ushort protocol_id = ToUInt16(receive_buffer.ToArray(), 2);
					if (protocol_id != PROTOCOL_ID)
					{
						Error = Errors.WRONG_PROTOCOL_ID;
						return;
					}
					// Acquire data length and check it                    
					ushort len = ToUInt16(receive_buffer.ToArray(), 4);
					if ((receive_buffer.Count - 6) != len)
					{
						Error = Errors.WRONG_MESSAGE_LEN;
						return;
					}
					// Analize destination address, if not present in database, discard message and continue
					unit_id = receive_buffer[6];
					if (!ModbusDatabase.Any(x => x.UnitID == unit_id))
						return;
					// Message is ok, remove MBAP header for reception buffer                    
					receive_buffer.RemoveRange(0, MBAP_HEADER_LEN);
					break;
			}
			// Adjust data and build response
			AdjAndReply(send_buffer, receive_buffer, flux, unit_id, transaction_id);
		}

		/// <summary>
		/// Adjust received data and build response
		/// </summary>
		/// <param name="send_buffer">Send buffer</param>
		/// <param name="receive_buffer">Receive buffer</param>
		/// <param name="flux">Object for flux gest</param>
		/// <param name="unit_id">Slave id</param>
		/// <param name="transaction_id">Transaction id of TCP slave</param>
		void AdjAndReply(List<byte> send_buffer, List<byte> receive_buffer, object flux, byte unit_id, ushort transaction_id)
		{
			ushort sa, sa1, qor, qor1, bc, val, and_mask, or_mask;
			int bytes;
			bool cv;
			bool[] ret_vals;
			ModbusCodes mdbcode;

			// Empting reception buffer
			send_buffer.Clear();
			// Adjust data
			mdbcode = (ModbusCodes)receive_buffer[0];
			switch (mdbcode)
			{
				case ModbusCodes.READ_COILS:
					// Read received commands
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).Coils.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_COILS_IN_READ_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.COIL_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.READ_COILS);
					bytes = (qor / 8) + (qor % 8 == 0 ? 0 : 1);
					send_buffer.Add((byte)bytes);
					ret_vals = new bool[bytes * 8];
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unit_id).Coils.ToList().GetRange(sa, qor).CopyTo(ret_vals);
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					for (int ii = 0; ii < bytes; ii++)
						send_buffer.Add(EightBitToByte(ret_vals, ii * 8));
					break;

				case ModbusCodes.READ_DISCRETE_INPUTS:
					// Read received commands                    
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).DiscreteInputs.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_DISCRETE_INPUTS_IN_READ_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.DISCRETE_INPUTS_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.READ_DISCRETE_INPUTS);
					bytes = (qor / 8) + (qor % 8 == 0 ? 0 : 1);
					send_buffer.Add((byte)bytes);
					ret_vals = new bool[bytes * 8];
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unit_id).DiscreteInputs.ToList().GetRange(sa, qor).CopyTo(ret_vals);
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					for (int ii = 0; ii < bytes; ii++)
						send_buffer.Add(EightBitToByte(ret_vals, ii * 8));
					break;

				case ModbusCodes.READ_HOLDING_REGISTERS:
					// Read received commands
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_HOLDING_REGISTERS_IN_READ_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.READ_HOLDING_REGISTERS);
					send_buffer.Add((byte)(2 * qor));
					try
					{
						for (int ii = 0; ii < (qor * 2); ii += 2)
							send_buffer.AddRange(GetBytes(ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa + (ii / 2)]));
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					break;

				case ModbusCodes.READ_INPUT_REGISTERS:
					// Read received commands
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).InputRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_INPUT_REGISTERS_IN_READ_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.INPUT_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.READ_INPUT_REGISTERS);
					send_buffer.Add((byte)(2 * qor));
					try
					{
						for (int ii = 0; ii < (qor * 2); ii += 2)
							send_buffer.AddRange(GetBytes(ModbusDatabase.Single(x => x.UnitID == unit_id).InputRegisters[sa + (ii / 2)]));
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					break;

				case ModbusCodes.WRITE_SINGLE_COIL:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					val = ToUInt16(receive_buffer.ToArray(), 3);
					switch (val)
					{
						case 0x0000:
							cv = false;
							break;

						case 0xFF00:
							cv = true;
							break;

						default:
							Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
							cv = false; // Dummy
							break;
					}
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).Coils.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (Error == Errors.EXCEPTION_ILLEGAL_DATA_VALUE)
					{
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.COIL_REGISTERS, sa, 1))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (Error == Errors.WRONG_WRITE_SINGLE_COIL_VALUE)
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_ILLEGAL_DATA_VALUE);
						break;
					}
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unit_id).Coils[sa] = cv;
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.WRITE_SINGLE_COIL);
					send_buffer.AddRange(GetBytes(sa));
					send_buffer.AddRange(GetBytes(val));
					break;

				case ModbusCodes.WRITE_SINGLE_REGISTER:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					val = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((val >= 0x0000) && (val <= 0xFFFF)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa, 1))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa] = val;
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.WRITE_SINGLE_REGISTER);
					send_buffer.AddRange(GetBytes(sa));
					send_buffer.AddRange(GetBytes(val));
					break;

				case ModbusCodes.WRITE_MULTIPLE_COILS:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).Coils.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_COILS_IN_WRITE_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.COIL_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					bc = receive_buffer[5];
					byte[] buffer = receive_buffer.GetRange(6, bc).ToArray();
					BitArray ba = new BitArray(buffer);
					try
					{
						ba.CopyTo(ModbusDatabase.Single(x => x.UnitID == unit_id).Coils, sa);
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_COILS);
					send_buffer.AddRange(GetBytes(sa));
					send_buffer.AddRange(GetBytes(qor));
					break;

				case ModbusCodes.WRITE_MULTIPLE_REGISTERS:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!((qor >= 1) && (qor <= MAX_HOLDING_REGISTERS_IN_WRITE_MSG)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa, qor))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					bc = receive_buffer[5];
					try
					{
						for (int ii = 0; ii < bc; ii += 2)
						{
							ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa + (ii / 2)] = ToUInt16(receive_buffer.ToArray(), 6 + ii);
						}
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_REGISTERS);
					send_buffer.AddRange(GetBytes(sa));
					send_buffer.AddRange(GetBytes(qor));
					break;

				case ModbusCodes.MASK_WRITE_REGISTER:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					and_mask = ToUInt16(receive_buffer.ToArray(), 3);
					or_mask = ToUInt16(receive_buffer.ToArray(), 5);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!(((and_mask >= 0x0000) && (and_mask <= 0xFFFF)) || ((and_mask >= 0x0000) && (and_mask <= 0xFFFF))))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa, 1))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					try
					{
						ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa] =
							(ushort)((ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa] & and_mask) | (or_mask & (~and_mask)));
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Reply
					send_buffer.Add((byte)ModbusCodes.MASK_WRITE_REGISTER);
					send_buffer.AddRange(GetBytes(sa));
					send_buffer.AddRange(GetBytes(and_mask));
					send_buffer.AddRange(GetBytes(or_mask));
					break;

				case ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS:
					// Adjusting
					sa = ToUInt16(receive_buffer.ToArray(), 1);
					qor = ToUInt16(receive_buffer.ToArray(), 3);
					sa1 = ToUInt16(receive_buffer.ToArray(), 5);
					qor1 = ToUInt16(receive_buffer.ToArray(), 7);
					if (ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters.Length == 0)
					{
						Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if (!(((qor >= 1) && (qor <= MAX_HOLDING_REGISTERS_TO_READ_IN_READWRITE_MSG)) ||
						((qor1 >= 1) && (qor1 <= MAX_HOLDING_REGISTERS_TO_WRITE_IN_READWRITE_MSG))))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					if ((!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa, qor)) ||
						(!IsAllRegistersPresent(unit_id, ModbusDBTables.HOLDING_REGISTERS, sa1, qor1)))
					{
						Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
						BuildExceptionMessage(send_buffer, mdbcode, Error);
						break;
					}
					bc = receive_buffer[9];
					// First: Exec writing...
					try
					{
						for (int ii = 0; ii < bc; ii += 2)
							ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa1 + (ii / 2)] = ToUInt16(receive_buffer.ToArray(), 10 + ii);
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					// Second: Exec reading and prepare the reply
					send_buffer.Add((byte)ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS);
					send_buffer.AddRange(GetBytes((ushort)(qor * 2)));
					try
					{
						for (int ii = 0; ii < (qor * 2); ii += 2)
							send_buffer.AddRange(GetBytes(ModbusDatabase.Single(x => x.UnitID == unit_id).HoldingRegisters[sa + (ii / 2)]));
					}
					catch
					{
						BuildExceptionMessage(send_buffer, mdbcode, Errors.EXCEPTION_SLAVE_DEVICE_FAILURE);
						break;
					}
					break;

				default:
					Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
					BuildExceptionMessage(send_buffer, mdbcode, Error);
					break;
			}
			// Send response
			SendReply(send_buffer, flux, unit_id, transaction_id);
		}

		/// <summary>
		/// Send the response
		/// </summary>
		/// <param name="send_buffer">Send buffer</param>
		/// <param name="flux">Object for flux gest</param>
		/// <param name="unit_id">Slave ID</param>
		void SendReply(List<byte> send_buffer, object flux, byte unit_id, ushort transaction_id)
		{
			switch (_connectionType)
			{
				case ConnectionType.SERIAL_ASCII:
					// Add unit ID
					send_buffer.Insert(0, unit_id);
					// Enqueue LRC
					send_buffer.Add(LRC.CalcLRC(send_buffer.ToArray(), 0, send_buffer.Count));
					// ASCII encoding
					send_buffer = GetASCIIBytesFromBinaryBuffer(send_buffer.ToArray()).ToList();
					// Add START character
					send_buffer.Insert(0, Encoding.ASCII.GetBytes(new char[] { ASCII_START_FRAME }).First());
					// Enqueue STOP chars
					send_buffer.AddRange(Encoding.ASCII.GetBytes(new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND }));
					break;

				case ConnectionType.SERIAL_RTU:
					// Add unit ID
					send_buffer.Insert(0, unit_id);
					// Enqueue CRC
					send_buffer.AddRange(BitConverter.GetBytes(CRC16.CalcCRC16(send_buffer.ToArray(), 0, send_buffer.Count)));
					// Wait for interframe delay
					Thread.Sleep(_interframeDelay);
					break;

				case ConnectionType.UDP_IP:
				case ConnectionType.TCP_IP:
					// Build MBAP header
					send_buffer.InsertRange(0, GetBytes(transaction_id));
					send_buffer.InsertRange(2, GetBytes(PROTOCOL_ID));
					send_buffer.InsertRange(4, GetBytes((ushort)(1 + send_buffer.Count - 4)));
					send_buffer.Insert(6, unit_id);
					break;
			}
			// Send the buffer
			WriteBuffer(flux, send_buffer.ToArray(), 0, send_buffer.Count);
		}

		#endregion
	}
}
