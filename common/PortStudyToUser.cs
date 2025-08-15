using chess.api.configuration;
using chess.api.dal;
using chess.api.models;
using ChessApi.dal;
using Microsoft.Data.SqlClient;

namespace chess.api.common
{
    public class PortStudyToUser
    {
        private readonly string _sqlConnectionString;
        private readonly static StudyDal _studyDal = new StudyDal();
        private readonly static PositionDal _positionDal = new PositionDal();


        public PortStudyToUser()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public bool UserHasStudy(Guid studyId, Guid userId)
        {
            var exists = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();

                var query = "select * from UserStudyStats where studyId = @id and userId = @userId"
                    .Replace("@id", studyId.SqlOrNull())
                    .Replace("@userId", userId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        exists = reader.Read();
                        reader.Close();
                    }
                }
            }

            return exists;
        }

        public void PortToUser(Guid studyId, Guid userId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                var exists = UserHasStudy(studyId, userId);

                if (!exists)
                {
                    connection.Open();
                    var study = _studyDal.GetById(studyId);

                    var importStudyStatement = $"insert into UserStudyStats (studyId, userId, lastStudied, Accuracy) values ({studyId.SqlOrNull()},{userId.SqlOrNull()},{DateTime.Now.SqlOrNull()}, -1);";
                    importStudyStatement += $"insert into StudyUser (studyId, userId) values ({studyId.SqlOrNull()},{userId.SqlOrNull()});";
                    using (var command = new SqlCommand(importStudyStatement, connection))
                    {
                        command.ExecuteNonQuery();
                    }


                    var position = _positionDal.GetById(study.PositionId.Value, depth: 100);

                    var list = GenerateInserts(studyId, position, userId);

                    var chunkSize = 50;
                    for(int i = 0; i < list.Count; i += chunkSize)
                    {
                        var part = list.Skip(i).Take(chunkSize);
                        var insertStatememnt = "insert into UserPositionStats (positionId, userId, studyId, mistakes, lastStudied) values "
                            + string.Join(",", part);

                        using (var command = new SqlCommand(insertStatememnt, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                }

                connection.Close();
            }
        }

        private IList<string> GenerateInserts(Guid studyId, Position position, Guid UserId)
        {
            var inserts = new List<string>
            {
                $"('{position.Id}', '{UserId}', '{studyId}', 0, '{DateTime.Now}')"
            };

            foreach (var p in position.Positions)
            {
                inserts.AddRange(GenerateInserts(studyId, p, UserId));
            }

            return inserts;
        }


    }
}
