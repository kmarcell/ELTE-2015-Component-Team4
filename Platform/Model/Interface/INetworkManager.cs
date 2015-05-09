using System;
using System.Net.Sockets;
using GTInterfacesLibrary;
using GTInterfacesLibrary.MessageTypes;
using Platform.Events.EventsServerRelated;
using GamesEventArgs = Platform.Events.EventsServerRelated.GamesEventArgs;

namespace Platform.Model.Interface
{
    public interface INetworkManager
    {
        #region server related fields
        /// <summary>
        /// The length of the buffer to get the messages.
        /// </summary>
        Int32 BufferLength { get; }

        /// <summary>
        /// The socket to listen the messages.
        /// </summary>
        Socket Socket { get; }
        #endregion


        #region online state fields
        /// <summary>
        /// The instance of the currently loaded (selected) GameLogic in the platfrom.
        /// </summary>
        Game CurrentGame { get; }

        /// <summary>
        /// Property to check if the client is connected to the server.
        /// True if connected, otherwise false.
        /// </summary>
        Boolean Connected { get; }

        /// <summary>
        /// The given name of the player, at the login.
        /// </summary>
        String PlayerName { get; }
        #endregion


        #region event handlers
        /// <summary>
        /// The event will raise to inform user when the user is connected to the server.
        /// </summary>
        event EventHandler<EventArgs> ConnectAcceptedEvent;
        
        /// <summary>
        /// The event will raise to inform user when the user could not connect to the server due to server not responding error.
        /// </summary>
        event EventHandler<EventArgs> ConnectRejectedServerNotRespondingEvent;
        
        /// <summary>
        /// The event will raise to inform user when the user could not connect to the server due to an other user has already connected with the same name.
        /// </summary>
        event EventHandler<EventArgs> ConnectRejectedUsernameOccupied;

        /// <summary>
        /// The event will raise to inform user when the user is disconnected from the server in any case.
        /// </summary>
        event EventHandler<EventArgs> DisconnectedEvent;

        /// <summary>
        /// The event will raise to inform user when the user requested the registered open games on the server and get the list of it.
        /// </summary>
        event EventHandler<GamesEventArgs> OnlineGamesReceived;

        /// <summary>
        /// The event will raise to inform user when the user created a game and it gets registered on the server.
        /// </summary>
        event EventHandler<EventArgs> GameCreatedEvent;

        /// <summary>
        /// The event will raise to inform user when the user joined to an open game and the server rejected due to game started before with a different user.
        /// </summary>
        event EventHandler<EventArgs> GameJoinRejectedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game start) when the user joined to an open game and the server accepted.
        /// </summary>
        event EventHandler<GameEventArgs> GameJoinAcceptedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game end) when the played game has finished.
        /// </summary>
        event EventHandler<GameEventArgs> GameEndedEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game end) when the played game has cancelled.
        /// </summary>
        event EventHandler<GameEventArgs> GameCancelledEvent;

        /// <summary>
        /// The event will raise to inform user and game logic (game state) when the played game status has changed (one player made a step).
        /// </summary>
        event EventHandler<GameEventArgs> GameStatusReceived;
        #endregion


        #region Server-PlatformUI-GameLogic related functions
        /// <summary>
        /// Connect to the server with the given parameters by the user.
        /// </summary>
        /// <param name="address">The IP address of the server (f.e.: 192.168.86.1).</param>
        /// <param name="port">The port number server where the server listening (f.e.: 5503).</param>
        /// <param name="playerName">The player name to connect to the server.</param>
        void Connect(String address, Int32 port, String playerName);

        /// <summary>
        /// Disconnect from the server by the user.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Get online games request by the user.
        /// </summary>
        /// <remarks>
        /// To send message for server: SendMessage(MessageCode.GetOpenGames, game.TypeId) and wait for response.
        /// </remarks>
        /// <param name="game">The currently selected game.</param>
        void GetOnlineGames(GTGameInterface game);

        /// <summary>
        /// Create game on server by the user.
        /// </summary>
        /// <remarks>
        /// To send message for server: SendMessage(MessageCode.CreateGame, gameToServer); and wait for response.
        /// IGTGameLogicInterface should be converted to Game as xml message.
        /// </remarks>
        /// <param name="game">The currently selected game.</param>
        void CreateGame(GTGameInterface game);

        /// <summary>
        /// Join game by the user.
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.JoinGame, gameId) and wait for response.
        /// </remarks>
        /// <param name="gameId">The selected gameId to join.</param>
        void JoinGame(Int32 gameId);

        /// <summary>
        /// Send game state change as it was an action performed by the user and game logic forwarded.
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.ChangeGameState, game) and wait for response.
        /// </remarks>
        /// <param name="game">The currently played game.</param>
        void SendGameState(Game game);

        /// <summary>
        /// End game by the user (Cancel).
        /// </summary>
        /// <remarks>
        /// To send message to the server SendMessage(MessageCode.EndGame, playerName) and wait for response.
        /// </remarks>
        /// <param name="playerName">The player name who ends the game.</param>
        void EndGame(String playerName = null);
        #endregion


        #region server related functions
        /// <summary>
        /// Send message to the server with the given messagecode and content.
        /// </summary>
        /// <param name="code">The code of the message <see cref="MessageCode"/>.</param>
        /// <param name="content">The content of the message.</param>
        void SendMessage(MessageCode code, Object content = null);

        /// <summary>
        /// The connection completed event handler of the socket.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to check the connection result.</param>
        void ConnectionCompleted(object sender, SocketAsyncEventArgs e);

        /// <summary>
        /// The completed event handler of the socket after connected.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to check the recieveing message result and deserialize.</param>
        void ReceiveCompleted(object sender, SocketAsyncEventArgs e);

        /// <summary>
        /// Check send completed state. If not completed disconnect event should raised.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args to determine complete state.</param>
        void SendCompleted(object sender, SocketAsyncEventArgs e);
        #endregion
    }
}
