using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
	public static partial class RedditImageDownloaderEvents
	{
		public delegate void DownloadFileCompletedEvent(object sender, RedditDownloaderEventBaseArgs args);

		public delegate void DownloadProgressChangedEvent(object sender, DownloadChangedEventArgs args);

		public delegate void DownloadStartedEvent(object sender, DownloadStartedEventArgs args);

		public delegate void FileAlradyThereEvent(object sender, FileAlradyThereEventArgs args);
	}
}