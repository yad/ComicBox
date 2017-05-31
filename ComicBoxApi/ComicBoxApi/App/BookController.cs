using ComicBoxApi.App.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IFileProvider _fileProvider;

        private readonly ICacheService _cacheService;

        public BookController(IFileProvider fileProvider, ICacheService cacheService)
        {
            _fileProvider = fileProvider;
            _cacheService = cacheService;
        }

        [HttpGet("")]
        public BookContainer<string> Get()
        {
            return new BookInfoService(_fileProvider).GetInfo();
        }

        [HttpGet("{category}/{pagination}")]
        public BookContainer<Book> Get(string category, int pagination)
        {
            string cacheKey = $"allBooksForCategory_{category}";
            return _cacheService.LoadFromCache(cacheKey, () => new BookInfoService(_fileProvider).GetBookInfo(category))
                .WithPagination(pagination);
        }
        
        [HttpGet("{category}/{book}/{pagination}")]
        public BookContainer<Book> Get(string category, string book, int pagination)
        {
            string cacheKey = $"chaptersForBook_{category}_{book}";
            return _cacheService.LoadFromCache(cacheKey, () => new BookInfoService(_fileProvider).GetBookInfo(category, book))
                .WithPagination(pagination);
        }

        //[HttpGet("{category}/{book}/{chapter}")]
        //public BookContainer<string> Get(string category, string book, string chapter)
        //{
        //    return new BookInfoService(_fileProvider).GetInfo(category, book, chapter);
        //}

        [HttpGet("{category}/{book}/{chapter}/{page}")]
        public PageDetail Get(string category, string book, string chapter, int page)
        {
            return new BookInfoService(_fileProvider).GetDetail(category, book, chapter, page);
        }
    }
}
