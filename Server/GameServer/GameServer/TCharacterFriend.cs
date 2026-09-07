namespace GameServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TCharacterFriend
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TCharacterFriend()
        {
            this.CharacterID = 0;
        }
    
        public int Id { get; set; }
        public int FriendID { get; set; }
        public string FriendName { get; set; }
        public int Class { get; set; }
        public int Level { get; set; }
        public int CharacterID { get; set; }
    
        public virtual TCharacter Owner { get; set; }
    }
}
