using System;
using ConnectionInterface.MessageTypes;

namespace ServerInterface
{
    public interface IClientManager
    {
        String Player { get; }

        Game CurrentGame { get; }

        Boolean Connected { get; }

        void SendConnectionAccepted();

        void SendConnectionRejected();

        void SendOnlineGames(int gameTypeHashCode);

        void SendCreateGame(String player, Game game);

        void SendJoinGameAccepted(Game game);

        void SendJoinGameRejected();

        void SendGameState(Byte[] gameState);

        void SendEndGame(String player);

        void SendMessage(MessageCode code, Object content = null);
    }
}
