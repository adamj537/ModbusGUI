using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Modbus
{
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
}
