using AutoMapper.Execution;
using chess.api.configuration;
using chess.api.models;
using Microsoft.Data.SqlClient;

namespace chess.api.dal
{
    public class AuthDal
    {
        private readonly string _sqlConnectionString;
        public AuthDal()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }
        public Guid AddRefreshToken(Guid userId)
        {
            var refreshToken = Guid.NewGuid();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();

                var query = $"insert into Refresh (refreshToken, userId, expiresOn) values (@refreshToken,@userId,@expiresOn)"
                    .Replace("@refreshToken", refreshToken.SqlOrNull())
                    .Replace("@userId", userId.SqlOrNull())
                    .Replace("@expiresOn", DateTime.Now.AddDays(7).SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            return refreshToken;
        }

        public RefreshTokenRotation RotateRefreshToken(Guid token)
        {
            var refreshIsValid = false;
            Guid userId = default(Guid);
            Guid refreshToken = default(Guid);
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();

                var query = $"select userId, expiresOn from Refresh where refreshToken = '{token}'";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        refreshIsValid = reader.Read();
                        if (refreshIsValid)
                        {
                            userId = reader.GetGuid(0);
                            var expiry = reader.GetDateTime(1);
                            refreshIsValid = expiry > DateTime.UtcNow;
                        }
                        reader.Close();
                    }
                }

                if (!refreshIsValid)
                {
                    throw new UnauthorizedAccessException();
                }

                query = $"delete from Refresh where refreshToken = '{token}';";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }


            refreshToken = AddRefreshToken(userId);

            return new RefreshTokenRotation()
            {
                Token = refreshToken,
                UserId = userId
            };
        }
    }

    public class RefreshTokenRotation
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
    }
}
