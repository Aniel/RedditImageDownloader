using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageDownloader.Downloader
{
    public enum ImageTypes
    {
        [ImageType(".jpg", "JPEG File Interchange Format (JFIF)")]
        JPG,
        [ImageType(".bmp", "device independent bitmap (DIB) file format")]
        BMP,
        [ImageType(".png", "")]
        PNG,
        [ImageType(".gif", "")]
        GIF,
        None
    }

    public static class ImageTypesUtil
    {
        public static IEnumerable<ImageTypes> GetValues() => Enum.GetValues(typeof(ImageTypes)).Cast<ImageTypes>();
    }

    public static class ImageTypesExtensions
    {
        public static string getEnding(this ImageTypes val)
        {
            var attribs = getImageTypeAttributes(val);
            return attribs.Length > 0 ? attribs[0].TypeEnding : null;
        }

        public static string getFullName(this ImageTypes val)
        {
            var attribs = getImageTypeAttributes(val);
            return attribs.Length > 0 ? attribs[1].FullName : null;
        }

        private static ImageTypeAttribute[] getImageTypeAttributes(ImageTypes val)
        {
            Type type = val.GetType();
            FieldInfo fieldInfo = type.GetField(val.ToString());
            ImageTypeAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(ImageTypeAttribute), false) as ImageTypeAttribute[];
            return attribs;
        }
    }

    internal class ImageTypeAttribute : Attribute
    {
        public string TypeEnding { get; set; }
        public string FullName { get; set; }
        public ImageTypeAttribute(string typeEnding, string fullName)
        {
            TypeEnding = typeEnding;
            FullName = fullName;
        }
    }
}
