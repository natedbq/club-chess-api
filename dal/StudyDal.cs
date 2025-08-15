using chess.api.common;
using chess.api.configuration;
using chess.api.dal;
using chess.api.models;
using chess.api.services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace ChessApi.dal
{
    public class StudyDal
    {
        private readonly string _sqlConnectionString;
        private readonly PortStudyToUser _portToStudy = new PortStudyToUser();

        public StudyDal()
        {
            //Context = new Context("D:\\projects\\data\\chess-game");
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public IList<Study> GetStudiesByUserId(Guid userId)
        {
            var studies = new List<Study>();
            var query = "Select id,title,summaryFEN,description,positionId,perspective,tags,focus_tags,accuracy from Study as a inner join UserStudyStats as b on a.id = b.studyId"
                + $" where id in (select studyId from StudyUser where UserId = {userId.SqlOrNull()}) and b.userId = '{userId}';";

            using(var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                using(var command = new SqlCommand(query, connection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var study = new Study();
                            study.Id = reader.GetGuid(0);
                            study.Title = reader.GetString(1).Trim();
                            study.SummaryFEN = reader.GetString(2).Trim();
                            study.Description = reader.IsDBNull(3) ? null : reader.GetString(3).Trim();
                            study.PositionId = reader.GetGuid(4);
                            study.Perspective = (Color)reader.GetInt32(5);
                            study.Tags = reader.IsDBNull(6) ? new List<string>() : reader.GetString(6).Trim().Split(",");
                            study.FocusTags = reader.IsDBNull(7) ? new List<string>() : reader.GetString(7).Trim().Split(",");
                            study.Score = reader.IsDBNull(8) ? null : reader.GetDouble(8);
                            studies.Add(study);
                        }

                        reader.Close();
                    }
                }
                connection.Close();
            }

            return studies;
        }

        public IList<Study> GetStudiesByClubId(Guid clubId)
        {
            var studies = new List<Study>();
            var query = "Select id,title,summaryFEN,description,positionId,perspective,tags,focus_tags from Study";
                //+ "as a inner join UserStudyStats as b on a.id = b.studyId "
                //+ $" where id in (select studyId from StudyClub where clubId = {clubId.SqlOrNull()}) and b.userId = '{userId}'";

            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var study = new Study();
                            study.Id = reader.GetGuid(0);
                            study.Title = reader.GetString(1).Trim();
                            study.SummaryFEN = reader.GetString(2).Trim();
                            study.Description = reader.IsDBNull(3) ? null : reader.GetString(3).Trim();
                            study.PositionId = reader.GetGuid(4);
                            study.Perspective = (Color)reader.GetInt32(5);
                            study.Tags = reader.IsDBNull(6) ? new List<string>() : reader.GetString(6).Trim().Split(",");
                            study.FocusTags = reader.IsDBNull(7) ? new List<string>() : reader.GetString(7).Trim().Split(",");
                            study.Score = reader.IsDBNull(8) ? 0 : reader.GetDouble(8);
                            studies.Add(study);
                        }

                        reader.Close();
                    }
                }
                connection.Close();
            }

            return studies;
        }

        public void Study(Guid studyId, Guid userId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var query = $"update UserStudyStats set lastStudied=getdate() where studyId = {studyId.SqlOrNull()} and userId = {userId.SqlOrNull()}";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public Study GetById(Guid studyId, Guid userId = default(Guid))
        {
            Study study = null;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                var query = "Select id,title,summaryFEN,description,positionId,perspective,tags,focus_tags,accuracy from Study as a inner join UserStudyStats as b on a.id = b.studyId"
                    +$" where id = '{studyId}'";
                query = query.Replace("@studyId", studyId.SqlOrNull())
                    .Replace("@userId", userId.SqlOrNull());

                if(userId != default(Guid))
                {
                    query += $" where b.userId = '{userId}';";
                }

                connection.Open();

                using(var command = new SqlCommand(query, connection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            study = new Study();
                            study.Id = reader.GetGuid(0);
                            study.Title = reader.GetString(1).Trim();
                            study.SummaryFEN = reader.GetString(2).Trim();
                            study.Description = reader.IsDBNull(3) ? null : reader.GetString(3).Trim();
                            study.PositionId = reader.GetGuid(4);
                            study.Perspective = (Color)reader.GetInt32(5);
                            study.Tags = reader.IsDBNull(6) ? new List<string>() : reader.GetString(6).Split(",");
                            study.FocusTags = reader.IsDBNull(7) ? new List<string>() : reader.GetString(7).Trim().Split(",");
                            study.Score = reader.IsDBNull(8) ? null : reader.GetDouble(8);
                        }
                        reader.Close();
                    }
                }

                connection.Close();
            }
            return study;
        }

        public void Delete(Guid studyId)
        {
            var query = $"delete from study where id = '{studyId}';" +
                $"delete from StudyUser where studyId = '{studyId}';"+
                $"delete from userStudyStats where studyId = '{studyId}';";

            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public async Task Save(Study study)
        {
            var exists = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select * from study where id = @id".Replace("@id", study.Id.SqlOrNull());
                using(var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        exists = reader.Read();
                        reader.Close();
                    }
                }

                if (exists)
                {
                    Update(study, connection);
                }
                else
                {
                    New(study, connection);
                }

                connection.Close();
            }
        }

        private void New(Study study, SqlConnection connection)
        {
            var query = $"insert into Study (id, title, summaryFen, description, positionId, perspective,tags,focus_tags,score,owner) " +
                $"values ('{study.Id}',{study.Title.SqlOrNull()},'{study.SummaryFEN}','{study.Description}','{study.PositionId.Value}',{(int)study.Perspective}," 
                + $" {string.Join(",", study.Tags).SqlOrNull()}, {string.Join(",", study.FocusTags).SqlOrNull()},{study.Score},{study.Owner.Id.SqlOrNull()});"
                + $"insert into StudyUser (studyId, userId) values ('{study.Id}','{study.Owner.Id}');"
                + $"insert into UserStudyStats (studyId,userId,lastStudied,accuracy) values ('{study.Id}','{study.Owner.Id}','{DateTime.Now}',100);";
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
                var e = i;
            }

        }

        public void UpdateScore(Guid studyId, Guid userId, double score)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var query = $"update UserStudyStats set accuracy={score} where studyId = {studyId.SqlOrNull()} and userId = {userId.SqlOrNull()}";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void Update(Study study, SqlConnection connection)
        {
            var query = $"update study set description=@description,summaryFEN=@summaryFEN,tags=@tags,focus_tags=@focus_tags,score=@score where id = @id"
                .Replace("@description",study.Description.SqlOrNull())
                .Replace("@summaryFEN",study.SummaryFEN.SqlOrNull())
                .Replace("@tags", string.Join(",", study.Tags).SqlOrNull())
                .Replace("@id",study.Id.SqlOrNull())
                .Replace("@score",study.Score.ToString())
                .Replace("@focus_tags", string.Join(",", study.FocusTags).SqlOrNull());
            using (var command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static IList<Study> _studyList = new List<Study>()
            {
                new Study()
                {
                    Id = Guid.NewGuid(),
                    Title = "New Study",
                    Description = "Begin a new study",
                    Position = new Position()
                    {
                        Id= Guid.NewGuid(),
                        Title = "",
                        Tags = new List<string>(),
                        Description = "",
                        Move = new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1","-"),
                        Positions = new List<Position>()
                        {
                            new Position()
                            {
                                Id= Guid.NewGuid(),
                                Title = "",
                                Tags = new List<string>(),
                                Description = "",
                                Move = new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1","e4"),
                                Positions = new List<Position>()
                                {
                                    new Position(){
                                        Id= Guid.NewGuid(),
                                        Title = "",
                                        Tags = new List<string>(),
                                        Description = "",
                                        Move = new Move("rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2","c6"),
                                        Positions = new List<Position>()
                                        {
                                            new Position(){
                                                Id= Guid.NewGuid(),
                                                Title = "",
                                                Tags = new List<string>(),
                                                Description = "",
                                                Move = new Move("rnbqkbnr/pp1ppppp/2p5/8/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2","d4"),
                                                Positions = new List<Position>()
                                                {
                                                    new Position(){
                                                        Id= Guid.NewGuid(),
                                                        Title = "",
                                                        Tags = new List<string>(),
                                                        Description = "",
                                                        Move = new Move("rnbqkbnr/pp2pppp/2p5/3p4/3PP3/8/PPP2PPP/RNBQKBNR w KQkq d6 0 3","d5"),
                                                        Positions = new List<Position>()
                                                        {
                                                            new Position(){
                                                                Id= Guid.NewGuid(),
                                                                Title = "",
                                                                Tags = new List<string>(),
                                                                Description = "",
                                                                Move = new Move("rnbqkbnr/pp2pppp/2p5/3pP3/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3","e5"),
                                                                Positions = new List<Position>()
                                                                {
                                                                    new Position(){
                                                                        Id= Guid.NewGuid(),
                                                                        Title = "",
                                                                        Tags = new List<string>(),
                                                                        Description = "",
                                                                        Move = new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 3","c5")
                                                                    }
                                                                }
                                                            },
                                                            new Position(){
                                                                Id= Guid.NewGuid(),
                                                                Title = "",
                                                                Tags = new List<string>(),
                                                                Description = "",
                                                                Move = new Move("rnbqkbnr/pp2pppp/2p5/3P4/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3","exd5"),
                                                                Positions = new List<Position>()
                                                                {
                                                                    new Position(){
                                                                        Id= Guid.NewGuid(),
                                                                        Title = "",
                                                                        Tags = new List<string>(),
                                                                        Description = "",
                                                                        Move = new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4","cxd5")
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Perspective = Color.Black
                },
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Danish Gambit",
                //    Description = "Fun!",
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Danish Gambit",
                //        Description = "The best opening on the market.",
                //        MovesToPosition = new List<Move>()
                //        {

                //            new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "-"),
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "e4"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", "e5")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Title = "Danish Gambit",
                //            Tags = new List<string>(){"Danish Gambit", "Gambit" },
                //            Description = "The opening position.",
                //            Move = new Move("rnbqkbnr/pppp1ppp/8/4p3/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2", "d4"),
                //            Continuations = new List<Continuation>() {
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Title = "Danish Gambit",
                //                    Description = "Black accepts the trade offer.",
                //                    MovesToPosition = new List<Move>()
                //                    {

                //                        new Move("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPP2PPP/RNBQKBNR w KQkq - 0 3", "exd4")
                //                    },
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pppp1ppp/8/8/3pP3/2P5/PP3PPP/RNBQKBNR b KQkq - 0 3", "c3"),
                //                        Title = "Danish Gambit",
                //                        Tags = new List<string>(){"Danish Gambit", "Gambit" },
                //                        Description = "White offers Black a piece of Candy.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                Id = Guid.NewGuid(),
                //                                Title = "Danish Gambit",
                //                                Description = "Black accepts the candy.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pppp1ppp/8/8/4P3/2p5/PP3PPP/RNBQKBNR w KQkq - 0 4", "dxc3")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/2p5/PP3PPP/RNBQK1NR b KQkq", "Bc4"),
                //                                    Title = "Danish Gambit",
                //                                    Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                    Description = "Delighted, White tells Black he can pet his puppy if he climbs into his van.",
                //                                    Continuations = new List<Continuation>()
                //                                    {
                //                                        new Continuation()
                //                                        {
                //                                            Id = Guid.NewGuid(),
                //                                            Title = "Danish Gambit",
                //                                            Description = "Excited by the new friendship, Black climbs inside the van.",
                //                                            MovesToPosition = new List<Move>()
                //                                            {

                //                                                new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/8/Pp3PPP/RNBQK1NR w KQkq - 0 5", "cxb2")
                //                                            },
                //                                            Position = new Position()
                //                                            {
                //                                                Id = Guid.NewGuid(),
                //                                                Move = new Move("rnbqkbnr/pppp1ppp/8/8/2B1P3/8/PB3PPP/RN1QK1NR b KQkq - 0 5", "Bxb2"),
                //                                                Title = "Danish Gambit",
                //                                                Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                                Description = "White closes the door poises himself to violently molest Black.",
                //                                                Continuations = new List<Continuation>()
                //                                                {
                //                                                    new Continuation()
                //                                                    {
                //                                                        Id = Guid.NewGuid(),
                //                                                        Title = "Danish Gambit",
                //                                                        Description = "Black is no stranger to sexual violence and responds sharply.",
                //                                                        MovesToPosition = new List<Move>()
                //                                                        {
                //                                                            new Move("rnbqk1nr/pppp1ppp/8/8/1bB1P3/8/PB3PPP/RN1QK1NR w KQkq - 1 6", "Bb4+")
                //                                                        },
                //                                                        Position = new Position()
                //                                                        {
                //                                                            Id = Guid.NewGuid(),
                //                                                            Move = new Move("rnbqk1nr/pppp1ppp/8/8/1bB1P3/8/PB3PPP/RN1Q1KNR b kq - 2 6", "Kf1"),
                //                                                            Title = "Danish Gambit",
                //                                                            Tags = new List<string>(){ "Danish Gambit", "Gambit" },
                //                                                            Description = "But it only serves to further arouse White, who devilishly enjoys a scuffle.",
                //                                                            Continuations = new List<Continuation>()
                //                                                        }
                //                                                    }
                //                                                }
                //                                            }
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    Perspective = Color.White
                //},
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Caro-Kann",
                //    Description = "The correct response to 1. e5.",
                //    Perspective = Color.Black,
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Caro-Kann",
                //        Description = "The best opening on the market.",
                //        MovesToPosition = new List<Move>()
                //        {

                //            new Move("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "-"),
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "e4"),
                //            new Move("rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2", "c6"),
                //            new Move("rnbqkbnr/pp1ppppp/2p5/8/3PP3/8/PPP2PPP/RNBQKBNR b KQkq d3 0 2", "d4")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Move = new Move("rnbqkbnr/pp2pppp/2p5/3p4/3PP3/8/PPP2PPP/RNBQKBNR w KQkq d6 0 3", "d5"),
                //            Title = "Caro-Kann",
                //            Tags = new List<string>() { "Caro-Kann" },
                //            Description = "Black fights for control of the center",
                //            Continuations = new List<Continuation>()
                //            {
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Title = "Advanced Caro-Kann",
                //                    Description = "White fights for space, playing directly into Black's hand.",
                //                    MovesToPosition = new List<Move>(),
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pp2pppp/2p5/3pP3/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3", "e5"),
                //                        Title = "Advanced Caro-Kann",
                //                        Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann" },
                //                        Description = "White fights for space, playing directly into Black's hand.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                 Title = "Advanced Caro-Kann",
                //                                 Id = Guid.NewGuid(),
                //                                Description = "The Rozman method.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4", "c5"),
                //                                    new Move("rnbqkbnr/pp2pppp/8/2ppP3/3P4/2P5/PP3PPP/RNBQKBNR b KQkq - 0 4", "c3"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/2ppP3/3P4/2P5/PP3PPP/RNBQKBNR w KQkq - 1 5", "Nc6"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/2ppP3/3P4/2P2N2/PP3PPP/RNBQKB1R b KQkq - 2 5", "Nf3")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("r2qkbnr/pp2pppp/2n5/2ppP3/3P2b1/2P2N2/PP3PPP/RNBQKB1R w KQkq - 3 6", "Bg4"),
                //                                    Title = "Advanced Caro-Kann",
                //                                    Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann" },
                //                                    Description = "Black has successfuly employed the Rozman method. White will likely lose a pawn.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            },
                //                            new Continuation()
                //                            {
                //                                 Id = Guid.NewGuid(),
                //                                 Title = "Bayonett Attack",
                //                                Description = "The pro method.",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pPb2/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 1 4", "Bf5"),
                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pPb2/3P2P1/8/PPP2P1P/RNBQKBNR b KQkq g3 0 4", "g4"),
                //                                    new Move("rn1qkbnr/pp2pppp/2p5/3pP3/3Pb1P1/8/PPP2P1P/RNBQKBNR w KQkq - 1 5", "Be4")
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("rn1qkbnr/pp2pppp/2p5/3pP3/3Pb1P1/5P2/PPP4P/RNBQKBNR b KQkq - 0 5", "f3"),
                //                                    Title = "Bayonett Attack",
                //                                    Tags = new List<string>() { "Caro-Kann", "Advanced Caro-Kann", "Bayonett Attack" },
                //                                    Description = "White sacrifices the integrity of his position to bully the bishop's nuts.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            }
                //                        }
                //                    }
                //                },
                //                new Continuation()
                //                {
                //                    Id = Guid.NewGuid(),
                //                    Description = "White cowars before Black, liquidating material to preserve equality for as long as possible.",
                //                    MovesToPosition = new List<Move>()
                //                    {
                //                        new Move("rnbqkbnr/pp2pppp/2p5/3P4/3P4/8/PPP2PPP/RNBQKBNR b KQkq - 0 3", "exd5")
                //                    },
                //                    Position = new Position()
                //                    {
                //                        Id = Guid.NewGuid(),
                //                        Move = new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/8/PPP2PPP/RNBQKBNR w KQkq - 0 4", "cxd5"),
                //                        Title = "Exchange Caro-Kann",
                //                        Tags = new List<string>() { "Caro-Kann", "Exchange Caro-Kann" },
                //                        Description = "White cowars before Black, liquidating material to preserve equality for as long as possible.",
                //                        Continuations = new List<Continuation>()
                //                        {
                //                            new Continuation()
                //                            {
                //                                 Id = Guid.NewGuid(),
                //                                Description = "Indeed...",
                //                                MovesToPosition = new List<Move>()
                //                                {

                //                                    new Move("rnbqkbnr/pp2pppp/8/3p4/3P4/5N2/PPP2PPP/RNBQKB1R b KQkq - 1 4", "Nf3"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/3p4/3P4/5N2/PPP2PPP/RNBQKB1R w KQkq - 2 5", "Nc6"),
                //                                    new Move("r1bqkbnr/pp2pppp/2n5/1B1p4/3P4/5N2/PPP2PPP/RNBQK2R b KQkq - 3 5", "Bb5"),
                //                                    new Move("r2qkbnr/pp2pppp/2n5/1B1p4/3P2b1/5N2/PPP2PPP/RNBQK2R w KQkq - 4 6", "Bg4"),
                //                                    new Move("r2qkbnr/pp2pppp/2n5/1B1p4/3P2b1/5N2/PPP2PPP/RNBQ1RK1 b kq - 5 6", "O-O"),
                //                                },
                //                                Position = new Position()
                //                                {
                //                                    Id = Guid.NewGuid(),
                //                                    Move = new Move("r2qkbnr/pp3ppp/2n1p3/1B1p4/3P2b1/5N2/PPP2PPP/RNBQ1RK1 w kq - 0 7", "e6"),
                //                                    Title = "Exchange Caro-Kann",
                //                                    Tags = new List<string>() { "Caro-Kann", "Exchange Caro-Kann" },
                //                                    Description = "White and Black play chess.",
                //                                    Continuations = new List<Continuation>()
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},
                //new Study()
                //{
                //    Id = Guid.NewGuid(),
                //    Title = "Scotch",
                //    Description = "The correct response to 1. e5",
                //    Perspective = Color.White,
                //    Continuation = new Continuation()
                //    {
                //        Id = Guid.NewGuid(),
                //        Title = "Scotch",
                //        Description = "",
                //        MovesToPosition = new List<Move>()
                //        {
                //            new Move("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "-"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", "-"),
                //            new Move("rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", "-"),
                //            new Move("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", "-")
                //        },
                //        Position = new Position()
                //        {
                //            Id = Guid.NewGuid(),
                //            Move = new Move("r1bqkbnr/pppp1ppp/2n5/4p3/3PP3/5N2/PPP2PPP/RNBQKB1R b KQkq d3 0 3", "-"),
                //            Title = "Scotch",
                //            Tags = new List<string>() { "Scotch" },
                //            Description = "Black is caught with his pants down after 3. d4.",
                //            Continuations = new List<Continuation>()
                //        }
                //    }
                //}
            };
    }
}
