using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
	public static class ImageTypesHelper
	{
		public static IEnumerable<ImageTypes> GetValues() => Enum.GetValues(typeof(ImageTypes)).Cast<ImageTypes>();
	}
}