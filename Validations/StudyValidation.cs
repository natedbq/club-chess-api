using chess.api.configuration;
using chess.api.Exceptions;
using Microsoft.Data.SqlClient;

namespace chess.api.Validations
{
    public class StudyValidation
    {
        private readonly string _sqlConnectionString;

        public StudyValidation()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void UserCanViewStudy(Guid userId, Guid studyId)
        {
            var canView = UserIsOwner(userId, studyId) || UserConnectsThroughClub(userId, studyId);
            if(!canView)
            {
                throw new BusinessRuleException("User does not have permission to view this study.");
            }
        }

        public void UserCanEditStudy(Guid userId, Guid studyId)
        {
            var canEdit = UserIsOwner(userId, studyId);
            if(!canEdit)
            {
                throw new BusinessRuleException("User does not have permission to edit this study.");
            }
        }

        private bool UserIsOwner(Guid userId, Guid studyId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from study where owner = @userId and id = @studyId"
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@studyId", studyId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var isTrue = reader.Read();
                        reader.Close();

                        return isTrue;
                    }
                }
            }
        }

        private bool UserConnectsThroughClub(Guid userId, Guid studyId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from [user] u inner join ClubUser cu on cu.userId = u.id inner join StudyClub sc on cu.clubId = sc.clubId"
                    +" where u.id = @userId and sc.studyId = @studyId;"
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@studyId", studyId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var isTrue = reader.Read();
                        reader.Close();

                        return isTrue;
                    }
                }
            }
        }
    }
}
