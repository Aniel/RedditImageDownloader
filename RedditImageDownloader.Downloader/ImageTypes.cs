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
}