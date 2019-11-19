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

		// Transmission buffer
		protected List<byte> _sendBuffer = new List<byte>();

		// Reception buffer
		protected List<byte> _receiveBuffer = new List<byte>();

		// Modbus transaction ID (only Modbus TCP/UDP)
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
		public bool IsConnected { get; set; } = false;

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

		#region Helper Methods

		/// <summary>
		/// Execute a query to a destination master
		/// </summary>
		/// <param name="deviceAddress">Salve device address</param>
		/// <param name="messageLength">Message lenght</param>
		protected void Query(byte deviceAddress, ushort messageLength)
		{
			int rcv;
			long tmo;

			// Build the message according to the selected protocol.
			switch (_connectionType)
			{
				case ConnectionType.SERIAL_ASCII:
					// Insert device address in front of the message.
					_sendBuffer.Insert(0, deviceAddress);

					// Calculate message LCR.
					byte[] lrc = GetASCIIBytesFromBinaryBuffer(new byte[] { LRC.CalcLRC(_sendBuffer.ToArray(), 0, _sendBuffer.Count) });

					// Convert the message from binary to ASCII.
					_sendBuffer = GetASCIIBytesFromBinaryBuffer(_sendBuffer.ToArray()).ToList();

					// Add LRC at the end of the message.
					_sendBuffer.AddRange(lrc);

					// Insert the start frame chararacter at the start of the message.
					_sendBuffer.Insert(0, Encoding.ASCII.GetBytes(new char[] { ASCII_START_FRAME }).First());

					// Insert stop frame characters at the end of the message.
					char[] endFrame = new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND };
					_sendBuffer.AddRange(Encoding.ASCII.GetBytes(endFrame));
					break;

				case ConnectionType.SERIAL_RTU:
					// Insert device address in front of the message.
					_sendBuffer.Insert(0, deviceAddress);

					// Append CRC16 to end of message.
					_sendBuffer.AddRange(BitConverter.GetBytes(CRC16.CalcCRC16(_sendBuffer.ToArray(), 0, _sendBuffer.Count)));

					// Wait for interframe delay.
					Thread.Sleep(_interframeDelay);
					break;

				// For Modbus TCP/UDP, build MBAP header.
				case ConnectionType.UDP_IP:
				case ConnectionType.TCP_IP:
					// Transaction ID (incremented by 1 on each trasmission)
					_sendBuffer.InsertRange(0, GetBytes(_transactionID));

					// Protocol ID (fixed value)
					_sendBuffer.InsertRange(2, GetBytes(PROTOCOL_ID));

					// Message length
					_sendBuffer.InsertRange(4, GetBytes(messageLength));

					// Remote unit ID
					_sendBuffer.Insert(6, deviceAddress);
					break;
			}

			// Send the message.
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
				throw new ModbusTimeoutException("Timeout waiting for response.");
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
					throw new ModbusTimeoutException("Wrong message length.");
				}
				switch (_connectionType)
				{
					case ConnectionType.SERIAL_ASCII:
						// Check and remove start char
						if (_receiveBuffer[0] != _sendBuffer[0])
						{
							throw new ModbusTimeoutException("Start character not found.");
						}
						_receiveBuffer.RemoveRange(0, 1);
						// Check and remove stop chars
						char[] orig_end_frame = new char[] { ASCII_STOP_FRAME_1ST, ASCII_STOP_FRAME_2ND };
						char[] rec_end_frame = Encoding.ASCII.GetChars(_receiveBuffer.GetRange(_receiveBuffer.Count - 2, 2).ToArray());
						if (!orig_end_frame.SequenceEqual(rec_end_frame))
						{
							throw new ModbusTimeoutException("End characters not found.");
						}
						_receiveBuffer.RemoveRange(_receiveBuffer.Count - 2, 2);
						// Convert receive buffer from ASCII to binary
						_receiveBuffer = GetBinaryBufferFromASCIIBytes(_receiveBuffer.ToArray()).ToList();
						// Check and remove message LRC
						byte lrc_calculated = LRC.CalcLRC(_receiveBuffer.ToArray(), 0, _receiveBuffer.Count - 1);
						byte lrc_received = _receiveBuffer[_receiveBuffer.Count - 1];
						if (lrc_calculated != lrc_received)
						{
							throw new ModbusResponseException("Wrong LRC");
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
							throw new ModbusResponseException("Wrong CRC.");
						}
						// Check message consistency
						byte addr = _receiveBuffer[0];
						if (addr != _sendBuffer[0])
						{
							throw new ModbusResponseException("Wrong response address.");
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
							throw new ModbusResponseException("Wrong transaction ID.");
						}
						ushort pid = ToUInt16(_receiveBuffer.ToArray(), 2);
						if (pid != PROTOCOL_ID)
						{
							throw new ModbusResponseException("Wrong transaction ID.");
						}
						ushort len = ToUInt16(_receiveBuffer.ToArray(), 4);
						if ((_receiveBuffer.Count - MBAP_HEADER_LEN + 1) < len)
						{
							throw new ModbusResponseException("Wrong message length.");
						}
						byte uid = _receiveBuffer[6];
						if (uid != _sendBuffer[6])
						{
							throw new ModbusResponseException("Wrong device address.");
						}

						// Let only useful bytes in receive buffer.
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
							throw new ModbusIllegalFunctionException();

						case 2:
							throw new ModbusIllegalDataAddressException();

						case 3:
							throw new ModbusIllegalDataValueException();

						case 4:
							throw new ModbusSlaveDeviceFailureException();
					}
				}
			}
		}

		#endregion

		#region Protocol Methods

		/// <summary>
		/// Read coil registers.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be read</param>
		/// <param name="numCoils">Number of coils to be read</param>
		/// <returns>Array of registers from slave</returns>
		public bool[] ReadCoils(byte deviceAddress, ushort startAddress, ushort numCoils)
		{
			if (numCoils < 1)
			{
				throw new ModbusRequestException("Zero coils requested.");
			}
			if (numCoils > MAX_COILS_IN_READ_MSG)
			{
				throw new ModbusRequestException("Too many coils requested.");
			}

			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.READ_COILS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes(numCoils));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			BitArray ba = new BitArray(_receiveBuffer.GetRange(2, _receiveBuffer.Count - 2).ToArray());
			bool[] ret = new bool[ba.Count];
			ba.CopyTo(ret, 0);
			return ret;
		}

		/// <summary>
		/// Read a set of discrete inputs.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be read</param>
		/// <param name="numInputs">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public bool[] ReadDiscreteInputs(byte deviceAddress, ushort startAddress, ushort numInputs)
		{
			if (numInputs < 1)
			{
				throw new ModbusRequestException("Zero discrete inputs requested.");
			}
			if (numInputs > MAX_DISCRETE_INPUTS_IN_READ_MSG)
			{
				throw new ModbusRequestException("Too many coils requested.");
			}

			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.READ_DISCRETE_INPUTS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes(numInputs));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			BitArray ba = new BitArray(_receiveBuffer.GetRange(2, _receiveBuffer.Count - 2).ToArray());
			bool[] ret = new bool[ba.Count];
			ba.CopyTo(ret, 0);
			return ret;
		}

		/// <summary>
		/// Read a set of holding registers.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be read</param>
		/// <param name="numRegisters">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public ushort[] ReadHoldingRegisters(byte deviceAddress, ushort startAddress, ushort numRegisters)
		{
			if (numRegisters < 1)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (numRegisters > MAX_HOLDING_REGISTERS_IN_READ_MSG)
			{
				throw new ModbusRequestException("Too many registers requested.");
			}

			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.READ_HOLDING_REGISTERS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes(numRegisters));
			
			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
			{
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			}

			return ret.ToArray();
		}

		/// <summary>
		/// Read a set of input registers.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be read</param>
		/// <param name="numRegisters">Number of registers to be read</param>
		/// <returns>Array of readed registers</returns>
		public ushort[] ReadInputRegisters(byte deviceAddress, ushort startAddress, ushort numRegisters)
		{
			if (numRegisters < 1)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (numRegisters > MAX_INPUT_REGISTERS_IN_READ_MSG)
			{
				throw new ModbusRequestException("Too many registers requested.");
			}

			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.READ_INPUT_REGISTERS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes(numRegisters));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			return ret.ToArray();
		}

		/// <summary>
		/// Write a coil register.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="coilAddress">coil's address</param>
		/// <param name="value">Value to write</param>
		public void WriteSingleCoil(byte deviceAddress, ushort coilAddress, bool value)
		{
			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear(); _sendBuffer.Add((byte)ModbusCodes.WRITE_SINGLE_COIL);
			_sendBuffer.AddRange(GetBytes(coilAddress));
			_sendBuffer.AddRange(GetBytes((ushort)(value == true ? 0xFF00 : 0x0000)));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
			ushort regval = ToUInt16(_receiveBuffer.ToArray(), 3);

			// Check for errors in the response.
			if (addr != coilAddress)
			{
				throw new ModbusResponseException("Wrong response address.");
			}
			if ((regval == 0xFF00) && !value)
			{
				throw new ModbusResponseException("Wrong response value.");
			}
		}

		/// <summary>
		/// Write a holding register.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="registerAddress">Register address</param>
		/// <param name="value">Value to write</param>
		public void WriteSingleRegister(byte deviceAddress, ushort registerAddress, ushort value)
		{
			// Set message length of six bytes.
			ushort messageLength = 6;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_SINGLE_REGISTER);
			_sendBuffer.AddRange(GetBytes(registerAddress));
			_sendBuffer.AddRange(GetBytes(value));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
			if (addr != registerAddress)
			{
				throw new ModbusResponseException("Wrong response address.");
			}
		}

		/// <summary>
		/// Write a set of coil registers.
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be write</param>
		/// <param name="values">Array of values to write</param>
		public void WriteMultipleCoils(byte deviceAddress, ushort startAddress, bool[] values)
		{
			if (values == null)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (values.Length < 1)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (values.Length > MAX_COILS_IN_WRITE_MSG)
			{
				throw new ModbusRequestException("Too many registers requested.");
			}

			// Set the message length based on how many boolean values we are sending.
			byte byteCount = (byte)((values.Length / 8) + ((values.Length % 8) == 0 ? 0 : 1));
			ushort messageLength = (ushort)(1 + 6 + byteCount);

			// Transform the array of boolean values into an array of bytes.
			byte[] data = new byte[byteCount];
			BitArray ba = new BitArray(values);
			ba.CopyTo(data, 0);

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_COILS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add(byteCount);
			_sendBuffer.AddRange(data);

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			ushort sa = ToUInt16(_receiveBuffer.ToArray(), 1);
			ushort nr = ToUInt16(_receiveBuffer.ToArray(), 3);

			// Check for errors in the response.
			if (sa != startAddress)
			{
				throw new ModbusResponseException("Wrong response address.");
			}
			if (nr != values.Length)
			{
				throw new ModbusResponseException("Wrong response registers.");
			}
		}

		/// <summary>
		/// Write a set of holding registers
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="startAddress">Address of first register to be write</param>
		/// <param name="values">Array of values to write</param>
		public void WriteMultipleRegisters(byte deviceAddress, ushort startAddress, ushort[] values)
		{
			if (values == null)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (values.Length < 1)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if (values.Length > MAX_HOLDING_REGISTERS_IN_WRITE_MSG)
			{
				throw new ModbusRequestException("Too many registers requested.");
			}

			// Set the message length according to how many registers we are writing.
			ushort messageLength = (ushort)(7 + (values.Length * 2));

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear();
			_sendBuffer.Add((byte)ModbusCodes.WRITE_MULTIPLE_REGISTERS);
			_sendBuffer.AddRange(GetBytes(startAddress));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add((byte)(values.Length * 2));
			for (int ii = 0; ii < values.Length; ii++)
			{
				_sendBuffer.AddRange(GetBytes(values[ii]));
			}

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			ushort sa = ToUInt16(_receiveBuffer.ToArray(), 1);
			ushort nr = ToUInt16(_receiveBuffer.ToArray(), 3);

			// Check for errors in the response.
			if (sa != startAddress)
			{
				throw new ModbusResponseException("Wrong response address.");
			}
			if (nr != values.Length)
			{
				throw new ModbusResponseException("Wrong response registers.");
			}

		}

		/// <summary>
		/// Make an AND and OR mask of a single holding register
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="registerAddress">Register address</param>
		/// <param name="andMask">AND mask</param>
		/// <param name="orMask">OR mask</param>
		public void MaskWriteRegister(byte deviceAddress, ushort registerAddress, ushort andMask, ushort orMask)
		{
			// Set message length to eight bytes.
			ushort messageLength = 8;

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear(); _sendBuffer.Add((byte)ModbusCodes.MASK_WRITE_REGISTER);
			_sendBuffer.AddRange(GetBytes(registerAddress));
			_sendBuffer.AddRange(GetBytes(andMask));
			_sendBuffer.AddRange(GetBytes(orMask));

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Check address.
			ushort addr = ToUInt16(_receiveBuffer.ToArray(), 1);
			if (registerAddress != addr)
			{
				throw new ModbusResponseException("Wrong response address.");
			}
			// Check AND mask
			ushort am = ToUInt16(_receiveBuffer.ToArray(), 3);
			if (andMask != am)
			{
				throw new ModbusResponseException("Wrong response AND mask.");
			}
			// Check OR mask
			ushort om = ToUInt16(_receiveBuffer.ToArray(), 5);
			if (orMask != om)
			{
				throw new ModbusResponseException("Wrong response OR mask.");
			}

		}

		/// <summary>
		/// Read and write a set of holding registers in a single shot
		/// </summary>
		/// <param name="deviceAddress">Slave device address</param>
		/// <param name="readStartAddress">Address of first registers to be read</param>
		/// <param name="readNumRegisters">Number of registers to be read</param>
		/// <param name="writeStartAddress">Address of first registers to be write</param>
		/// <param name="values">Array of values to be write</param>
		/// <returns>Array of readed registers</returns>
		/// <remarks>
		/// Write is the first operation, than the read operation
		/// </remarks>
		public ushort[] ReadWriteMultipleRegisters(byte deviceAddress, ushort readStartAddress, ushort readNumRegisters, ushort writeStartAddress, ushort[] values)
		{
			if (values == null)
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if ((readNumRegisters < 1) || (values.Length < 1))
			{
				throw new ModbusRequestException("Zero registers requested.");
			}
			if ((readNumRegisters > MAX_HOLDING_REGISTERS_TO_READ_IN_READWRITE_MSG) ||
				(values.Length > MAX_HOLDING_REGISTERS_TO_WRITE_IN_READWRITE_MSG))
			{
				throw new ModbusRequestException("Too many registers requested.");
			}

			// Set message length according to how many registers we are writing.
			ushort messageLength = (ushort)(11 + (values.Length * 2));

			// Increase transaction ID (only used for TCP/UDP).
			_transactionID++;

			// Form the request.
			_sendBuffer.Clear(); _sendBuffer.Add((byte)ModbusCodes.READ_WRITE_MULTIPLE_REGISTERS);
			_sendBuffer.AddRange(GetBytes(readStartAddress));
			_sendBuffer.AddRange(GetBytes(readNumRegisters));
			_sendBuffer.AddRange(GetBytes(writeStartAddress));
			_sendBuffer.AddRange(GetBytes((ushort)values.Length));
			_sendBuffer.Add((byte)(values.Length * 2));
			for (int ii = 0; ii < values.Length; ii++)
			{
				_sendBuffer.AddRange(GetBytes(values[ii]));
			}

			// Send the request and receive the response.
			Query(deviceAddress, messageLength);

			// Parse the response.
			List<ushort> ret = new List<ushort>();
			for (int ii = 0; ii < _receiveBuffer[1]; ii += 2)
			{
				ret.Add(ToUInt16(_receiveBuffer.ToArray(), ii + 2));
			}

			return ret.ToArray();
		}

		#endregion
	}
}
