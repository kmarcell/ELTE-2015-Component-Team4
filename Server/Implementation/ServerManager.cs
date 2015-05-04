using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GTInterfacesLibrary.MessageTypes;
using Server.Interfaces;

namespace Server.Implementation
{
    public class ServerManager : IDisposable, IServerManager
    {
        private const Int32 MServerPort = 5503;
        private static ServerManager _mServerManagerInstance;
        private Socket _ServerSocket;
        private Boolean _Running;
        private readonly List<ClientManager> _Clients;

        private String MServerIp { get; set; }

        public String ServerIp
        {
            get { return MServerIp; }
        }

        public Int32 ServerPort
        {
            get { return MServerPort; }
        }

        public static ServerManager ServerManagerInstance
        {
            get 
            {
                return _mServerManagerInstance ?? (_mServerManagerInstance = new ServerManager()); 
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public Boolean Running
        {
            get { return _ServerSocket != null && _ServerSocket.IsBound; }
        }

        private ServerManager()
        {
            _Clients = new List<ClientManager>();
        }

        public void Start()
        {
            try
            {
                var serverName = Dns.GetHostName();
                var ipEntry = Dns.GetHostEntry(serverName);
                var addressList = ipEntry.AddressList.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToList();

                if (addressList.Count < 1)
                {
                    throw new Exception("Gameserver start error! No available local address.");
                }

                MServerIp = addressList[0].ToString();
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _ServerSocket.Bind(new IPEndPoint(addressList[0], MServerPort));
                _ServerSocket.Listen(10);

                _Running = true;
            }
            catch
            {
                Stop();
            }

            var serverThread = new Thread(WatchNewConnections);
            serverThread.Start();

            var watchThread = new Thread(WatchCurrentConnections);
            watchThread.Start();
        }

        public void Stop()
        {
            if (!_Running) 
                return;

            _Running = false;
            foreach (var client in _Clients)
                client.Disconnect();

            if (_ServerSocket != null && _ServerSocket.IsBound)
                _ServerSocket.Dispose();
        }

        private void WatchNewConnections()
        {
            try
            {
                while (_Running)
                {
                    var clientSocket = _ServerSocket.Accept();
                    _Clients.Add(new ClientManager(clientSocket));
                }
            }
            catch
            {
                Stop();
            }
        }

        private void WatchCurrentConnections()
        {
            do 
            {
                Thread.Sleep(10000);
                for (var i = _Clients.Count - 1; i >= 0; i--)
                {
                    if (!_Clients[i].Connected)
                    {
                        _Clients.RemoveAt(i);
                    }
                }
            }
            while (_Running);
        }

        public void MessagePlayer(String player, MessageCode messageCode, Object messageContent)
        {
            ClientManager clientManager = _Clients.FirstOrDefault(x => x.Player.Equals(player));
            if (clientManager != null)
                clientManager.SendMessage(messageCode, messageContent);
        }

        public override String ToString()
        {
            if (_ServerSocket != null && _ServerSocket.LocalEndPoint != null)
            {
                return "Játékszerver fut a következő helyen: " + _ServerSocket.LocalEndPoint;
            }

            return "A játékszerver jelenleg nem fut.";
        }
    }
}
