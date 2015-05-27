using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
    public class RedditImageDownloader
    {
        #region events
        public event RedditImageDownloaderEvents.DownloadProgressChangedEvent DownloadProgressChangedEvent;
        public event RedditImageDownloaderEvents.DownloadStartedEvent DownloadStartetEvent;
        public event RedditImageDownloaderEvents.DownloadFileCompletedEvent FileDonwloadCompletedEvent;
        public event RedditImageDownloaderEvents.FileAlradyThereEvent FileAlradyThereEvent;
        #endregion

        #region vars
        private string _localPath;
        private string _fileNamePrefix;
        private string _subreddit;
        private Catogory _catogory;
        private int _numberOfImages;
        private string currentFileName;
        private string currentUri;
        private ImageTypes currentImageType;
        private int currentFileNumber = 1;
        #endregion

        public RedditImageDownloader(string localPath, string fileName, string subreddit, Catogory catogory, int numberOfImages)
        {
            this._localPath = localPath;
            _fileNamePrefix = fileName;
            _subreddit = subreddit;
            _catogory = catogory;
            _numberOfImages = numberOfImages;
        }

        public async Task SaveImagesAsync()
        {
            var reddit = new Reddit();
            var subreddit = await reddit.GetSubredditAsync(_subreddit);
            var posts = GetSelectedPosts(subreddit, _catogory);
            currentFileNumber = 1;
            foreach (var post in posts)
            {
                if (!(currentFileNumber <= _numberOfImages))
                    return;
                if (checkUrl(post.Url.ToString()))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += OnDownloadProgressChanged;
                        client.DownloadFileCompleted += RaiseFileDownloadCompleteEvent;
                        currentUri = post.Url.ToString();
                        FileNameBuilder();
                        var filePathName = _localPath + currentFileName;
                        if (!File.Exists(filePathName))
                        {
                            RaiseDownloadStartetEvent(this);
                            await client.DownloadFileTaskAsync(post.Url, filePathName);
                            currentFileNumber++;
                        }
                        else
                        {
                            RaiseFileAlreadyThereEvent(this);
                        }
                    }
                }
            }
        }

        public void SaveImages()
        {
            var reddit = new Reddit();
            var subreddit = reddit.GetSubreddit(_subreddit);
            var posts = GetSelectedPosts(subreddit, _catogory);
            currentFileNumber = 1;
            foreach (var post in posts)
            {
                if (!(currentFileNumber <= _numberOfImages))
                    return;
                if (checkUrl(post.Url.ToString()))
                {
                    using (var client = new WebClient())
                    {
                        currentUri = post.Url.ToString();
                        FileNameBuilder();
                        var filePathName = _localPath + currentFileName;
                        if (!File.Exists(filePathName))
                        {
                            client.DownloadFile(post.Url, filePathName);
                            RaiseFileDownloadCompleteEvent(this, null);
                            currentFileNumber++;
                        }
                        else
                        {
                            RaiseFileAlreadyThereEvent(this);
                        }
                    }
                }
            }
        }


        private bool checkUrl(string url) => checkIfUrlIsImage(url) && !string.IsNullOrEmpty(url);

        private bool checkIfUrlIsImage(string url)
        {
            foreach (ImageTypes type in ImageTypesUtil.GetValues())
            {
                if (type != ImageTypes.None && url.EndsWith(type.getEnding()))
                {
                    currentImageType = type;
                    return true;
                }
            }
            return false;
        }

        #region Helper
        private IEnumerable<Post> GetSelectedPosts(Subreddit subreddit, Catogory selectedCategory)
        {
            switch (selectedCategory)
            {
                case Catogory.Hot:
                    return subreddit.Hot;
                case Catogory.New:
                    return subreddit.New;
                case Catogory.Posts:
                    return subreddit.Posts;
                case Catogory.UnmoderatedLinks:
                    return subreddit.UnmoderatedLinks;
                default:
                    throw new Exception("Please select a category for the search.");
            }
        }

        private void FileNameBuilder()
        {
            currentFileName = $"{_fileNamePrefix}-{currentUri.GetHashCode()}{currentImageType.getEnding()}";
        }

        public enum Catogory
        {
            UnmoderatedLinks,
            Posts,
            Hot,
            New
        }
        #endregion

        #region EventHandling
        private void RaiseFileAlreadyThereEvent(object sender)
        {
            if (FileAlradyThereEvent != null)
                FileAlradyThereEvent(sender, new RedditImageDownloaderEvents.FileAlradyThereEventArgs(currentFileName, currentUri, currentFileNumber));
        }
        private void RaiseFileDownloadCompleteEvent(object sender, AsyncCompletedEventArgs args)
        {
            if (FileDonwloadCompletedEvent != null)
                FileDonwloadCompletedEvent(sender, new RedditImageDownloaderEvents.DownloadFileCompletedeventArgs(args, currentFileName, currentUri, currentFileNumber));
        }

        private void RaiseDownloadStartetEvent(object sender)
        {
            if (DownloadStartetEvent != null) DownloadStartetEvent(sender, new RedditImageDownloaderEvents.DownloadStartedEventArgs(currentFileName, currentUri, currentFileNumber));
        }
        private void OnDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            if (DownloadProgressChangedEvent != null) DownloadProgressChangedEvent(sender, new RedditImageDownloaderEvents.DownloadChangedEventArgs(downloadProgressChangedEventArgs, currentFileName, currentUri, currentFileNumber));
        }
        #endregion
    }
}
