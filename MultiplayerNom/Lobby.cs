using ByteNom;

namespace MultiplayerNom
{
    /// <summary>
    ///     Provides a default lobby implementation. This class can not be inheirted.
    /// </summary>
    /// <typeparam name="TRoom">The type of the room.</typeparam>
    public sealed class Lobby<TRoom> : LobbyBase<User> where TRoom : class, IRoom, new()
    {
        /// <summary>
        ///     Called when a message is received.
        /// </summary>
        /// <param name="user">The user sending the message.</param>
        /// <param name="message">The received message.</param>
        protected override void OnMessage(User user, Message message)
        {
            if (message.Type == "join")
            {
                if (message.Count < 1) return;
                var roomId = message[0] as string;
                if (roomId != null)
                {
                    // Get or create a room with the given id
                    IRoom room = this.Server.Contains(roomId)
                        ? this.Server.Get<IRoom>(roomId)
                        : this.Server.AddRoom<TRoom>(roomId);

                    user.MoveToRoom(room);
                }
            }
        }
    }
}