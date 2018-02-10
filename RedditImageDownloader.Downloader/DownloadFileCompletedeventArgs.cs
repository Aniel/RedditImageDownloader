using System.ComponentModel;

namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public class DownloadFileCompletedeventArgs : RedditDownloaderEventBaseArgs
		{
			public DownloadFileCompletedeventArgs(AsyncCompletedEventArgs parent, string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
			{
				Parent = parent;
			}

			public AsyncCompletedEventArgs Parent { get; set; }
		}
	}
}