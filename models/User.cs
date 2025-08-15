namespace chess.api.models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IList<SimpleStudy> Studies { get; set; }

        public IList<Club> Clubs { get; set; }
    }

    public class SimpleUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
