using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    /// <summary>
    /// The ServerManager interface represents a server, it can start, and stop on the configured port number and one free ip address.
    /// In addition it should also check new connections and observer the clients which are already connected.
    /// </summary>
    public interface IServerManager
    {
        #region public fields
        /// <summary>
        /// Get the IP address of the server.
        /// </summary>
        String ServerIp { get; }

        /// <summary>
        /// Get the port number of the server.
        /// </summary>
        Int32 ServerPort { get; }

        /// <summary>
        /// Get the state of the server. True if running and bound, otherwise false.
        /// </summary>
        Boolean Running { get; }
        #endregion


        #region managing server
        /// <summary>
        /// Start of the server in an free IP and port, and start listening for new connections and observer current connection in new thread.
        /// <remarks>
        /// Listening for new connections is implemented in <see cref="WatchNewConnections"/>.
        /// Observe current connections is implemented in <see cref="WatchCurrentConnections"/>.
        /// </remarks>
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the the server, set running to false, dispose socket and inform all the connected client, about the server connection.
        /// </summary>
        void Stop();

        /// <summary>
        /// Watch new connections on the server until the server is running. 
        /// </summary>
        /// <remarks>
        /// If there is a new connection, then add to the client list.
        /// </remarks>
        void WatchNewConnections();

        /// <summary>
        /// Watch current connections on the server until the server is running. 
        /// </summary>
        /// <remarks>
        /// If there is a new connection, then remove from the client list.
        /// </remarks>
        void WatchCurrentConnections();

        /// <summary>
        /// Send a message to the player who is connected to the server.
        /// </summary>
        /// <param name="player">The name of the player who should recieve the message.</param>
        /// <param name="messageCode">The code of message <see cref="MessageCode"/>.</param>
        /// <param name="messageContent">The content of the message corresponfing to the message code.</param>
        void MessagePlayer(String player, MessageCode messageCode, Object messageContent);
        #endregion
    }
}
