using ComicBoxApi.App.Cache;
using Microsoft.AspNetCore.Mvc;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookInfoService _bookInfoService;

        private readonly ICacheService _cacheService;

        public BookController(IBookInfoService bookInfoService, ICacheService cacheService)
        {
            _bookInfoService = bookInfoService;
            _cacheService = cacheService;
        }

        [HttpGet("{category}/{pagination}")]
        public BookContainer<Book> Get(string category, int pagination)
        {
            string cacheKey = $"allBooksForCategory_{category}";
            return _cacheService.LoadFromCache(cacheKey, () => _bookInfoService.GetBookInfo(category))
                .WithPagination(pagination);
        }
        
        [HttpGet("{category}/{book}/{pagination}")]
        public BookContainer<Book> Get(string category, string book, int pagination)
        {
            string cacheKey = $"chaptersForBook_{category}_{book}";
            return _cacheService.LoadFromCache(cacheKey, () => _bookInfoService.GetBookInfo(category, book))
                .WithPagination(pagination);
        }

        [HttpGet("{category}/{book}/{chapter}/{page}")]
        public PageDetail Get(string category, string book, string chapter, int page)
        {
            return _bookInfoService.GetDetail(category, book, chapter, page);
        }
    }
}
