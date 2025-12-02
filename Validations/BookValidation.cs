using chess.api.configuration;
using chess.api.models;
using Microsoft.Data.SqlClient;

namespace chess.api.Validations
{
    public class BookValidation
    {
        private readonly string _sqlConnectionString;

        public BookValidation()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void UserCanEditBook(Guid userId, Guid bookId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var query = "select studyId from Book where id = @id".Replace("@bookId", bookId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var studyId = reader.GetGuid(0);
                        BusinessValidation.Study.UserCanEditStudy(userId, studyId);
                    }
                }
                connection.Close();
            }
        }
        public void UserCanEditChapter(Guid userId, Guid chapterId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var query = $"select studyId from Book as b where b.id in (select bookId from chapter as c where c.id = {chapterId.SqlOrNull()})";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var studyId = reader.GetGuid(0);
                        BusinessValidation.Study.UserCanEditStudy(userId, studyId);
                    }
                }
                connection.Close();
            }
        }
        public void UserCanEditPage(Guid userId, Guid pageId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var query = $"select studyId from Book as b where b.id in (select bookId from chapter as c where c.id in (select chapterId from page as p where p.id = {pageId.SqlOrNull()}))";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var studyId = reader.GetGuid(0);
                        BusinessValidation.Study.UserCanEditStudy(userId, studyId);
                    }
                }
                connection.Close();
            }
        }
    }
}
