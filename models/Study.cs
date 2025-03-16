namespace chess.api.models
{
    public interface IStudy
    {

    }

    public class Study: IStudy
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Position Position { get; set; }
        public Color Perspective { get; set; }
        public string SummaryFEN { get; set; }
    }

    public class SimpleStudy: IStudy
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Color Perspective { get; set; }
        public string SummaryFEN { get; set; }
    }
}
