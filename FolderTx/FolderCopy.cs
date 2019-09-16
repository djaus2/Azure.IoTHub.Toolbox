using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;



namespace FileCopy
{
    public delegate void UpdateProgress(int filecount, int foldercount);
    public delegate void UpdateStatus(int filecount, int foldercount, string status);
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public static class FolderCopy
    {
        private static UpdateProgress updateProgress;
        public static UpdateStatus updateStatus;
        public static StorageFolder src;
        private static StorageFolder dest;



        public static async Task<StorageFolder> ChooseFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.FileTypeFilter.Add(".docx");
            folderPicker.FileTypeFilter.Add(".xlsx");
            folderPicker.FileTypeFilter.Add(".pptx");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                //OutputTextBlock.Text = "Picked folder: " + folder.Name;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
            return folder;
        }

        private static bool _cancel;
        public static void Cancel()
        {
            _cancel = true;
        }

        public static int FileCount { get; set; }
        public static int FolderCount { get; set; }
        public static async Task CopyAsync(StorageFolder source, StorageFolder destination, bool isTop)
        {
            if (_cancel)
                return;
            // If the destination exists, delete it.

            IStorageItem targetFolder;

            targetFolder = await destination.TryGetItemAsync(source.DisplayName);

            if (isTop)
            if (targetFolder is StorageFolder)
                await targetFolder.DeleteAsync();
            

            targetFolder = await destination.CreateFolderAsync(source.DisplayName);

            // Get all files (shallow) from source
            var queryOptions = new QueryOptions
            {
                IndexerOption = IndexerOption.DoNotUseIndexer,  // Avoid problems cause by out of sync indexer
                FolderDepth = FolderDepth.Shallow,
            };
            var queryFiles = source.CreateFileQueryWithOptions(queryOptions);
            var files = await queryFiles.GetFilesAsync();

            // Copy files into target folder
            foreach (var storageFile in files)
            {
                await storageFile.CopyAsync((StorageFolder)targetFolder, storageFile.Name, NameCollisionOption.ReplaceExisting);
                FileCount++;
                updateProgress?.Invoke(FileCount, FolderCount);
                if (_cancel)
                    return;
            }

            // Get all folders (shallow) from source
            var queryFolders = source.CreateFolderQueryWithOptions(queryOptions);
            var folders = await queryFolders.GetFoldersAsync();

            // For each folder call CopyAsync with new destination as destination
            foreach (var storageFolder in folders)
            {
                await CopyAsync(storageFolder, (StorageFolder)targetFolder,false);
                FolderCount++;
                if (_cancel)
                    return;
            }
        }

        public static async Task<string> GetSrc()
        {
            src = await ChooseFolder();
            if (src==null)
                return "";
            else
                return src.Path;
        }

        public static async Task<string> GetDest()
        {
            dest = await ChooseFolder();
            if (dest == null)
                return "";
            else
                return dest.Path;
        }

        public static async Task CopyFolder(UpdateProgress updateProg, UpdateStatus updateStat)
        {
            _cancel = false;
            updateProgress = updateProg;
            updateStatus = updateStat;
            FileCount = 0;
            FolderCount = 1;
            updateStatus?.Invoke(FileCount, FolderCount, string.Format("Folder {0} copy in progress... Will delete folder on target if it exists first.",src.Name));
            await CopyAsync(src, dest,true);
            if (!_cancel)
                updateStatus?.Invoke(FileCount, FolderCount, "Folder copy done");
            else
                updateStatus?.Invoke(FileCount, FolderCount, "Folder copy cancelled");

        }

        public static async Task WriteSettings(UpdateStatus updateStatus, string settingsStr)
        {
            _cancel = false;
            string settingsFilename = "settings.json";

            Windows.Storage.StorageFile settingsFile = await Search(settingsFilename, dest);
            if (settingsFile != null)
            {
                updateStatus?.Invoke(FileCount, FolderCount, string.Format("Folder copy done. Copying Toolbox app settings to target. Searching for {0} at target first.", settingsFilename));
                await Windows.Storage.FileIO.WriteTextAsync(settingsFile, settingsStr,Windows.Storage.Streams.UnicodeEncoding.Utf8);
                updateStatus?.Invoke(FileCount, FolderCount, string.Format("Done... And copied settings to {0}", settingsFile.Path));
            }
            else
            {
                updateStatus?.Invoke(FileCount, FolderCount,string.Format("Copy cancelled or {0} file not found in target", settingsFilename));
            }
        }

        public static async Task<StorageFile> Search(string filename, StorageFolder destination)
        {
            if (_cancel)
                return null;
          

            // Get all files (shallow) from source
            var queryOptions = new QueryOptions
            {
                IndexerOption = IndexerOption.DoNotUseIndexer,  // Avoid problems cause by out of sync indexer
                FolderDepth = FolderDepth.Shallow,
            };
            var queryFiles = destination.CreateFileQueryWithOptions(queryOptions);
            var files = await queryFiles.GetFilesAsync();

            var fil = from f in files where f.Name == filename select f;

            if (fil.Count() > 0)
                return fil.First();

           
            // Get all folders (shallow) from source
            var queryFolders = destination.CreateFolderQueryWithOptions(queryOptions);
            var folders = await queryFolders.GetFoldersAsync();

            // For each folder, call query
            foreach (var storageFolder in folders)
            {
                var fill = await Search(filename,storageFolder);
                if (fill != null)
                    return fill;
                if (_cancel)
                    return null;
            }
            return null;
        }

        public static async Task<List<StorageFile>> Search4Files(string filename, StorageFolder destination)
        {
            if (_cancel)
                return null;


            // Get all files (shallow) from source
            var queryOptions = new QueryOptions
            {
                IndexerOption = IndexerOption.DoNotUseIndexer,  // Avoid problems cause by out of sync indexer
                FolderDepth = FolderDepth.Shallow,
            };
            var queryFiles = destination.CreateFileQueryWithOptions(queryOptions);
            var files = await queryFiles.GetFilesAsync();

            var fil = from f in files where f.Name.Contains(filename) select f;

            if (fil.Count() > 0)
                return fil.ToList<StorageFile>();


            // Get all folders (shallow) from source
            var queryFolders = destination.CreateFolderQueryWithOptions(queryOptions);
            var folders = await queryFolders.GetFoldersAsync();

            // For each folder, call query
            foreach (var storageFolder in folders)
            {
                var fill = await Search4Files(filename, storageFolder);
                if (fill != null)
                    return fill;
                if (_cancel)
                    return null;
            }
            return null;
        }

        

        public async static Task CopyAppFilesOnly( string appFilename, UpdateStatus updateStatus)
        {
            var files = await Search4Files(appFilename, src);
            if (files == null)
            {
                updateStatus?.Invoke(FileCount, FolderCount, string.Format("No app files for {0} found to copy", appFilename));

            }
            else
            {
                string[] folders = src.Path.Split(new char[] { '\\' });
                string subfolder = folders[folders.Length - 1];
                StorageFolder destinationForAppFiles = await dest.GetFolderAsync(subfolder);
                updateStatus?.Invoke(FileCount, FolderCount, string.Format("{0} app files for {1} found to copy",files.Count(), appFilename));
                foreach (var storageFile in files)
                {
                    await storageFile.CopyAsync(destinationForAppFiles, storageFile.Name, NameCollisionOption.ReplaceExisting);
                    //FileCount++;
                    //updateProgress?.Invoke(FileCount, FolderCount);
                    if (_cancel)
                        return;
                }
                updateStatus?.Invoke(FileCount, FolderCount, string.Format("Updated {0} files for {1} app.", files.Count(), appFilename));

            }
        }
    }
}
