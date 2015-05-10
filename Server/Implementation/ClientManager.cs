using System;
using System.Net.Sockets;
using System.Threading;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using Server.Interfaces;

namespace Server.Implementation
{
    /// <summary>
    /// The ClientManager class represents the method to recieve message and reply depending its <see cref="MessageCode"/> to the clients.
    /// </summary>
    public class ClientManager : IDisposable, IClientManager
    {
        #region private fields
        /// <summary>
        /// The length of the Buffer.
        /// </summary>
        private readonly Int32 _MBufferLength;

        /// <summary>
        /// The socket of the server, to recieve messages.
        /// </summary>
        private readonly Socket _MSocket;
        #endregion


        #region public fields
        /// <summary>
        /// The current playerName name.
        /// </summary>
        public String Player { get; private set; }

        /// <summary>
        /// The current game of the client.
        /// </summary>
        public Game CurrentGame { get; private set; }

        /// <summary>
        /// Determine wether the client is connected or not.
        /// </summary>
        public Boolean Connected { get { return _MSocket.Connected; } }
        #endregion


        #region constructor
        /// <summary>
        /// Create a new clientmanager for recievent and sending messages corresponding to <see cref="MessageCode"/>.
        /// </summary>
        /// <param name="socket">Socket where the server should listen to its client.</param>
        public ClientManager(Socket socket)
        {
            _MSocket = socket;
            _MBufferLength = 20000000;
            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }
        #endregion


        #region client related functions
        /// <summary>
        /// Recieve message from the client and reply depending on its <see cref="MessageCode"/>.
        /// </summary>
        public void ReceiveMessage()
        {
            try
            {
                while (_MSocket.Connected)
                {
                    var buffer = new Byte[_MBufferLength];
                    var receivedBytes = _MSocket.Receive(buffer);
                    var message = Message.Deserialize(buffer, receivedBytes);

                    switch (message.Code)
                    {
                        case MessageCode.Login:
                            var playerNameToLogin = (message.Content as String);
                            if (DataManager.DataManagerInstance.LoginPlayer(playerNameToLogin))
                            {
                                Player = playerNameToLogin;
                                SendConnectionAccepted();
                            }
                            else
                                SendConnectionRejected();
                            break;
                        case MessageCode.Disconnect:
                            Disconnect();
                            break;
                        case MessageCode.GetOpenGames:
                            var gameId = Convert.ToInt32(message.Content);
                            SendOnlineGames(gameId);
                            break;
                        case MessageCode.CreateGame:
                            SendCreateGame(Player, message.Content as Game);
                            break;
                        case MessageCode.JoinGame:
                            if (DataManager.DataManagerInstance.JoinGame(Convert.ToInt32(message.Content), Player))
                            {
                                CurrentGame = DataManager.DataManagerInstance.GetGame(Convert.ToInt32(message.Content));
                                SendJoinGameAccepted(CurrentGame);
                            }
                            else
                            {
                                SendJoinGameRejected();
                            }
                            break;
                        case MessageCode.ChangeGameState:
                            SendGameState(message.Content as Game);
                            break;
                        case MessageCode.EndGame:
                            SendEndGame(message.Content as String);
                            break;
                    }
                }
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Send message to the client.
        /// </summary>
        /// <param name="code">The code of the message <see cref="MessageCode"/></param>
        /// <param name="content">The content of the message corresponding its messagecode.</param>
        public void SendMessage(MessageCode code, Object content = null)
        {
            try
            {
                Byte[] buffer = Message.Serialize(code, content);
                if (buffer != null)
                    _MSocket.Send(buffer);
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Send connection accepted message to the client.
        /// </summary>
        public void SendConnectionAccepted()
        {
            SendMessage(MessageCode.ConnectAccepted);
        }

        /// <summary>
        /// Send connection rejected message to the client.
        /// </summary>
        public void SendConnectionRejected()
        {
            SendMessage(MessageCode.ConnectRejected);
        }

        /// <summary>
        /// Send disconnect to the client, close- and dispose socket.
        /// </summary>
        public void Disconnect()
        {
            if (CurrentGame != null && CurrentGame.Phase != GamePhase.Opened)
            {
                DataManager.DataManagerInstance.EndGame(CurrentGame, (Player == CurrentGame.FirstPlayer) ? CurrentGame.SecondPlayer : CurrentGame.FirstPlayer);
            }
            DataManager.DataManagerInstance.LogoutPlayer(Player);

            if (_MSocket == null || !_MSocket.Connected) 
                return;
            
            _MSocket.Shutdown(SocketShutdown.Both);
            _MSocket.Close();
        }

        /// <summary>
        /// Send online games to the client.
        /// </summary>
        /// <param name="id">The id of the game.</param>
        public void SendOnlineGames(Int32 id)
        {
            SendMessage(MessageCode.GetOpenGames, DataManager.DataManagerInstance.GetOpenGames(Player, id));
        }

        /// <summary>
        /// Send create game to the client.
        /// </summary>
        /// <param name="player">The creator playerName.</param>
        /// <param name="game">The created game.</param>
        public void SendCreateGame(String player, Game game)
        {
            CurrentGame = game;
            CurrentGame.FirstPlayer = Player;
            DataManager.DataManagerInstance.CreateGame(CurrentGame);
            SendMessage(MessageCode.CreateGame, CurrentGame);
        }

        /// <summary>
        /// Send join game accepted to the client.
        /// </summary>
        /// <param name="game">The game to join.</param>
        public void SendJoinGameAccepted(Game game)
        {
            SendMessage(MessageCode.JoinAccepted, game);
        }

        /// <summary>
        /// Send join game rejected to the client.
        /// </summary>
        public void SendJoinGameRejected()
        {
            SendMessage(MessageCode.JoinRejected);
        }

        /// <summary>
        /// Send game state to the client.
        /// </summary>
        /// <param name="game">The game with the new state.</param>
        public void SendGameState(Game game)
        {
            if (CurrentGame != null && CurrentGame.Phase == GamePhase.Playing)
            {
                DataManager.DataManagerInstance.ChangeGameState(Player, game);
            }
        }

        /// <summary>
        /// Send end game for every connected clients of the game.
        /// </summary>
        /// <param name="playerName">The name of the winner.</param>
        public void SendEndGame(String playerName)
        {
            if (CurrentGame != null && CurrentGame.Phase != GamePhase.Ended)
            {
                DataManager.DataManagerInstance.EndGame(CurrentGame, Player, playerName);
            }
            CurrentGame = null;
        }

        /// <summary>
        /// Dispose the instance should end the currently played games if any is played by the client and close the socket.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
        #endregion
    }
}
