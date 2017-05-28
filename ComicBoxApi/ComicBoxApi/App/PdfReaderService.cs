using iTextSharp.text.pdf;
using System;
using System.Linq;

namespace ComicBoxApi.App
{
    public interface IPdfReaderService : IDisposable
    {
        byte[] ReadImageFirstPage();

        byte[] ReadImageAtPage(int page);
    }

    public class PdfReaderService : IPdfReaderService
    {
        private readonly PdfReader _pdfReader;

        public PdfReaderService(string filename)
        {
            _pdfReader = new PdfReader(filename);
        }

        public byte[] ReadImageFirstPage()
        {
            return ReadImageAtPage(1);
        }

        public byte[] ReadImageAtPage(int page)
        {
            var currentPage = _pdfReader.GetPageN(page);
            var resources = (PdfDictionary)PdfReader.GetPdfObject(currentPage.Get(PdfName.Resources));
            var xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.Xobject));
            var pdfName = xobject.Keys.OfType<PdfName>().Single();
            var pdfObject = (PrIndirectReference)xobject.Get(pdfName);
            var stream = (PrStream)_pdfReader.GetPdfObject(pdfObject.Number);
            return PdfReader.GetStreamBytesRaw(stream);
        }

        public void Dispose()
        {
            _pdfReader.Close();
        }
    }
}
