using System;
using System.Net.Sockets;
using System.Threading;
using ConnectionInterface.GameEvents;
using ConnectionInterface.MessageTypes;
using ServerInterface;

namespace Server
{
    public class ClientManager : IDisposable, IClientManager
    {
        private const Int32 BufferLength = 20000000;
        private readonly Socket _Socket;

        public String Player { get; private set; }

        public Game CurrentGame { get; private set; }

        public Boolean Connected { get { return _Socket.Connected; } }

        public ClientManager(Socket socket)
        {
            _Socket = socket;
            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }

        private void ReceiveMessage()
        {
            try
            {
                while (_Socket.Connected)
                {
                    var buffer = new Byte[BufferLength];
                    var receivedBytes = _Socket.Receive(buffer);
                    var message = Message.Deserialize(buffer, receivedBytes);

                    switch (message.Code)
                    {
                        case MessageCode.Login:
                            var playerNameToLogin = (message.Content as String);
                            Player = DataManager.DataManagerInstance.LoginPlayer(playerNameToLogin);
                            if (Player != null)
                                SendConnectionAccepted();
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
                            SendGameState(message.Content as Byte[]);
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

        public void SendConnectionAccepted()
        {
            SendMessage(MessageCode.ConnectAccepted);
        }

        public void SendConnectionRejected()
        {
            SendMessage(MessageCode.ConnectAccepted);
        }

        public void Disconnect()
        {
            if (CurrentGame != null && CurrentGame.Phase != GamePhase.Opened)
            {
                DataManager.DataManagerInstance.EndGame(CurrentGame, (Player == CurrentGame.FirstPlayer) ? CurrentGame.SecondPlayer : CurrentGame.FirstPlayer);
            }
            DataManager.DataManagerInstance.LogoutPlayer(Player);

            if (_Socket == null || !_Socket.Connected) 
                return;
            
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
        }

        public void SendOnlineGames(int id)
        {
            SendMessage(MessageCode.GetOpenGames, DataManager.DataManagerInstance.GetOpenGames(Player, id));
        }

        public void SendCreateGame(String player, Game game)
        {
            CurrentGame = game;
            CurrentGame.FirstPlayer = Player;
            DataManager.DataManagerInstance.CreateGame(CurrentGame);
            SendMessage(MessageCode.CreateGame, CurrentGame);
        }

        public void SendJoinGameAccepted(Game game)
        {
            SendMessage(MessageCode.JoinAccepted, game);
        }

        public void SendJoinGameRejected()
        {
            SendMessage(MessageCode.JoinRejected);
        }

        public void SendGameState(Byte[] gameState)
        {
            if (CurrentGame != null && CurrentGame.Phase == GamePhase.Playing)
            {
                DataManager.DataManagerInstance.ChangeGameState(Player, CurrentGame, gameState);
            }
        }

        public void SendEndGame(String player)
        {
            if (CurrentGame != null && CurrentGame.Phase == GamePhase.Playing)
            {
                DataManager.DataManagerInstance.EndGame(CurrentGame, Player, player);
            }
            CurrentGame = null;
        }

        public void SendMessage(MessageCode code, Object content = null)
        {
            try
            {
                Byte[] buffer = Message.Serialize(code, content);
                if (buffer != null)
                    _Socket.Send(buffer);
            }
            catch
            {
                Disconnect();
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
