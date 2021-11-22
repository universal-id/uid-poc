using System.IO;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using IO = System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UniversalIdentity.Library.Storage
{
    public class FileRepository
    {
        public string Path;
        public bool SyncsWithFileStore;

        public FileRepository(string path)
        {
            this.Path = path;
        }

        public void Init()
        {
            if (!Directory.Exists(this.Path)) throw new Exception($"Expected path '{this.Path}' to exist at init time.");

            var boxRepositoryPath = IO.Path.Combine(this.Path, FileRepositoryHelper.BoxFolder);
            if (!Directory.Exists(boxRepositoryPath)) Directory.CreateDirectory(boxRepositoryPath);

            var boxObjectsPath = IO.Path.Combine(boxRepositoryPath, FileRepositoryHelper.Objects);
            if (!Directory.Exists(boxObjectsPath)) Directory.CreateDirectory(boxObjectsPath);

            var rootHashHex = FileRepositoryHelper.CreateFolderObject(this, null);

            var firstIternationHashHex = FileRepositoryHelper.CreateIteration(this, rootHashHex);

            var repositoryBoxLatestPath = IO.Path.Combine(this.Path, FileRepositoryHelper.BoxFolder, FileRepositoryHelper.Latest);
            File.WriteAllText(repositoryBoxLatestPath, rootHashHex);
        }

        // Create or update one file
        public void UpdateOneFile(string basePath, string fileName, string fileContents)
        {
            var changes = new [] { new SimpleChange() 
                { 
                    ObjectType = ObjectType.File,
                    Type = ChangeType.Update,
                    Contents = fileContents,
                    Path = IO.Path.Combine(basePath, fileName)
                }
            };

            ChangeHelper.ProcessChanges(this, changes);
        }
        
        // public static Dictionary<string, Dictionary<string, FolderEntry>> FolderHashIndex = new Dictionary<string, Dictionary<string, FolderEntry>>(StringComparer.OrdinalIgnoreCase);

        // public Dictionary<string, FolderEntry> GetOrAddFolderToHashIndex(string folderHash)
        // {
        //     Dictionary<string, FolderEntry> folderIndex = null;
        //     if (FolderHashIndex.TryGetValue(folderHash, out folderIndex))
        //     {
        //         return folderIndex;
        //     }

        //     folderIndex = new Dictionary<string, FolderEntry>(StringComparer.OrdinalIgnoreCase);
        //     var repositoryBoxLatestPath = IO.Path.Combine(this.Path, FileHelper.BoxFolder, FileHelper.Latest);
        //     foreach (var line in ReadObjectLines(folderHash, ObjectType.Folder))
        //     {
        //         if (line == null) break;
        //         var lineSegments = line.Split('-');
        //         if (lineSegments.Count() != 3) throw new Exception($"Expected line '{line}' from '{repositoryBoxLatestPath}' to have 3 segments.");
        //         var key = $"{lineSegments[0]}-{lineSegments[2]}";
        //         ObjectType objectType;
        //         if (!Enum.TryParse<ObjectType>(lineSegments[0], true, out objectType)) throw new Exception($"Expected line '{line}' from '{repositoryBoxLatestPath}' to start with valid object type.");
        //         folderIndex.Add(key, new FolderEntry(objectType, lineSegments[1], lineSegments[2]));
        //     }

        //     FolderHashIndex.Add(folderHash, folderIndex);
        //     return folderIndex;
        // }

        public string GetFileContents(string filePath, string fileName)
        {
            var repositoryBoxLatestPath = IO.Path.Combine(this.Path, FileRepositoryHelper.BoxFolder, FileRepositoryHelper.Latest);
            var folderHash = File.ReadAllText(repositoryBoxLatestPath);
            FolderInfo folderInfo = FileRepositoryHelper.GetFolderInfo(this, folderHash);
            var segments = FileRepositoryHelper.GetSegments(filePath);
            FolderEntry folderEntry = null;

            if (segments != null)
            {
                foreach(var segment in segments)
                {
                    if (folderInfo.Entries.TryGetValue(segment, out folderEntry))
                    {
                        // throw new Exception($"Expected folder '{folderInfo.Hash}' under path '{filePath}' to contain path segment '{segment}'");
                        folderHash = folderEntry.Hash;
                        folderInfo = FileRepositoryHelper.GetFolderInfo(this, folderHash);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (folderInfo.Entries.TryGetValue(fileName, out folderEntry))
            {
                var fileHash = folderEntry.Hash;
                if (string.IsNullOrEmpty(fileHash)) throw new Exception($"Expected entry for file '{fileName}' under under path '{filePath}' to have a valid hash");
                var repositoryBoxFilePath = IO.Path.Combine(this.Path, FileRepositoryHelper.BoxFolder, FileRepositoryHelper.Objects, fileHash);
                folderInfo = FileRepositoryHelper.GetFolderInfo(this, folderEntry.Hash);
                var fileContents = File.ReadAllText(repositoryBoxFilePath);
                return fileContents;
            }
            else
            {
                return null;
            }
        }

        public void DeleteOneFile(string basePath, string fileName)
        {
            var changes = new [] { new SimpleChange() 
                { 
                    ObjectType = ObjectType.File,
                    Type = ChangeType.Delete,
                    Path = IO.Path.Combine(basePath, fileName)
                }
            };

            ChangeHelper.ProcessChanges(this, changes);
        }
    }
}