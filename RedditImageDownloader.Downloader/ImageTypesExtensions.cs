using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
	public static class ImageTypesExtensions
	{
		public static string GetEnding(this ImageTypes val)
		{
			var attribs = GetImageTypeAttributes(val);
			return attribs.Length > 0 ? attribs[0].TypeEnding : null;
		}

		public static string GetFullName(this ImageTypes val)
		{
			var attribs = GetImageTypeAttributes(val);
			return attribs.Length > 0 ? attribs[1].FullName : null;
		}

		private static ImageTypeAttribute[] GetImageTypeAttributes(ImageTypes val)
		{
			Type type = val.GetType();
			FieldInfo fieldInfo = type.GetField(val.ToString());
			ImageTypeAttribute[] attribs = fieldInfo.GetCustomAttributes(
				typeof(ImageTypeAttribute), false) as ImageTypeAttribute[];
			return attribs;
		}
	}
}