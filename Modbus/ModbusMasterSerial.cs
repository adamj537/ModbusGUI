using System;
using System.IO.Ports;

namespace Modbus
{
	/// <summary>
	/// Modbus serial master class
	/// </summary>
	public sealed class ModbusMasterSerial : ModbusMaster, IDisposable
	{
		#region Fields

		/// <summary>
		/// Serial port instance
		/// </summary>
		private readonly SerialPort _serialPort;

		/// <summary>
		/// Character timeout
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
					_connectionType = ConnectionType.SerialRTU;
					break;

				case ModbusSerialType.ASCII:
					_connectionType = ConnectionType.SerialASCII;
					break;
			}

			// Set serial port instance.
			_serialPort = new SerialPort(port, baudrate, parity, databits, stopbits)
			{
				Handshake = handshake
			};

			// Get interframe delay.
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
			// Open the serial port.
			_serialPort.Open();

			// If the port opened successfully...
			if (_serialPort.IsOpen)
			{
				// Clear the buffers.
				_serialPort.DiscardInBuffer();
				_serialPort.DiscardOutBuffer();
				IsConnected = true;
			}
		}

		/// <summary>
		/// Disconnect function
		/// </summary>
		public override void Disconnect()
		{
			_serialPort?.Close();
			IsConnected = false;
		}

		/// <summary>
		/// Send trasmission buffer
		/// </summary>
		protected override void Send()
		{
			_serialPort.Write(_sendBuffer.ToArray(), 0, _sendBuffer.Count);

			// Reset timeout counter.
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

		/// <summary>
		/// Clean up the objects
		/// </summary>
		public void Dispose()
		{
			// Close the serial port.
			_serialPort?.Close();

			// Remember that we've closed the port.
			IsConnected = false;

			// Dispose of the serial port object.
			_serialPort?.Dispose();
		}
	}
}
