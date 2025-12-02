using chess.api.common;
using chess.api.configuration;
using chess.api.models;
using Microsoft.Data.SqlClient;

namespace chess.api.dal
{
    public class BookDal
    {
        private readonly string _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=chess;Trusted_Connection=True;TrustServerCertificate=True";

        public async Task<IList<Book>> GetBooksByStudyId(Guid studyId)
        {
            var books = new List<Book>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select id,studyId,title,[order] from Book where studyId = @id".Replace("@id", studyId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new Book();
                            book.Id = reader.GetGuid(0);
                            book.StudyId = reader.GetGuid(1);
                            book.Title = reader.GetString(2);
                            book.Order = reader.GetInt32(3);
                            books.Add(book);
                        }
                        reader.Close();
                    }
                }

                connection.Close();
            }

            return books.OrderBy(b => b.Order).ToList();
        }

        public async Task<IList<Chapter>> GetChaptersByBookId(Guid bookId)
        {
            var chapters = new List<Chapter>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select id,bookId,title,[order] from Chapter where bookId = @id".Replace("@id", bookId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chapter = new Chapter();
                            chapter.Id = reader.GetGuid(0);
                            chapter.BookId = reader.GetGuid(1);
                            chapter.Title = reader.GetString(2);
                            chapter.Order = reader.GetInt32(3);
                            chapters.Add(chapter);
                        }
                        reader.Close();
                    }
                }

                connection.Close();
            }

            return chapters.OrderBy(c => c.Order).ToList();
        }

        public async Task<IList<Page>> GetPagesByChapterId(Guid chapterId)
        {
            var pages = new List<Page>();
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select id,bookId,title,[order],fen,text,fastForward,hide,plans from Page where bookId = @id".Replace("@id", chapterId.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var page = new Page();
                            page.Id = reader.GetGuid(0);
                            page.ChapterId = reader.GetGuid(1);
                            page.Title = reader.GetString(2);
                            page.Order = reader.GetInt32(3);
                            page.FEN = reader.GetString(4);
                            page.Text = reader.GetString(5);
                            page.FastForward = reader.GetInt32(6) == 1;
                            page.Hide = reader.GetInt32(7) == 1;
                            page.Plans = reader.GetString(8);
                            pages.Add(page);
                        }
                        reader.Close();
                    }
                }

                connection.Close();
            }

            return pages.OrderBy(p => p.Order).ToList();
        }

        public async Task SaveBook(Book book)
        {
            var exists = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select * from Book where id = @id".Replace("@id", book.Id.SqlOrNull());
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        exists = reader.Read();
                        reader.Close();
                    }
                }

                if (!exists)
                {
                    NewBook(book, connection);
                }
                else
                {
                    UpdateBook(book, connection);
                }

                connection.Close();
            }
        }

        private void NewBook(Book book, SqlConnection connection)
        {
            var query = $"insert into Book (id, studyId, title, order) values ({book.Id.SqlOrNull()},{book.StudyId.SqlOrNull()}, {book.Title.SqlOrNull()}, {book.Order})";
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
                var e = i;
            }
        }

        private void UpdateBook(Book book, SqlConnection connection)
        {
            var query = $"update Book set studyId={book.StudyId.SqlOrNull()}, title={book.Title.SqlOrNull()}, order={book.Order} where id = {book.Id.SqlOrNull()}";
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
                var e = i;
            }
        }

        public async Task SaveChapter(Chapter chapter)
        {
            var exists = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select * from Chapter where id = @id".Replace("@id", chapter.Id.SqlOrNull());
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
                    UpdateChapter(chapter, connection);
                }else
                {
                    NewChapter(chapter, connection);
                }

                connection.Close();
            }
        }

        private void NewChapter(Chapter chapter, SqlConnection connection)
        {
            var query = "insert into Chapter (id, bookId, title, [order]) "
                +$"values ({chapter.Id.SqlOrNull()},{chapter.BookId.SqlOrNull()},{chapter.Title.SqlOrNull()},{chapter.Order})";
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
            }
        }

        private void UpdateChapter(Chapter chapter, SqlConnection connection)
        {
            var query = $"update Chapter set title={chapter.Title.SqlOrNull()}, [order]={chapter.Order} where id = {chapter.Id.SqlOrNull()}";
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
            }
        }



        public async Task SavePage(Page page)
        {
            var exists = false;
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var query = "select * from Page where id = @id".Replace("@id", page.Id.SqlOrNull());
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
                    UpdatePage(page, connection);
                }
                else
                {
                    NewPage(page, connection);
                }

                connection.Close();
            }
        }

        private void NewPage(Page page, SqlConnection connection)
        {
            var query = "insert into Page (id, chapterId, title, [order], FEN, [Text], FastForward, Hide, Plans) "
                + "values (@id,@chapterId,@title,@order,@fen,@text,@fastorWard,@hide,@plans)"
                .Replace("@id",page.Id.SqlOrNull())
                .Replace("@chapterId",page.ChapterId.SqlOrNull())
                .Replace("@title",page.Title.SqlOrNull())
                .Replace("@order",page.Order.ToString())
                .Replace("@fen",page.FEN.SqlOrNull())
                .Replace("@text",page.Text.SqlOrNull())
                .Replace("@fastForward", (page.FastForward ? 1 : 0).ToString())
                .Replace("@hide", (page.Hide ? 1 : 0).ToString())
                .Replace("@plans",page.Plans.SqlOrNull());
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
            }
        }

        private void UpdatePage(Page page, SqlConnection connection)
        {
            var query = "update Page set title=@title,[order]=@order,fen=@fen,[text]=@text,fastForward=@fastForward,hide=@hide,plans=@plans "
                + "where id = @id"
                .Replace("@id", page.Id.SqlOrNull())
                .Replace("@title", page.Title.SqlOrNull())
                .Replace("@order", page.Order.ToString())
                .Replace("@fen", page.FEN.SqlOrNull())
                .Replace("@text", page.Text.SqlOrNull())
                .Replace("@fastForward", (page.FastForward ? 1 : 0).ToString())
                .Replace("@hide", (page.Hide ? 1 : 0).ToString())
                .Replace("@plans", page.Plans.SqlOrNull());
            using (var command = new SqlCommand(query, connection))
            {
                var i = command.ExecuteNonQuery();
            }
        }

        public async Task DeleteBook(Guid id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var queries = $"delete from page where chapterId in (select id from chapter where bookId = {id.SqlOrNull()});"
                    + $"delete from chapter where bookId = {id.SqlOrNull()};"
                    + $"delete from Book where id = {id.SqlOrNull()};";
                using (var command = new SqlCommand(queries, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public async Task DeleteChapter(Guid id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var queries = $"delete from page where chapterId = {id.SqlOrNull()};"
                    + $"delete from chapter where id = {id.SqlOrNull()};";
                using (var command = new SqlCommand(queries, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public async Task DeletePage(Guid id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();

                var queries = $"delete from page where id = {id.SqlOrNull()}";
                using (var command = new SqlCommand(queries, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
