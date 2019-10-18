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
    public sealed partial class GetAndDeployProxySvc : Page
    {
        public GetAndDeployProxySvc()
        {
            this.InitializeComponent();
        }


        public bool Ready
        {
            get
            {
                return FileCopy.FolderCopy.Ready;
            }
        }



        private  void Page_Loaded(object sender, RoutedEventArgs e)
        {
            h1.SubRegion = this.Info;
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



  

  

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderCopy.Cancel();
        }




        private async void Button_Get_ProxySvcApp(object sender, RoutedEventArgs e)
        {
            await Download_ZipFile();
            await Locate_Target();
            await UnzipFile();
            await CopyAppSettings();
        }

        private async Task Download_ZipFile()
        {


            try
            {
                this.IsEnabled = false;
                //https://stackoverflow.com/questions/50856714/get-a-softwarebitmap-from-http-get-in-uwp
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://www.sportronics.com.au/media/ProxySvc.zip");
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
                    await storageFolder.CreateFileAsync("ProxySvc.zip",
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

        private async Task Locate_Target()
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

        private async Task UnzipFile()
        {
            try
            {
                this.IsEnabled = false;
                //var localizationDirectory = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var localFolder = ApplicationData.Current.LocalFolder;
                var archive = await localFolder.GetFileAsync("ProxySvc.zip");
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

        private Task LoadFolders()
        {
            throw new NotImplementedException();
        }

        private async Task CopyAppSettings()
        {
            string settingsStr = ApplicationSettings.SaveSettingsToConsoleDeviceAppSettings();
            await FolderCopy.WriteSettings(UpdateStatus, settingsStr);
        }


    }
}
