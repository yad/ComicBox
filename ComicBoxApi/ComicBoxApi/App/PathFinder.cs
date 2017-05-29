using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicBoxApi.App
{
    public enum ListMode
    {
        All,
        OnlyDirectories,
        OnlyFiles
    }

    public class PathFinder
    {
        private readonly List<string> _subpaths;

        private readonly IFileProvider _fileProvider;

        public static string AbsoluteBasePath { get; set; }

        public PathFinder(IFileProvider fileProvider)
        {
            _subpaths = new List<string>();
            _fileProvider = fileProvider;
        }

        public void SetPathContext(params string[] subpaths)
        {
            _subpaths.Clear();
            AppendPathContext(subpaths);
        }

        public void AppendPathContext(params string[] subpaths)
        {
            _subpaths.AddRange(subpaths);
        }

        public string GetRelativePath()
        {
            return Path.Combine(_subpaths.ToArray());
        }

        public string GetThumbnailPathForFile(string file)
        {
            return Path.Combine(GetRelativePath(), file);
        }

        public IReadOnlyCollection<IFileInfo> GetDirectoryContents(ListMode listMode)
        {
            IEnumerable<IFileInfo> directoryContents = _fileProvider.GetDirectoryContents(GetRelativePath());

            switch(listMode)
            {
                case ListMode.OnlyDirectories:
                    directoryContents = directoryContents.Where(f => f.IsDirectory);
                    break;
                case ListMode.OnlyFiles:
                    directoryContents = directoryContents.Where(f => !f.IsDirectory);
                    break;
                default:
                    break;
            }

            return directoryContents.ToArray();
        }

        public IFileInfo GetThumbnailFileInfoForFile(string file)
        {
            return _fileProvider.GetFileInfo(GetThumbnailPathForFile(file));
        }

        public string LocateFile(string file)
        {
            return Path.Combine(GetRelativePath(), file);
        }

        public string LocateFirstFile(string matchExtension)
        {
            PathFinder pathFinder = new PathFinder(_fileProvider);
            pathFinder.SetPathContext(_subpaths.ToArray());

            return LocateFirstFile(pathFinder, string.Empty, matchExtension);
        }

        private static string LocateFirstFile(PathFinder pathFinder, string appendPath, string matchExtension)
        {
            pathFinder.AppendPathContext(appendPath);
            var currentDir = pathFinder.GetDirectoryContents(ListMode.All);
            var firstPdf = currentDir.FirstOrDefault(f => matchExtension.Equals(Path.GetExtension(f.Name)));
            if (firstPdf != null)
            {
                return Path.Combine(pathFinder.GetRelativePath(), firstPdf.Name);
            }
            else
            {
                return LocateFirstFile(pathFinder, currentDir.First(f => f.IsDirectory).Name, matchExtension);
            }
        }
    }
}
