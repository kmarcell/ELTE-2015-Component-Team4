using System;
using System.Net;
using System.Net.Sockets;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;
using GamesEventArgs = Platform.Events.EventsServerRelated.GamesEventArgs;
using INetworkManager = Platform.Model.Interface.INetworkManager;

namespace Platform.Model
{
    public class NetworkManager : IDisposable, INetworkManager
    {
        #region server related fields
        /// <summary>
        /// The length of the buffer to get the messages.
        /// </summary>
        public Int32 BufferLength { get; private set; }

        /// <summary>
        /// The socket to listen the messages.
        /// </summary>
        public Socket Socket { get; private set; }
        #endregion


        #region online state fields
        /// <summary>
        /// The instance of the currently loaded (selected) GameLogic in the platfrom.
        /// </summary>
        public Game CurrentGame { get; private set; }

        /// <summary>
        /// Property to check if the client is connected to the server.
        /// True if connected, otherwise false.
        /// </summary>
        public Boolean Connected { get { return Socket != null && Socket.Connected; } }

        /// <summary>
        /// The given name of the player, at the login.
        /// </summary>
        public String PlayerName { get; private set; }
        #endregion


        #region constructor
        /// <summary>
        /// Constructor to create new instance of <see cref="NetworkManager"/> and set BufferLength (20000000).
        /// </summary>
        public NetworkManager()
        {
            BufferLength = 20000000;
        }
        #endregion


        #region event handlers
        /// <summary>
        /// The event will raise to inform user when the user is connected to the server.
        /// </summary>
        public event EventHandler<EventArgs> ConnectAcceptedEvent;

        /// <summary>
        /// The event will raise to inform user when the user could not connect to the server due to server not responding error.
        /// </summary>
        public event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;

        /// <summary>
        /// The event will raise to inform user when the user could not connect to the server due to an other user has already connected with the same name.
        /// </summary>
        public event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;

        /// <summary>
        /// The event will raise to inform user when the user is disconnected from the server in any case.
        /// </summary>
        public event EventHandler<EventArgs> DisconnectedEvent;

        /// <summary>
        /// The event will raise to inform user when the user requested the registered open games on the server and get the list of it.
        /// </summary>
        public event EventHandler<GamesEventArgs> OnlineGamesReceived;

        /// <summary>
        /// The event will raise to inform user when the user created a game and it gets registered on the server.
        /// </summary>
        public event EventHandler<EventArgs> GameCreatedEvent;

        /// <summary>
        /// The event will raise to inform user when the user joined to an open game and the server rejected due to game started before with a different user.
        /// </summary>
        public event EventHandler<EventArgs> GameJoinRejectedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game start) when the user joined to an open game and the server accepted.
        /// </summary>
        public event EventHandler<GameEventArgs> GameJoinAcceptedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game end) when the played game has finished.
        /// </summary>
        public event EventHandler<GameEventArgs> GameEndedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game end) when the played game has cancelled.
        /// </summary>
        public event EventHandler<GameEventArgs> GameCancelledEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game state) when the played game status has changed (one player made a step).
        /// </summary>
        public event EventHandler<GameEventArgs> GameStatusReceived;
        #endregion


        #region server-platformUI-gameLogic related functions
        /// <summary>
        /// Connect to the server with the given parameters by the user.
        /// </summary>
        /// <param name="address">The IP address of the server (f.e.: 192.168.86.1).</param>
        /// <param name="port">The port number server where the server listening (f.e.: 5503).</param>
        /// <param name="playerName">The player name to connect to the server.</param>
        public void Connect(String address, Int32 port, String playerName)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            PlayerName = playerName;

            var socketAsyncEventArgs = new SocketAsyncEventArgs
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port)
            };
            socketAsyncEventArgs.Completed += ConnectionCompleted;
            Socket.ConnectAsync(socketAsyncEventArgs);
        }

        /// <summary>
        /// Disconnect from the server by the user.
        /// </summary>
        public void Disconnect()
        {
            if (Socket != null && Socket.Connected)
            {
                SendMessage(MessageCode.Disconnect);
                DisconnectedEvent(this, EventArgs.Empty);

                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close(1000);
            }
        }

        /// <summary>
        /// Get online games request by the user.
        /// </summary>
        /// <remarks>
        /// To send message for server: SendMessage(MessageCode.GetOpenGames, game.Id) and wait for response.
        /// </remarks>
        /// <param name="game">The currently selected game.</param>
        public void GetOnlineGames(IGTGameLogicInterface game) 
        {
            SendMessage(MessageCode.GetOpenGames, game.Id);
        }

        /// <summary>
        /// Create game on server by the user.
        /// </summary>
        /// <remarks>
        /// To send message for server: SendMessage(MessageCode.CreateGame, gameToServer); and wait for response.
        /// IGTGameLogicInterface should be converted to Game as xml message.
        /// </remarks>
        /// <param name="game">The currently selected game.</param>
        public void CreateGame(IGTGameLogicInterface game)
        {
            var gameToServer = new Game
            {
                Id = game.Id, 
                Name = game.Name, 
                Description = game.Description,
                FirstPlayer = PlayerName,
                Phase = GamePhase.Opened
            };

            SendMessage(MessageCode.CreateGame, gameToServer);
        }

        /// <summary>
        /// Join game by the user.
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.JoinGame, gameId) and wait for response.
        /// </remarks>
        /// <param name="gameId">The selected gameId to join.</param>
        public void JoinGame(Int32 gameId)
        {
            SendMessage(MessageCode.JoinGame, gameId);
        }

        /// <summary>
        /// Send game state change as it was an action performed by the user and game logic forwarded.
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.ChangeGameState, game) and wait for response.
        /// </remarks>
        /// <param name="game">The currently played game.</param>
        public void SendGameState(Game game) 
        {
            SendMessage(MessageCode.ChangeGameState, game);
        }

        /// <summary>
        /// End game by the user (Cancel).
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.EndGame, playerName) and wait for response.
        /// </remarks>
        /// <param name="playerName">The player name who ends the game.</param>
        public void EndGame(String playerName = null)
        {
            SendMessage(MessageCode.EndGame, playerName);
        }
        #endregion


        #region server related functions
        /// <summary>
        /// Dispose to disconnect from the server.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// Send serialized message to the server with the given messagecode and content.
        /// </summary>
        /// <param name="code">The code of the message <see cref="MessageCode"/>.</param>
        /// <param name="content">The content of the message.</param>
        public void SendMessage(MessageCode code, Object content = null)
        {
            if (Socket != null && Socket.Connected)
            {
                Byte[] buffer = Message.Serialize(code, content);

                if (buffer != null)
                {
                    var sendAsyncEventArgs = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = Socket.RemoteEndPoint
                    };

                    sendAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
                    sendAsyncEventArgs.Completed += SendCompleted;
                    Socket.SendAsync(sendAsyncEventArgs);
                }
            }
        }

        /// <summary>
        /// The completed event handler of the socket.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to check the connection result.</param>
        public void ConnectionCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var receiveAsyncEventArgs = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = e.RemoteEndPoint
                };

                receiveAsyncEventArgs.SetBuffer(new Byte[BufferLength], 0, BufferLength);
                receiveAsyncEventArgs.Completed += ReceiveCompleted;
                Socket.ReceiveAsync(receiveAsyncEventArgs);

                e.Dispose();

                SendMessage(MessageCode.Login, PlayerName);
            }
            else
            {
                ConnectRejectedServerNotRespondingEvent(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The completed event handler of the socket after connected.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to check the recieveing message result and deserialize.</param>
        public void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                var message = Message.Deserialize(e.Buffer, e.BytesTransferred);

                switch (message.Code)
                {
                    case MessageCode.ConnectAccepted:
                        ConnectAcceptedEvent(this, EventArgs.Empty);
                        break;
                    case MessageCode.ConnectRejected:
                        ConnectRejectedUsernameOccupied(this, EventArgs.Empty);
                        break;
                    case MessageCode.Disconnect:
                        Socket.Shutdown(SocketShutdown.Both);
                        Socket.Close(1000);
                        break;
                    case MessageCode.ChangeGameState:
                        GameStatusReceived(this, new GameEventArgs { Game = message.Content as Game });
                        break;
                    case MessageCode.GetOpenGames:
                        OnlineGamesReceived(this, new GamesEventArgs { Games = message.Content as Game[] });
                        break;
                    case MessageCode.CreateGame:
                        GameCreatedEvent(this, EventArgs.Empty);
                        break;
                    case MessageCode.JoinAccepted:
                        CurrentGame = message.Content as Game;
                        GameStatusReceived(this, new GameEventArgs { Game = CurrentGame });
                        GameJoinAcceptedEvent(this, new GameEventArgs { Game = CurrentGame });
                        break;
                    case MessageCode.JoinRejected:
                        GameJoinRejectedEvent(this, EventArgs.Empty);
                        break;
                    case MessageCode.EndGame:
                        var game = message.Content as Game;
                        if (game != null && game.Phase == GamePhase.Ended)
                        {
                            GameStatusReceived(this, new GameEventArgs { Game = game });
                            GameEndedEvent(this, new GameEventArgs {Game = game});
                        }

                        else
                        {
                            if (game != null)
                            {
                                game.Phase = GamePhase.Ended;
                                GameStatusReceived(this, new GameEventArgs { Game = game });
                                GameCancelledEvent(this, new GameEventArgs { Game = game });
                            }
                        }
                        break;
                }

                e.Dispose();
                var receiveAsyncEventArgs = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = e.RemoteEndPoint
                };
                receiveAsyncEventArgs.SetBuffer(new Byte[BufferLength], 0, BufferLength);
                receiveAsyncEventArgs.Completed += ReceiveCompleted;
                Socket.ReceiveAsync(receiveAsyncEventArgs);
            }
            else
            {
                DisconnectedEvent(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Check send completed state. If not completed disconnect event should raised.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to determine complete state.</param>
        public void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && e.BytesTransferred > 0)
            {
                DisconnectedEvent(this, EventArgs.Empty);
            }
            e.Dispose();
        }
        #endregion
    }
}
