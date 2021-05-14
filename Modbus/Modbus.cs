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
		SerialRTU = 0,

		/// <summary>
		/// Modbus serial ASCII
		/// </summary>
		SerialASCII = 1,

		/// <summary>
		/// Modbus TCP/IP
		/// </summary>
		TCPIP = 2,

		/// <summary>
		/// Modbus UDP
		/// </summary>
		UDPIP = 3
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
		Master = 0,

		/// <summary>
		/// Modbus slave
		/// </summary>
		Slave = 1
	}

	/// <summary>
	/// Tabelle del database modbus
	/// </summary>
	public enum ModbusDBTable
	{
		DiscreteInputsRegisters = 0,
		CoilRegisters = 1,
		InputRegisters = 2,
		HoldingRegisters = 3
	}

	/// <summary>
	/// Modbus calling codes
	/// </summary>
	internal enum ModbusCode
	{
		ReadCoils = 0x01,
		ReadDiscreteInputs = 0x02,
		ReadHoldingRegisters = 0x03,
		ReadInputRegisters = 0x04,
		WriteSingleCoil = 0x05,
		WriteSingleRegister = 0x06,
		ReadExceptionStatus = 0x07,
		Diagnostic = 0x08,
		GetComEventCounter = 0x0B,
		GetComEventLog = 0x0C,
		WriteMultipleCoils = 0x0F,
		WriteMultipleRegisters = 0x10,
		ReportSlaveID = 0x11,
		ReadFileRecord = 0x14,
		WriteFileRecord = 0x15,
		MaskWriteRegister = 0x16,
		ReadWriteMultipleRegisters = 0x17,
		ReadFifoQueue = 0x18,
		ReadDeviceIdentification = 0x2B
	}

	/// <summary>
	/// Error codes
	/// </summary>
	public enum ModbusError
	{
		NoError = 0,
		RxTimeout = -1,
		WrongTransactionID = -2,
		WrongProtocolID = -3,
		WrongResponseUnitID = -4,
		WrongResponseFunctionCode = -5,
		WrongMessageLength = -6,
		WrongResponseAddress = -7,
		WrongResponseRegisters = -8,
		WrongResponseValue = -9,
		WrongCRC = -10,
		WrongLRC = -11,
		StartCharNotFound = -12,
		EndCharsNotFound = -13,
		WrongResponseAndMask = -14,
		WrongResponseOrMask = -15,
		ThreadBlockRequest = -16,
		WrongWriteSingleCoilValue = -17,
		TooManyRegistersRequested = -18,
		ZeroRegistersRequested = -19,
		ExceptionIllegalFunction = -20,
		ExceptionIllegalDataAddress = -21,
		ExceptionIllegalDataValue = -22,
		ExceptionSlaveDeviceFailure = -23,
		ExceptionAcknoledge = -24,
		ExceptionSlaveDeviceBusy = -25,
		ExceptionMemoryParityError = -26,
		ExceptionGatewayPathUnavailable = -27,
		ExceptionGatewayTargetDeviceFailedToRespond = -28,
		WrongRegisterAddress = -29
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
		protected const ushort ProtocolID = 0x0000;

		/// <summary>
		/// Default rx timeout in milliseconds
		/// </summary>
		const int DefaultRxTimeout = 6000;

		/// <summary>
		/// Length in bytes of MBAP header        
		/// </summary>
		protected const int MbapHeaderLength = 7;

		/// <summary>
		/// Start frame character (only Modbus serial ASCII)
		/// </summary>
		protected const char AsciiStartFrame = ':';

		/// <summary>
		/// End frame first character (only Modbus serial ASCII)
		/// </summary>
		protected const char AsciiStopFrame1ST = '\x0D';

		/// <summary>
		/// End frame second character (only Modbus serial ASCII)
		/// </summary>
		protected const char AsciiStopFrame2ND = '\x0A';

		/// <summary>
		/// Max number of coil registers that can be read
		/// </summary>
		public const ushort MaxCoilsInReadMessage = 2000;

		/// <summary>
		/// Max number of discrete inputs registers that can be read
		/// </summary>
		public const ushort MaxDiscreteInputsInReadMessage = 2000;

		/// <summary>
		/// Max number of holding registers that can be read
		/// </summary>
		public const ushort MaxHoldingRegistersInReadMessage = 125;

		/// <summary>
		/// Max number of input registers that can be read
		/// </summary>
		public const ushort MaxInputRegistersInReadMessage = 125;

		/// <summary>
		/// Max number of coil registers that can be written
		/// </summary>
		public const ushort MaxCoilsInWriteMessage = 1968;

		/// <summary>
		/// Max number of holding registers that can be written
		/// </summary>
		public const ushort MaxHoldingRegistersInWriteMessage = 123;

		/// <summary>
		/// Max number of holding registers that can be read in a read/write message
		/// </summary>
		public const ushort MaxHoldingRegistersToReadInReadWriteMessage = 125;

		/// <summary>
		/// Max number of holding registers that can be written in a read/write message
		/// </summary>
		public const ushort MaxHoldingRegistersToWriteInReadWriteMessage = 121;

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
		public int RxTimeout { get; set; } = DefaultRxTimeout;

		#endregion

		#region Helper Methods

		/// <summary>
		/// Return an array of bytes from an unsigned 16 bit integer using BIG ENDIAN codification
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <returns>Bytes array</returns>
		protected static byte[] GetBytes(ushort value)
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
		protected static byte[] GetASCIIBytesFromBinaryBuffer(byte[] buffer)
		{
			// Check for null parameter.
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

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
		protected static byte[] GetBinaryBufferFromASCIIBytes(byte[] buffer)
		{
			// Check for null parameter.
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

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
		protected static ushort ToUInt16(byte[] value, int offset)
		{
			// Check for null parameter.
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			return (ushort)((value[offset] << 8) | (value[offset + 1] & 0x00FF));
		}

		/// <summary>
		/// Return a byte from an 8-bit boolean array
		/// </summary>
		/// <param name="array">Array booleano di 8 bit</param>
		/// <param name="offset">Array offset</param>
		/// <returns>Byte returned</returns>
		protected static byte EightBitToByte(bool[] array, int offset)
		{
			// Check for null parameter.
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

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
		protected static int GetInterframeDelay(SerialPort serialPort)
		{
			// Check for null parameter.
			if (serialPort == null)
			{
				throw new ArgumentNullException(nameof(serialPort));
			}

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
		protected static int GetIntercharDelay(SerialPort serialPort)
		{
			// Check for null parameter.
			if (serialPort == null)
			{
				throw new ArgumentNullException(nameof(serialPort));
			}

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
