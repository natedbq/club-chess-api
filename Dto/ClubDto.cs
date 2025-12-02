namespace chess.api.Dto
{



    public class InviteToJoinDto
    {
        public string ToUsername { get; set; }
        public string FromUsername { get; set; }
        public Guid ClubId { get; set; }
        public string Message { get; set; }
    }
    public class RequestToJoinDto
    {
        public string FromUsername { get; set; }
        public Guid ClubId { get; set; }
        public string Message { get; set; }
    }

    public class NewClubDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
