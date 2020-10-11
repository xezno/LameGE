using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Engine.Utils.FileUtils
{
    public partial class FileArchive
    {
        public ImmutableList<FileArchiveFile> Files { get; }

        public FileArchive()
        {
            Files = ImmutableList<FileArchiveFile>.Empty;
        }

        public FileArchive(List<FileArchiveFile> files)
        {
            Files = files.ToImmutableList();
        }

        public void AddFile(FileArchiveFile file)
        {
            throw new NotImplementedException();
        }

        public static FileArchive LoadFromFile(string filePath)
        {
            // Archive header:
            // 4 bytes - magic number: ALEX (Amazingly Lame Engine, Xezno -- thanks dotequals) 
            // 4 bytes - file version maj: 01
            // 4 bytes - file version min: 00
            // 12 bytes - padding
            // --------------------
            // Archive directory
            // 4 bytes - file count
            // for each file:
            // 4 bytes - file name length
            // n bytes - file name string (null-terminated)
            // 8 bytes - file location
            // 8 bytes - file length (max 9.2 yottabytes, but technically limited by file location)
            // 4 bytes - compression method (00 for uncompressed, 01 for zstd)
            // 8 bytes - padding
            // --------------------
            // File data:
            // for each file:
            // 4 bytes - padding
            // n bytes - file data
            // 4 bytes - padding

            using var fileStream = File.Open(filePath, FileMode.Open);
            using var binaryReader = new BinaryReader(fileStream);

            // Archive header
            var magicNumber = binaryReader.ReadBytes(4);
            if (!Enumerable.SequenceEqual(magicNumber, new[] { (byte)'A', (byte)'L', (byte)'E', (byte)'X' }))
                throw new Exception("Not a valid ALEX file.");

            var fileMajor = binaryReader.ReadInt32();
            var fileMinor = binaryReader.ReadInt32();

            if (fileMajor != 01 || fileMinor != 00)
                throw new Exception($"Unknown file version {fileMajor}.{fileMinor}");

            binaryReader.ReadBytes(12); // Padding

            // Archive directory
            var files = new List<FileArchiveFile>();
            var fileCount = binaryReader.ReadInt32();
            for (int i = 0; i < fileCount; ++i)
            {
                var fileNameLength = binaryReader.ReadInt32();
                var fileNameBytes = binaryReader.ReadBytes(fileNameLength);
                var fileName = BitConverter.ToString(fileNameBytes);

                var fileLocation = binaryReader.ReadInt64();
                var fileLength = binaryReader.ReadInt64();
                var fileCompressionMethod = (CompressionMethod)binaryReader.ReadInt32();

                if (fileCompressionMethod != CompressionMethod.None)
                    throw new NotImplementedException($"Compression not yet implemented (requested {fileCompressionMethod})");

                binaryReader.ReadBytes(8); // Padding

                files.Add(new FileArchiveFile(
                    fileName,
                    fileLocation,
                    fileLength,
                    fileCompressionMethod
                ));
            }

            // File data
            foreach (var file in files)
            {
                binaryReader.ReadBytes(4); // Padding
                binaryReader.BaseStream.Seek(file.FileLocation, SeekOrigin.Begin);
                var fileData = binaryReader.ReadBytes((int)file.FileLength); // TODO: support long for reading from stream (i.e. remove cast)
                binaryReader.ReadBytes(4); // Padding
            }

            return new FileArchive(files);
        }

        public static FileArchive LoadFromData()
        {
            throw new NotImplementedException();
        }
    }
}
