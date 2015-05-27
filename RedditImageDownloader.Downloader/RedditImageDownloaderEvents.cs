using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
    public class RedditImageDownloaderEvents
    {
        public abstract class RedditDownloaderEventBaseArgs : EventArgs
        {
            public string FileName { get; set; }
            public string Url { get; set; }
            public int ImageNumber { get; set; }

            public RedditDownloaderEventBaseArgs(string fileName, string url, int imageNumber)
            {
                FileName = fileName;
                Url = url;
                ImageNumber = imageNumber;
            }
        }

        public delegate void DownloadProgressChangedEvent(object sender, DownloadChangedEventArgs args);
        public class DownloadChangedEventArgs : RedditDownloaderEventBaseArgs
        {
            public DownloadChangedEventArgs(DownloadProgressChangedEventArgs parent, string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
            {
                Parent = parent;
            }

            public DownloadProgressChangedEventArgs Parent { get; set; }
        }

        public delegate void DownloadFileCompletedEvent(object sender, RedditDownloaderEventBaseArgs args);
        public class DownloadFileCompletedeventArgs : RedditDownloaderEventBaseArgs
        {
            public DownloadFileCompletedeventArgs(AsyncCompletedEventArgs parent, string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
            {
                Parent = parent;
            }

            public AsyncCompletedEventArgs Parent { get; set; }
        }

        public delegate void DownloadStartedEvent(object sender, DownloadStartedEventArgs args);
        public class DownloadStartedEventArgs : RedditDownloaderEventBaseArgs
        {
            public DownloadStartedEventArgs(string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
            {
            }
        }

        public delegate void FileAlradyThereEvent(object sender, FileAlradyThereEventArgs args);
        public class FileAlradyThereEventArgs : RedditDownloaderEventBaseArgs
        {
            public FileAlradyThereEventArgs(string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
            {
            }
        }
    }
}
