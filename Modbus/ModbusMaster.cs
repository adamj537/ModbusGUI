using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Modbus
{
	/// <summary>
	/// Base abstract class for Modbus master instances
	/// </summary>
	public abstract class ModbusMaster : ModbusBase
	{
		#region Fields

		/// <summary>
		/// Trasmission buffer
		/// </summary>
		protected List<byte> _sendBuffer = new List<byte>();

		/// <summary>
		/// Reception buffer
		/// </summary>
		protected List<byte> _receiveBuffer = new List<byte>();

		/// <summary>
		/// Modbus transaction ID (only Modbus TCP/UDP)
		/// </summary>
		protected ushort _transactionID = 0;

		/// <summary>
		/// Interchar timeout timer
		/// </summary>
		protected Stopwatch _timeoutStopwatch;

		#endregion

		#region Properties

		/// <summary>
		/// Remote host connection status
		/// </summary>
		public bool Connected { get; set; } = false;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ModbusMaster()
		{
			// Set device type.
			_deviceType = DeviceType.MASTER;

			// Initialize interchar timeout timer.
			_timeoutStopwatch = new Stopwatch();
		}

		#endregion

		#region Connection Methods

		/// <summary>
		/// Open connection
		/// </summary>
		public abstract void Connect();

		/// <summary>
		/// Close connection
		/// </summary>
		public abstract void Disconnect();

		#endregion

		#region Send/Receive Methods

		/// <summary>
		/// Function to send trasminission buffer
		/// </summary>
		protected abstract void Send();

		/// <summary>
		/// Function to read a byte from a device
		/// </summary>
		/// <returns>Byte readed or -1 if no data are present</returns>
		protected abstract int ReceiveByte();

		#endregion

		#region Protocol Methods

		/// <summary>
		/// Init a new Modbus TCP/UDP message
		/// </summary>
		protected void InitTCPUDPMasterMessage()
		{
			// Increase transaction ID.
			_transactionID++;

			// Empty transmit buffer.
			_sendBuffer.Clear();
		}

		/// <summary>
		/// Build MBAP header for Modbus TCP/UDP
		/// </summary>
		/// <param name="dest_address">Destination address</param>
		/// <param name="message_len">Message length</param>
		protected void BuildMBAPHeader(byte dest_address, ushort message_len)
		{
			// Transaction ID (incremented by 1 on each trasmission)
			_sendBuffer.InsertRange(0, GetBytes(_transactionID));

			// Protocol ID (fixed value)
			_sendBuffer.InsertRange(2, GetBytes(PROTOCOL_ID));

			// Message length
			_sendBuffer.InsertRange(4, GetBytes(message_len));

			// Remote unit ID
			_sendBuffer.Insert(6, dest_address);
		}

		/// <summary>
		/// Exec a query to a destination master
		/// </summary>
		/// <param name="unit_id">Salve device address</param>
		/// <param name="msg_len">Message lenght</param>
		protected void Query(byte unit_id, ushort msg_len)
		{
			int rcv;
			long tmo;

			// Set errors to null
			Error = Errors.NO_ERROR;
			// Start to build message
			switch (_connectionType)
			{
				case ConnectionType.SERIAL_ASCII:
					// Add destination device address
					_sendBuffer.Insert(0, unit_id);
					// Calc message LCR
					byte[] lrc = GetASCIIBytesFromBinaryBuffer(new byte[] { LRC.CalcLRC(_sendBuffer.ToArray(), 0, _sendBuffer.Count) });
					// Convert 'send_buffer' from binary to ASCII
					_sendBuffer = GetASCIIBytesFromBinaryBuffer(_sendBuffer.ToArray()).ToList();
					// Add LRC at the end of the message
					_sendBuffer.AddRange(lrc);
					// Insert the start frame char
					_sendBuffer.Insert(0, Encoding.ASCII.GetBytes(new char[] { ASCII_START_FRAME }).First());
					// Insert stop frame chars
					char[] end_frame = new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND };
					_sendBuffer.AddRange(Encoding.ASCII.GetBytes(end_frame));
					break;

				case ConnectionType.SERIAL_RTU:
					// Insert 'unit_id' in front of the message
					_sendBuffer.Insert(0, unit_id);
					// Append CRC16
					_sendBuffer.AddRange(BitConverter.GetBytes(CRC16.CalcCRC16(_sendBuffer.ToArray(), 0, _sendBuffer.Count)));
					// Wait for interframe delay
					Thread.Sleep(_interframeDelay);
					break;

				case ConnectionType.UDP_IP:
				case ConnectionType.TCP_IP:
					BuildMBAPHeader(unit_id, msg_len);
					break;
			}
			// Send trasmission buffer
			Send();
			// Start receiving...
			_receiveBuffer.Clear();
			bool done = false;
			bool in_ric = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			do
			{
				rcv = ReceiveByte();
				if (rcv > -1)
				{
					if (!in_ric)
						in_ric = true;
					if (_connectionType == ConnectionType.SERIAL_ASCII)
					{
						if ((byte)rcv == Encoding.ASCII.GetBytes(new char[] { ASCII_START_FRAME }).First())
							_receiveBuffer.Clear();
					}
					_receiveBuffer.Add((byte)rcv);
				}
				else if ((rcv == -1) && in_ric)
					done = true;
				tmo = sw.ElapsedMilliseconds;
			} while ((!done) && (RxTimeout > tmo));
			_timeoutStopwatch.Stop();
			sw.Stop();
			if (tmo >= RxTimeout)
			{
				Error = Errors.RX_TIMEOUT;
				return;
			}
			else
			{
				int min_frame_length;
				switch (_connectionType)
				{
					default:
					case ConnectionType.SERIAL_RTU:
						min_frame_length = 5;
						break;

					case ConnectionType.SERIAL_ASCII:
						min_frame_length = 11;
						break;

					case ConnectionType.UDP_IP:
					case ConnectionType.TCP_IP:
						min_frame_length = 9;
						break;
				}
				if (_receiveBuffer.Count < min_frame_length)
				{
					Error = Errors.WRONG_MESSAGE_LEN;
					return;
				}
				switch (_connectionType)
				{
					case ConnectionType.SERIAL_ASCII:
						// Check and remove start char
						if (_receiveBuffer[0] != _sendBuffer[0])
						{
							Error = Errors.START_CHAR_NOT_FOUND;
							return;
						}
						_receiveBuffer.RemoveRange(0, 1);
						// Check and remove stop chars
						char[] orig_end_frame = new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND };
						char[] rec_end_frame = Encoding.ASCII.GetChars(_receiveBuffer.GetRange(_receiveBuffer.Count - 2, 2).ToArray());
						if (!orig_end_frame.SequenceEqual(rec_end_frame))
						{
							Error = Errors.END_CHARS_NOT_FOUND;
							break;
						}
						_receiveBuffer.RemoveRange(_receiveBuffer.Count - 2, 2);
						// Convert receive buffer from ASCII to binary
						_receiveBuffer = GetBinaryBufferFromASCIIBytes(_receiveBuffer.ToArray()).ToList();
						// Check and remove message LRC
						byte lrc_calculated = LRC.CalcLRC(_receiveBuffer.ToArray(), 0, _receiveBuffer.Count - 1);
						byte lrc_received = _receiveBuffer[_receiveBuffer.Count - 1];
						if (lrc_calculated != lrc_received)
						{
							Error = Errors.WRONG_LRC;
							break;
						}
						_receiveBuffer.RemoveRange(_receiveBuffer.Count - 1, 1);
						// Remove address byte
						_receiveBuffer.RemoveRange(0, 1);
						break;

					case ConnectionType.SERIAL_RTU:
						// Check message 16-bit CRC
						ushort calc_crc = CRC16.CalcCRC16(_receiveBuffer.ToArray(), 0, _receiveBuffer.Count - 2);
						ushort rec_crc = BitConverter.ToUInt16(_receiveBuffer.ToArray(), _receiveBuffer.Count - 2);
						if (rec_crc != calc_crc)
						{
							Error = Errors.WRONG_CRC;
							return;
						}
						// Check message consistency
						byte addr = _receiveBuffer[0];
						if (addr != _sendBuffer[0])
						{
							Error = Errors.WRONG_RESPONSE_ADDRESS;
							return;
						}
						// Remove address
						_receiveBuffer.RemoveRange(0, 1);
						// Remove CRC
						_receiveBuffer.RemoveRange(_receiveBuffer.Count - 2, 2);
						break;

					case ConnectionType.UDP_IP:
					case ConnectionType.TCP_IP:
						// Check MBAP header
						ushort tid = ToUInt16(_receiveBuffer.ToArray(), 0);
						if (tid != _transactionID)
						{
							Error = Errors.WRONG_TRANSACTION_ID;
							return;
						}
						ushort pid = ToUInt16(_receiveBuffer.ToArray(), 2);
						if (pid != PROTOCOL_ID)
						{
							Error = Errors.WRONG_TRANSACTION_ID;
							return;
						}
						ushort len = ToUInt16(_receiveBuffer.ToArray(), 4);
						if ((_receiveBuffer.Count - MBAP_HEADER_LEN + 1) < len)
						{
							Error = Errors.WRONG_MESSAGE_LEN;
							return;
						}
						byte uid = _receiveBuffer[6];
						if (uid != _sendBuffer[6])
						{
							Error = Errors.WRONG_RESPONSE_UNIT_ID;
							return;
						}
						// Let only useful bytes in receive buffer                       
						_receiveBuffer.RemoveRange(0, MBAP_HEADER_LEN);
						break;
				}
				// Controllo eventuali messaggi di errore
				if (_receiveBuffer[0] > 0x80)
				{
					// E' stato segnalato un errore, controllo l'exception code
					switch (_receiveBuffer[1])
					{
						case 1:
							Error = Errors.EXCEPTION_ILLEGAL_FUNCTION;
							break;

						case 2:
							Error = Errors.EXCEPTION_ILLEGAL_DATA_ADDRESS;
							break;

						case 3:
							Error = Errors.EXCEPTION_ILLEGAL_DATA_VALUE;
							break;

						case 4:
							Error = Errors.EXCEPTION_SLAVE_DEVICE_FAILURE;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Read coil registers
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be read</param>
		/// <param name="len">Number of registers to be read</param>
		/// <returns>Array of registers from slave</returns>
		public bool[] ReadCoils(byte unit_id, ushort start_address, ushort len)
		{
			if (len < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if (len > MAX_COILS_IN_READ_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return null;
			}
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.READ_COILS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes(len));
			Query(unit_id, msg_len);
			if (Error != Errors.NO_ERROR)
				return null;
			BitArray ba = new BitArray(_receiveBuffer.GetRange(2, _receiveBuffer.Count - 2).ToArray());
			bool[] ret = new bool[ba.Count];
			ba.CopyTo(ret, 0);
			return ret;
		}

		/// <summary>
		/// Read a set of discrete inputs
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be read</param>
		/// <param name="len">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public bool[] ReadDiscreteInputs(byte unit_id, ushort start_address, ushort len)
		{
			if (len < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if (len > MAX_DISCRETE_INPUTS_IN_READ_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return null;
			}
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.READ_DISCRETE_INPUTS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes(len));
			Query(unit_id, msg_len);
			if (Error != Errors.NO_ERROR)
				return null;
			BitArray ba = new BitArray(_receiveBuffer.GetRange(2, _receiveBuffer.Count - 2).ToArray());
			bool[] ret = new bool[ba.Count];
			ba.CopyTo(ret, 0);
			return ret;
		}

		/// <summary>
		/// Read a set of holding registers
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be read</param>
		/// <param name="len">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public ushort[] ReadHoldingRegisters(byte unit_id, ushort start_address, ushort len)
		{
			if (len < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if (len > MAX_HOLDING_REGISTERS_IN_READ_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return null;
			}
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.READ_HOLDING_REGISTERS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes(len));
			Query(unit_id, msg_len);
			if (Error != Errors.NO_ERROR)
				return null;
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			return ret.ToArray();
		}

		/// <summary>
		/// Read a set of input registers
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be read</param>
		/// <param name="len">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public ushort[] ReadInputRegisters(byte unit_id, ushort start_address, ushort len)
		{
			if (len < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if (len > MAX_INPUT_REGISTERS_IN_READ_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return null;
			}
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.READ_INPUT_REGISTERS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes(len));
			Query(unit_id, msg_len);
			if (Error != Errors.NO_ERROR)
				return null;
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			return ret.ToArray();
		}

		/// <summary>
		/// Write a coil register
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="address">Register address</param>
		/// <param name="value">Value to write</param>
		public void WriteSingleCoil(byte unit_id, ushort address, bool value)
		{
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_SINGLE_COIL);
			_sendBuffer.AddRange(GetBytes(address));
			_sendBuffer.AddRange(GetBytes((ushort)(value == true ? 0xFF00 : 0x0000)));
			Query(unit_id, msg_len);
			if (Error == Errors.NO_ERROR)
			{
				ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
				ushort regval = ToUInt16(_receiveBuffer.ToArray(), 3);
				if (addr != address)
				{
					Error = Errors.WRONG_RESPONSE_ADDRESS;
					return;
				}
				if ((regval == 0xFF00) && !value)
				{
					Error = Errors.WRONG_RESPONSE_VALUE;
					return;
				}
			}
		}

		/// <summary>
		/// Write an holding register
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="address">Register address</param>
		/// <param name="value">Value to write</param>
		public void WriteSingleRegister(byte unit_id, ushort address, ushort value)
		{
			ushort msg_len = 6;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_SINGLE_REGISTER);
			_sendBuffer.AddRange(GetBytes(address));
			_sendBuffer.AddRange(GetBytes(value));
			Query(unit_id, msg_len);
			if (Error == Errors.NO_ERROR)
			{
				ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
				if (addr != address)
				{
					Error = Errors.WRONG_RESPONSE_ADDRESS;
					return;
				}
			}
		}

		/// <summary>
		/// Write a set of coil registers
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be write</param>
		/// <param name="values">Array of values to write</param>
		public void WriteMultipleCoils(byte unit_id, ushort start_address, bool[] values)
		{
			if (values == null)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return;
			}
			if (values.Length < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return;
			}
			if (values.Length > MAX_COILS_IN_WRITE_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return;
			}
			byte byte_cnt = (byte)((values.Length / 8) + ((values.Length % 8) == 0 ? 0 : 1));
			ushort msg_len = (ushort)(1 + 6 + byte_cnt);
			byte[] data = new byte[byte_cnt];
			BitArray ba = new BitArray(values);
			ba.CopyTo(data, 0);
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_COILS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add(byte_cnt);
			_sendBuffer.AddRange(data);
			Query(unit_id, msg_len);
			if (Error == Errors.NO_ERROR)
			{
				ushort sa = ToUInt16(_receiveBuffer.ToArray(), 1);
				ushort nr = ToUInt16(_receiveBuffer.ToArray(), 3);
				if (sa != start_address)
				{
					Error = Errors.WRONG_RESPONSE_ADDRESS;
					return;
				}
				if (nr != values.Length)
				{
					Error = Errors.WRONG_RESPONSE_REGISTERS;
					return;
				}
			}
		}

		/// <summary>
		/// Write a set of holding registers
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="start_address">Address of first register to be write</param>
		/// <param name="values">Array of values to write</param>
		public void WriteMultipleRegisters(byte unit_id, ushort start_address, ushort[] values)
		{
			if (values == null)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return;
			}
			if (values.Length < 1)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return;
			}
			if (values.Length > MAX_HOLDING_REGISTERS_IN_WRITE_MSG)
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return;
			}
			ushort msg_len = (ushort)(7 + (values.Length * 2));
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_REGISTERS);
			_sendBuffer.AddRange(GetBytes(start_address));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add((byte)(values.Length * 2));
			for (int ii = 0; ii < values.Length; ii++)
				_sendBuffer.AddRange(GetBytes(values[ii]));
			Query(unit_id, msg_len);
			if (Error == Errors.NO_ERROR)
			{
				ushort sa = ToUInt16(_receiveBuffer.ToArray(), 1);
				ushort nr = ToUInt16(_receiveBuffer.ToArray(), 3);
				if (sa != start_address)
				{
					Error = Errors.WRONG_RESPONSE_ADDRESS;
					return;
				}
				if (nr != values.Length)
				{
					Error = Errors.WRONG_RESPONSE_REGISTERS;
					return;
				}
			}
		}

		/// <summary>
		/// Make an AND and OR mask of a single holding register
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="address">Register address</param>
		/// <param name="and_mask">AND mask</param>
		/// <param name="or_mask">OR mask</param>
		public void MaskWriteRegister(byte unit_id, ushort address, ushort and_mask, ushort or_mask)
		{
			ushort msg_len = 8;
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.MASK_WRITE_REGISTER);
			_sendBuffer.AddRange(GetBytes(address));
			_sendBuffer.AddRange(GetBytes(and_mask));
			_sendBuffer.AddRange(GetBytes(or_mask));
			Query(unit_id, msg_len);
			if (Error == Errors.NO_ERROR)
			{
				// Check address
				ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
				if (address != addr)
				{
					Error = Errors.WRONG_RESPONSE_ADDRESS;
					return;
				}
				// Check AND mask
				ushort am = ToUInt16(_receiveBuffer.ToArray(), 3);
				if (and_mask != am)
				{
					Error = Errors.WRONG_RESPONSE_AND_MASK;
					return;
				}
				// Check OR mask
				ushort om = ToUInt16(_receiveBuffer.ToArray(), 5);
				if (or_mask != om)
				{
					Error = Errors.WRONG_RESPONSE_OR_MASK;
					return;
				}
			}
		}

		/// <summary>
		/// Read and write a set of holding registers in a single shot
		/// </summary>
		/// <param name="unit_id">Slave device address</param>
		/// <param name="read_start_address">Address of first registers to be read</param>
		/// <param name="read_len">Number of registers to be read</param>
		/// <param name="write_start_address">Address of first registers to be write</param>
		/// <param name="values">Array of values to be write</param>
		/// <returns>Array of readed registers</returns>
		/// <remarks>
		/// Write is the first operation, than the read operation
		/// </remarks>
		public ushort[] ReadWriteMultipleRegisters(byte unit_id, ushort read_start_address, ushort read_len, ushort write_start_address, ushort[] values)
		{
			if (values == null)
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if ((read_len < 1) || (values.Length < 1))
			{
				Error = Errors.ZERO_REGISTERS_REQUESTED;
				return null;
			}
			if ((read_len > MAX_HOLDING_REGISTERS_TO_READ_IN_READWRITE_MSG) || (values.Length > MAX_HOLDING_REGISTERS_TO_WRITE_IN_READWRITE_MSG))
			{
				Error = Errors.TOO_MANY_REGISTERS_REQUESTED;
				return null;
			}
			ushort msg_len = (ushort)(11 + (values.Length * 2));
			InitTCPUDPMasterMessage();
			_sendBuffer.Add((byte)ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS);
			_sendBuffer.AddRange(GetBytes(read_start_address));
			_sendBuffer.AddRange(GetBytes(read_len));
			_sendBuffer.AddRange(GetBytes(write_start_address));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add((byte)(values.Length * 2));
			for (int ii = 0; ii < values.Length; ii++)
				_sendBuffer.AddRange(GetBytes(values[ii]));
			Query(unit_id, msg_len);
			if (Error != Errors.NO_ERROR)
				return null;
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			return ret.ToArray();
		}

		#endregion
	}
}
