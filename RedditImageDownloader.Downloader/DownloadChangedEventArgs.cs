using System.Net;

namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public class DownloadChangedEventArgs : RedditDownloaderEventBaseArgs
		{
			public DownloadChangedEventArgs(DownloadProgressChangedEventArgs parent, string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
			{
				Parent = parent;
			}

			public DownloadProgressChangedEventArgs Parent { get; set; }
		}
	}
}