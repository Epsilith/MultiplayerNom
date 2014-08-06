using System.Collections.Generic;
using System.Linq;
using ByteNom;

namespace MultiplayerNom
{
    public abstract class Room<TUser> : IRoomInternal where TUser : User, new()
    {
        private readonly Dictionary<UserHandle, TUser> _users = new Dictionary<UserHandle, TUser>();
        private IServerInternal _server;

        public bool Closed { get; private set; }

        protected TUser[] Users
        {
            get { return this._users.Values.ToArray(); }
        }

        public IServer Server
        {
            get { return this._server; }
        }

        public string RoomId { get; private set; }

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

                if (this.UserCount == 0 && this.OnLastUserLeave())
                {
                    this.Close();
                }
            }
        }

        protected virtual bool AllowUserJoin(TUser user)
        {
            return true;
        }

        protected virtual void OnJoin(TUser user)
        {
        }

        protected virtual void OnMessage(TUser user, Message message)
        {
        }

        protected virtual void OnLeave(TUser user)
        {
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual bool OnLastUserLeave()
        {
            return true;
        }

        protected void Broadcast(string type, params object[] args)
        {
            this.Broadcast(new Message(type, args));
        }

        protected void Broadcast(Message message)
        {
            foreach (TUser user in this.Users)
            {
                user.Send(message);
            }
        }

        protected void Close()
        {
            ((IRoomInternal)this).Disactivate();
        }
    }
}