using Engine.Utils.DebugUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Utils.FileUtils
{
    public class FileSystem
    {
        private static Dictionary<string, FileArchive> mountPoints { get; set; } = new Dictionary<string, FileArchive>();

        public static void LoadArchive(string archivePath) 
        {
            var mountPoint = Path.GetFileNameWithoutExtension(archivePath);
            if (mountPoints.ContainsKey(mountPoint))
            {
                Logging.Log($"{mountPoint} already exists as a mount point.", Logging.Severity.High);
                return;
            }

            mountPoints.Add(mountPoint, FileArchive.LoadFromFile(archivePath));
        }

        private static (string, string) ParseMountPoint(string path)
        {
            /*
             * Mount point is always /mountpoint/path
             * Examples:
             *  - /Models/rainier/scene.gltf
             *  - /Scripts/Player.cs
             */

            if (path.StartsWith("/"))
                path = path.Substring(1);

            var mountPoint = path.Substring(0, path.IndexOf("/"));
            var fsPath = path.Substring(path.IndexOf("/") + 1);

            return (mountPoint, fsPath);
        }

        public static Asset GetAsset(string path)
        {
            var parsedMountPoint = ParseMountPoint(path.Replace("\\", "/"));
            var mountPoint = parsedMountPoint.Item1;
            var fsPath = parsedMountPoint.Item2;
            if (!mountPoints.ContainsKey(mountPoint))
            {
                Logging.Log($"{parsedMountPoint} isn't mounted.", Logging.Severity.High);
                return Asset.Empty;
            }

            var mountedArchive = mountPoints[mountPoint];

            var files = mountedArchive.Files.Where(file => file.FileName.Replace("\\", "/").Equals(fsPath, StringComparison.CurrentCultureIgnoreCase));
            if (files.Count() == 0)
            {
                Logging.Log($"{path} wasn't found.", Logging.Severity.High);
                return Asset.Empty;
            }

            var file = files.First();
            return new Asset(path, file.FileData);
        }
    }
}
