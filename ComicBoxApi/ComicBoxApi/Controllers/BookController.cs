using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicBoxApi.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IFileProvider _fileProvider;

        public BookController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return GetDirectoryContents();
        }

        [HttpGet("{book}")]
        public IEnumerable<string> Get(string book)
        {
            return GetDirectoryContents(book);
        }

        [HttpGet("{book}/{chapter}")]
        public IEnumerable<string> Get(string book, string chapter)
        {
            return GetDirectoryContents(book, chapter);
        }

        [HttpGet("{book}/{chapter}/{page}")]
        public string Get(string book, string chapter, int page)
        {
            byte[] bytes = new byte[0];
            var subpath = CombinePath(book, chapter);
            var file = _fileProvider.GetFileInfo(subpath).PhysicalPath;
            using (PdfReaderService pdfReader = new PdfReaderService(file))
            {
                var image = pdfReader.ReadImageAtPage(page);
                return Convert.ToBase64String(image);
            }
        }

        private static readonly string basePath = "Ebooks";

        private string CombinePath(params string[] subpaths)
        {
            List<string> allPaths = subpaths.ToList();
            allPaths.Insert(0, basePath);

            return Path.Combine(allPaths.ToArray());
        }

        private IEnumerable<string> GetDirectoryContents(params string[] subpaths)
        {
            var subpath = CombinePath(subpaths);

            return _fileProvider.GetDirectoryContents(subpath).Select(d => d.Name);
        }
    }
}
