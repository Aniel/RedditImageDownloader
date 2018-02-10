namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public class DownloadStartedEventArgs : RedditDownloaderEventBaseArgs
		{
			public DownloadStartedEventArgs(string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
			{
			}
		}
	}
}