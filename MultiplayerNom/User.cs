using System.Net;
using ByteNom;

namespace MultiplayerNom
{
    /// <summary>
    ///     Represents a user inside a room.
    /// </summary>
    public class User
    {
        private UserHandle _handle;

        /// <summary>
        ///     Gets the identifier of this user. (Unique to this server instance)
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Id
        {
            get { return this._handle.UserId; }
        }

        /// <summary>
        /// Gets the remote end point of this user.
        /// </summary>
        /// <value>
        /// The remote end point.
        /// </value>
        public EndPoint EndPoint
        {
            get { return this._handle.Connection.EndPoint; }
        }

        internal void Activate(UserHandle handle)
        {
            this._handle = handle;
        }

        /// <summary>
        ///     Moves this player to the specified room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <returns></returns>
        public bool MoveToRoom(IRoom room)
        {
            return this._handle.MoveToRoom((IRoomInternal)room);
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(Message message)
        {
            this._handle.Connection.Send(message);
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="args">The message arguments.</param>
        public void Send(string type, params object[] args)
        {
            this._handle.Connection.Send(type, args);
        }

        /// <summary>
        ///     Disconnects this user from the server.
        /// </summary>
        public void Disconnect()
        {
            this._handle.Connection.Disconnect();
        }
    }
}