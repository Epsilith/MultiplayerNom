using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ByteNom;

namespace MultiplayerNom
{
    public class MultiplayerServer<TLobby> : Server, IServer where TLobby : class, IRoom, new()
    {
        private readonly Dictionary<string, IRoomInternal> _rooms = new Dictionary<string, IRoomInternal>();
        private readonly UserManager _userManager = new UserManager();
        private IRoomInternal _lobbyRoomInternal;

        public MultiplayerServer(int port)
            : base(port)
        {
            this.Init();
        }

        public MultiplayerServer(IPAddress ipAddress, int port)
            : base(ipAddress, port)
        {
            this.Init();
        }

        private void Init()
        {
            this._lobbyRoomInternal = this.AddRoomInternal<TLobby>("Lobby");

            this.ConnectionReceived += (sender, connection) =>
            {
                this._userManager.RegisterUser(connection, this, this._lobbyRoomInternal);
            };
        }

        public TLobby Lobby
        {
            get { return this.Get<TLobby>("Lobby"); }
        }


        public string[] Rooms {
            get { return this._rooms.Keys.ToArray(); }
        }

        public T AddRoom<T>(string roomId) where T : class, IRoom, new()
        {
            return (T)this.AddRoomInternal<T>(roomId);
        }

        internal IRoomInternal AddRoomInternal<T>(string roomId) where T : class, IRoom, new()
        {
            IRoomInternal room = this.EnableRoom<T>(roomId);
            this._rooms.Add(roomId, room);
            return room;
        }

        public bool Contains(string roomId)
        {
            return this._rooms.ContainsKey(roomId);
        }

        public T Get<T>(string roomId) where T : class, IRoom
        {
            return (T)this._rooms[roomId];
        }

        public T TryGet<T>(string roomId) where T : class, IRoom
        {
            return this._rooms[roomId] as T;
        }

        private IRoomInternal EnableRoom<T>(string roomId) where T : class, IRoom, new()
        {
            var room = new T();
            var returnValue = room as IRoomInternal;
            if (returnValue == null)
                throw new ArgumentException("The given type must inherit Room!");

            returnValue.Activate(this, roomId);
            return returnValue;
        }

        protected override void Dispose(bool disposing)
        {
            this._lobbyRoomInternal.Disactivate();
            base.Dispose(disposing);
        }
    }
}