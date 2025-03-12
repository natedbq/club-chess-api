namespace chess.api.models
{
    public class Position
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<string> Tags { get; set; } //this is a good spot to call out the opening name
        public string Description { get; set; }
        public IList<Continuation> Continuations { get; set; }
        public Move Move { get; set; }
    }
}
