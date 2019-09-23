using FileCopy;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Azure_IoTHub_Toolbox_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeployToDevice : Page
    {
        public DeployToDevice()
        {
            this.InitializeComponent();
        }
        private List<StorageFolder> fldrs;
        List<string> Folders { get; set; }

        private  void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {

            //foreach (StorageFolder folder in fldrs)

            //{
            //    System.Diagnostics.Debug.WriteLine(folder.Name);

            //    // To iterate over the files in each folder, uncomment the following lines. 
            //    // foreach(StorageFile file in await folder.GetFilesAsync())
            //    //    Debug.WriteLine(" " + file.Name);
            //}
           // ListviewFolders.ItemsSource = Folders;
            ////foreach (StorageFolder folder in fldrs)
            ////{
            ////    Debug.WriteLine(folder.Name);

            ////    // To iterate over the files in each folder, uncomment the following lines. 
            ////    // foreach(StorageFile file in await folder.GetFilesAsync())
            ////    //    Debug.WriteLine(" " + file.Name);
            ////}

            ////foreach (var fldr in fldrs)
            ////{
            ////    AudioFilesLV.Items.Add(item.Path.ToString());
            ////}
            //var picker = new Windows.Storage.Pickers.FolderPicker();
            //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            //picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            //picker.FileTypeFilter.Add(".jpg");
            //picker.FileTypeFilter.Add(".jpeg");
            //picker.FileTypeFilter.Add(".png");
            //Windows.Storage.StorageFolder fldr = await picker.PickSingleFolderAsync();
        }

        private  void ListviewTransports2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListviewFolders.SelectedIndex;
            if (index < fldrs.Count())
            {
                var folder = fldrs[index];
                var sdf = folder.Path;//.GetFilesAsync();
                //await CopyFolder(sdf, @"c:\temp\one");
                tbSrcFolder.Text = sdf;
                FolderCopy.src = folder;
            }
            this.DataContext = null;
            this.DataContext = this;

        }

        ////https://stackoverflow.com/questions/10389701/how-to-create-a-recursive-function-to-copy-all-files-and-folders
        //public async static Task CopyFolder(string sourceFolder, string destFolder)
        //{
        //    var picker = new Windows.Storage.Pickers.FolderPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
        //    picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        //    //picker.FileTypeFilter.Add(".jpg");
        //    //picker.FileTypeFilter.Add(".jpeg");
        //    //picker.FileTypeFilter.Add(".png");
        //    Windows.Storage.StorageFolder fldr = await picker.PickSingleFolderAsync();

        //    //if (!Directory.Exists(destFolder))
        //    //    Directory.CreateDirectory(destFolder);

        //    destFolder = fldr.Path;

        //    string[] files = Directory.GetFiles(sourceFolder);
        //    foreach (string file in files)
        //    {
        //        string name = Path.GetFileName(file);
        //        string dest = Path.Combine(destFolder, name);
        //        File.Copy(file, dest);
        //    }
        //    string[] folders = Directory.GetDirectories(sourceFolder);
        //    foreach (string folder in folders)
        //    {
        //        string name = Path.GetFileName(folder);
        //        string dest = Path.Combine(destFolder, name);
        //        await CopyFolder(folder, dest);
        //    }
        //}

        public bool Ready
        {
            get
            {
                return FileCopy.FolderCopy.Ready;
            }
        }

        private async Task LoadFolders()
        {
            try
            {
                var localizationDirectory =  await ApplicationData.Current.LocalFolder.GetFolderAsync(@"Devices");
                //var localizationDirectory = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Devices");
                var fldrs1 = await localizationDirectory.GetFoldersAsync();
                var fldrs2 = from f in fldrs1 where f.Name != "Assets" select f;
                fldrs = fldrs2.ToList<StorageFolder>();

                var lst = from f in fldrs select f.Name;
                Folders = lst.ToList();
                ListviewFolders.ItemsSource = Folders;


                //await UnzipFile();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Devices folder not available or problem enumerating folders therein : " + ex.Message);
            }
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            h1.SubRegion = this.Info;
            await LoadFolders();
            this.DataContext = null;
            this.DataContext = this;
        }

        void UpdateProgress(int filecount, int foldercount)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbNumFiles.Text = filecount.ToString();
                    tbNumFolders.Text = foldercount.ToString();
                    if (filecount > pb.Maximum)
                    {
                        while (filecount > pb.Maximum)
                            filecount -= (int)pb.Maximum;
                    }
                    pb.Value = filecount;
                    if (foldercount > pb2.Maximum)
                    {
                        while (foldercount > pb2.Maximum)
                            foldercount -= (int)pb2.Maximum;
                    }
                    pb2.Value = foldercount;
                });
            });
        }

        void UpdateStatus(int filecount, int foldercount, string status)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbNumFiles.Text = filecount.ToString();
                    tbNumFolders.Text = foldercount.ToString();
                    tbStatus.Text = status;
                });
            });
        }

        //StorageFolder src;
        //StorageFolder dest;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //src = await ChooseFolder();
            var resp = await FolderCopy.GetSrc();
            if (!string.IsNullOrEmpty(resp))
            {
                tbSrcFolder.Text = resp;
            }
            this.DataContext = null;
            this.DataContext = this;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //dest = await ChooseFolder();
            var resp = await FolderCopy.GetDest();
            if (!string.IsNullOrEmpty(resp))
            {
                tbTargetFolder.Text = resp;
            }
            this.DataContext = null;
            this.DataContext = this; 
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //await CopyAsync(src, dest);
            await FolderCopy.CopyFolder(UpdateProgress, UpdateStatus);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderCopy.Cancel();
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            await CopyAppSettings();
        }

        private async Task CopyAppSettings()
        {
            string settingsStr = ApplicationSettings.SaveSettingsToConsoleDeviceAppSettings();
            await FolderCopy.WriteSettings(UpdateStatus, settingsStr);
        }

        private static string appFileName = "DNCore_Console_DeviceApp";
        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            await FolderCopy.CopyAppFilesOnly(appFileName, UpdateStatus);
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {

            await UnzipFile();
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {


            try
            {
                this.IsEnabled = false;
                //https://stackoverflow.com/questions/50856714/get-a-softwarebitmap-from-http-get-in-uwp
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://www.sportronics.com.au/media/Devices.zip");
                var stream = await response.Content.ReadAsStreamAsync();
                //IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
               //Windows.Storage.Streams.Buffer buff = new Windows.Storage.Streams.Buffer((uint)stream.Length);
                //var zipo = await randomAccessStream.ReadAsync(buff, 0, InputStreamOptions.None);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                var buff = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer);

                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile zipFile =
                    await storageFolder.CreateFileAsync("Devices.zip",
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);
                //var stream2 = await zipFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                //buff = new Windows.Storage.Streams.Buffer(buffer);
                //await stream2.WriteAsync(buff);
                await Windows.Storage.FileIO.WriteBufferAsync(zipFile, buff);
                this.IsEnabled = true;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Download of Devices.zip failed: " + ex.Message);
                this.IsEnabled = true;
            }
        }

        private async Task UnzipFile()
        {
            try
            {
                this.IsEnabled = false;
                //var localizationDirectory = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var localFolder = ApplicationData.Current.LocalFolder;
                var archive = await localFolder.GetFileAsync("Devices.zip");
                ZipFile.ExtractToDirectory(archive.Path, localFolder.Path,true); //Overwrite existing
                this.IsEnabled = true;
                await LoadFolders();
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unzip of Devices.zip failed: " + ex.Message);
                this.IsEnabled = true;
            }
        }

        private async Task UnzipFileFromInstall()
        {
            try
            {
                this.IsEnabled = false;
                var localizationDirectory =  Windows.ApplicationModel.Package.Current.InstalledLocation; ;
                var localFolder = ApplicationData.Current.LocalFolder;
                var archive = await localizationDirectory.GetFileAsync("Devices.zip");
                ZipFile.ExtractToDirectory(archive.Path, localFolder.Path, true); //Overwrite existing
                this.IsEnabled = true;
                await LoadFolders();
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unzip of Devices.zip failed: " + ex.Message);
                this.IsEnabled = true;
            }
        }
    }
}
