namespace chess.api.models
{
    public class Club
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<User> Members { get; set; }
        public User Owner { get; set; }
        public IList<SimpleStudy> Studies { get; set; }
    }
}
