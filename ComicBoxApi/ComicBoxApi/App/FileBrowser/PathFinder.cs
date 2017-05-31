using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace ComicBoxApi.App.FileBrowser
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

        private string LocateFirstFile(PathFinder pathFinder, string appendPath, string matchExtension)
        {
            pathFinder.AppendPathContext(appendPath);
            var currentDir = pathFinder.GetDirectoryContents(ListMode.All);
            var firstFile = currentDir.FirstOrDefault(f => matchExtension.Equals(Path.GetExtension(f.Name)));
            if (firstFile != null)
            {
                return Path.Combine(pathFinder.GetRelativePath(), firstFile.Name);
            }
            else
            {
                return LocateFirstFile(pathFinder, currentDir.First(f => f.IsDirectory).Name, matchExtension);
            }
        }

        public bool IsNextFileExists(string file, string matchExtension)
        {
            return !string.IsNullOrEmpty(LocateNextFile(file, matchExtension));
        }

        private string LocateNextFile(string file, string matchExtension)
        {
            PathFinder pathFinder = new PathFinder(_fileProvider);
            pathFinder.SetPathContext(_subpaths.TakeWhile(subpath => !subpath.Equals(file)).ToArray());
            var currentDir = pathFinder.GetDirectoryContents(ListMode.OnlyFiles).Where(f => matchExtension.Equals(Path.GetExtension(f.Name)));
            var nextFile = currentDir.SkipWhile(f => !f.Name.Equals(file)).Skip(1).FirstOrDefault();
            return Path.Combine(pathFinder.GetRelativePath(), nextFile.Name);
        }
    }
}
