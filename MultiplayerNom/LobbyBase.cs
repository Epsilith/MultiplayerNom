namespace MultiplayerNom
{
    /// <summary>
    ///     Represents a lobby room.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    public abstract class LobbyBase<TUser> : Room<TUser>, ILobbyBase where TUser : User, new()
    {
        /// <summary>
        ///     Called when the last user leaves.
        /// </summary>
        /// <returns>
        ///     <c>false</c>
        /// </returns>
        protected override sealed bool OnLastUserLeave()
        {
            // Do not close the lobby room even after the last player leaves
            return false;
        }
    }
}