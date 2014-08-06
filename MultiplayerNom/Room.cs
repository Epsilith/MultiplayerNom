using System.Collections.Generic;
using System.Linq;
using ByteNom;

namespace MultiplayerNom
{
    public abstract class Room<TUser> : IRoomInternal where TUser : BaseUser, new()
    {
        private readonly Dictionary<UserHandle, TUser> _users = new Dictionary<UserHandle, TUser>();

        protected TUser[] Users
        {
            get { return this._users.Values.ToArray(); }
        }

        public IServer Server { get; private set; }
        public string RoomId { get; private set; }

        protected int PlayerCount
        {
            get { return this._users.Count; }
        }

        void IRoomInternal.Activate(IServer server, string roomId)
        {
            this.Server = server;
            this.RoomId = roomId;
            this.OnCreate();
        }

        void IRoomInternal.Disactivate()
        {
            this.OnDestroy();
        }

        bool IRoomInternal.AddUser(UserHandle handle)
        {
            var u = new TUser();
            u.Activate(handle);

            if (this.AllowUserJoin(u))
            {
                this._users.Add(handle, u);
                this.OnJoin(u);
                return true;
            }
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
    }
}