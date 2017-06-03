using ComicBoxApi.App.FileBrowser;
using ComicBoxApi.App.PdfReader;
using ComicBoxApi.App.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicBoxApi.App
{
    public interface IBookInfoService
    {
        BookContainer<Book> GetBookInfo(params string[] subpaths);
        PageDetail GetDetail(string category, string book, string chapter, int page);
        BookContainer<string> GetInfo(params string[] subpaths);
    }

    public class BookInfoService : IBookInfoService
    {
        private readonly IFilePathFinder _filePathFinder;
        private readonly IImageService _imageService;
        public static readonly string DefaultFileContainerExtension = ".pdf";

        public BookInfoService(IFilePathFinder filePathFinder, IImageService imageService)
        {
            _filePathFinder = filePathFinder;
            _imageService = imageService;
        }

        public BookContainer<string> GetInfo(params string[] subpaths)
        {
            _filePathFinder.SetPathContext(subpaths);

            var dirInfo = _filePathFinder.GetDirectoryContents(ListMode.OnlyDirectories);
            var thumbnail = new ThumbnailProvider(_filePathFinder, _imageService).GetThumbnail("thumbnail");
            return new BookContainer<string>(Convert.ToBase64String(thumbnail), dirInfo.Select(d => d.Name));
        }

        public BookContainer<Book> GetBookInfo(params string[] subpaths)
        {
            _filePathFinder.SetPathContext(subpaths);

            List<Book> books = new List<Book>();
            var dirInfo = _filePathFinder.GetDirectoryContents(ListMode.All);
            foreach (var container in dirInfo.Where(f => f.IsDirectory || DefaultFileContainerExtension.Equals(Path.GetExtension(f.Name))))
            {
                _filePathFinder.SetPathContext(subpaths);
                if (container.IsDirectory)
                {
                    _filePathFinder.AppendPathContext(container.Name);
                }
                var thumbnail = new ThumbnailProvider(_filePathFinder, _imageService).GetThumbnail(container.Name);
                books.Add(new Book(container.Name, Convert.ToBase64String(thumbnail)));
            }

            return new BookContainer<Book>(string.Empty, books);
        }

        public PageDetail GetDetail(string category, string book, string chapter, int page)
        {
            _filePathFinder.SetPathContext(category, book, chapter);

            using (PdfReaderService pdfReader = new PdfReaderService(_filePathFinder.GetPath().AbsolutePath))
            {
                var image = pdfReader.ReadImageAtPage(page);

                string fileContent = Convert.ToBase64String(image);
                return new PageDetail(chapter, page)
                    .WithContent(fileContent)
                    .WithPrevious(GetPreviousPageAndChapter(pdfReader, page, chapter))
                    .WithNext(GetNextPageAndChapter(pdfReader, page, chapter));
            }
        }

        private PageDetail GetPreviousPageAndChapter(PdfReaderService pdfReader, int page, string chapter)
        {
            PageDetail previousPageAndChapter;
            int previousPage = page - 1;

            if (pdfReader.IsPageExists(previousPage))
            {
                previousPageAndChapter = new PageDetail(chapter, previousPage);
            }
            else
            {
                var previousChapter = _filePathFinder.GetPreviousFileNameOrDefault(chapter, DefaultFileContainerExtension);
                if (previousChapter != null)
                {
                    int previousChapterLastPage = GetPreviousChapterLastPage(previousChapter.PhysicalPath);
                    previousPageAndChapter = new PageDetail(previousChapter.Name, previousChapterLastPage);
                }
                else
                {
                    previousPageAndChapter = PageDetail.NotFound;
                }
            }

            return previousPageAndChapter;
        }

        private static int GetPreviousChapterLastPage(string previousChapterFullPath)
        {
            using (PdfReaderService pdfReaderPreviousFile = new PdfReaderService(previousChapterFullPath))
            {
                return pdfReaderPreviousFile.GetLastPageNumber();
            }
        }

        private PageDetail GetNextPageAndChapter(PdfReaderService pdfReader, int page, string chapter)
        {
            PageDetail nextPageAndChapter;
            int nextPage = page + 1;

            if (pdfReader.IsPageExists(nextPage))
            {
                nextPageAndChapter = new PageDetail(chapter, nextPage);
            }
            else
            {
                var nextChapter = _filePathFinder.GetNextFileNameOrDefault(chapter, DefaultFileContainerExtension);
                if (nextChapter != null)
                {
                    const int nextChapterFirstPage = 1;
                    nextPageAndChapter = new PageDetail(nextChapter.Name, nextChapterFirstPage);
                }
                else
                {
                    nextPageAndChapter = PageDetail.NotFound;
                }
            }

            return nextPageAndChapter;
        }
    }
}
