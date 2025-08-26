using chess.api.configuration;
using chess.api.Exceptions;
using chess.api.models;
using Microsoft.Data.SqlClient;

namespace chess.api.Validations
{
    public class ClubValidation
    {
        private readonly string _sqlConnectionString;

        public ClubValidation()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void UserCanViewClub(Guid userId, Guid clubId)
        {
            var canView = UserInClub(userId, clubId) || UserOwnsClub(userId, clubId);

            if (!canView)
            {
                throw new BusinessRuleException("User does not have permission to view this club.");
            }
        }

        public void UserCanEditClub(Guid userId, Guid clubId)
        {
            var canEdit = UserOwnsClub(userId, clubId);
            if (!canEdit)
            {
                throw new BusinessRuleException("User does not have permission to edit this club.");
            }
        }

        public void UserIsAdmin(Guid userId, Guid clubId)
        {
            var isOwner = UserOwnsClub(userId, clubId);
            if (!isOwner)
            {
                throw new BusinessRuleException("User does not have does not have admin privileges for this club.");
            }
        }

        public void UserIsInvited(string username, Guid clubId)
        {
            var inviteExists = InviteExists(username, clubId);
            if (!inviteExists)
            {
                throw new BusinessRuleException("User has not been invited to this club.");
            }
        }

        private bool UserInClub(Guid userId, Guid clubId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from ClubUser where userId = @userId and clubId = @clubId"
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@clubId", clubId.SqlOrNull());
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

        private bool UserOwnsClub(Guid userId, Guid clubId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from club where owner = @userId and id = @clubId"
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@clubId", clubId.SqlOrNull());
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

        private bool InviteExists(string username, Guid clubId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from ClubInvite where toUsername = @username and clubId = @clubId"
                    .Replace("@username", username.SqlOrNull())
                    .Replace("@clubId", clubId.SqlOrNull());
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
