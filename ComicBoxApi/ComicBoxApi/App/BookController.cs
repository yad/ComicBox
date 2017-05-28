using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IFileProvider _fileProvider;

        public BookController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpGet("")]
        public BookContainer<string> Get()
        {
            return new BookInfoService(_fileProvider).GetInfo();
        }

        [HttpGet("{category}")]
        public BookContainer<Book> Get(string category)
        {
            return new BookInfoService(_fileProvider).GetBookInfo(category);
        }

        [HttpGet("{category}/{book}")]
        public BookContainer<Book> Get(string category, string book)
        {
            return new BookInfoService(_fileProvider).GetBookInfo(category, book);
        }

        [HttpGet("{category}/{book}/{chapter}")]
        public BookContainer<string> Get(string category, string book, string chapter)
        {
            return new BookInfoService(_fileProvider).GetInfo(category, book, chapter);
        }

        [HttpGet("{category}/{book}/{chapter}/{page}")]
        public PageDetail Get(string category, string book, string chapter, int page)
        {
            return new BookInfoService(_fileProvider).GetDetail(category, book, chapter, page);
        }
    }
}
