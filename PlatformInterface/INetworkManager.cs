using System;
using ConnectionInterface.MessageTypes;
using PlatformInterface.EventsServerRelated;

namespace PlatformInterface
{
    public interface INetworkManager
    {
        Boolean Connected { get; }

        Player Player { get; }

        event EventHandler<ConnectionChangeEventArgs> ConnectionChangedEvent;

        event EventHandler<EventArgs> ConnectAcceptedEvent;

        event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;

        event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;

        event EventHandler<EventArgs> DisconnectedEvent;

        event EventHandler<GamesEventArgs> OnlineGamesReceived;

        event EventHandler<EventArgs> GameCreatedEvent;

        void Connect(String address, Int32 port, String player);

        void Disconnect();

        void GetOnlineGames(Int32 gameTypeId);

        void CreateGame(Game game);

        void JoinGame(Int32 gameId);

        void SendGameState(Byte[] state);

        void EndGame(Player player = null);

    }
}
