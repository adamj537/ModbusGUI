using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Modbus
{
	/// <summary>
	/// UDP data class
	/// </summary>
	internal class UDPData
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
}
