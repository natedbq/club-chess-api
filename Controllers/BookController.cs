using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using chess.api.Validations;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly BookDal _bookDal;

        public BookController(ILogger<BookController> logger)
        {
            _logger = logger;
            _bookDal = new BookDal();
        }

        //[Authorize]
        [HttpGet("/study/{id}")]
        public async Task<IList<Book>> GetByStudyId(Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Study.UserCanViewStudy(userId, studyId);

            var books = await _bookDal.GetBooksByStudyId(studyId);

            var tasks = books.Select(async (book) =>
            {
                var chapters = await _bookDal.GetChaptersByBookId(book.Id);
                var pageTasks = chapters.Select(async (chapter) =>
                {
                    chapter.Pages = await _bookDal.GetPagesByChapterId(chapter.Id);
                });

                Task.WaitAll(pageTasks.ToArray());
                book.Chapters = chapters;
            });

            Task.WaitAll(tasks.ToArray());

            return books;
        }

        [HttpPost("delete/{id}")]
        public async Task DeleteBook(Guid id)
        {
            BusinessValidation.Book.UserCanEditBook(GetUserId(), id);
            await _bookDal.DeleteBook(id);
        }

        [HttpPost("delete/chapter/{id}")]
        public async Task DeleteChapter(Guid id)
        {
            BusinessValidation.Book.UserCanEditChapter(GetUserId(), id);
            await _bookDal.DeleteChapter(id);
        }

        [HttpPost("delete/chapter/page/{id}")]
        public async Task DeletePage(Guid id)
        {
            BusinessValidation.Book.UserCanEditPage(GetUserId(), id);
            await _bookDal.DeletePage(id);
        }

        [HttpPost("")]
        public async Task SaveBook(Book book)
        {
            await _bookDal.SaveBook(book);
        }

        [HttpPost("chapter")]
        public async Task SaveChapter(Chapter chapter)
        {
            await _bookDal.SaveChapter(chapter);
        }

        [HttpPost("chapter/page")]
        public async Task SavePage(Page page)
        {
            await _bookDal.SavePage(page);
        }


        private Guid GetUserId()
        {
            return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
    }
}
