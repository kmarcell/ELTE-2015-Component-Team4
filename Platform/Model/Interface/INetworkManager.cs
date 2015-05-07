using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.MessageTypes;
using Platform.Events.EventsServerRelated;
using GamesEventArgs = Platform.Events.EventsServerRelated.GamesEventArgs;

namespace Platform.Model.Interface
{
    public interface INetworkManager
    {
        Game CurrentGame { get; }

        Boolean Connected { get; }

        String PlayerName { get; }
        
        event EventHandler<EventArgs> ConnectAcceptedEvent;

        event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;

        event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;

        event EventHandler<EventArgs> DisconnectedEvent;

        event EventHandler<GamesEventArgs> OnlineGamesReceived;

        event EventHandler<EventArgs> GameCreatedEvent;

        event EventHandler<EventArgs> GameJoinRejectedEvent;

        event EventHandler<EventArgs> GameJoinAcceptedEvent;

        event EventHandler<GameEventArgs> GameEndedEvent;

        event EventHandler<GameEventArgs> GameCancelledEvent;

        event EventHandler<GameEventArgs> GameStatusReceived;

        void Connect(String address, Int32 port, String playerName);

        void Disconnect();

        void GetOnlineGames(IGTGameLogicInterface game);

        void CreateGame(IGTGameLogicInterface game, int hashCode);

        void JoinGame(Int32 gameId);

        void SendGameState(Game game);

        void EndGame(String player = null);
    }
}
