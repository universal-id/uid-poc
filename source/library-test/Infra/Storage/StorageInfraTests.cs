using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;

namespace UniversalIdentity.Library.Test.Infra.Storage;

public class StorageInfraTests : TestsBase
{
    public StorageInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// Developer can programmatically
    /// - Programmatically create empty storage
    /// - Cannot programmatically create a non-existent storage object
    /// - All API calls dealing with empty or non-existent storage throw exception
    public void EmptyIdBoxStorageServiceTest()
    {
        using (var testContext = new TestContext(nameof(EmptyIdBoxStorageServiceTest), this))
        {
            try { new IdBoxStorageService(null); Assert.True(false); }
            catch {}

            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            testContext.Info($">{tempPath}");
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxStorageService = new IdBoxStorageService(uniqueString);
            idBoxStorageService.IdBoxStorage.Should().NotBeNull("Service and main IdBox are created.");

            try { idBoxStorageService.CreateSeedIdentity(); Assert.True(false); }
            catch {}
        }   
    }

    // [Fact] /// Developer can programmatically
    // /// - Create a seed identity - given access to user and agent service objects with private keys
    // public void IdBoxStorageServiceCreateSeedIdentityTest()
    // {
    //     using (var testContext = new TestContext(nameof(IdBoxStorageServiceCreateSeedIdentityTest), this))
    //     {
    //         var tempPath = Path.GetTempPath();
    //         var uniqueString = testContext.Uuid.ToString() + "-id-box";
    //         tempPath = Path.Combine(tempPath, uniqueString);
    //         if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
    //         var idBoxStorageService = new IdBoxStorageService(uniqueString);
    //         idBoxStorageService.IdBoxStorage.Should().NotBeNull("Service and main IdBox are created.");

    //         var identityStorage = idBoxStorageService.CreateSeedIdentity();
    //         identityStorage.Should().NotBeNull();
    //     }   
    // }

     [Fact] /// Developer can programmatically:
    /// - Programmatically create empty storage object
    /// - Not create a non-existent storage object
    /// - All API calls dealing with empty or non-existent storage throw exception
    public void EmptyIdBoxStorageTest()
    {
        using (var testContext = new TestContext(nameof(EmptyIdBoxStorageTest), this))
        {
            try { new IdBoxStorageService(null); Assert.True(false); }
            catch {}

            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxStorage = new IdBoxStorage(uniqueString);
            idBoxStorage.Repository.Should().NotBeNull("Repository is created under the storage object.");

            try { idBoxStorage.InitializeStorage(); Assert.True(false); }
            catch {}
        }   
    }

    [Fact] /// Developer can programmatically
    /// - Create a seed core identity object (using user and agent service objects with private keys)
    public void IdBoxStorageCreateSeedIdentityTest()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            var idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            var seedIdentityStorage = new IdentityStorage()
            {
                
            };
        }   
    }

    [Fact] /// Developer can programmatically
    /// - Programmatically create empty storage repository
    /// - Cannot programmatically create a non-existent storage repository
    /// - All API calls dealing with empty or non-existent storage repository throw exception
    public void EmptyRepositoryTest()
    {
        using (var testContext = new TestContext(nameof(EmptyRepositoryTest), this))
        {
            try { new FileRepository(null); Assert.True(false); }
            catch {}

            var fileStorageRepository = new FileRepository("Some invalid string.");

            try { fileStorageRepository.Init(); Assert.True(false); }
            catch {}
        }   
    }

    [Fact] /// Developer can programmatically
    /// - Initiate a repository 
    public void RepositoryInitTest()
    {
        using (var testContext = new FileRepositoryTestContext(nameof(RepositoryInitTest), this))
        {
            var fileStorageRepository = testContext.FileRepository;
            var boxRepositoryPath = System.IO.Path.Combine(fileStorageRepository.Path, FileRepositoryHelper.BoxFolder);
            Directory.Exists(boxRepositoryPath).Should().BeFalse();

            fileStorageRepository.Init();
            Directory.Exists(boxRepositoryPath).Should().BeTrue();
        }   
    }

    [Fact] /// Developer can programmatically
    /// - access a file at the root of repository 
    public void RepositoryRootFileAccessTest()
    {
        using (var testContext = new FileRepositoryTestContext(nameof(RepositoryInitTest), this))
        {
            var fileStorageRepository = testContext.FileRepository;
            fileStorageRepository.Init();
            
            var filePath = "";
            var fileName = "test.txt";
            var fileContents = "This is a test file.";
            fileStorageRepository.UpdateOneFile(filePath, fileName, fileContents);
            var repositoryBoxObjectPath = FileRepositoryHelper.GetBoxObjectPath(fileStorageRepository, fileContents);
            File.Exists(repositoryBoxObjectPath).Should().BeTrue();

            string updatedFileContents = "This is an updated test file.";
            fileStorageRepository.UpdateOneFile(filePath, fileName, updatedFileContents);
            var repositoryBoxUpdatedObjectPath = FileRepositoryHelper.GetBoxObjectPath(fileStorageRepository, updatedFileContents);
            File.Exists(repositoryBoxUpdatedObjectPath).Should().BeTrue();

            fileStorageRepository.DeleteOneFile(filePath, fileName);
            var deletedFileContents = fileStorageRepository.GetFileContents(filePath, fileName);
            deletedFileContents.Should().BeNull();   
        }  
    }

    [Fact] /// Developer can programmatically
    /// - access a file at the root of repository 
    public void RepositoryFileAccessInFolderTest()
    {
        using (var testContext = new FileRepositoryTestContext(nameof(RepositoryFileAccessInFolderTest), this))
        {
            var fileStorageRepository = testContext.FileRepository;
            fileStorageRepository.Init();
            
            var filePath = "folder";
            var fileName = "test.txt";
            var fileContents = "This is a test file.";
            fileStorageRepository.UpdateOneFile(filePath, fileName, fileContents);

            var repositoryBoxObjectPath = FileRepositoryHelper.GetBoxObjectPath(fileStorageRepository, fileContents);
            File.Exists(repositoryBoxObjectPath).Should().BeTrue();

            string updatedFileContents = "This is an updated test file.";
            fileStorageRepository.UpdateOneFile(filePath, fileName, updatedFileContents);
              var repositoryBoxUpdatedObjectPath = FileRepositoryHelper.GetBoxObjectPath(fileStorageRepository, updatedFileContents);
            File.Exists(repositoryBoxUpdatedObjectPath).Should().BeTrue();

            fileStorageRepository.DeleteOneFile(filePath, fileName);            
            var deletedFileContents = fileStorageRepository.GetFileContents(filePath, fileName);
            deletedFileContents.Should().BeNull();
        }    
    }
}