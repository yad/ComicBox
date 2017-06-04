using ComicBoxApi.App.Cache;
using ComicBoxApi.App.Worker;
using Microsoft.AspNetCore.Mvc;

namespace ComicBoxApi.App
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookInfoService _bookInfoService;

        private readonly ICacheService _cacheService;

        private readonly ThumbnailWorker _thumbnailWorker;

        public BookController(IBookInfoService bookInfoService, ICacheService cacheService, ThumbnailWorker thumbnailWorker)
        {
            _bookInfoService = bookInfoService;
            _cacheService = cacheService;
            _thumbnailWorker = thumbnailWorker;
        }

        [HttpGet("{category}/{pagination}")]
        public BookContainer<Book> Get(string category, int pagination)
        {
            string cacheKey = $"allBooksForCategory_{category}";
            var thumbnailWorkerStatus = _thumbnailWorker.GetStatus();
            return _cacheService.TryLoadFromCache(cacheKey, () => _bookInfoService.GetBookInfo(category), !thumbnailWorkerStatus.IsInProgress)
                .WithPagination(pagination);
        }
        
        [HttpGet("{category}/{book}/{pagination}")]
        public BookContainer<Book> Get(string category, string book, int pagination)
        {
            string cacheKey = $"chaptersForBook_{category}_{book}";
            var thumbnailWorkerStatus = _thumbnailWorker.GetStatus();
            return _cacheService.TryLoadFromCache(cacheKey, () => _bookInfoService.GetBookInfo(category, book), !thumbnailWorkerStatus.IsInProgress)
                .WithPagination(pagination);
        }

        [HttpGet("{category}/{book}/{chapter}/{page}")]
        public PageDetail Get(string category, string book, string chapter, int page)
        {
            return _bookInfoService.GetDetail(category, book, chapter, page);
        }
    }
}
