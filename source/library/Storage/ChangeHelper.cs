using System.IO;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using IO = System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UniversalIdentity.Library.Storage
{
    public static class ChangeHelper
    {

        // Prerequisite: All new and updated files with contents are added to repository with hashes
        // Passes: 1. Identify all change graph nodes, including affected folders, in a tree structure.
        // 2. Recursively work up from leaf files, and update parent folders
        // 3. Commit all parent folders after changes are finalized
        // (optional) [under staging "/"] 4. Recursively work down the tree and make appropriate updates
        public static void ProcessChanges(FileRepository repository, IEnumerable<SimpleChange> changes)
        {
            // No changes
            if (!changes.GetEnumerator().MoveNext()) return;

            // Construct root change graph node
            var repositoryBoxLatestPath = IO.Path.Combine(repository.Path, FileRepositoryHelper.BoxFolder, FileRepositoryHelper.Latest);
            var rootFolderHash = File.ReadAllText(repositoryBoxLatestPath);
            var rootNode = GetChangeNodeFromExistingFolder(repository, string.Empty, string.Empty, rootFolderHash);

            // Pass 1 - Identify all change graph nodes, including affected folders, in a tree structure.
            // Iterate through each change entry
            foreach(var change in changes)
            {
                // Start from existing root folder
                var (name, segments) = FileRepositoryHelper.GetNameAndSegments(change.Path);
                var parentNode = rootNode;
                var currentPath = string.Empty;

                // Iterate through the object segments starting from one under root
                foreach(var segment in segments)
                {
                    // Append to existing path to get current path
                    currentPath += $"/{segment}";

                    // Get or create current node if not already exists
                    ChangeNode currentNode = null;
                    if (!parentNode.Children.TryGetValue(segment, out currentNode))
                    {
                        FolderEntry folderEntry = null;
                        if (parentNode.PreviousFolder.Entries.TryGetValue(segment, out folderEntry))
                        {
                            currentNode = GetChangeNodeFromExistingFolder(repository, currentPath, segment, folderEntry?.Hash);
                        }
                        else
                        {
                            currentNode = GetChangeNodeForNewFolder(repository, currentPath, segment);
                        }

                        currentNode.Parent = parentNode;
                    }

                    // Connect parent and child nodes
                    parentNode.Children.Add(segment, currentNode);
                    parentNode = currentNode;
                }

                // Create change node for leaf file, as well as connect parent and child
                var objectNode = new ChangeNode()
                {
                    Name = name,
                    Parent = parentNode,
                    ObjectType = ObjectType.File,
                    FileSimpleChange = change
                };
                parentNode.Children.Add(name, objectNode);
            }

            // Pass 2 - Recursively work up from leaf files, and update parent folders
            Pass2Recursive(rootNode);   

            // Pass 3 - Commit all parent folders after changes are finalized
            var newRootHash = Pass3Recursive(repository, rootNode);
            File.WriteAllText(repositoryBoxLatestPath, newRootHash);

        }

        public static ChangeNode GetChangeNodeFromExistingFolder(FileRepository repository, string folderPath, string folderName, string folderHash)
        {
            var folderInfo = FileRepositoryHelper.GetFolderInfo(repository, folderHash);
            var rootNode = new ChangeNode() 
            {
                Name = folderName,
                PreviousObject = folderInfo,
                ObjectType = ObjectType.Folder
            };
            return rootNode;
        }

        public static ChangeNode GetChangeNodeForNewFolder(FileRepository repository, string folderPath, string folderName)
        {
            var rootNode = new ChangeNode() 
            {
                Name = folderName,
                PreviousObject = null,
                ObjectType = ObjectType.Folder
            };
            return rootNode;
        }

        // Pass 2 - Recursively work up from leaf files, and update parent folders
        public static bool Pass2Recursive(ChangeNode startNode)
        {
            if(startNode.ObjectType == ObjectType.Folder)
            {
                if(startNode.NextFolder == null)
                {
                    if(startNode.PreviousFolder != null)
                    {
                        startNode.NextObject = CloneFolderInfo(startNode.PreviousFolder);
                    }
                    else
                    {
                        startNode.NextObject = new FolderInfo() { Type = ObjectType.Folder };
                    }
                }

                var keepFolder = false;
                foreach (var childKeyValuePair in startNode.Children)
                {
                    var childName  = childKeyValuePair.Key;
                    var childNode  = childKeyValuePair.Value;

                    if (!Pass2Recursive(childNode))
                    {
                        if (!startNode.NextFolder.Entries.ContainsKey(childName))
                        {
                            startNode.NextFolder.Entries.Add(childName, new FolderEntry()
                            {
                                Type = childNode.ObjectType,
                                Name = childName,
                            });
                        }

                        keepFolder = true;
                    }
                    else
                    {
                        if (startNode.NextFolder.Entries.ContainsKey(childName))
                        {
                            startNode.NextFolder.Entries.Remove(childName);
                        }
                    }                    
                }

                return !keepFolder || startNode.NextFolder.Entries.Count == 0;
            }
            else if(startNode.ObjectType == ObjectType.File)
            {
                if(startNode.FileSimpleChange.Type == ChangeType.Delete)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // This should not happen
                throw new System.Exception("Expected at least on object info to exist.");
            }
        }

         // Pass 3 - Recursively work up from leaf files, and update parent folders
        public static string Pass3Recursive(FileRepository repository, ChangeNode startNode)
        {
            if(startNode.ObjectType == ObjectType.Folder)
            {
                foreach (var childKeyValuePair in startNode.Children)
                {
                    var childName  = childKeyValuePair.Key;
                    var childNode  = childKeyValuePair.Value;

                    var hash = Pass3Recursive(repository, childNode);
                    if (startNode.NextFolder.Entries.ContainsKey(childName))
                    {
                        startNode.NextFolder.Entries[childName].Hash = hash;
                    }
                }

                var folderHash = FileRepositoryHelper.CreateFolderObject(repository, startNode.NextFolder.Entries.Values);
                return folderHash;
            }
            else if(startNode.ObjectType == ObjectType.File)
            {
                var fileContents = startNode.FileSimpleChange.Contents;
                if(fileContents != null)
                {
                    var fileHash = FileRepositoryHelper.GetHashHex(fileContents);
                    var repositoryBoxObjectPath = IO.Path.Combine(repository.Path, FileRepositoryHelper.BoxFolder, FileRepositoryHelper.Objects, fileHash);
                    File.WriteAllText(repositoryBoxObjectPath, fileContents);
                    return fileHash;
                }

                return null;
                // Do nothing
            }
            else
            {
                // This should not happen
                throw new System.Exception("Expected at least on object info to exist.");
            }
        }

        public static FolderInfo CloneFolderInfo(FolderInfo folderInfo)
        {
            var result = new FolderInfo() 
            {
                Type = ObjectType.Folder
            };

            foreach (var entry in folderInfo.Entries)
            {
                result.Entries.Add(entry.Key, new FolderEntry() 
                { 
                    Name = entry.Value.Name,
                    Hash = entry.Value.Hash,
                    Type = entry.Value.Type,
                });
            }

            return result;
        }
    }


      public class SimpleChange
        {
            public ObjectType ObjectType { get; set; }
            public ChangeType Type { get; set; }
            public string Path { get; set; }
            public string Contents { get; set; } // To add support for other file and later for any stream
        }

        public enum ChangeType 
        { 
            Unspecified = 0,
            Update, // Create or update
            Delete, 
            Rename
        };


    public class ChangeNode
    {
        public string Name { get; set; }
        public ChangeNode? Parent { get; set; }
        public ObjectType ObjectType { get; set; }
        public Dictionary<string, ChangeNode> Children = new Dictionary<string, ChangeNode>(StringComparer.OrdinalIgnoreCase);
        public ObjectInfo PreviousObject { get; set; }
        public FolderInfo PreviousFolder { get { return (FolderInfo)this.PreviousObject; } }
        public ObjectInfo NextObject { get; set; }
        public FolderInfo NextFolder { get { return (FolderInfo)this.NextObject; }}
        public SimpleChange FileSimpleChange { get; set; }
    }

    public enum ObjectType
    {
        Unspecified = 0,
        File,
        Folder,
        Iteration
    }

    public record FolderEntry
    {
        public ObjectType Type { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string GetLine()
        {
            return $"{Enum.GetName(typeof(ObjectType), this.Type).ToLowerInvariant()}-{this.Hash}-{this.Name}";
        }
    }

    public class FolderInfo : ObjectInfo
    {
        public Dictionary<string, FolderEntry> Entries = new Dictionary<string, FolderEntry>();
    }

    public class ObjectInfo
    {
        public ObjectType Type { get; set; }
        public string Hash { get; set; }
    }
}