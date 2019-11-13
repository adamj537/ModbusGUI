/****************************************************************************
 
 Modbus - Free .NET Modbus Library
 
 Author  : Simone Assunti
 License : Freeware open source

*****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Text;

namespace Modbus
{
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
}
