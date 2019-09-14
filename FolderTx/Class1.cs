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
        public static async Task CopyAsync(StorageFolder source, StorageFolder destination)
        {
            if (_cancel)
                return;
            // If the destination exists, delete it.
            var targetFolder = await destination.TryGetItemAsync(source.DisplayName);

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
                await CopyAsync(storageFolder, (StorageFolder)targetFolder);
                FolderCount++;
                if (_cancel)
                    return;
            }
        }

        public static async Task<string> GetSrc()
        {
            src = await ChooseFolder();
            return src.Path;
        }

        public static async Task<string> GetDest()
        {
            dest = await ChooseFolder();
            return dest.Path;
        }

        public static async Task CopyFolder(UpdateProgress updateProg, UpdateStatus updateStat)
        {
            _cancel = false;
            updateProgress = updateProg;
            updateStatus = updateStat;
            FileCount = 0;
            FolderCount = 0;
            updateStatus?.Invoke(FileCount, FolderCount, "In Progress");
            await CopyAsync(src, dest);
            if (!_cancel)
                updateStatus?.Invoke(FileCount, FolderCount, "Done");
            else
                updateStatus?.Invoke(FileCount, FolderCount, "Cancelled");

        }


    }
}
