using System.IO;

namespace ComicBoxApi.App.FileBrowser
{
    public class FilePath
    {
        public string RelativePath { get; private set; }

        public string AbsolutePath { get; private set; }

        public FilePath(string rootPath, params string[] relativePaths)
        {
            RelativePath = Path.Combine(relativePaths);
            AbsolutePath = Path.Combine(rootPath, RelativePath);
        }
    }
}
