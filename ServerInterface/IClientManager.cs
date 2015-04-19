using System;
using ConnectionInterface.MessageTypes;

namespace ServerInterface
{
    public interface IClientManager
    {
        Player Player { get; }

        Game CurrentGame { get; }

        Boolean Connected { get; }

        void SendConnectionAccepted();

        void SendConnectionRejected();

        void SendOnlineGames();

        void SendCreateGame(Player player, Game game);

        void SendJoinGameAccepted(Game game);

        void SendJoinGameRejected();

        void SendGameState(Byte[] gameState);

        void SendEndGame(Player player);

        void SendMessage(MessageCode code, Object content = null);
    }
}
