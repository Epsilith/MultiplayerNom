using ByteNom;

namespace MultiplayerNom
{
    public class Lobby<TRoom> : Room<BaseUser> where TRoom : class, IRoom, new()
    {
        protected override void OnMessage(BaseUser user, Message message)
        {
            if (message.Type == "join")
            {
                if (message.Count >= 1)
                {
                    var roomId = message[0] as string;

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