using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ByteNom;

namespace MultiplayerNom
{
    /// <summary>
    ///     Represents a multiplayer server that hosts multiple rooms and a lobby.
    /// </summary>
    /// <typeparam name="TLobby">The type of the lobby.</typeparam>
    public class MultiplayerServer<TLobby> : Server, IServerInternal where TLobby : class, ILobbyBase, new()
    {
        private const string LobbyRoomName = "Lobby";
        private readonly Dictionary<string, IRoomInternal> _rooms = new Dictionary<string, IRoomInternal>();
        private readonly UserManager _userManager = new UserManager();
        private IRoomInternal _lobbyRoomInternal;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiplayerServer{TLobby}" /> class.
        /// </summary>
        /// <param name="port">The port to connect to.</param>
        public MultiplayerServer(int port)
            : base(port)
        {
            this.Init();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiplayerServer{TLobby}" /> class.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="port">The port.</param>
        public MultiplayerServer(IPAddress ipAddress, int port)
            : base(ipAddress, port)
        {
            this.Init();
        }

        /// <summary>
        ///     Gets the lobby room.
        /// </summary>
        /// <value>
        ///     The lobby room.
        /// </value>
        public TLobby Lobby
        {
            get { return this.Get<TLobby>(LobbyRoomName); }
        }
        
        ILobbyBase IServer.Lobby
        {
            get { return this.Lobby; }
        }

        /// <summary>
        ///     Gets the rooms in this server.
        /// </summary>
        /// <value>
        ///     The rooms.
        /// </value>
        public string[] Rooms
        {
            get { return this._rooms.Keys.ToArray(); }
        }

        /// <summary>
        ///     Adds a room with the given type T and the given room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>
        ///     The room.
        /// </returns>
        public T AddRoom<T>(string roomId) where T : class, IRoom, new()
        {
            return (T)this.EnableRoom<T>(roomId);
        }

        /// <summary>
        ///     Determines whether this instance contains the specified room identifier.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>
        ///     <c>true</c> if an instance is found; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string roomId)
        {
            return this._rooms.ContainsKey(roomId);
        }

        bool IServerInternal.Remove(string roomId)
        {
            if (roomId == LobbyRoomName)
                throw new InvalidOperationException("The lobby room may not be closed!");
            return this._rooms.Remove(roomId);
        }

        /// <summary>
        ///     Gets the room with the specified room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>
        ///     The room.
        /// </returns>
        public T Get<T>(string roomId) where T : class, IRoom
        {
            return (T)this._rooms[roomId];
        }

        /// <summary>
        ///     Tries to get the room with the specified room identifier.
        /// </summary>
        /// <typeparam name="T">The type of the room.</typeparam>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>
        ///     The room.
        /// </returns>
        public T TryGet<T>(string roomId) where T : class, IRoom
        {
            return this._rooms[roomId] as T;
        }

        private void Init()
        {
            this._lobbyRoomInternal = this.EnableRoom<TLobby>(LobbyRoomName);

            this.ConnectionReceived +=
                (sender, connection) => { this._userManager.RegisterUser(connection, this._lobbyRoomInternal); };
        }

        private IRoomInternal EnableRoom<T>(string roomId) where T : class, IRoom, new()
        {
            var room = new T();
            var returnValue = room as IRoomInternal;
            if (returnValue == null)
                throw new ArgumentException("The given type must inherit Room!");

            this._rooms.Add(roomId, returnValue);
            returnValue.Activate(this, roomId);
            return returnValue;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            this._lobbyRoomInternal.Disactivate();
            base.Dispose(disposing);
        }
    }
}