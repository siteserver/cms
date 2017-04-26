using System;
using System.IO;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.IO
{
    public class FileWatcherClass
    {
        public FileWatcherClass(string fullFileName)
        {
            FileDependency(fullFileName);
        }

        private FileSystemWatcher _mFileSystemWatcher;
        public delegate void FileChange(object sender, EventArgs e);

        //The OnFileChange event is fired when the file changes.
        public event FileChange OnFileChange;

        public void FileDependency(string fullFileName)
        {
            //Validate file.
            var fileInfo = new FileInfo(fullFileName);
            if (!fileInfo.Exists)
            {
                FileUtils.WriteText(fullFileName, ECharset.utf_8, string.Empty);
            }

            //Get path from full file name.
            var path = Path.GetDirectoryName(fullFileName);

            //Get file name from full file name.
            var fileName = Path.GetFileName(fullFileName);

            //Initialize new FileSystemWatcher.
            _mFileSystemWatcher = new FileSystemWatcher
            {
                Path = path,
                Filter = fileName,
                EnableRaisingEvents = true
            };
            _mFileSystemWatcher.Changed += fileSystemWatcher_Changed;
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnFileChange?.Invoke(sender, e);
        }
    }
}
