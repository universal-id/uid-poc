using System.IO;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using IO = System.IO; 
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System;

namespace UniversalIdentity.Library.Storage
{
    public static class FileRepositoryHelper
    {
        public const string BoxFolder = ".box"; // BoxFolderName
        public const string Latest = "latest"; // LatestFileName
        public const string Objects = "objects"; // ObjectsFolderName

        public static string CreateFolderObject(FileRepository repository, IEnumerable<FolderEntry> folderEntries)
        {
            var folderContentStringBuilder = new StringBuilder();
            if(folderEntries != null)
            {
                foreach (FolderEntry entry in folderEntries)
                {
                    var objectTypeName = Enum.GetName(typeof(ObjectType), entry.Type).ToLowerInvariant();
                    folderContentStringBuilder.AppendLine($"{objectTypeName}-{entry.Hash}-{entry.Name}");
                }
            }

            var folderHash = GetHashHex(folderContentStringBuilder.ToString());
            var repositoryBoxObjectPath = IO.Path.Combine(repository.Path, BoxFolder, Objects, folderHash);
            File.WriteAllLines(repositoryBoxObjectPath, new[] { folderContentStringBuilder.ToString() });
            return folderHash;
        }

        public static string GetHashHex(string content)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(content);
            byte[] hashData = Sha3Keccack.Current.CalculateHash(data);
            var hashHex = hashData.ToHex(true);
            return hashHex;
        }

        public static string CreateIteration(FileRepository repository, string rootHashHex)
        {
            var iterationContent = "type=iteration";  
            iterationContent += @"\n{rootHashHex}";
            var hashHex = GetHashHex(iterationContent);
            var repositoryBoxObjectPath = IO.Path.Combine(repository.Path, BoxFolder, Objects, hashHex);
            File.WriteAllLines(repositoryBoxObjectPath, new[] { iterationContent });
            return hashHex;
        }

        public static (string, IEnumerable<string>) GetNameAndSegments(string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) return (null, null);

            var segments = GetSegments(filePath);
            var segmentCount = segments.Count();
            var name = segments.Last();
            
            return (name, segments.Take(segmentCount - 1));
        }

        public static IEnumerable<string> GetSegments(string filePath)
        {
            filePath.Trim();
            filePath.Trim('/');

            if(string.IsNullOrEmpty(filePath)) return null;

            var segments = filePath.Split('/');
            return segments.Where(segment => !string.IsNullOrEmpty(segment));
        }

        public static FolderInfo GetFolderInfo(FileRepository repository, string folderHash)
        {
            var repositoryBoxObjectPath = IO.Path.Combine(repository.Path, BoxFolder, Objects, folderHash);
            
            var folderInfo = new FolderInfo()
            {
                Hash = folderHash
            };

            var lines = File.ReadAllLines(repositoryBoxObjectPath);
            foreach (var line in lines)
            {
                if(!string.IsNullOrEmpty(line))
                {
                    var (entryType, entryHash, entryName) = ParseFolderLine(line);    
                    folderInfo.Entries.Add(entryName, new FolderEntry()
                    {
                        Type = entryType,
                        Hash = entryHash,
                        Name = entryName
                    });
                }
            }

            return folderInfo;
        }

        public static FolderInfo GetFileInfo(FileRepository repository, string objectHash)
        {
            var repositoryBoxObjectPath = IO.Path.Combine(repository.Path, BoxFolder, Objects, objectHash);

            var objectInfo = new FolderInfo()
            {
                Type = ObjectType.File,
                Hash = objectHash
            };

            return objectInfo;
        }

        public static (ObjectType, string, string) ParseFolderLine(string line)
        {
            var lineParts = line.Trim().Split('-');
            if (lineParts.Count() != 3) throw new Exception($"Expected 3 parts when parsing line '{line}'");
            ObjectType objectType;
            if (!Enum.TryParse<ObjectType>(lineParts[0], true, out objectType)) throw new Exception($"Expected valid object type '{lineParts[0]}' when parsing line '{line}'");
            var hash = lineParts[1];
            var name = lineParts[2];
            return (objectType, hash, name);
        }

        public static string GetBoxObjectPath(FileRepository fileStorageRepository, string fileContents)
        {
            var fileData = System.Text.Encoding.UTF8.GetBytes(fileContents);
            byte[] hashData = Sha3Keccack.Current.CalculateHash(fileData);
            var hashHex = hashData.ToHex(true);
            var repositoryBoxObjectPath = Path.Combine(fileStorageRepository.Path, BoxFolder, Objects, hashHex);
            return repositoryBoxObjectPath;
        }
    }
}