using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ComicBoxApi.App.Imaging
{
    public static class FileInfoExtension
    {
        public static byte[] ToByteArray(this IFileInfo fileInfo)
        {
            using (Stream input = fileInfo.CreateReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
