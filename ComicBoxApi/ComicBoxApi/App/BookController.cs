using Microsoft.AspNetCore.Mvc;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookInfoService _bookInfoService;

        public BookController(IBookInfoService bookInfoService)
        {
            _bookInfoService = bookInfoService;
        }

        [HttpGet("{category}")]
        public BookContainer<Book> Get(string category)
        {
            return _bookInfoService.GetBookList(category);
        }
        
        [HttpGet("{category}/{book}")]
        public BookContainer<Book> Get(string category, string book)
        {
            return _bookInfoService.GetBookList(category, book);
        }

        [HttpGet("{category}/{book}/{chapter}/{page}")]
        public PageDetail Get(string category, string book, string chapter, int page)
        {
            return _bookInfoService.GetDetail(category, book, chapter, page);
        }
    }
}
