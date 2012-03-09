using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace IISExpressProxy.Models
{
	class AppProxy : IApplicationProxy
	{
		private int _internalPort;
		private int _externalPort;
		private TcpListener _curListener;
		public void Start(int internalPort, int externalPort)
		{
			//We are already listening on a port
			if (_curListener != null)
				throw new InvalidOperationException("We are already listening on a port");

			_internalPort = internalPort;
			_externalPort = externalPort;

			//Start new thread
			new Thread(WaitForConnection).Start();
		}

		private bool _endRequested;
		private void WaitForConnection()
		{
			_endRequested = false;

			_curListener = new TcpListener(new IPEndPoint(IPAddress.Any, _externalPort));
			_curListener.Start();

			while (!_endRequested)
			{
				try
				{
					TcpClient client = _curListener.AcceptTcpClient();
					new Thread(ForwardRequests).Start(client);
				}
				catch { }
			}//end while
		}

		private readonly TimeSpan TIMEOUTTHRESHOLD = new TimeSpan(0, 1, 0);
		private const int BUFFSIZE = 5120;
		private void ForwardRequests(object client)
		{
			using (TcpClient externalClient = (TcpClient)client)
			using (TcpClient internalHost = new TcpClient())
			{
				//Connect to host
				internalHost.Connect(new IPEndPoint(IPAddress.Loopback, _internalPort));

				//Grab streams
				using (NetworkStream clientStream = externalClient.GetStream())
				using (NetworkStream hostStream = internalHost.GetStream())
				{
					//Read from host and client, forward on any requests received
					DateTime lastActivity = DateTime.Now;
					byte[] buffer = new byte[BUFFSIZE];
					int read = 0;					
					while (DateTime.Now - lastActivity < TIMEOUTTHRESHOLD)
					{
						//Read from client, write to host
						bool somethingRead = false;
						while (externalClient.Connected && externalClient.Available > 0)
						{
							read = clientStream.Read(buffer, 0, BUFFSIZE);
							hostStream.Write(buffer, 0, read);
							somethingRead = true;
						}//end while externalClient

						//Read from host, write to client
						while (internalHost.Connected && internalHost.Available > 0)
						{
							read = hostStream.Read(buffer, 0, BUFFSIZE);
							clientStream.Write(buffer, 0, read);
							somethingRead = true;
						}//end while externalClient

						if (_endRequested)
							return;

						if(!somethingRead)
							Thread.Sleep(100);						
					}//end while
				}//end using
			}//end using
		}

		public void End()
		{
			_endRequested = true;
			if (_curListener != null)
				_curListener.Stop();
			_curListener = null;
		}

		public void Dispose()
		{
			End();
		}
	}
}
