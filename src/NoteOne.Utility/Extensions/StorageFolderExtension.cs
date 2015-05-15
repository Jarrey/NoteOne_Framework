using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace NoteOne_Utility.Extensions
{
    public static class StorageFolderExtension
    {
        public static async Task<bool> CheckFileExisted(this StorageFolder folder, string fileName)
        {
            return folder != null &&
                   (await (
                              folder.CreateFileQueryWithOptions(
                                  new QueryOptions
                                      {
                                          FolderDepth = FolderDepth.Shallow,
                                          UserSearchFilter = "System.FileName:\"" + fileName + "\""
                                      })).GetFilesAsync()).Count > 0
                       ? true
                       : false;
        }

        public static async Task<IList<StorageFile>> GetStorageFiles(this StorageFolder folder,
                                                                     IEnumerable<string> fileExtensions,
                                                                     FolderDepth depthOption = FolderDepth.Deep,
                                                                     uint startIndex = 0,
                                                                     uint maxNumberOfItems = uint.MaxValue)
        {
            try
            {
                if (folder != null)
                {
                    var queryOptions = new QueryOptions
                        {
                            FolderDepth = depthOption
                        };
                    foreach (string fileExtension in fileExtensions)
                        queryOptions.FileTypeFilter.Add(fileExtension);
                    return
                        (await
                         folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync(startIndex, maxNumberOfItems))
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            return null;
        }
    }
}