namespace chess.api.models
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid StudyId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public IList<Chapter> Chapters { get; set; }
    }

    public class Chapter
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public IList<Page> Pages { get; set; }
    }

    public class Page
    {
        public Guid Id { get; set; }
        public Guid ChapterId { get; set; }
        public string Title { get; set; }
        public string FEN { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
        public bool FastForward { get; set; }
        public bool Hide { get; set; }
        public string Plans { get; set; }

    }
}
