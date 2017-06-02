using ComicBoxApi.App.FileBrowser;
using ComicBoxApi.App.Thumbnail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
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

        public static readonly string DefaultFileContainerExtension = ".pdf";

        public BookInfoService(IFilePathFinder filePathFinder)
        {
            _filePathFinder = filePathFinder;
        }

        public BookContainer<string> GetInfo(params string[] subpaths)
        {
            _filePathFinder.SetPathContext(subpaths);

            var dirInfo = _filePathFinder.GetDirectoryContents(ListMode.OnlyDirectories);
            var thumbnail = new ThumbnailProvider(_filePathFinder).GetThumbnail("thumbnail");
            return new BookContainer<string>(Convert.ToBase64String(thumbnail), dirInfo.Select(d => d.Name));
        }

        public BookContainer<Book> GetBookInfo(params string[] subpaths)
        {
            _filePathFinder.SetPathContext(subpaths);

            List<Book> books = new List<Book>();
            var dirInfo = _filePathFinder.GetDirectoryContents(ListMode.All);
            foreach(var container in dirInfo.Where(f => f.IsDirectory || DefaultFileContainerExtension.Equals(Path.GetExtension(f.Name))))
            {
                _filePathFinder.SetPathContext(subpaths);
                if (container.IsDirectory)
                {
                    _filePathFinder.AppendPathContext(container.Name);
                }
                var thumbnail = new ThumbnailProvider(_filePathFinder).GetThumbnail(container.Name);
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
                string nextPageOrChapter = GetNextPageOrChapter(pdfReader, page + 1, chapter);

                string fileContent = Convert.ToBase64String(image);
                return new PageDetail(fileContent, nextPageOrChapter);
            }            
        }

        private string GetNextPageOrChapter(PdfReaderService pdfReader, int nextPage, string chapter)
        {
            string nextPageOrChapter;

            if (pdfReader.IsPageExists(nextPage))
            {
                nextPageOrChapter = "#NEXT_PAGE#";
            }
            else
            {
                nextPageOrChapter = _filePathFinder.GetNextFileNameOrDefault(chapter, DefaultFileContainerExtension);
            }

            return nextPageOrChapter;
        }
    }
}
