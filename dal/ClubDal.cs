using AutoMapper.Execution;
using chess.api.configuration;
using chess.api.Controllers;
using chess.api.models;
using ChessApi.repository;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using static System.Reflection.Metadata.BlobBuilder;

namespace chess.api.dal
{
    public class ClubDal
    {

        private readonly string _sqlConnectionString;
        private readonly static SimpleStudyRepository _studyRepository = new SimpleStudyRepository();
        private readonly static UserDal _userDal = new UserDal();

        public ClubDal()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public async Task AddStudy(Guid clubId, Guid studyId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"insert into StudyClub (clubId, studyId) values ('{clubId}','{studyId}');";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task InviteToJoin(InviteToJoinModel request)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"insert into ClubInvite (toUsername,fromUsername,clubId, message) values "
                    + "(@toUser,@fromUser,@clubId,@message);"
                    .Replace("@clubId", request.ClubId.SqlOrNull())
                    .Replace("@toUser", request.ToUsername.SqlOrNull())
                    .Replace("@fromUser", request.FromUsername.SqlOrNull())
                    .Replace("@message", request.Message.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task RequestToJoin(RequestToJoinModel request)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"insert into ClubRequest (clubId, userId, message) values (@clubId,(select id from [User] where username = @username),@message);"
                    .Replace("@clubId",request.ClubId.SqlOrNull())
                    .Replace("@userId",request.FromUsername.SqlOrNull())
                    .Replace("@message",request.Message.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task RemoveStudy(Guid clubId, Guid studyId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = "delete from StudyClub where clubId = @clubId and studyId = @studyId;"
                    .Replace("@clubId", clubId.SqlOrNull())
                    .Replace("@studyId", studyId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task AddMember(Guid clubId, string username)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = ("insert into ClubUser (clubId, userId) values (@clubId, (select id from [user] where username = @username));"
                    +"delete from ClubInvite where clubId = @clubId and toUsername = @username;")
                    .Replace("@clubId", clubId.SqlOrNull())
                    .Replace("@username", username.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public async Task DeclineInvite(Guid clubId, string username)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = "delete from ClubInvite where clubId = @clubId and toUsername = @username;"
                        .Replace("@clubId", clubId.SqlOrNull())
                        .Replace("@username", username.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task RemoveMember(Guid clubId, string username)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"delete from ClubUser where clubId = @clubId and userId = (select id from [user] where username = @username)"
                    .Replace("@clubId", clubId.SqlOrNull())
                    .Replace("@username", username.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<Guid> CreateClub(ClubPostModel club)
        {
            var id = Guid.NewGuid();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"insert into Club (id, name, description, owner) values (@id,@name,@description,@owner)"
                    .Replace("@id", id.SqlOrNull())
                    .Replace("@name", club.Name.SqlOrNull())
                    .Replace("@description", club.Description.SqlOrNull())
                    .Replace("@owner", club.OwnerId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                query = $"select username from [user] where id = '{club.OwnerId}';";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var username = reader.GetString(0);
                            await AddMember(id, username);
                        }
                        reader.Close();
                    }
                }
            }


            return id;
        }

        public async Task<bool> HasMember(Guid clubId, Guid userId)
        {
            var isMember = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();


                var query = $"select * from ClubUser where clubId = '{clubId}' and userId = '{userId}';";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        isMember = reader.Read();

                        reader.Close();
                    }
                }
            }

            return isMember;
        }
        public async Task<IList<ClubInvite>> GetInvitesForClub(Guid clubId)
        {
            var invites = new List<ClubInvite>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();


                var query = $"select toUsername, fromUsername, message, picUrl, name from ClubInvite ci "
                    +" inner join Club c on ci.clubId = c.id"
                    +$" where clubId = '{clubId}'";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var invite = new ClubInvite();
                            invite.ToUsername = reader.GetString(0);
                            invite.FromUsername = reader.GetString(1);
                            invite.Message = reader.GetString(2);
                            invite.ClubPic = reader.IsDBNull(3) ? null : reader.GetString(3);
                            invite.ClubName = reader.GetString(4);
                            invite.ClubId = clubId;
                            invites.Add(invite);
                        }
                        reader.Close();
                    }
                }
            }

            return invites;
        }

        public async Task<IList<Club>> GetClubsByUserId(Guid userId)
        {
            var clubs = new List<Club>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();


                var query = $"select id, name, description, owner, picUrl from Club where id in (select userid from ClubUser where clubId = {userId.SqlOrNull()})";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var club = new Club();
                            club.Id = reader.GetGuid(0);
                            club.Name = reader.GetString(1);
                            club.Description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            club.Owner = await _userDal.GetSimpleUserById(reader.GetGuid(3));
                            club.Studies = _studyRepository.GetStudiesByClubId(club.Id);
                            club.Members = await _userDal.GetSimpleUsersByClubId(club.Id);
                            club.PicUrl = reader.IsDBNull(4) ? null : reader.GetString(4);
                            clubs.Add(club);
                        }
                        reader.Close();
                    }
                }
            }

            return clubs;
        }


        public async Task<IList<Club>> GetClubs()
        {
            var clubs = new List<Club>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();


                var query = $"select id, name, description, owner, picUrl from Club";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var club = new Club();
                            club.Id = reader.GetGuid(0);
                            club.Name = reader.GetString(1);
                            club.Description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            club.Owner = await _userDal.GetSimpleUserById(reader.GetGuid(3));
                            club.Studies = new List<SimpleStudy>();
                            club.Members = await _userDal.GetSimpleUsersByClubId(club.Id);
                            club.PicUrl = reader.IsDBNull(4) ? null : reader.GetString(4);
                            clubs.Add(club);
                        }

                        reader.Close();
                    }
                }
            }

            return clubs;
        }

        public async Task<Club> GetClubById(Guid clubId, Guid userId = default(Guid))
        {
            Club club = null;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"select id, name, description, owner, picUrl from Club where id = {clubId.SqlOrNull()}";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            club = new Club();
                            club.Id = reader.GetGuid(0);
                            club.Name = reader.GetString(1);
                            club.Description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            club.Owner = await _userDal.GetSimpleUserById(reader.GetGuid(3));
                            club.Studies = _studyRepository.GetStudiesByClubId(club.Id, userId);
                            club.Members = await _userDal.GetSimpleUsersByClubId(club.Id);
                            club.PicUrl = reader.IsDBNull(4) ? null : reader.GetString(4);
                        }

                        reader.Close();
                    }
                }
            }

            return club;
        }
    }
}
