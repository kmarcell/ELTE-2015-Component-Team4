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
    /// <summary>
    /// The ServerManager class represents a server, it can start, and stop on the configured port number and one free ip address.
    /// In addition it should also check new connections and observer the clients which are already connected.
    /// </summary>
    public class ServerManager : IDisposable, IServerManager
    {
        #region private fields
        /// <summary>
        /// The port number of the server.
        /// </summary>
        private Int32 _MServerPort;
        
        /// <summary>
        /// The IP address of the server.
        /// </summary>
        private String _MServerIp;
        
        /// <summary>
        /// The current instance of ServerManager for singleton model.
        /// </summary>
        private static ServerManager _mServerManagerInstance;

        /// <summary>
        /// The server socket.
        /// </summary>
        private Socket _MServerSocket;

        /// <summary>
        /// The flag which determine of the state of the server, true if running otherwise false.
        /// </summary>
        private Boolean _MRunning;

        /// <summary>
        /// The list of connected clients in the server.
        /// </summary>
        private readonly List<ClientManager> _MClients;
        #endregion


        #region public fields
        /// <summary>
        /// Get the IP address of the server.
        /// </summary>
        public String ServerIp
        {
            get { return _MServerIp; }
        }

        /// <summary>
        /// Get the port number of the server.
        /// </summary>
        public Int32 ServerPort
        {
            get { return _MServerPort; }
        }

        /// <summary>
        /// Get the state of the server. True if running and bound, otherwise false.
        /// </summary>
        public Boolean Running
        {
            get { return _MServerSocket != null && _MServerSocket.IsBound; }
        }
        #endregion


        #region constructor
        /// <summary>
        /// Create new instance of <see cref="ServerManager"/>.
        /// </summary>
        private ServerManager()
        {
            _MClients = new List<ClientManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ServerManager ServerManagerInstance
        {
            get
            {
                return _mServerManagerInstance ?? (_mServerManagerInstance = new ServerManager());
            }
        }
        #endregion


        #region managing server
        /// <summary>
        /// Dispose the instance of <see cref="ServerManager"/> should Stop the server.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Start of the server in an free IP and port, and start listening for new connections and observer current connection in new thread.
        /// <remarks>
        /// Listening for new connections is implemented in <see cref="WatchNewConnections"/>.
        /// Observe current connections is implemented in <see cref="WatchCurrentConnections"/>.
        /// </remarks>
        /// </summary>
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

                _MServerIp = addressList[0].ToString();
                _MServerPort = 5503;
                _MServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _MServerSocket.Bind(new IPEndPoint(addressList[0], _MServerPort));
                _MServerSocket.Listen(10);
                _MRunning = true;
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

        /// <summary>
        /// Stop the the server, set running to false, dispose socket and inform all the connected client, about the server connection.
        /// </summary>
        public void Stop()
        {
            if (!_MRunning) 
                return;

            _MRunning = false;
            foreach (var client in _MClients)
                client.Disconnect();

            if (_MServerSocket != null && _MServerSocket.IsBound)
                _MServerSocket.Dispose();
        }

        /// <summary>
        /// Watch new connections on the server until the server is running. 
        /// </summary>
        /// <remarks>
        /// If there is a new connection, then add to the <see cref="_MClients"/>.
        /// </remarks>
        public void WatchNewConnections()
        {
            try
            {
                while (_MRunning)
                {
                    var clientSocket = _MServerSocket.Accept();
                    _MClients.Add(new ClientManager(clientSocket));
                }
            }
            catch
            {
                Stop();
            }
        }

        /// <summary>
        /// Watch current connections on the server until the server is running. 
        /// </summary>
        /// <remarks>
        /// If there is a lost connection, then remove from the <see cref="_MClients"/>.
        /// </remarks>
        public void WatchCurrentConnections()
        {
            do 
            {
                Thread.Sleep(10000);
                for (var i = _MClients.Count - 1; i >= 0; i--)
                {
                    if (!_MClients[i].Connected)
                    {
                        _MClients.RemoveAt(i);
                    }
                }
            }
            while (_MRunning);
        }

        /// <summary>
        /// Send a message to the player who is connected to the server.
        /// </summary>
        /// <param name="player">The name of the player who should recieve the message.</param>
        /// <param name="messageCode">The code of message <see cref="MessageCode"/>.</param>
        /// <param name="messageContent">The content of the message corresponfing to the message code.</param>
        public void MessagePlayer(String player, MessageCode messageCode, Object messageContent)
        {
            ClientManager clientManager = _MClients.FirstOrDefault(x => x.Player.Equals(player));
            if (clientManager != null)
                clientManager.SendMessage(messageCode, messageContent);
        }
        #endregion
    }
}
