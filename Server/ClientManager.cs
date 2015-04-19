using System;
using System.Net.Sockets;
using System.Threading;
using Server.Utilities;

namespace Server
{
    public class ClientManager : IDisposable
    {
        private const Int32 BufferLength = 20000000;
        private readonly Socket _Socket;

        public Player Player { get; private set; }

        public Game CurrentGame { get; private set; }

        public Boolean Connected { get { return _Socket.Connected; } }

        public ClientManager(Socket socket)
        {
            _Socket = socket;
            var receiveThread = new Thread(CommunicateWithClient);
            receiveThread.Start();
        }

        private void CommunicateWithClient()
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
                            var playerToLogin = (message.Content as Player);
                            Player = DataManager.DataManagerInstance.LoginPlayer(playerToLogin.Name);
                            if (Player != null)
                                SendMessage(MessageCode.ConnectAccepted);
                            else
                                SendMessage(MessageCode.ConnectRejected);
                            break;
                        case MessageCode.Disconnect:
                            Disconnect();
                            break;
                        case MessageCode.GetGames:
                            SendMessage(MessageCode.GetGames, DataManager.DataManagerInstance.OnlineGames);
                            break;
                        case MessageCode.GetOpenGames:
                            SendMessage(MessageCode.GetOpenGames, DataManager.DataManagerInstance.GetOpenGames(Player, Convert.ToInt32(message.Content)));
                            break;
                        case MessageCode.GetOnlinePlayers:
                            SendMessage(MessageCode.GetOnlinePlayers, DataManager.DataManagerInstance.OnlinePlayers);
                            break;
                        case MessageCode.CreateGame:
                            CurrentGame = message.Content as Game;
                            CurrentGame.FirstPlayer = Player;
                            DataManager.DataManagerInstance.CreateGame(CurrentGame);
                            SendMessage(MessageCode.CreateGame, CurrentGame);
                            break;
                        case MessageCode.JoinGame:
                            if (DataManager.DataManagerInstance.JoinGame(Convert.ToInt32(message.Content), Player))
                            {
                                CurrentGame = DataManager.DataManagerInstance.GetGame(Convert.ToInt32(message.Content));
                                SendMessage(MessageCode.JoinAccepted, CurrentGame);
                            }
                            else
                            {
                                SendMessage(MessageCode.JoinRejected);
                            }
                            break;
                        case MessageCode.ChangeGameState:
                            if (CurrentGame != null && CurrentGame.Phase == GamePhase.Playing)
                            {
                                DataManager.DataManagerInstance.ChangeGameState(Player, CurrentGame, message.Content as Byte[]);
                            }
                            break;
                        case MessageCode.EndGame:
                            if (CurrentGame != null && CurrentGame.Phase == GamePhase.Playing)
                            {
                                DataManager.DataManagerInstance.EndGame(CurrentGame, Player, message.Content as Player);
                            }
                            CurrentGame = null;
                            break;
                    }
                }
            }
            catch
            {
                Disconnect();
            }
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

        public void Disconnect()
        {
            if (CurrentGame != null && CurrentGame.Phase != GamePhase.Completed)
            {
                DataManager.DataManagerInstance.EndGame(CurrentGame, (Player == CurrentGame.FirstPlayer) ? CurrentGame.SecondPlayer : CurrentGame.FirstPlayer);
            }
            DataManager.DataManagerInstance.LogoutPlayer(Player);

            if (_Socket == null || !_Socket.Connected) 
                return;
            
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
