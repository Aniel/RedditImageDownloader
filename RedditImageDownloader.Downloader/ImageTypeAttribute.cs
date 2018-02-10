using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ImageTypeAttribute : Attribute
	{
		public ImageTypeAttribute(string typeEnding, string fullName)
		{
			TypeEnding = typeEnding;
			FullName = fullName;
		}

		public string FullName { get; set; }
		public string TypeEnding { get; set; }
	}
}