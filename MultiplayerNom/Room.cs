using System.Collections.Generic;
using System.Linq;
using ByteNom;

namespace MultiplayerNom
{
    /// <summary>
    ///     Represents a room.
    /// </summary>
    /// <typeparam name="TUser">The user class used for users in this room.</typeparam>
    public abstract class Room<TUser> : IRoomInternal where TUser : User, new()
    {
        private readonly Dictionary<UserHandle, TUser> _users = new Dictionary<UserHandle, TUser>();
        private IServerInternal _server;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Room{TUser}" /> is closed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if closed; otherwise, <c>false</c>.
        /// </value>
        public bool Closed { get; private set; }

        /// <summary>
        ///     Gets the users in this room.
        /// </summary>
        /// <value>
        ///     The users.
        /// </value>
        protected TUser[] Users
        {
            get { return this._users.Values.ToArray(); }
        }

        /// <summary>
        ///     Gets the server object for this instance.
        /// </summary>
        /// <value>
        ///     The server.
        /// </value>
        public IServer Server
        {
            get { return this._server; }
        }

        /// <summary>
        ///     Gets the room identifier.
        /// </summary>
        /// <value>
        ///     The room identifier.
        /// </value>
        public string RoomId { get; private set; }

        /// <summary>
        ///     Gets the user count.
        /// </summary>
        /// <value>
        ///     The user count.
        /// </value>
        protected int UserCount
        {
            get { return this._users.Count; }
        }

        void IRoomInternal.Activate(IServerInternal server, string roomId)
        {
            this._server = server;
            this.RoomId = roomId;
            this.OnCreate();
        }

        void IRoomInternal.Disactivate()
        {
            this.Closed = true;
            // Kick everyone out of the room!
            foreach (TUser user in this.Users)
            {
                user.Disconnect();
            }
            this.OnDestroy();
            this._server.Remove(this.RoomId);
        }

        bool IRoomInternal.AddUser(UserHandle handle)
        {
            var u = new TUser();
            u.Activate(handle);

            if (!this.Closed && this.AllowUserJoin(u))
            {
                this._users.Add(handle, u);
                this.OnJoin(u);
                return true;
            }
            this.CheckLastLeave();
            u.Send("joinDenied");
            return false;
        }

        void IRoomInternal.HandleMessage(UserHandle handle, Message message)
        {
            TUser u;
            if (this._users.TryGetValue(handle, out u))
            {
                this.OnMessage(u, message);
            }
        }

        void IRoomInternal.RemoveUser(UserHandle handle)
        {
            TUser u;
            if (this._users.TryGetValue(handle, out u))
            {
                this._users.Remove(handle);
                this.OnLeave(u);
                this.CheckLastLeave();
            }
        }

        private void CheckLastLeave()
        {
            if (this.UserCount == 0 && this.OnLastUserLeave())
            {
                this.Close();
            }
        }

        /// <summary>
        ///     Called when a user tries to join the room.
        ///     Override this method to choose whether a user is allowed to join a room or not.
        ///     Allows all players to join by default.
        /// </summary>
        /// <param name="user">The user trying to join.</param>
        /// <returns><c>true</c> if the user is allowed to join; otherwise, <c>false</c>.</returns>
        protected virtual bool AllowUserJoin(TUser user)
        {
            return true;
        }

        /// <summary>
        ///     Called when a user joins the room.
        ///     Override this method to perform the needed tasks whenever a user joins the room.
        /// </summary>
        /// <param name="user">The user.</param>
        protected virtual void OnJoin(TUser user)
        {
        }

        /// <summary>
        ///     Called when a message is received from a user in this room.
        ///     Override this method to handle messages.
        /// </summary>
        /// <param name="user">The user sending the message.</param>
        /// <param name="message">The received message.</param>
        protected virtual void OnMessage(TUser user, Message message)
        {
        }

        /// <summary>
        ///     Called when a user leaves the room.
        ///     Override this method to perform the needed tasks whenever a user leaves the room.
        /// </summary>
        /// <param name="user">The user.</param>
        protected virtual void OnLeave(TUser user)
        {
        }

        /// <summary>
        ///     Called when the room is created. (This method is called on server startup for lobby rooms, and for other rooms,
        ///     before the first player joins.)
        ///     Override this method to set things up before users join.
        /// </summary>
        protected virtual void OnCreate()
        {
        }

        /// <summary>
        ///     Called when the room is closed.
        ///     Override this method to clean things up.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        ///     Called when the last user in the room leaves.
        ///     Override this method to decide whether the room should close.
        ///     Always returns true by default.
        /// </summary>
        /// <returns><c>true</c> if the room should close; otherwise, <c>false</c>.</returns>
        protected virtual bool OnLastUserLeave()
        {
            return true;
        }

        /// <summary>
        ///     Broadcasts the specified message to all players in the room.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="args">The message arguments.</param>
        protected void Broadcast(string type, params object[] args)
        {
            this.Broadcast(new Message(type, args));
        }

        /// <summary>
        ///     Broadcasts the specified message to all players in the room.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Broadcast(Message message)
        {
            foreach (TUser user in this.Users)
            {
                user.Send(message);
            }
        }

        /// <summary>
        ///     Closes this room. (It is illegal to call Close() in a lobby room)
        /// </summary>
        protected void Close()
        {
            ((IRoomInternal)this).Disactivate();
        }
    }
}