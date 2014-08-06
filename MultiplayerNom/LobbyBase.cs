using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiplayerNom
{
    public abstract class LobbyBase<TUser> : Room<TUser>, ILobbyBase where TUser : User, new()
    {
        protected override bool OnLastUserLeave()
        {
            // Do not close the lobby room even after the last player leaves
            return false;
        }
    }
}
