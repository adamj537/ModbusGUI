/****************************************************************************
 
 Modbus - Free .NET Modbus Library
 
 Author  : Simone Assunti
 License : Freeware open source

*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace Modbus
{
	#region Custom Events Args

	/// <summary>
	/// Event args for remote endpoint connection
	/// </summary>
	public sealed class ModbusTCPUDPClientConnectedEventArgs : EventArgs
	{
		/// <summary>
		/// Remote EndPoint
		/// </summary>
		public IPEndPoint RemoteEndPoint { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remoteEndPoint">Remote EndPoint</param>
		public ModbusTCPUDPClientConnectedEventArgs(IPEndPoint remoteEndPoint)
		{
			RemoteEndPoint = remoteEndPoint;
		}
	}

	#endregion

	#region Enumerations

	/// <summary>
	/// Connection types
	/// </summary>
	public enum ConnectionType
	{
		/// <summary>
		/// Modbus serial RTU
		/// </summary>
		SERIAL_RTU = 0,

		/// <summary>
		/// Modbus serial ASCII
		/// </summary>
		SERIAL_ASCII = 1,

		/// <summary>
		/// Modbus TCP/IP
		/// </summary>
		TCP_IP = 2,

		/// <summary>
		/// Modbus UDP
		/// </summary>
		UDP_IP = 3
	}

	/// <summary>
	/// Type of modbus serial
	/// </summary>
	public enum ModbusSerialType
	{
		/// <summary>
		/// Modbus RTU
		/// </summary>
		RTU = 0,

		/// <summary>
		/// Modbus ASCII
		/// </summary>
		ASCII = 1
	}

	/// <summary>
	/// Type of device
	/// </summary>
	public enum DeviceType
	{
		/// <summary>
		/// Modbus master
		/// </summary>
		MASTER = 0,

		/// <summary>
		/// Modbus slave
		/// </summary>
		SLAVE = 1
	}

	/// <summary>
	/// Tabelle del database modbus
	/// </summary>
	public enum ModbusDBTables
	{
		DISCRETE_INPUTS_REGISTERS = 0,
		COIL_REGISTERS = 1,
		INPUT_REGISTERS = 2,
		HOLDING_REGISTERS = 3
	}

	/// <summary>
	/// Modbus calling codes
	/// </summary>
	enum ModbusCodes
	{
		READ_COILS = 0x01,
		READ_DISCRETE_INPUTS = 0x02,
		READ_HOLDING_REGISTERS = 0x03,
		READ_INPUT_REGISTERS = 0x04,
		WRITE_SINGLE_COIL = 0x05,
		WRITE_SINGLE_REGISTER = 0x06,
		READ_EXCEPTION_STATUS = 0x07,
		DIAGNOSTIC = 0x08,
		GET_COM_EVENT_COUNTER = 0x0B,
		GET_COM_EVENT_LOG = 0x0C,
		WRITE_MULTIPLE_COILS = 0x0F,
		WRITE_MULTIPLE_REGISTERS = 0x10,
		REPORT_SLAVE_ID = 0x11,
		READ_FILE_RECORD = 0x14,
		WRITE_FILE_RECORD = 0x15,
		MASK_WRITE_REGISTER = 0x16,
		READ_WRITE_MULTIPLE_REGISTERS = 0x17,
		READ_FIFO_QUEUE = 0x18,
		READ_DEVICE_IDENTIFICATION = 0x2B
	}

	/// <summary>
	/// Error codes
	/// </summary>
	public enum Errors
	{
		NO_ERROR = 0,
		RX_TIMEOUT = -1,
		WRONG_TRANSACTION_ID = -2,
		WRONG_PROTOCOL_ID = -3,
		WRONG_RESPONSE_UNIT_ID = -4,
		WRONG_RESPONSE_FUNCTION_CODE = -5,
		WRONG_MESSAGE_LEN = -6,
		WRONG_RESPONSE_ADDRESS = -7,
		WRONG_RESPONSE_REGISTERS = -8,
		WRONG_RESPONSE_VALUE = -9,
		WRONG_CRC = -10,
		WRONG_LRC = -11,
		START_CHAR_NOT_FOUND = -12,
		END_CHARS_NOT_FOUND = -13,
		WRONG_RESPONSE_AND_MASK = -14,
		WRONG_RESPONSE_OR_MASK = -15,
		THREAD_BLOCK_REQUEST = -16,
		WRONG_WRITE_SINGLE_COIL_VALUE = -17,
		TOO_MANY_REGISTERS_REQUESTED = -18,
		ZERO_REGISTERS_REQUESTED = -19,
		EXCEPTION_ILLEGAL_FUNCTION = -20,
		EXCEPTION_ILLEGAL_DATA_ADDRESS = -21,
		EXCEPTION_ILLEGAL_DATA_VALUE = -22,
		EXCEPTION_SLAVE_DEVICE_FAILURE = -23,
		EXCEPTION_ACKNOLEDGE = -24,
		EXCEPTION_SLAVE_DEVICE_BUSY = -25,
		EXCEPTION_MEMORY_PARITY_ERROR = -26,
		EXCEPTION_GATEWAY_PATH_UNAVAILABLE = -27,
		EXCEPTION_GATEWAY_TARGET_DEVICE_FAILED_TO_RESPOND = -28,
		WRONG_REGISTER_ADDRESS = -29
	}

	#endregion

	#region Base abstract class

	/// <summary>
	/// Base abstract class
	/// </summary>
	public abstract class ModbusBase
	{
		#region Constants

		/// <summary>
		/// Modbus protocol identifier (only TCP and UDP)
		/// </summary>
		protected const ushort PROTOCOL_ID = 0x0000;

		/// <summary>
		/// Default rx timeout in milliseconds
		/// </summary>
		const int DEFAULT_RX_TIMEOUT = 6000;

		/// <summary>
		/// Length in bytes of MBAP header        
		/// </summary>
		protected const int MBAP_HEADER_LEN = 7;

		/// <summary>
		/// Start frame character (only Modbus serial ASCII)
		/// </summary>
		protected const char ASCII_START_FRAME = ':';

		/// <summary>
		/// End frame first character (only Modbus serial ASCII)
		/// </summary>
		protected const char ASCII_STOP_FRAME_1ST = '\x0D';

		/// <summary>
		/// End frame second character (only Modbus serial ASCII)
		/// </summary>
		protected const char ASCII_STOP_FRAME_2ND = '\x0A';

		/// <summary>
		/// Max number of coil registers that can be read
		/// </summary>
		public const ushort MAX_COILS_IN_READ_MSG = 2000;

		/// <summary>
		/// Max number of discrete inputs registers that can be read
		/// </summary>
		public const ushort MAX_DISCRETE_INPUTS_IN_READ_MSG = 2000;

		/// <summary>
		/// Max number of holding registers that can be read
		/// </summary>
		public const ushort MAX_HOLDING_REGISTERS_IN_READ_MSG = 125;

		/// <summary>
		/// Max number of input registers that can be read
		/// </summary>
		public const ushort MAX_INPUT_REGISTERS_IN_READ_MSG = 125;

		/// <summary>
		/// Max number of coil registers that can be written
		/// </summary>
		public const ushort MAX_COILS_IN_WRITE_MSG = 1968;

		/// <summary>
		/// Max number of holding registers that can be written
		/// </summary>
		public const ushort MAX_HOLDING_REGISTERS_IN_WRITE_MSG = 123;

		/// <summary>
		/// Max number of holding registers that can be read in a read/write message
		/// </summary>
		public const ushort MAX_HOLDING_REGISTERS_TO_READ_IN_READWRITE_MSG = 125;

		/// <summary>
		/// Max number of holding registers that can be written in a read/write message
		/// </summary>
		public const ushort MAX_HOLDING_REGISTERS_TO_WRITE_IN_READWRITE_MSG = 121;

		#endregion

		#region Fields

		/// <summary>
		/// Connection type
		/// </summary>
		protected ConnectionType _connectionType;

		/// <summary>
		/// Device type
		/// </summary>
		protected DeviceType _deviceType;

		/// <summary>
		/// Delay between two Modbus serial RTU frame (milliseconds)
		/// </summary>
		protected int _interframeDelay;

		/// <summary>
		/// Delay between two Modbus serial RTU characters (milliseconds)
		/// </summary>
		protected int _intercharDelay;

		#endregion

		#region Properties

		/// <summary>
		/// Reception timeout (milliseconds)
		/// </summary>
		public int RxTimeout { get; set; } = DEFAULT_RX_TIMEOUT;

		/// <summary>
		/// Error code
		/// </summary>
		public Errors Error { get; set; }

		#endregion

		#region Helper Methods

		/// <summary>
		/// Return an array of bytes from an unsigned 16 bit integer using BIG ENDIAN codification
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <returns>Bytes array</returns>
		protected byte[] GetBytes(ushort value)
		{
			byte[] array = new byte[2];

			array[0] = (byte)(value >> 8);
			array[1] = (byte)(value & 0x00FF);

			return array;
		}

		/// <summary>
		/// Return an array of bytes coded in ASCII according to Modbus specification
		/// </summary>
		/// <param name="buffer">Buffer to codify</param>
		/// <returns>Buffer codified</returns>
		/// <remarks>
		/// Example of codification : Byte = 0x5B
		/// Codified in two chars   : 0x35 = '5' and 0x42 = 'B' in ASCII
		/// The returned vector is exactly the double of the introduced one.
		/// </remarks>
		protected byte[] GetASCIIBytesFromBinaryBuffer(byte[] buffer)
		{
			List<char> chars = new List<char>();
			for (int i = 0, j = 0; i < buffer.Length * 2; i++)
			{
				char ch;
				byte val = (byte)((i % 2) == 0 ? buffer[j] >> 4 : buffer[j] & 0x0F);
				switch (val)
				{
					default:
					case 0x00: ch = '0'; break;
					case 0x01: ch = '1'; break;
					case 0x02: ch = '2'; break;
					case 0x03: ch = '3'; break;
					case 0x04: ch = '4'; break;
					case 0x05: ch = '5'; break;
					case 0x06: ch = '6'; break;
					case 0x07: ch = '7'; break;
					case 0x08: ch = '8'; break;
					case 0x09: ch = '9'; break;
					case 0x0A: ch = 'A'; break;
					case 0x0B: ch = 'B'; break;
					case 0x0C: ch = 'C'; break;
					case 0x0D: ch = 'D'; break;
					case 0x0E: ch = 'E'; break;
					case 0x0F: ch = 'F'; break;
				}
				chars.Add(ch);
				if ((i % 2) != 0)
				{
					j++;
				}
			}

			return Encoding.ASCII.GetBytes(chars.ToArray());
		}

		/// <summary>
		/// Return a binary buffer from a byte array codified in ASCII according to Modbus specification
		/// </summary>
		/// <param name="buffer">ASCII codified buffer</param>
		/// <returns>Binary buffer</returns>
		/// <remarks>
		/// Example of codification : Char1 = 0x35 ('5') and Char2 = 0x42 ('B')
		/// Byte decodified         : Byte = 0x5B
		/// The returned vector is exactly the half of the introduced one
		/// </remarks>
		protected byte[] GetBinaryBufferFromASCIIBytes(byte[] buffer)
		{
			List<byte> ret = new List<byte>();
			char[] chars = Encoding.ASCII.GetChars(buffer);
			byte bt = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				byte tmp;
				switch (chars[i])
				{
					default:
					case '0': tmp = 0x00; break;
					case '1': tmp = 0x01; break;
					case '2': tmp = 0x02; break;
					case '3': tmp = 0x03; break;
					case '4': tmp = 0x04; break;
					case '5': tmp = 0x05; break;
					case '6': tmp = 0x06; break;
					case '7': tmp = 0x07; break;
					case '8': tmp = 0x08; break;
					case '9': tmp = 0x09; break;
					case 'A': tmp = 0x0A; break;
					case 'B': tmp = 0x0B; break;
					case 'C': tmp = 0x0C; break;
					case 'D': tmp = 0x0D; break;
					case 'E': tmp = 0x0E; break;
					case 'F': tmp = 0x0F; break;
				}
				if (i % 2 != 0)
				{
					bt |= tmp;
					ret.Add(bt);
					bt = 0;
				}
				else
					bt = (byte)(tmp << 4);
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Return a 16 bit unsigned integer from two bytes according to BIG ENDIAN codification
		/// </summary>
		/// <param name="value">Source byte array</param>
		/// <param name="offset">Buffer offset</param>
		/// <returns>Integer returned</returns>
		protected ushort ToUInt16(byte[] value, int offset)
		{
			return (ushort)((value[offset] << 8) | (value[offset + 1] & 0x00FF));
		}

		/// <summary>
		/// Return a byte from an 8-bit boolean array
		/// </summary>
		/// <param name="array">Array booleano di 8 bit</param>
		/// <param name="offset">Array offset</param>
		/// <returns>Byte returned</returns>
		protected byte EightBitToByte(bool[] array, int offset)
		{
			if (array.Length < 8)
				throw new Exception(MethodBase.GetCurrentMethod().Name + ": The array must be at least 8-bit length!");
			byte ret = 0x00;
			for (int ii = 0; ii < 8; ii++)
			{
				switch (array[offset + ii])
				{
					case true:
						ret |= (byte)(1 << ii);
						break;

					case false:
						ret &= (byte)(~(1 << ii));
						break;
				}
			}
			return ret;
		}

		#endregion

		#region Protocol Methods

		/// <summary>
		/// Get delay time between two modbus RTU frame in milliseconds
		/// </summary>
		/// <param name="serialPort">Serial Port</param>
		/// <returns>Calculated delay (milliseconds)</returns>
		protected int GetInterframeDelay(SerialPort serialPort)
		{
			int delay;

			if (serialPort.BaudRate > 19200)
				delay = 2;   // Fixed value = 1.75ms up rounded
			else
			{
				int nbits = 1 + serialPort.DataBits;
				nbits += serialPort.Parity == Parity.None ? 0 : 1;
				switch (serialPort.StopBits)
				{
					case StopBits.One:
						nbits += 1;
						break;

					case StopBits.OnePointFive: // Ceiling
					case StopBits.Two:
						nbits += 2;
						break;
				}
				delay = Convert.ToInt32(Math.Ceiling(1 / ((serialPort.BaudRate / (nbits * 3.5d)) / 1000)));
			}

			return delay;
		}

		/// <summary>
		/// Get max delay time in milliseconds between received chars in modbus RTU trasmission
		/// </summary>
		/// <param name="serialPort">Serial Port</param>
		/// <returns>Calculated delay (milliseconds)</returns>
		protected int GetIntercharDelay(SerialPort serialPort)
		{
			int delay;

			if (serialPort.BaudRate > 19200)
				delay = 1;   // Fixed value = 0.75 ms up rounded
			else
			{
				int nbits = 1 + serialPort.DataBits;
				nbits += serialPort.Parity == Parity.None ? 0 : 1;
				switch (serialPort.StopBits)
				{
					case StopBits.One:
						nbits += 1;
						break;

					case StopBits.OnePointFive: // Ceiling
					case StopBits.Two:
						nbits += 2;
						break;
				}
				delay = Convert.ToInt32(Math.Ceiling(1 / ((serialPort.BaudRate / (nbits * 1.5d)) / 1000)));
			}

			return delay;
		}

		#endregion
	}

	#endregion

	#region Base abstract class for Modbus master instances

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

	#endregion

	#region Base abstract class for Modbus slave instances

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

	#endregion

	#region Serial Modbus Slave Class

	/// <summary>
	/// Serial Modbus Slave Class
	/// </summary>
	public sealed class ModbusSlaveSerial : ModbusSlave
	{
		#region Fields

		/// <summary>
		/// Serial Port instance
		/// </summary>
		SerialPort _serialPort;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="modbus_db">Modbus database</param>
		/// <param name="type">Type of serial modbus protocol (RTU or ASCII)</param>
		/// <param name="port">Serial port name</param>
		/// <param name="baudrate">Baudrate</param>
		/// <param name="databits">Data bits</param>
		/// <param name="parity">Parity</param>
		/// <param name="stopbits">Stop bits</param>
		/// <param name="handshake">Control flux</param>
		public ModbusSlaveSerial(Datastore[] modbus_db, ModbusSerialType type, string port, int baudrate, int databits, Parity parity, StopBits stopbits, Handshake handshake)
			: base(modbus_db)
		{
			// Set modbus serial protocol type
			switch (type)
			{
				case ModbusSerialType.ASCII:
					_connectionType = ConnectionType.SERIAL_ASCII;
					break;

				case ModbusSerialType.RTU:
					_connectionType = ConnectionType.SERIAL_RTU;
					break;
			}
			// Set serial port instance
			_serialPort = new SerialPort(port, baudrate, parity, databits, stopbits);
			_serialPort.Handshake = handshake;
			// Calc interframe delay
			_interframeDelay = GetInterframeDelay(_serialPort);
			// Calc interchar delay
			_intercharDelay = GetIntercharDelay(_serialPort);
		}

		#endregion

		/// <summary>
		/// Thread for gest incoming calls
		/// </summary>
		protected override void GuestRequests()
		{
			try
			{
				List<byte> send_buffer = new List<byte>();
				List<byte> receive_buffer = new List<byte>();
				while (_run)
				{
					IncomingMessagePolling(send_buffer, receive_buffer, _serialPort);
					Thread.Sleep(1);
				}
			}
			catch { }
		}

		/// <summary>
		/// Start listening function
		/// </summary>
		public override void StartListening()
		{
			_serialPort.Open();
			if (_serialPort.IsOpen)
			{
				_serialPort.DiscardInBuffer();
				_serialPort.DiscardOutBuffer();
				if (_guestRequest == null)
				{
					_run = true;
					_guestRequest = new Thread(GuestRequests);
					_guestRequest.Start();
				}
			}
		}

		/// <summary>
		/// Stop listening function
		/// </summary>
		public override void StopListening()
		{
			if (_guestRequest != null)
			{
				_run = false;
				_guestRequest.Join();
				_guestRequest = null;
			}
			if (_serialPort != null)
			{
				if (_serialPort.IsOpen)
					_serialPort.Close();
			}
		}
	}

	#endregion

	#region Modbus TCP Slave Class

	/// <summary>
	/// Modbus TCP Slave class
	/// </summary>
	public sealed class ModbusSlaveTCP : ModbusSlave
	{
		#region Fields

		private List<IPEndPoint> remote_clients_connected = new List<IPEndPoint>();

		/// <summary>
		/// Listener TCP
		/// </summary>
		private TcpListener _tcpl;

		/// <summary>
		/// Manual reset event
		/// </summary>
		private ManualResetEvent _mre = new ManualResetEvent(false);

		#endregion

		#region Events

		/// <summary>
		/// Client connected event
		/// </summary>
		public event EventHandler<ModbusTCPUDPClientConnectedEventArgs> TCPClientConnected;

		/// <summary>
		/// Client disconnected event
		/// </summary>
		public event EventHandler<ModbusTCPUDPClientConnectedEventArgs> TCPClientDisconnected;

		#endregion

		#region Properties

		/// <summary>
		/// Connected clients
		/// </summary>
		public IPEndPoint[] RemoteClientsConnected => remote_clients_connected.ToArray();

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="modbus_db">Modbus database array</param>
		/// <param name="local_address">Local listening IP addresses</param>
		/// <param name="port">Listening TCP port</param>
		public ModbusSlaveTCP(Datastore[] modbus_db, IPAddress local_address, int port) :
			base(modbus_db)
		{
			// Set device states
			_connectionType = ConnectionType.TCP_IP;
			// Crete TCP listener
			_tcpl = new TcpListener(local_address, port);
		}

		#endregion

		#region Module Functions

		/// <summary>
		/// Check if TcpClient is already connected
		/// </summary>
		/// <param name="client">TcpClient istance to check</param>
		/// <returns>Connection status</returns>
		bool IsClientConnected(TcpClient client)
		{
			bool ret = true;

			if (client.Client.Poll(0, SelectMode.SelectRead))
			{
				byte[] check_connection = new byte[1];
				if (client.Client.Receive(check_connection, SocketFlags.Peek) == 0)
				{
					ret = false;
				}
			}

			return ret;
		}

		#endregion

		#region Input connections callback

		/// <summary>
		/// Input connections callback
		/// </summary>
		/// <param name="ar"></param>
		void DoAcceptTcpClientCallback(IAsyncResult ar)
		{
			TcpListener listener = null;
			TcpClient client = null;
			NetworkStream ns = null;
			IPEndPoint ipe = null;

			try
			{
				List<byte> send_buffer = new List<byte>();
				List<byte> receive_buffer = new List<byte>();

				// Copy listener instance
				listener = (TcpListener)ar.AsyncState;
				// Get client
				client = listener.EndAcceptTcpClient(ar);
				// Fire event
				ipe = (IPEndPoint)client.Client.RemoteEndPoint;
				remote_clients_connected.Add(ipe);
				TCPClientConnected?.Invoke(this, new ModbusTCPUDPClientConnectedEventArgs(ipe));
				// Set manual reset event
				_mre.Set();
				// Get network stream
				ns = client.GetStream();
				// Processing incoming connections
				while (_run)
				{
					if (!IsClientConnected(client))
						break;
					IncomingMessagePolling(send_buffer, receive_buffer, ns);
					Thread.Sleep(1);
				}
			}
			catch { }
			finally
			{
				// Close IO stream
				if (ns != null)
					ns.Close();
				// Close client connection                
				if (client != null)
					client.Close();
				// Fire event
				if (ipe != null)
				{
					remote_clients_connected.Remove(ipe);
					TCPClientDisconnected?.Invoke(this, new ModbusTCPUDPClientConnectedEventArgs(ipe));
				}
			}
		}

		#endregion

		#region Process thread for incoming connections

		/// <summary>
		/// Corpo del thread del gestore delle chiamate in ingresso
		/// </summary>
		protected override void GuestRequests()
		{
			while (_run)
			{
				// Reset event
				_mre.Reset();
				// Async call to incoming connections
				_tcpl.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), _tcpl);
				// Wait for event
				_mre.WaitOne();
			}
		}

		#endregion

		#region Start and Stop Methods

		/// <summary>
		/// Start listening
		/// </summary>
		public override void StartListening()
		{
			_tcpl.Start();
			if (_guestRequest == null)
			{
				_run = true;
				_guestRequest = new Thread(GuestRequests);
				_guestRequest.Start();
			}
		}

		/// <summary>
		/// Stop listening
		/// </summary>
		public override void StopListening()
		{
			if (_guestRequest != null)
			{
				_run = false;
				_mre.Set();
				_guestRequest.Join();
				_guestRequest = null;
			}
			_tcpl.Stop();
		}

		#endregion
	}

	#endregion

	#region Modbus Master TCP Class

	/// <summary>
	/// Modbus Master TCP Class
	/// </summary>
	public class ModbusMasterTCP : ModbusMaster
	{
		#region Fields

		/// <summary>
		/// Remote hostname or IP address
		/// </summary>
		private string _remoteHost;

		/// <summary>
		/// Remote host Modbus TCP listening port
		/// </summary>
		readonly int _port;

		/// <summary>
		/// TCP Client
		/// </summary>
		private TcpClient _tcpClient;

		/// <summary>
		/// Network stream
		/// </summary>
		private NetworkStream _networkStream;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remoteHost">Remote hostname or IP address</param>
		/// <param name="port">Remote host Modbus TCP listening port</param>
		public ModbusMasterTCP(string remoteHost, int port)
		{
			// Set device states
			_connectionType = ConnectionType.TCP_IP;
			// Set socket client
			_remoteHost = remoteHost;
			_port = port;
		}

		#endregion

		/// <summary>
		/// Connect
		/// </summary>
		public override void Connect()
		{
			if (_tcpClient == null)
				_tcpClient = new TcpClient();
			_tcpClient.Connect(_remoteHost, _port);
			if (_tcpClient.Connected)
			{
				_networkStream = _tcpClient.GetStream();
				Connected = true;
			}
		}

		/// <summary>
		/// Disconnect
		/// </summary>
		public override void Disconnect()
		{
			_networkStream.Close();
			_tcpClient.Close();
			_tcpClient = null;
			Connected = false;
		}

		/// <summary>
		/// Send trasmission buffer
		/// </summary>
		protected override void Send()
		{
			_networkStream.Write(_sendBuffer.ToArray(), 0, _sendBuffer.Count);
		}

		/// <summary>
		/// Read a byte from network stream
		/// </summary>
		/// <returns>Readed byte or <c>-1</c> if there are any bytes</returns>
		protected override int ReceiveByte()
		{
			if (_networkStream.DataAvailable)
				return _networkStream.ReadByte();
			else
				return -1;
		}
	}

	#endregion

	#region Modbus Slave UDP Class

	/// <summary>
	/// Modbus Slave UDP Class
	/// </summary>
	public sealed class ModbusSlaveUDP : ModbusSlave
	{
		#region Fields

		/// <summary>
		/// UDP Listener
		/// </summary>
		UdpClient _udpListener;

		/// <summary>
		/// Manual reset event
		/// </summary>
		ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="modbus_db">Modbus databases array</param>
		/// <param name="local_address">Local listening IP addresses</param>
		/// <param name="port">Listening TCP port</param>
		public ModbusSlaveUDP(Datastore[] modbus_db, IPAddress local_address, int port) :
			base(modbus_db)
		{
			// Set device states
			_connectionType = ConnectionType.UDP_IP;
			// Create UDP listener
			_udpListener = new UdpClient(new IPEndPoint(local_address, port));
		}

		#endregion

		/// <summary>
		/// Incoming connection callback
		/// </summary>
		/// <param name="ar"></param>
		void DoAcceptUdpDataCallback(IAsyncResult ar)
		{
			UdpClient listener = null;
			UDPData udp_data = null;
			byte[] rx_buffer = null;
			IPEndPoint remote_ep = null;

			try
			{
				List<byte> send_buffer = new List<byte>();
				List<byte> receive_buffer = new List<byte>();

				// Copy listener instance
				listener = (UdpClient)ar.AsyncState;
				// Get input frame and remote endpoint
				rx_buffer = listener.EndReceive(ar, ref remote_ep);
				// Istance UDPData class
				udp_data = new UDPData(listener, rx_buffer, remote_ep);
				// Set event
				_manualResetEvent.Set();
				// Process incoming call
				IncomingMessagePolling(send_buffer, receive_buffer, udp_data);
			}
			catch { }
			finally
			{
				if (udp_data != null)
					udp_data.Close();
			}
		}

		/// <summary>
		/// Incoming call process thread
		/// </summary>
		protected override void GuestRequests()
		{
			while (_run)
			{
				// Reset event
				_manualResetEvent.Reset();
				// Async call to process callback
				_udpListener.BeginReceive(new AsyncCallback(DoAcceptUdpDataCallback), _udpListener);
				// wait for event
				_manualResetEvent.WaitOne();
			}
		}

		/// <summary>
		/// Start listen
		/// </summary>
		public override void StartListening()
		{
			if (_guestRequest == null)
			{
				_run = true;
				_guestRequest = new Thread(GuestRequests);
				_guestRequest.Start();
			}
		}

		/// <summary>
		/// Stop listen
		/// </summary>
		public override void StopListening()
		{
			if (_guestRequest != null)
			{
				_run = false;
				_guestRequest.Join();
				_guestRequest = null;
			}
			_udpListener.Close();
		}
	}

	#endregion

	#region Modbus Master UDP class

	/// <summary>
	/// Modbus master UDP class
	/// </summary>
	public class ModbusMasterUDP : ModbusMaster
	{
		#region Fields

		/// <summary>
		/// Remote hostname or IP address
		/// </summary>
		private string _remoteHost;

		/// <summary>
		/// Remote Modbus TCP port
		/// </summary>
		private int _port;

		/// <summary>
		/// Temporary receive buffer
		/// </summary>
		private List<byte> _tmpRxBuffer = new List<byte>();

		/// <summary>
		/// UDP Client
		/// </summary>
		private UdpClient _udpClient;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remote_host">Remote hostname or IP address</param>
		/// <param name="port">Remote Modbus TCP port</param>
		public ModbusMasterUDP(string remote_host, int port)
		{
			// Set device states
			_connectionType = ConnectionType.UDP_IP;
			// Set socket client
			_remoteHost = remote_host;
			_port = port;
			_udpClient = new UdpClient();
		}

		#endregion

		/// <summary>
		/// Connect function
		/// </summary>
		public override void Connect()
		{
			_udpClient.Connect(_remoteHost, _port);
			if (_udpClient.Client.Connected)
				Connected = true;
		}

		/// <summary>
		/// Disconnect function
		/// </summary>
		public override void Disconnect()
		{
			_udpClient.Close();
			Connected = false;
		}

		/// <summary>
		/// Send trasmission buffer
		/// </summary>
		protected override void Send()
		{
			_udpClient.Send(_sendBuffer.ToArray(), _sendBuffer.Count);
		}

		/// <summary>
		/// Read a byte from network stream
		/// </summary>
		/// <returns>Readed byte or <c>-1</c> if there are any bytes</returns>
		protected override int ReceiveByte()
		{
			IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);

			// Check if there are available bytes
			if (_udpClient.Available > 0)
			{
				// Enqueue bytes to temporary rx buffer
				_tmpRxBuffer.AddRange(_udpClient.Receive(ref ipe));
			}

			if (_tmpRxBuffer.Count > 0)
			{
				// There are available bytes in temporary rx buffer, read the first and delete it
				byte ret = _tmpRxBuffer[0];
				_tmpRxBuffer.RemoveAt(0);
				return ret;
			}
			else
				return -1;
		}
	}

	#endregion

	#region Modbus Serial Master Class

	/// <summary>
	/// Modbus serial master class
	/// </summary>
	public sealed class ModbusMasterSerial : ModbusMaster
	{
		#region Fields

		/// <summary>
		/// Serial port instance
		/// </summary>
		private SerialPort _serialPort;

		/// <summary>
		/// Char timeout
		/// </summary>
		private long _charTimeout;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Type of serial protocol</param>
		/// <param name="port">Serial port name</param>
		/// <param name="baudrate">Baudrate</param>
		/// <param name="databits">Data bits</param>
		/// <param name="parity">Parity</param>
		/// <param name="stopbits">Stop bits</param>
		/// <param name="handshake">Handshake</param>
		public ModbusMasterSerial(ModbusSerialType type, string port, int baudrate, int databits, Parity parity, StopBits stopbits, Handshake handshake)
		{
			// Set device states
			switch (type)
			{
				case ModbusSerialType.RTU:
					_connectionType = ConnectionType.SERIAL_RTU;
					break;

				case ModbusSerialType.ASCII:
					_connectionType = ConnectionType.SERIAL_ASCII;
					break;
			}
			// Set serial port instance
			_serialPort = new SerialPort(port, baudrate, parity, databits, stopbits);
			_serialPort.Handshake = handshake;
			// Get interframe delay
			_interframeDelay = GetInterframeDelay(_serialPort);
			// Get interchar delay
			_intercharDelay = GetIntercharDelay(_serialPort);
		}

		#endregion

		/// <summary>
		/// Connect function
		/// </summary>
		public override void Connect()
		{
			_serialPort.Open();
			if (_serialPort.IsOpen)
			{
				_serialPort.DiscardInBuffer();
				_serialPort.DiscardOutBuffer();
				Connected = true;
			}
		}

		/// <summary>
		/// Disconnect function
		/// </summary>
		public override void Disconnect()
		{
			_serialPort.Close();
			Connected = false;
		}

		/// <summary>
		/// Send trasmission buffer
		/// </summary>
		protected override void Send()
		{
			_serialPort.Write(_sendBuffer.ToArray(), 0, _sendBuffer.Count);
			// Reset timeout counter
			_charTimeout = 0;
		}

		/// <summary>
		/// Read a byte from stream
		/// </summary>
		/// <returns>Readed byte or <c>-1</c> if there are any bytes</returns>
		protected override int ReceiveByte()
		{
			bool done = false;
			int value;

			// Await 1 char...
			if (!_timeoutStopwatch.IsRunning)
				_timeoutStopwatch.Start();

			do
			{
				if (_serialPort.BytesToRead > 0)
				{
					value = _serialPort.ReadByte();
					done = true;
				}
				else
					value = -1;
			} while ((!done) && ((_timeoutStopwatch.ElapsedMilliseconds - _charTimeout) < _intercharDelay));

			if (done)
				_charTimeout = _timeoutStopwatch.ElapsedMilliseconds;   // Char received with no errors...reset timeout counter for next char!

			return value;
		}
	}

	#endregion

	#region UDP Data class

	/// <summary>
	/// UDP data class
	/// </summary>
	class UDPData
	{
		#region Fields

		/// <summary>
		/// Input stream
		/// </summary>
		MemoryStream _inputStream;

		/// <summary>
		/// UDP Client
		/// </summary>
		UdpClient _client;

		/// <summary>
		/// Remote endpoint
		/// </summary>
		IPEndPoint _remoteEndPoint;

		#endregion

		#region Properties

		/// <summary>
		/// Get stream length
		/// </summary>
		public long Length => _inputStream.Length;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="udp_client">UDP client</param>
		/// <param name="rx_data">Received data buffer</param>
		/// <param name="remote_endpoint">Remote endpoint</param>
		public UDPData(UdpClient udp_client, byte[] rx_data, IPEndPoint remote_endpoint)
		{
			_client = udp_client;
			_remoteEndPoint = remote_endpoint;
			_inputStream = new MemoryStream(rx_data);
		}

		#endregion

		/// <summary>
		/// Read a byte from input stream
		/// </summary>
		/// <returns>Readed byte or <c>-1</c> if there are any bytes</returns>
		public int ReadByte()
		{
			return _inputStream.ReadByte();
		}

		/// <summary>
		/// Send an response buffer
		/// </summary>
		/// <param name="buffer">Buffer to send</param>
		/// <param name="offset">Buffer offset</param>
		/// <param name="size">Data length</param>
		public void WriteResp(byte[] buffer, int offset, int size)
		{
			byte[] tmp_buffer = new byte[size];
			Buffer.BlockCopy(buffer, offset, tmp_buffer, 0, size);
			_client.Send(tmp_buffer, size, _remoteEndPoint);
		}

		/// <summary>
		/// Close input stream
		/// </summary>
		public void Close()
		{
			if (_inputStream != null)
				_inputStream.Close();
		}
	}

	#endregion

	#region Datastore class

	/// <summary>
	/// Datastore class
	/// </summary>
	public sealed class Datastore
	{
		#region Constants

		/// <summary>
		/// Max DB elements
		/// </summary>
		private const int MAX_ELEMENTS = 65536;

		#endregion

		#region Properties

		/// <summary>
		/// Dicrete Input registers (read-only - 1 bit)
		/// </summary>
		public bool[] DiscreteInputs { get; set; }

		/// <summary>
		/// Coils registers (read/write - 1 bit)
		/// </summary>
		public bool[] Coils { get; set; }

		/// <summary>
		/// Input registers (read-only - 16 bit)
		/// </summary>
		public ushort[] InputRegisters { get; set; }

		/// <summary>
		/// Holding registers (read/write - 16 bit)
		/// </summary>
		public ushort[] HoldingRegisters { get; set; }

		/// <summary>
		/// Device ID
		/// </summary>
		public byte UnitID { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="uid">Device ID</param>
		/// <param name="n_discrete_inputs">Input registers (read-only - 1 bit)</param>
		/// <param name="n_coils">Coils registers (read/write - 1 bit)</param>
		/// <param name="n_input_registers">Input registers (read-only - 16 bit)</param>
		/// <param name="n_holding_registers">Holding registers (read/write - 16 bit)</param>
		public Datastore(byte uid, int n_discrete_inputs, int n_coils, int n_input_registers, int n_holding_registers)
		{
			// Set device ID
			UnitID = uid;
			// Validate values and set db length
			if (((n_discrete_inputs >= 0) && (n_discrete_inputs <= MAX_ELEMENTS)) &&
				((n_coils >= 0) && (n_coils <= MAX_ELEMENTS)) &&
				((n_input_registers >= 0) && (n_input_registers <= MAX_ELEMENTS)) &&
				((n_holding_registers >= 0) && (n_holding_registers <= MAX_ELEMENTS)))
			{
				DiscreteInputs = new bool[n_discrete_inputs];
				Coils = new bool[n_coils];
				InputRegisters = new ushort[n_input_registers];
				HoldingRegisters = new ushort[n_holding_registers];
			}
			else
				throw new Exception("Database definition wrong , each set of records must be between 0 and e " + MAX_ELEMENTS.ToString() + "!");
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="uid">Device ID</param>
		/// <remarks>The database if initialized at the maximum capacity allowed</remarks>
		public Datastore(byte uid)
		{
			// Set device ID
			UnitID = uid;
			// Set DB length
			DiscreteInputs = new bool[MAX_ELEMENTS];
			Coils = new bool[MAX_ELEMENTS];
			InputRegisters = new ushort[MAX_ELEMENTS];
			HoldingRegisters = new ushort[MAX_ELEMENTS];
		}

		#endregion
	}

	#endregion
}
