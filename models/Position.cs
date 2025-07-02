namespace chess.api.models
{
    public class Position
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public IList<string> Tags { get; set; } //this is a good spot to call out the opening name
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public IList<Position> Positions { get; set; }
        public Move Move { get; set; }
        public string Plans { get; set; }
        public bool IsKeyPosition { get; set; }
        public DateTime LastStudied { get; set; }
        public long Mistakes { get; set; }
    }
}
