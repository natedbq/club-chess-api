using chess.api.configuration;
using chess.api.Controllers;
using chess.api.models;
using ChessApi.repository;
using Microsoft.Data.SqlClient;
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

        public async Task AddMember(Guid clubId, Guid userId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"insert into ClubUser (clubId, userId) values (@clubId, @userId)"
                    .Replace("@clubId", clubId.SqlOrNull())
                    .Replace("@userId", userId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task RemoveMember(Guid clubId, Guid userId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var query = $"delete from ClubUser where clubId = @clubId and userId = @userId"
                    .Replace("@clubId", clubId.SqlOrNull())
                    .Replace("@userId", userId.SqlOrNull());
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
            }

            await AddMember(id, club.OwnerId);

            return id;
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
                            reader.Close();
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

        public async Task<Club> GetClubById(Guid clubId)
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
                            club.Studies = _studyRepository.GetStudiesByClubId(club.Id);
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
