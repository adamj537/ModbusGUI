using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Modbus
{
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
}
