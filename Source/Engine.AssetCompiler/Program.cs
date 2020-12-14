using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Engine.AssetCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> archivePaths = new List<string>();
            foreach (var directory in Directory.GetDirectories(args[0]))
            {
                Logging.Log($"Writing archive for directory {directory}");
                WriteArchive(directory);
                archivePaths.Add(Path.GetFileName(directory));
                Logging.Log($"Done");
            }
            Logging.Log($"Written all archives");

            WriteArchiveList(args[0], archivePaths);
            Logging.Log($"Written archive list");
        }

        private static void WriteArchiveList(string directory, List<string> archivePaths)
        {
            var jsonObject = new List<string>();
            foreach (var archive in archivePaths)
            {
                jsonObject.Add(archive);
            }
            File.WriteAllText($"{directory}/archives.json", JsonConvert.SerializeObject(jsonObject));
        }

        private static void WriteArchive(string directory)
        {
            var files = new List<FileArchiveFile>();

            ReadFilesFromDirectory(ref files, directory, directory);

            Logging.Log($"Writing archive contents...");
            FileArchive fileArchive = new FileArchive(files);
            fileArchive.WriteToFile($"{directory}.alex");
        }

        private static void ReadFilesFromDirectory(ref List<FileArchiveFile> files, string directory, string rootDirectory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                var relativePath = Path.GetRelativePath(rootDirectory, file);
                files.Add(new FileArchiveFile(
                    relativePath,
                    0,
                    0,
                    CompressionMethod.None,
                    File.ReadAllBytes(file)
                ));

                Logging.Log($"Indexed {file} ({relativePath})...");
            }

            foreach (var subDir in Directory.GetDirectories(directory))
            {
                ReadFilesFromDirectory(ref files, subDir, rootDirectory);
            }
        }
    }
}
