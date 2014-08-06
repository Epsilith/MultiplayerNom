namespace MultiplayerNom
{
    public interface IServer
    {
        string[] Rooms { get; }
        T AddRoom<T>(string roomId) where T : class, IRoom, new();
        T Get<T>(string roomId) where T : class, IRoom;
        T TryGet<T>(string roomId) where T : class, IRoom;
        bool Contains(string roomId);
    }
}