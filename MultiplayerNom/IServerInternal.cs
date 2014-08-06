namespace MultiplayerNom
{
    internal interface IServerInternal : IServer
    {
        bool Remove(string roomId);
    }
}