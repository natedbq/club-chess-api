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
}
