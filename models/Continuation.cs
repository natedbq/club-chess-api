namespace chess.api.models
{
    public class Continuation
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<Move> MovesToPosition { get; set; } //as FEN string
        public Position Position { get; set; }
    }
}
