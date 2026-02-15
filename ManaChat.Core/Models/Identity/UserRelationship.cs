using ManaChat.Core.Enums.Identity;

namespace ManaChat.Core.Models.Identity
{
    public class UserRelationship
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RecipientUserId { get; set; }
        public RelationshipType RelationshipType { get; set; }
        public bool Bookmarked { get; set; }
    }
}
