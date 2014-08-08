using ByteNom;

namespace MultiplayerNom
{
    internal interface IRoomInternal : IRoom
    {
        void Activate(IMultiplayerServerInternal multiplayerServer, string roomId);
        void Disactivate();
        bool AddUser(UserHandle handle);
        void RemoveUser(UserHandle handle);
        void HandleMessage(UserHandle handle, Message message);
    }
}