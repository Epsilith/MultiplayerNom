using ByteNom;

namespace MultiplayerNom
{
    internal class UserManager
    {
        private int _userIdCounter;

        public UserHandle RegisterUser(TcpConnection connection, IRoomInternal room)
        {
            return new UserHandle(connection, room, this._userIdCounter++);
        }
    }
}