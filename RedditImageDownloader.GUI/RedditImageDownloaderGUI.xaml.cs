using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RedditImageDownloader.Downloader;

namespace RedditImageDownloader.GUI
{
	/// <summary>
	/// Interaktionslogik für RedditImageDownloaderGUI.xaml
	/// </summary>
	public partial class RedditImageDownloaderGUI : MetroWindow
	{
		public string Subreddit = "r/earthporn";
		private Downloader.RedditImageDownloader.Catogory Catogory = Downloader.RedditImageDownloader.Catogory.Hot;
		private int NumberOfImages = 10;
		private string Path = System.Windows.Forms.Application.StartupPath;
		private string Prefix = "Downloader";

		public RedditImageDownloaderGUI()
		{
			InitializeComponent();
			LoadSettings();
			setNumberOfPosts();
			setSubreddit();
			setPrefix();
			_setCatorogy();
			setPath();
		}

		private void _setCatorogy()
		{
			ComboboxCatogory.SelectedIndex = (int)Catogory;
		}

		private void ButtonCheckSubredditIsNotOkay_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(TextBoxSubreddit.Text))
				{
					Subreddit = TextBoxSubreddit.Text;
					var check = new RedditSharp.Reddit();
					var sub = check.GetSubreddit(Subreddit);
					ToggleButtonSubredditCheck();
					setIsButtonsEnabled(true);
					setSubreddit();
				}
			}
			catch (Exception)
			{
				ToggleButtonSubredditCheck();
				setIsButtonsEnabled(false);
			}
		}

		private void ButtonClearLog_Click(object sender, RoutedEventArgs e)
		{
			Log.Items.Clear();
		}

		private void ComboboxCatogory_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Catogory = (Downloader.RedditImageDownloader.Catogory)ComboboxCatogory.SelectedIndex;
		}

		private void DownloaderOnDownloadProgressChanged(object sender, RedditImageDownloaderEvents.DownloadChangedEventArgs args)
		{
			Dispatcher.Invoke(() =>
			{
				var parent = args.Parent;
				ResponseTestProgressBar.Value = parent.ProgressPercentage;
				Log.Items[Log.Items.Count - 1] = $"Downloading Image {args.ImageNumber}/{NumberOfImages}: \"{args.FileName}\" from \"{args.Url}\" ({parent.BytesReceived}b/{parent.TotalBytesToReceive}b({parent.ProgressPercentage}%))\n";
			});
		}

		private void DownloaderOnDownloadStartet(object sender, RedditImageDownloaderEvents.DownloadStartedEventArgs args)
		{
			Dispatcher.Invoke(() => Log.Items.Add($"Downloading Image {args.ImageNumber}/{NumberOfImages}: \"{args.FileName}\" from \"{args.Url}\""));
		}

		private void DownloaderOnFileAlreadyExist(object sender, RedditImageDownloaderEvents.FileAlradyThereEventArgs args)
		{
			Dispatcher.Invoke(() => Log.Items.Add($"File {args.FileName} is already there"));
		}

		private void DownloaderOnFileCompleted(object sender, RedditImageDownloaderEvents.RedditDownloaderEventBaseArgs args)
		{
			Dispatcher.Invoke(() => Log.Items[Log.Items.Count - 1] = $"Downloading Image {args.ImageNumber}/{NumberOfImages} : \"{args.FileName}\" from \"{args.Url}\" (Completed))");
		}

		private void LoadSettings()
		{
			Subreddit = Properties.Settings.Default.Subreddit;
			NumberOfImages = Properties.Settings.Default.NumberOfImages;
			Prefix = Properties.Settings.Default.Prefix;
			Path = Properties.Settings.Default.Path;
			Catogory = (Downloader.RedditImageDownloader.Catogory)Properties.Settings.Default.Catogory;
		}

		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveSettings();
		}

		private void SaveSettings()
		{
			Properties.Settings.Default.Subreddit = Subreddit;
			Properties.Settings.Default.NumberOfImages = NumberOfImages;
			Properties.Settings.Default.Prefix = Prefix;
			Properties.Settings.Default.Path = Path;
			Properties.Settings.Default.Catogory = (int)Catogory;
			Properties.Settings.Default.Save();
		}

		private void SelectPath_Click(object sender, RoutedEventArgs e)
		{
			var folderDialog = new FolderBrowserDialog
			{
				SelectedPath = Path
			};

			DialogResult result = folderDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Path = folderDialog.SelectedPath;
				setPath();
			}
		}

		private void setIsButtonsEnabled(bool value)
		{
			StartDownloadAsyncButton.IsEnabled = value;
		}

		private void setNumberOfPosts()
		{
			var newNumber = 0;
			if (!int.TryParse(TextBoxNumberOfItems.Text.Trim(), out newNumber))
			{
				newNumber = NumberOfImages;
			}
			TextBoxNumberOfItems.Text = newNumber.ToString();
			NumberOfImages = newNumber;
			Properties.Settings.Default.NumberOfImages = NumberOfImages;
			Properties.Settings.Default.Save();
		}

		private void setPath()
		{
			if (!Path.EndsWith(@"\", StringComparison.Ordinal))
				Path += @"\";
			TextBoxPath.Text = Path;
		}

		private void setPrefix()
		{
			if (!string.IsNullOrEmpty(TextBoxPrefix.Text.Trim()))
			{
				Prefix = TextBoxPrefix.Text;
			}
			else
			{
				TextBoxPrefix.Text = Prefix;
			}
		}

		private void setSubreddit()
		{
			if (!string.IsNullOrEmpty(TextBoxPrefix.Text.Trim()))
			{
				Subreddit = TextBoxSubreddit.Text;
			}
			else
			{
				TextBoxSubreddit.Text = Subreddit;
			}
		}

		private async void StartDownloadAsyncButton_OnClick(object sender, RoutedEventArgs e)
		{
			//Init downloader
			var downloader = new Downloader.RedditImageDownloader(Path, Prefix, Subreddit, Catogory, NumberOfImages);

			//Subscribe to the status update eventn
			downloader.DownloadProgressChangedEvent += DownloaderOnDownloadProgressChanged;
			downloader.FileDonwloadCompletedEvent += DownloaderOnFileCompleted;
			downloader.FileAlradyThereEvent += DownloaderOnFileAlreadyExist;
			downloader.DownloadStartetEvent += DownloaderOnDownloadStartet;

			setIsButtonsEnabled(false);

			StartDownloadAsyncButton.Content = "Downloading";
			Dispatcher.CheckAccess();
			//Start downloadAsync
			var task = downloader.SaveImagesAsync();
			//Do somthing while async is running
			Log.Items.Add("Download Async started");
			//Wait for download completion
			await task;

			Log.Items.Add("Download Async completed");
			Log.Items.Add("--------------------------------");
			StartDownloadAsyncButton.Content = "Download Async";
			setIsButtonsEnabled(true);
		}

		private void TextBoxNumberOfItems_LostFocus(object sender, RoutedEventArgs e)
		{
			setNumberOfPosts();
		}

		private void TextBoxNumberOfItems_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			setNumberOfPosts();
		}

		private void TextBoxPath_LostFocus(object sender, RoutedEventArgs e)
		{
			setPath();
		}

		private void TextBoxPath_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			setPath();
		}

		private void TextBoxPrefix_LostFocus(object sender, RoutedEventArgs e)
		{
			setPrefix();
		}

		private void TextBoxPrefix_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			setPrefix();
		}

		private void TextBoxSubreddit_TextChanged(object sender, TextChangedEventArgs e)
		{
			setIsButtonsEnabled(false);
			ButtonCheckSubredditIsOkay.Visibility = Visibility.Hidden;
			ButtonCheckSubredditIsNotOkay.Visibility = Visibility.Visible;
		}

		private void ToggleButtonSubredditCheck()
		{
			if (ButtonCheckSubredditIsOkay.Visibility == Visibility.Hidden)
				ButtonCheckSubredditIsOkay.Visibility = Visibility.Visible;
			else
				ButtonCheckSubredditIsOkay.Visibility = Visibility.Hidden;

			if (ButtonCheckSubredditIsNotOkay.Visibility == Visibility.Hidden)
				ButtonCheckSubredditIsNotOkay.Visibility = Visibility.Visible;
			else
				ButtonCheckSubredditIsNotOkay.Visibility = Visibility.Hidden;
		}
	}
}