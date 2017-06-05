using Microsoft.AspNetCore.Mvc;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class ThumbnailController : Controller
    {
        private readonly IBookInfoService _bookInfoService;

        public ThumbnailController(IBookInfoService bookInfoService)
        {
            _bookInfoService = bookInfoService;
        }

        [HttpGet("{category}/{pagination}")]
        public BookContainer<Book> Get(string category, int pagination)
        {
            return _bookInfoService.GetBookThumbnails(category).WithPagination(pagination);
        }
        
        [HttpGet("{category}/{book}/{pagination}")]
        public BookContainer<Book> Get(string category, string book, int pagination)
        {
            return _bookInfoService.GetBookThumbnails(category, book).WithPagination(pagination);
        }
    }
}
