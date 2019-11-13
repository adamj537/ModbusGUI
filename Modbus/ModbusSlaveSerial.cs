using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace Modbus
{
	/// <summary>
	/// Serial Modbus Slave Class
	/// </summary>
	public sealed class ModbusSlaveSerial : ModbusSlave
	{
		#region Fields

		/// <summary>
		/// Serial Port instance
		/// </summary>
		private SerialPort _serialPort;

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
}
