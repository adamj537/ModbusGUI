using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Modbus
{
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
				IsConnected = true;
		}

		/// <summary>
		/// Disconnect function
		/// </summary>
		public override void Disconnect()
		{
			_udpClient.Close();
			IsConnected = false;
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
}
