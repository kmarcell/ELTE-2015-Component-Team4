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
        private const Int32 BufferLength = 20000000;

        private Socket _Socket;

        public Boolean Connected { get { return _Socket != null && _Socket.Connected; } }

        public String PlayerName { get; private set; }

        public event EventHandler<EventArgs> ConnectAcceptedEvent;
        public event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;
        public event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;
        public event EventHandler<EventArgs> DisconnectedEvent;
        public event EventHandler<GamesEventArgs> OnlineGamesReceived;
        public event EventHandler<EventArgs> GameCreatedEvent;
        public event EventHandler<EventArgs> GameJoinRejectedEvent;
        public event EventHandler<EventArgs> GameJoinAcceptedEvent;
        public event EventHandler<GameEventArgs> GameEndedEvent;
        public event EventHandler<GameEventArgs> GameCancelledEvent;
        public event EventHandler<GameEventArgs> GameStatusReceived;


        public void Connect(String address, Int32 port, String playerName)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            PlayerName = playerName;

            var socketAsyncEventArgs = new SocketAsyncEventArgs
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port)
            };
            socketAsyncEventArgs.Completed += Connection_Completed;
            _Socket.ConnectAsync(socketAsyncEventArgs);
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (_Socket != null && _Socket.Connected)
            {
                SendMessage(MessageCode.Disconnect);
                DisconnectedEvent(this, EventArgs.Empty);

                _Socket.Shutdown(SocketShutdown.Both);
                _Socket.Close(1000);
            }
        }

        public void GetOnlineGames(GTGameLogicInterface<GTGameSpaceElementInterface, IPosition> game) 
        {
            SendMessage(MessageCode.GetOpenGames, game.Id);
        }


        public void CreateGame(GTGameLogicInterface<GTGameSpaceElementInterface, IPosition> game, int hashCode)
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

        public void JoinGame(Int32 gameId)
        {
            SendMessage(MessageCode.JoinGame, gameId);
        }

        public void SendGameState(Game game) 
        {
            SendMessage(MessageCode.ChangeGameState, game);
        }

        public void EndGame(String playerName = null)
        {
            SendMessage(MessageCode.EndGame, playerName);
        }

        private void SendMessage(MessageCode code, Object content = null)
        {
            if (_Socket != null && _Socket.Connected)
            {
                Byte[] buffer = Message.Serialize(code, content);

                if (buffer != null)
                {
                    var sendAsyncEventArgs = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = _Socket.RemoteEndPoint
                    };

                    sendAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
                    sendAsyncEventArgs.Completed += Send_Completed;
                    _Socket.SendAsync(sendAsyncEventArgs);
                }
            }
        }

        private void Connection_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var receiveAsyncEventArgs = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = e.RemoteEndPoint
                };

                receiveAsyncEventArgs.SetBuffer(new Byte[BufferLength], 0, BufferLength);
                receiveAsyncEventArgs.Completed += Receive_Completed;
                _Socket.ReceiveAsync(receiveAsyncEventArgs);

                e.Dispose();

                SendMessage(MessageCode.Login, PlayerName);
            }
            else
            {
                ConnectRejectedServerNotRespondingEvent(this, EventArgs.Empty);
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
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
                        _Socket.Shutdown(SocketShutdown.Both);
                        _Socket.Close(1000);
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
                        GameJoinAcceptedEvent(this, new GameEventArgs { Game = message.Content as Game });
                        //TODO
                        //game started event
                        break;
                    case MessageCode.JoinRejected:
                        GameJoinRejectedEvent(this, EventArgs.Empty);
                        break;
                    case MessageCode.EndGame:
                        var game = message.Content as Game;
                        if(game != null && game.Phase == GamePhase.Ended)
                            GameEndedEvent(this, new GameEventArgs { Game = game });
                        else
                            GameCancelledEvent(this, new GameEventArgs { Game = game });
                        break;
                }

                e.Dispose();
                var receiveAsyncEventArgs = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = e.RemoteEndPoint
                };
                receiveAsyncEventArgs.SetBuffer(new Byte[BufferLength], 0, BufferLength);
                receiveAsyncEventArgs.Completed += Receive_Completed;
                _Socket.ReceiveAsync(receiveAsyncEventArgs);
            }
            else
            {
                DisconnectedEvent(this, EventArgs.Empty);
            }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && e.BytesTransferred > 0)
            {
                DisconnectedEvent(this, EventArgs.Empty);
            }
            e.Dispose();
        }
    }
}
