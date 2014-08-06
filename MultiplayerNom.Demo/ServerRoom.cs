using ByteNom;

namespace MultiplayerNom.Demo
{
    internal class MyUser : BaseUser
    {
        public string Name { get; set; }
    }

    internal class ServerRoom : Room<MyUser>
    {
        protected override void OnJoin(MyUser user)
        {
            user.Name = "Nub" + user.Id;
            this.Broadcast("join", user.Name);
        }

        protected override void OnMessage(MyUser user, Message message)
        {
            if (message.Type == "whoami")
            {
                user.Send("name", user.Name);
            }
        }

        protected override void OnLeave(MyUser user)
        {
            this.Broadcast("leave", user.Name);
        }
    }
}