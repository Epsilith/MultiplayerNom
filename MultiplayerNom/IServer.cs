namespace MultiplayerNom
{
    /// <summary>
    ///     Represents a multiplayer server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        ///     Gets the rooms in this server.
        /// </summary>
        /// <value>
        ///     The rooms.
        /// </value>
        string[] Rooms { get; }

        /// <summary>
        ///     Adds a room with the given type T and the given room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>The room.</returns>
        T AddRoom<T>(string roomId) where T : class, IRoom, new();

        /// <summary>
        ///     Gets the room with the specified room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>The room.</returns>
        T Get<T>(string roomId) where T : class, IRoom;

        /// <summary>
        ///     Gets the room with the specified room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>The room.</returns>
        T TryGet<T>(string roomId) where T : class, IRoom;

        /// <summary>
        ///     Determines whether this instance contains the specified room identifier.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns><c>true</c> if an instance is found; otherwise, <c>false</c>.</returns>
        bool Contains(string roomId);
    }
}