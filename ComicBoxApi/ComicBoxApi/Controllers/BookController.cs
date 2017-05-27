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
        public string Get(string book, string chapter, string page)
        {
            byte[] bytes = new byte[0];
            var subpath = CombinePath(book, chapter);
            var file = _fileProvider.GetFileInfo(subpath).PhysicalPath;
            PdfReader pdfReader = new PdfReader(file);
            var pg = pdfReader.GetPageN(int.Parse(page));
            PdfObject obj = FindImageInPDFDictionary(pg);
            if (obj != null)
            {
                int XrefIndex = Convert.ToInt32(((PrIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                PrStream stream = (PrStream)pdfReader.GetPdfObject(XrefIndex);
                bytes = PdfReader.GetStreamBytesRaw(stream);                
            }

            pdfReader.Close();
            return Convert.ToBase64String(bytes);
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

        private PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.Resources));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.Xobject));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {

                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.Subtype));

                        //image at the root of the pdf
                        if (PdfName.Image.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.Form.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.Group.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }
                    }
                }
            }
            return null;
        }
    }
}
