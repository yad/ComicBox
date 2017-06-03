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
            string file = string.Format("{0}.jpg", name);
            var fileInfo = _pathFinder.GetThumbnailFileInfoForFile(file);
            if (!fileInfo.Exists)
            {
                var defaultFileContainerExtension = BookInfoService.DefaultFileContainerExtension;
                var path = name.Contains(defaultFileContainerExtension) ? _pathFinder.LocateFile(name) : _pathFinder.LocateFirstFile(defaultFileContainerExtension);
                using (StreamWriter sw = new StreamWriter(_pathFinder.LocateFile(file).AbsolutePath))
                {
                    var fileContent = new PdfReaderService(path.AbsolutePath).ReadImageFirstPage();
                    var thumbnailContent = _imageService.ScaleAsThumbnail(fileContent);
                    sw.BaseStream.Write(thumbnailContent, 0, thumbnailContent.Length);
                }

                fileInfo = _pathFinder.GetThumbnailFileInfoForFile(file);
            }

            return fileInfo;
        }        
    }
}
