using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Xunit;
using SharpFileSystem.IO;
using SharpFileSystem.SharpZipLib;

namespace SharpFileSystem.Tests.SharpZipLib
{
    
    public class SharpZipLibFileSystemTest : IDisposable
    {
        private Stream zipStream;
        private SharpZipLibFileSystem fileSystem;
 
        public SharpZipLibFileSystemTest()
        {
            var memoryStream = new MemoryStream();
            zipStream = memoryStream;
            var zipOutput = new ZipOutputStream(zipStream);

            var fileContentString = "this is a file";
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContentString);
            zipOutput.PutNextEntry(new ZipEntry("textfileA.txt")
            {
                Size = fileContentBytes.Length
            });
            zipOutput.Write(fileContentBytes);
            zipOutput.PutNextEntry(new ZipEntry("directory/fileInDirectory.txt"));
            zipOutput.Finish();

            memoryStream.Position = 0;
            fileSystem = SharpZipLibFileSystem.Open(zipStream);
        }
 
        public void Dispose()
        {
            fileSystem.Dispose();
            zipStream.Dispose();
        }

        private readonly FileSystemPath directoryPath = FileSystemPath.Parse("/directory/");
        private readonly FileSystemPath textfileAPath = FileSystemPath.Parse("/textfileA.txt");
        private readonly FileSystemPath fileInDirectoryPath = FileSystemPath.Parse("/directory/fileInDirectory.txt");

        [Fact]
        public void GetEntitiesOfRootTest()
        {
            Assert.Equal(new[]
            {
                textfileAPath,
                directoryPath
            }, fileSystem.GetEntities(FileSystemPath.Root).ToArray());
        }

        [Fact]
        public void GetEntitiesOfDirectoryTest()
        {
            Assert.Equal(new[]
            {
                fileInDirectoryPath
            }, fileSystem.GetEntities(directoryPath).ToArray());
        }

        [Fact]
        public void ExistsTest()
        {
            Assert.True(fileSystem.Exists(FileSystemPath.Root));
            Assert.True(fileSystem.Exists(textfileAPath));
            Assert.True(fileSystem.Exists(directoryPath));
            Assert.True(fileSystem.Exists(fileInDirectoryPath));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/nonExistingFile")));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/nonExistingDirectory/")));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/directory/nonExistingFileInDirectory")));
        }
    }
}
