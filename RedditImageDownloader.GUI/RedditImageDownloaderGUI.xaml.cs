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
        public string CurrentSubreddit = "r/earthporn";
        private string Path = System.Windows.Forms.Application.StartupPath;
        private string Prefix = "Downloader";
        private int NumberOfImages = 10;
        private Downloader.RedditImageDownloader.Catogory Catogory = Downloader.RedditImageDownloader.Catogory.Hot;

        public RedditImageDownloaderGUI()
        {
            InitializeComponent();
            setNumberOfPosts();
            setSubreddit();
            setPrefix();
            setCatorogy();
            setPath();
        }

        private void StartDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            //Init downloader
            var Downloader = new Downloader.RedditImageDownloader(Path, Prefix, CurrentSubreddit, Catogory, NumberOfImages);

            StartDownloadButton.Content = "Downloading";
            setIsButtonsEnabled(false);

            Log.Items.Add("Download started");


            //Downlaod images
            Downloader.SaveImages();

            Log.Items.Add("Download completed");
            Log.Items.Add("--------------------------------");

            StartDownloadButton.Content = "Download";
            setIsButtonsEnabled(true);
        }

        private async void StartDownloadAsyncButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Init downloader
            var downloader = new Downloader.RedditImageDownloader(Path, Prefix, CurrentSubreddit, Catogory, NumberOfImages);

            //Subscribe to the status update eventn
            downloader.DownloadProgressChangedEvent += DownloaderOnDownloadProgressChanged;
            downloader.FileDonwloadCompletedEvent += DownloaderOnFileCompleted;
            downloader.FileAlradyThereEvent += DownloaderOnFileAlreadyExist;
            downloader.DownloadStartetEvent += DownloaderOnDownloadStartet;

            setIsButtonsEnabled(false);

            StartDownloadAsyncButton.Content = "Downloading";

            //Start downloadAsync
            var task = downloader.SaveImagesAsync();

            //Do somthing while async task is running
            Log.Items.Add("Download Async started");

            //Wait for downlaod completion
            await task;

            Log.Items.Add("Download Async completed");
            Log.Items.Add("--------------------------------");
            StartDownloadAsyncButton.Content = "Download Async";

            setIsButtonsEnabled(true);

        }

        #region DownloaderOnEventHandler
        private void DownloaderOnDownloadProgressChanged(object sender, RedditImageDownloaderEvents.DownloadChangedEventArgs args)
        {
            var parent = args.Parent;
            ResponseTestProgressBar.Value = parent.ProgressPercentage;
            Log.Items[Log.Items.Count - 1] = $"Downloading Image {args.ImageNumber}/{NumberOfImages}: \"{args.FileName}\" from \"{args.Url}\" ({parent.BytesReceived}b/{parent.TotalBytesToReceive}b({parent.ProgressPercentage}%))\n";
        }

        private void DownloaderOnDownloadStartet(object sender, RedditImageDownloaderEvents.DownloadStartedEventArgs args)
        {
            Log.Items.Add($"Downloading Image {args.ImageNumber}/{NumberOfImages}: \"{args.FileName}\" from \"{args.Url}\"");
        }

        private void DownloaderOnFileAlreadyExist(object sender, RedditImageDownloaderEvents.FileAlradyThereEventArgs args)
        {
            Log.Items.Add($"File {args.FileName} is already there");
        }

        private void DownloaderOnFileCompleted(object sender, RedditImageDownloaderEvents.RedditDownloaderEventBaseArgs args)
        {
            Log.Items[Log.Items.Count - 1] = $"Downloading Image {args.ImageNumber}/{NumberOfImages} : \"{args.FileName}\" from \"{args.Url}\" (Completed))";
        }
        #endregion

        #region UiHandling
        private void setIsButtonsEnabled(bool value)
        {
            StartDownloadAsyncButton.IsEnabled = value;
            StartDownloadButton.IsEnabled = value;
        }

        private void TextBoxNumberOfItems_LostFocus(object sender, RoutedEventArgs e)
        {
            setNumberOfPosts();
        }

        private void TextBoxNumberOfItems_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            setNumberOfPosts();
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
        }

        private void setSubreddit()
        {
            TextBoxSubreddit.Text = CurrentSubreddit;
        }

        private void setPath()
        {
            if (!Path.EndsWith(@"\"))
                Path += @"\";
            TextBoxPath.Text = Path;
        }

        private void setCatorogy()
        {
            switch (Catogory)
            {
                case Downloader.RedditImageDownloader.Catogory.UnmoderatedLinks:
                    ComboboxCatogory.SelectedIndex = 0;
                    break;
                case Downloader.RedditImageDownloader.Catogory.Posts:
                    ComboboxCatogory.SelectedIndex = 1;

                    break;
                case Downloader.RedditImageDownloader.Catogory.Hot:
                    ComboboxCatogory.SelectedIndex = 2;

                    break;
                case Downloader.RedditImageDownloader.Catogory.New:
                    ComboboxCatogory.SelectedIndex = 3;

                    break;
                default:
                    break;
            }
        }

        private void setPrefix()
        {
            if (!string.IsNullOrEmpty(TextBoxPrefix.Text.Trim()))
            {
                Prefix = TextBoxSubreddit.Text;
            }
            else
            {
                TextBoxPrefix.Text = Prefix;
            }
        }

        private void TextBoxPrefix_LostFocus(object sender, RoutedEventArgs e)
        {
            setPrefix();
        }

        private void TextBoxPrefix_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            setPrefix();
        }

        private void ComboboxCatogory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboboxCatogory.SelectedIndex)
            {
                case 0:
                    Catogory = Downloader.RedditImageDownloader.Catogory.UnmoderatedLinks;
                    break;
                case 1:
                    Catogory = Downloader.RedditImageDownloader.Catogory.Posts;
                    break;
                case 2:
                    Catogory = Downloader.RedditImageDownloader.Catogory.Hot;
                    break;
                case 3:
                    Catogory = Downloader.RedditImageDownloader.Catogory.New;
                    break;
                default:
                    break;
            }
        }

        private void TextBoxPath_LostFocus(object sender, RoutedEventArgs e)
        {
            setPath();
        }

        private void TextBoxPath_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            setPath();
        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = Path;

            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Path = folderDialog.SelectedPath;
                setPath();
            }
        }

        private void CheckSubredditButtonNotOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TextBoxSubreddit.Text))
                {
                    CurrentSubreddit = TextBoxSubreddit.Text;
                    var check = new RedditSharp.Reddit();
                    var sub = check.GetSubreddit(CurrentSubreddit);
                    CheckSubredditButtonOk.Visibility = Visibility.Visible;
                    CheckSubredditButtonNotOk.Visibility = Visibility.Hidden;
                    setIsButtonsEnabled(true);
                    setSubreddit();
                }
            }
            catch (Exception)
            {
                CheckSubredditButtonOk.Visibility = Visibility.Hidden;
                CheckSubredditButtonNotOk.Visibility = Visibility.Visible;
                setIsButtonsEnabled(false);
            }
        }

        private void TextBoxSubreddit_TextChanged(object sender, TextChangedEventArgs e)
        {
            setIsButtonsEnabled(false);
            CheckSubredditButtonOk.Visibility = Visibility.Hidden;
            CheckSubredditButtonNotOk.Visibility = Visibility.Visible;
        }

        private void ButtonClearLog_Click(object sender, RoutedEventArgs e)
        {
            Log.Items.Clear();
        }
        #endregion
    }
}
