using ByteNom;

namespace MultiplayerNom
{
    public sealed class Lobby<TRoom> : Room<User>, ILobbyBase where TRoom : class, IRoom, new()
    {
        protected override void OnMessage(User user, Message message)
        {
            if (message.Type == "join")
            {
                if (message.Count < 1) return;
                var roomId = message[0] as string;
                if (roomId != null)
                {
                    // Get or create a room with the given id
                    IRoom room = this.Server.Contains(roomId)
                        ? this.Server.Get<IRoom>(roomId)
                        : this.Server.AddRoom<TRoom>(roomId);

                    user.MoveToRoom(room);
                }
            }
        }
    }
}