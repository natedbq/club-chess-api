namespace chess.api.models
{
    public interface IStudy
    {

    }

    public class Study: IStudy
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Position? Position { get; set; }
        public Guid? PositionId { get; set; }
        public Color Perspective { get; set; }
        public string SummaryFEN { get; set; }
        public double Score { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> FocusTags { get; set; }
    }

    public class SimpleStudy: IStudy
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Color Perspective { get; set; }
        public string SummaryFEN { get; set; }
        public DateTime LastStudied { get; set; }
        public double Score { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> FocusTags { get; set; }
    }
}
