using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;

namespace UniversalIdentity.Library.Test.Infra.Storage;

public class FileRepositoryInfraTests : TestsBase
{
    public FileRepositoryInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

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