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
		private readonly Catogory _catogory;

		private readonly string _fileNamePrefix;

		private readonly string _localPath;

		private readonly int _numberOfImages;

		private readonly string _subreddit;

		private string _currentFileName;

		private int _currentFileNumber = 1;

		private ImageTypes _currentImageType;

		private string _currentUri;

		public RedditImageDownloader(string localPath, string fileName, string subreddit, Catogory catogory, int numberOfImages)
		{
			this._localPath = localPath;
			_fileNamePrefix = fileName;
			_subreddit = subreddit;
			_catogory = catogory;
			_numberOfImages = numberOfImages;
		}

		public event EventHandler<RedditImageDownloaderEvents.DownloadChangedEventArgs> DownloadProgressChangedEvent;

		public event EventHandler<RedditImageDownloaderEvents.DownloadStartedEventArgs> DownloadStartetEvent;

		public event EventHandler<RedditImageDownloaderEvents.RedditDownloaderEventBaseArgs> FileDonwloadCompletedEvent;

		public event EventHandler<RedditImageDownloaderEvents.FileAlradyThereEventArgs> FileAlradyThereEvent;

		public enum Catogory
		{
			UnmoderatedLinks,
			Posts,
			Hot,
			New
		}

		public void SaveImages()
		{
			var reddit = new Reddit();
			var subreddit = reddit.GetSubreddit(_subreddit);
			var posts = _getSelectedPosts(subreddit, _catogory);
			_currentFileNumber = 1;
			foreach (var post in posts)
			{
				if (!(_currentFileNumber <= _numberOfImages))
					return;
				if (_checkUrl(post.Url.ToString()))
				{
					using (var client = new WebClient())
					{
						_currentUri = post.Url.ToString();
						_fileNameBuilder();
						var filePathName = _localPath + _currentFileName;
						if (!File.Exists(filePathName))
						{
							client.DownloadFile(post.Url, filePathName);
							_raiseFileDownloadCompleteEvent(this, null);
							_currentFileNumber++;
						}
						else
						{
							_raiseFileAlreadyThereEvent(this);
						}
					}
				}
			}
		}

		public async Task SaveImagesAsync()
		{
			var reddit = new Reddit();
			var subreddit = await reddit.GetSubredditAsync(_subreddit).ConfigureAwait(false);
			var posts = _getSelectedPosts(subreddit, _catogory);
			_currentFileNumber = 1;
			foreach (var post in posts)
			{
				if (!(_currentFileNumber <= _numberOfImages))
					return;
				if (_checkUrl(post.Url.ToString()))
				{
					using (var client = new WebClient())
					{
						client.DownloadProgressChanged += _onDownloadProgressChanged;
						client.DownloadFileCompleted += _raiseFileDownloadCompleteEvent;
						_currentUri = post.Url.ToString();
						_fileNameBuilder();
						var filePathName = _localPath + _currentFileName;
						if (!File.Exists(filePathName))
						{
							_raiseDownloadStartetEvent(this);
							await client.DownloadFileTaskAsync(post.Url, filePathName).ConfigureAwait(false);
							_currentFileNumber++;
						}
						else
						{
							_raiseFileAlreadyThereEvent(this);
						}
					}
				}
			}
		}

		private bool _checkIfUrlIsImage(string url)
		{
			foreach (ImageTypes type in ImageTypesHelper.GetValues())
			{
				if (type != ImageTypes.None && url.EndsWith(type.GetEnding(), StringComparison.Ordinal))
				{
					_currentImageType = type;
					return true;
				}
			}
			return false;
		}

		private bool _checkUrl(string url) => _checkIfUrlIsImage(url) && !string.IsNullOrEmpty(url);

		private void _fileNameBuilder()
		{
			_currentFileName = $"{_fileNamePrefix}-{_currentUri.GetHashCode()}{_currentImageType.GetEnding()}";
		}

		private IEnumerable<Post> _getSelectedPosts(Subreddit subreddit, Catogory selectedCategory)
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

		private void _onDownloadProgressChanged(object sender,
			DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
		{
			DownloadProgressChangedEvent?.Invoke(sender, new RedditImageDownloaderEvents.DownloadChangedEventArgs(downloadProgressChangedEventArgs, _currentFileName, _currentUri, _currentFileNumber));
		}

		private void _raiseDownloadStartetEvent(object sender)
		{
			DownloadStartetEvent?.Invoke(sender, new RedditImageDownloaderEvents.DownloadStartedEventArgs(_currentFileName, _currentUri, _currentFileNumber));
		}

		private void _raiseFileAlreadyThereEvent(object sender)
		{
			FileAlradyThereEvent?.Invoke(sender, new RedditImageDownloaderEvents.FileAlradyThereEventArgs(_currentFileName, _currentUri, _currentFileNumber));
		}

		private void _raiseFileDownloadCompleteEvent(object sender, AsyncCompletedEventArgs args)
		{
			FileDonwloadCompletedEvent?.Invoke(sender, new RedditImageDownloaderEvents.DownloadFileCompletedeventArgs(args, _currentFileName, _currentUri, _currentFileNumber));
		}
	}
}