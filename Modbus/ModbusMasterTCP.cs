using System.Net.Sockets;

namespace Modbus
{
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
				IsConnected = true;
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
			IsConnected = false;
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
}
