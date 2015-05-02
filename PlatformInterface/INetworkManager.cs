using System;
using ConnectionInterface;
using ConnectionInterface.MessageTypes;
using PlatformInterface.EventsServerRelated;

namespace PlatformInterface
{
    public interface INetworkManager
    {
        Boolean Connected { get; }

        String PlayerName { get; }
        
        event EventHandler<EventArgs> ConnectAcceptedEvent;

        event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;

        event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;

        event EventHandler<EventArgs> DisconnectedEvent;

        event EventHandler<GamesEventArgs> OnlineGamesReceived;

        event EventHandler<EventArgs> GameCreatedEvent;

        void Connect(String address, Int32 port, String playerName);

        void Disconnect();

        void GetOnlineGames(IGame game);

        void CreateGame(IGame game, int hashCode);

        void JoinGame(Int32 gameId);

        void SendGameState(Game game);

        void EndGame(String player = null);
    }
}
