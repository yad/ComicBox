using ComicBoxApi.App.FileBrowser;
using ComicBoxApi.App.Thumbnail;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicBoxApi.App
{
    public class BookInfoService
    {
        private readonly PathFinder _pathFinder;

        public static readonly string DefaultFileContainerExtension = ".pdf";

        public BookInfoService(IFileProvider fileProvider)
        {
            _pathFinder = new PathFinder(fileProvider);
        }

        public BookContainer<string> GetInfo(params string[] subpaths)
        {
            _pathFinder.SetPathContext(subpaths);

            var dirInfo = _pathFinder.GetDirectoryContents(ListMode.OnlyDirectories);
            var thumbnail = new ThumbnailProvider(_pathFinder).GetThumbnail("thumbnail");
            return new BookContainer<string>(Convert.ToBase64String(thumbnail), dirInfo.Select(d => d.Name));
        }

        public BookContainer<Book> GetBookInfo(params string[] subpaths)
        {
            _pathFinder.SetPathContext(subpaths);

            List<Book> books = new List<Book>();
            var dirInfo = _pathFinder.GetDirectoryContents(ListMode.All);
            foreach(var container in dirInfo.Where(f => f.IsDirectory || DefaultFileContainerExtension.Equals(Path.GetExtension(f.Name))))
            {
                _pathFinder.SetPathContext(subpaths);
                if (container.IsDirectory)
                {
                    _pathFinder.AppendPathContext(container.Name);
                }
                var thumbnail = new ThumbnailProvider(_pathFinder).GetThumbnail(container.Name);
                books.Add(new Book(container.Name, Convert.ToBase64String(thumbnail)));
            }

            return new BookContainer<Book>(string.Empty, books);
        }

        public PageDetail GetDetail(string category, string book, string chapter, int page)
        {
            _pathFinder.SetPathContext(category, book, chapter);

            using (PdfReaderService pdfReader = new PdfReaderService(Path.Combine(PathFinder.AbsoluteBasePath, _pathFinder.GetRelativePath())))
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
                nextPageOrChapter = _pathFinder.GetNextFileNameOrDefault(chapter, DefaultFileContainerExtension);
            }

            return nextPageOrChapter;
        }
    }
}
