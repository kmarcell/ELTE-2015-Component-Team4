using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    /// <summary>
    /// The IClientManager interface represents the method to recieve message and reply depending its <see cref="MessageCode"/> to the clients.
    /// </summary>
    public interface IClientManager
    {
        #region public fields
        /// <summary>
        /// The current playerName name.
        /// </summary>
        String Player { get; }

        /// <summary>
        /// The current game of the client.
        /// </summary>
        Game CurrentGame { get; }

        /// <summary>
        /// Determine wether the client is connected or not.
        /// </summary>
        Boolean Connected { get; }
        #endregion


        #region client related functions
        /// <summary>
        /// Recieve message from the client and reply depending on its <see cref="MessageCode"/>.
        /// </summary>
        void ReceiveMessage();

        /// <summary>
        /// Send message to the client.
        /// </summary>
        /// <param name="code">The code of the message <see cref="MessageCode"/></param>
        /// <param name="content">The content of the message corresponding its messagecode.</param>
        void SendMessage(MessageCode code, Object content = null);

        /// <summary>
        /// Send connection accepted message to the client.
        /// </summary>
        void SendConnectionAccepted();

        /// <summary>
        /// Send connection rejected message to the client.
        /// </summary>
        void SendConnectionRejected();

        /// <summary>
        /// End game if any is running with the client, close- and dispose socket.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Send online games to the client.
        /// </summary>
        /// <param name="id">The id of the game.</param>
        void SendOnlineGames(Int32 id);

        /// <summary>
        /// Send create game to the client.
        /// </summary>
        /// <param name="player">The creator playerName.</param>
        /// <param name="game">The created game.</param>
        void SendCreateGame(String player, Game game);

        /// <summary>
        /// Send join game accepted to the client.
        /// </summary>
        /// <param name="game">The game to join.</param>
        void SendJoinGameAccepted(Game game);

        /// <summary>
        /// Send join game rejected to the client.
        /// </summary>
        void SendJoinGameRejected();

        /// <summary>
        /// Send game state to the client.
        /// </summary>
        /// <param name="game">The game with the new state.</param>
        void SendGameState(Game game);

        /// <summary>
        /// Send end game for every connected clients of the game.
        /// </summary>
        /// <param name="playerName">The name of the winner.</param>
        void SendEndGame(String playerName);
        #endregion
    }
}
