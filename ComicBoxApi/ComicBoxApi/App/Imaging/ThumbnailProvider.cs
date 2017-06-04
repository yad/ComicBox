using ComicBoxApi.App.FileBrowser;
using ComicBoxApi.App.PdfReader;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ComicBoxApi.App.Imaging
{
    public class ThumbnailProvider
    {
        private readonly IFilePathFinder _pathFinder;

        private readonly IImageService _imageService;

        public ThumbnailProvider(IFilePathFinder pathFinder, IImageService imageService)
        {
            _pathFinder = pathFinder;
            _imageService = imageService;
        }

        public byte[] GetThumbnail(string name)
        {
            var fileInfo = EnsureThumbnail(name);
            return fileInfo.ToByteArray();
        }

        private IFileInfo EnsureThumbnail(string name)
        {
            string defaultFileContainerExtension = BookInfoService.DefaultFileContainerExtension;
            var filePath = name.Contains(defaultFileContainerExtension) ? _pathFinder.LocateFile(name) : _pathFinder.LocateFirstFile(defaultFileContainerExtension);
            string thumbnailFileName = string.Format("{0}.jpg", filePath.FileName);
            var thumbnailFile = _pathFinder.LocateFile(thumbnailFileName);            
            var fileInfo = _pathFinder.GetThumbnailFileInfoForFile(thumbnailFile);
            if (!fileInfo.Exists)
            {
                using (StreamWriter sw = new StreamWriter(thumbnailFile.AbsolutePath))
                {
                    var fileContent = new PdfReaderService(filePath.AbsolutePath).ReadImageFirstPage();
                    var thumbnailContent = _imageService.ScaleAsThumbnail(fileContent);
                    sw.BaseStream.Write(thumbnailContent, 0, thumbnailContent.Length);
                }

                fileInfo = _pathFinder.GetThumbnailFileInfoForFile(thumbnailFile);
            }

            return fileInfo;
        }        
    }
}
