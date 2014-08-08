namespace MultiplayerNom
{
    internal interface IMultiplayerServerInternal : IMultiplayerServer
    {
        bool Remove(string roomId);
    }
}