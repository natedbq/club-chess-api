namespace chess.api.Validations
{
    public class BusinessValidation
    {
        public static readonly StudyValidation Study = new StudyValidation();
        public static readonly ClubValidation Club = new ClubValidation();
        public static readonly UserValidation User = new UserValidation();
        public static readonly BookValidation Book = new BookValidation();
    }
}
