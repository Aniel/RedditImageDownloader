using System;

namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public abstract class RedditDownloaderEventBaseArgs : EventArgs
		{
			protected RedditDownloaderEventBaseArgs(string fileName, string url, int imageNumber)
			{
				FileName = fileName;
				Url = url;
				ImageNumber = imageNumber;
			}

			public string FileName { get; set; }
			public int ImageNumber { get; set; }
			public string Url { get; set; }
		}
	}
}