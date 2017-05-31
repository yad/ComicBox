using ComicBoxApi.App.FileBrowser;
using Microsoft.Extensions.FileProviders;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ComicBoxApi.App.Thumbnail
{
    public class ThumbnailProvider
    {
        private readonly PathFinder _pathFinder;

        public ThumbnailProvider(PathFinder pathFinder)
        {
            _pathFinder = pathFinder;
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
                using (StreamWriter sw = new StreamWriter(Path.Combine(PathFinder.AbsoluteBasePath, _pathFinder.GetThumbnailPathForFile(file))))
                {
                    var fileContent = new PdfReaderService(Path.Combine(PathFinder.AbsoluteBasePath, path)).ReadImageFirstPage();
                    using (MemoryStream ms = new MemoryStream(fileContent))
                    {
                        ms.Position = 0;
                        var thumbnailContent = ScaleAsThumbnail(ms);
                        sw.BaseStream.Write(thumbnailContent, 0, thumbnailContent.Length);
                    }
                }

                fileInfo = _pathFinder.GetThumbnailFileInfoForFile(file);
            }

            return fileInfo;
        }

        public byte[] ScaleAsThumbnail(Stream image)
        {
            byte[] result;

            using (var fullBitmap = Bitmap.FromStream(image))
            {
                int sourceWidth = fullBitmap.Width;
                int sourceHeight = fullBitmap.Height;
                int sourceX = 0;
                int sourceY = 0;

                int destX = 0;
                int destY = 0;
                int destWidth = 0;
                int destHeight = 0;

                float dpi = 72.0f;
                float ratio = 2.54f / dpi;
                int maxside = (int)(5.2f / ratio);

                destHeight = maxside;
                destWidth = destHeight * sourceWidth / sourceHeight;

                using (Bitmap thumbnailBitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format16bppRgb565))
                {
                    thumbnailBitmap.SetResolution(dpi, dpi);

                    using (var graphics = Graphics.FromImage(thumbnailBitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        graphics.DrawImage(fullBitmap,
                            new Rectangle(destX, destY, destWidth, destHeight),
                            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                            GraphicsUnit.Pixel);

                        result = thumbnailBitmap.ToByteArray();
                    }
                }

                return result;
            }

        }
    }
}
