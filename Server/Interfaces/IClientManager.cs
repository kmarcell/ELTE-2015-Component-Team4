using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    public interface IClientManager
    {
        String Player { get; }

        Game CurrentGame { get; }

        Boolean Connected { get; }

        void SendConnectionAccepted();

        void SendConnectionRejected();

        void SendOnlineGames(int id);

        void SendCreateGame(String player, Game game);

        void SendJoinGameAccepted(Game game);

        void SendJoinGameRejected();

        void SendGameState(Game game);

        void SendEndGame(String player);

        void SendMessage(MessageCode code, Object content = null);
    }
}
