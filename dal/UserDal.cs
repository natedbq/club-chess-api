using chess.api.configuration;
using chess.api.Controllers;
using chess.api.Exceptions;
using chess.api.models;
using ChessApi.repository;
using Microsoft.Data.SqlClient;

namespace chess.api.dal
{
    public class UserDal
    {
        private readonly string _sqlConnectionString;
        private readonly static SimpleStudyRepository _studyRepository = new SimpleStudyRepository();

        public UserDal()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public async Task<SimpleUser> Authenticate(string username, string password)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var valid = false;
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

                var query = $"select password, id from [user] where username = {username.SqlOrNull()}";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            var hashedPassword = reader.GetString(0);
                            valid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                            if (valid)
                            {
                                var id = reader.GetGuid(1);
                                reader.Close();
                                return await GetSimpleUserById(id);
                            }
                            else
                            {
                                reader.Close();
                                throw new FailedToAuthenticateUserException();
                            }
                        }
                    }
                }
            }
            throw new FailedToAuthenticateUserException();
        }

        public async Task<IList<ClubInvite>> GetInvitesForUser(Guid userId)
        {
            IList<ClubInvite> invites = new List<ClubInvite>();

            try
            {
                using (var connection = new SqlConnection(_sqlConnectionString))
                {
                    await connection.OpenAsync();

                    var query = $"select toUsername, fromUsername, clubId, Message, (select name from club where id = clubId)," 
                        +"(select picUrl from club where id = clubId) from ClubInvite where toUsername = (select username from [user] where id = @userId)"
                        .Replace("@userId", userId.SqlOrNull());
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var invite = new ClubInvite();
                                invite.ToUsername = reader.GetString(0);
                                invite.FromUsername = reader.GetString(1);
                                invite.ClubId = reader.GetGuid(2);
                                invite.Message = reader.GetString(3);
                                invite.ClubName = reader.GetString(4);
                                invite.ClubPic = reader.GetString(5);
                                invites.Add(invite);
                            }
                            reader.Close();
                        }
                    }
                }

                return invites;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserById(Guid userId)
        {
            User user = null;

            try
            {
                using (var connection = new SqlConnection(_sqlConnectionString))
                {
                    await connection.OpenAsync();


                    var query = $"select username,firstname,lastname from [user] where id = {userId.SqlOrNull()}";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User();
                                user.Id = userId;
                                user.Username = reader.GetString(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                user.Studies = await _studyRepository.GetStudies(userId);
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                throw new InvalidUserIdException(userId);
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<SimpleUser> GetSimpleUserById(Guid userId)
        {
            SimpleUser user = null;

            try
            {
                using (var connection = new SqlConnection(_sqlConnectionString))
                {
                    await connection.OpenAsync();


                    var query = $"select username,firstname,lastname from [user] where id = {userId.SqlOrNull()}";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new SimpleUser();
                                user.Id = userId;
                                user.Username = reader.GetString(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                throw new InvalidUserIdException(userId);
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<SimpleUser>> GetSimpleUsersByClubId(Guid clubId)
        {
            var users = new List<SimpleUser>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();


                var query = $"select top 100 username,firstname,lastname from [user] where id in (select userid from ClubUser where clubId = {clubId.SqlOrNull()})";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new SimpleUser();
                            user.Username = reader.GetString(0);
                            user.FirstName = reader.GetString(1);
                            user.LastName = reader.GetString(2);
                            users.Add(user);
                        }
                        reader.Close();
                    }
                }
            }

            return users;
        }

        public async Task<Guid> CreateUser(NewUserModel user, string password)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var exists = false;

                var query = $"select * from [user] where username = {user.Username.SqlOrNull()}";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        exists = reader.Read();
                        reader.Close();
                    }
                }

                if (exists)
                {
                    throw new UsernameTakenException(user.Username);
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
                Guid userId = Guid.NewGuid();
                query = $"insert into [user] (id, username, password, firstname, lastname) values (@id, @username, @password, @firstname, @lastname)"
                    .Replace("@id", userId.SqlOrNull())
                    .Replace("@username", user.Username.SqlOrNull())
                    .Replace("@password", passwordHash.SqlOrNull())
                    .Replace("@firstname", user.FirstName.SqlOrNull())
                    .Replace("@lastname", user.LastName.SqlOrNull());


                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();

                return userId;
            }
        }
    }
}
