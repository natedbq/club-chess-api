using Microsoft.Identity.Client;

namespace chess.api.models
{
    public class Club
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<SimpleUser> Members { get; set; }
        public SimpleUser Owner { get; set; }
        public IList<SimpleStudy> Studies { get; set; }
        public string PicUrl { get; set; }
    }

    public class ClubInvite
    {
        public string ToUsername { get; set; }
        public string FromUsername { get; set; }
        public string Message { get; set; }
        public string ClubName { get; set; }
        public string ClubPic { get; set; }
        public Guid ClubId { get; set; }
    }
}
