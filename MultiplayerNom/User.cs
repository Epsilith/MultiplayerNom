using ByteNom;

namespace MultiplayerNom
{
    public class User
    {
        private UserHandle _handle;

        public int Id
        {
            get { return this._handle.UserId; }
        }


        internal void Activate(UserHandle handle)
        {
            this._handle = handle;
        }

        public bool MoveToRoom(IRoom room)
        {
            return this._handle.MoveToRoom((IRoomInternal)room);
        }

        public void Send(Message message)
        {
            this._handle.Connection.Send(message);
        }

        public void Send(string type, params object[] args)
        {
            this._handle.Connection.Send(type, args);
        }

        public void Disconnect()
        {
            this._handle.Connection.Disconnect();
        }
    }
}