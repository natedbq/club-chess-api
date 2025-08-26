using AutoMapper.Execution;
using chess.api.configuration;
using chess.api.Exceptions;
using chess.api.models;
using ChessApi.repository;
using Microsoft.Data.SqlClient;

namespace chess.api.Validations
{
    public class UserValidation
    {
        private readonly string _sqlConnectionString;

        public UserValidation()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void UserIsNamed(Guid userId, string username)
        {
            var isNamed = UserHasUsername(userId, username);
            if (!isNamed)
            {
                throw new BusinessRuleException($"User does not have permission to make actions on behalf of {username}.");
            }
        }

        private bool UserHasUsername(Guid userId, string username)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();


                var query = $"select 1 from [user] where id = @userId and username = @username"
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@username", username.SqlOrNull());
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
