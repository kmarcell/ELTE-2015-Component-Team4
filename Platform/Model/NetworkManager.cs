using System;
using System.Net;
using System.Net.Sockets;
using ConnectionInterface.MessageTypes;
using Platform.EventsServerRelated;

namespace Platform.Model
{
    public class NetworkManager : IDisposable
    {
        private const Int32 BufferLength = 20000000;

        private Socket _Socket;

        public Boolean Connected { get { return _Socket != null && _Socket.Connected; } }

        public Player Player { get; private set; }

        public event EventHandler<ConnectionChangeEventArgs> ConnectionChangedEvent;
        public event EventHandler<EventArgs> ConnectAcceptedEvent;
        public event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;
        public event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;
        public event EventHandler<EventArgs> DisconnectedEvent;


        public void Connect(String address, Int32 port, String player)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Player = new Player { Name = player };

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

        public void GetOpenGames(Int32 gameTypeId) 
        {
            SendMessage(MessageCode.GetOpenGames, gameTypeId);
        }


        internal void CreateGame(Game game)
        {
            game.FirstPlayer = Player;
            SendMessage(MessageCode.CreateGame, game);
        }

        internal void JoinGame(Int32 gameId)
        {
            SendMessage(MessageCode.JoinGame, gameId);
        }

        internal void SendGameState(Byte[] state) 
        {
            SendMessage(MessageCode.ChangeGameState, state);
        }

        internal void EndGame(Player player = null)
        {
            SendMessage(MessageCode.EndGame, player);
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

                SendMessage(MessageCode.Login, Player);
            }
            else
            {
                ConnectRejectedServerNotRespondingEvent(this, EventArgs.Empty);
                ConnectionChangedEvent(this, new ConnectionChangeEventArgs { IsConnected = false });
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
                        ConnectionChangedEvent(this, new ConnectionChangeEventArgs { IsConnected = true });
                        break;
                    case MessageCode.ConnectRejected:
                        ConnectRejectedUsernameOccupied(this, EventArgs.Empty);
                        break;
                    case MessageCode.Disconnect:
                        _Socket.Shutdown(SocketShutdown.Both);
                        _Socket.Close(1000);
                        DisconnectedEvent(this, EventArgs.Empty);
                        ConnectionChangedEvent(this, new ConnectionChangeEventArgs { IsConnected = false });
                        break;
                    case MessageCode.ChangeGameState:
                        //GameStatusReceived(this, new GameStateEventArgs { GameState = message.Content as Byte[] });
                        break;
                    case MessageCode.GetOpenGames:
                        //OpenGamesReceived(this, new GamesEventArgs { Games = message.Content as Game[] });
                        break;
                    case MessageCode.CreateGame:
                        //GameCreated(this, new GameEventArgs { Game = message.Content as Game });
                        break;
                    case MessageCode.JoinGame:
                        //GameStarted(this, new GameEventArgs { Game = message.Content as Game });
                        break;
                    case MessageCode.JoinAccepted:
                        //GameStarted(this, new GameEventArgs { Game = message.Content as Game });
                        break;
                    case MessageCode.JoinRejected:
                        //GameJoinFailed(this, EventArgs.Empty);
                        break;
                    case MessageCode.EndGame:
                        //GameEnded(this, new GameEventArgs { Game = message.Content as Game });
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
                ConnectionChangedEvent(this, new ConnectionChangeEventArgs { IsConnected = false });
            }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && e.BytesTransferred > 0)
            {
                ConnectionChangedEvent(this, new ConnectionChangeEventArgs { IsConnected = false });
            }
            e.Dispose();
        }
    }
}
