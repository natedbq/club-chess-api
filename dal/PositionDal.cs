using chess.api.configuration;
using chess.api.models;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace chess.api.dal
{
    public class PositionDal
    {
        private readonly string _sqlConnectionString;
        public PositionDal()
        {
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public async Task Save(Position position)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = GenerateSaveInstructions(position,  connection);
                using (var command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        var t = ex.Message;
                    }
                }
                connection.Close();
            }
        }

        public async void Delete(Guid id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = await GenerateCascadeDelete(id);
                using (var command = new SqlCommand(query, connection))
                {
                   await command.ExecuteNonQueryAsync();
                }
                connection.Close();
            }
        }

        public Position GetById(Guid id, int depth = 0)
        {
            using(var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var position = PrivateGetById(id, connection, depth);
                connection.Close();
                return position;
            }
        }



        public IList<Position> GetByParentId(Guid id, int depth = 0)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();
                var position = PrivateGetById(id, connection, depth+1);
                connection.Close();
                return position.Positions;
            }
        }

        private Position PrivateGetById(Guid id, SqlConnection sqlConnection, int depth = 0)
        {
            var position = new Position();
            var query = "select id,title,description,moveName,moveFEN,parentId,move_from,move_to,plans,tags,isKeyPosition,lastStudied,mistakes from Position where id = @id";
            query = query.Replace("@id", id.SqlOrNull());

            using(var command = new SqlCommand(query, sqlConnection))
            {
                using(var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        position.Tags = new List<string>();

                        position.Id = reader.GetGuid(0);
                        position.Title = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim();
                        position.Description = reader.IsDBNull(2) ? "" : reader.GetString(2).Trim();
                        position.Move = new Move(reader.GetString(4).Trim(), reader.GetString(3).Trim());
                        position.Move.From = reader.IsDBNull(6) ? "" :  reader.GetString(6).Trim();
                        position.Move.To = reader.IsDBNull(7) ? "" : reader.GetString(7).Trim();
                        position.ParentId = reader.IsDBNull(5) ? null : reader.GetGuid(5);
                        position.Positions = new List<Position>();
                        position.Plans = reader.IsDBNull(8) ? "" : reader.GetString(8).Trim();
                        position.Tags = reader.IsDBNull(9) ? new List<string>() : reader.GetString(9).Trim().Split(",").ToList();
                        position.IsKeyPosition = reader.IsDBNull(10) ? false : reader.GetInt32(10) == 1;
                        position.LastStudied = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);
                        position.Mistakes = reader.IsDBNull(12) ? 0 : reader.GetInt64(12);
                    }
                    else
                    {
                        return null;
                    }

                    reader.Close();
                }
            }

            if(depth > 0)
            {
                var childrenQuery = "select id from Position where parentId = @id;";
                childrenQuery = childrenQuery.Replace("@id", id.SqlOrNull());
                using (var command = new SqlCommand(childrenQuery, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var childGuids = new List<Guid>();
                        while (reader.Read())
                        {
                            childGuids.Add(reader.GetGuid(0));
                        }
                        reader.Close();

                        position.Positions = childGuids.Select(g => PrivateGetById(g, sqlConnection, depth - 1)).ToList();
                    }
                }
            }

            return position;
        }

        private string GenerateSaveInstructions(Position position, SqlConnection connection)
        {
            var checkExistsQuery = "select * from Position where id = @id".Replace("@id", position.Id.SqlOrNull());
            var exists = false;

            using (var command = new SqlCommand(checkExistsQuery, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    exists = reader.Read();
                    reader.Close();
                }

            }

            var query =  exists ? Update(position) : New(position);

            foreach (var child in position.Positions)
            {
                query += GenerateSaveInstructions(child, connection);
            }

            return query;
        }

        private string New(Position position)
        {
            var query = ("insert into Position (id,title,description,moveName,moveFEN,parentId,move_from,move_to,plans,tags,isKeyPosition,lastStudied,mistakes) "
                + "values ('@id',@title,@description,@moveName,@moveFEN,@parentId,@from,@to,@plans,@tags,@isKeyPosition,@lastStudied,@mistakes);\n");
            query = query.Replace("@id", position.Id.ToString())
                .Replace("@title", position.Title.SqlOrNull())
                .Replace("@description", position.Description.SqlOrNull())
                .Replace("@moveName", position.Move.Name.SqlOrNull())
                .Replace("@moveFEN", position.Move.FEN.SqlOrNull())
                .Replace("@parentId", position.ParentId.SqlOrNull())
                .Replace("@from", position.Move.From.SqlOrNull())
                .Replace("@to", position.Move.To.SqlOrNull())
                .Replace("@plans", position.Plans.SqlOrNull())
                .Replace("@tags", string.Join(",", position.Tags).SqlOrNull())
                .Replace("@isKeyPosition", position.IsKeyPosition ? "1" : "0")
                .Replace("@lastStudied", position.LastStudied.SqlOrNull())
                .Replace("@mistakes", position.Mistakes.ToString());

            return query;
        }

        private string Update(Position position)
        {
            var query = ("update position set title=@title,description=@description,move_from=@from,move_to=@to,plans=@plans,tags=@tags,isKeyPosition=@isKeyPosition,"
                +"lastStudied=@lastStudied,mistakes=@mistakes where id = @id;\n")
                .Replace("@title", position.Title.SqlOrNull())
                .Replace("@description", position.Description.SqlOrNull())
                .Replace("@id", position.Id.SqlOrNull())
                .Replace("@from", position.Move.From.SqlOrNull())
                .Replace("@to", position.Move.To.SqlOrNull())
                .Replace("@plans", position.Plans.SqlOrNull())
                .Replace("@tags", string.Join(",", position.Tags).SqlOrNull())
                .Replace("@isKeyPosition", position.IsKeyPosition ? "1" : "0")
                .Replace("@lastStudied", position.LastStudied.SqlOrNull())
                .Replace("@mistakes", position.Mistakes.ToString());

            return query;
        }


        private async Task<string> GenerateCascadeDelete(Guid id)
        {
            var query = "delete from Position where id = @id;\n";
            query = query.Replace("@id", id.SqlOrNull());

            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                var childrenQuery = "select id from Position where parentId = @parentId;";
                childrenQuery = childrenQuery.Replace("@parentId", id.SqlOrNull());

                await connection.OpenAsync();

                using (var command = new SqlCommand(childrenQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                query += await GenerateCascadeDelete(reader.GetGuid(0));
                            }
                    }
                }

                connection.Close();
            }

            return query;
        }
    }
}
