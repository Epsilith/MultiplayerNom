using ByteNom;

namespace MultiplayerNom
{
    internal class UserManager
    {
        private int _userIdCounter;

        public UserHandle RegisterUser(Connection connection, IServer server, IRoomInternal room)
        {
            return new UserHandle(connection, server, room, this._userIdCounter++);
        }
    }
}