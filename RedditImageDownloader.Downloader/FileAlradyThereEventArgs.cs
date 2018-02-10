namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public class FileAlradyThereEventArgs : RedditDownloaderEventBaseArgs
		{
			public FileAlradyThereEventArgs(string fileName, string url, int imageNumber) : base(fileName, url, imageNumber)
			{
			}
		}
	}
}