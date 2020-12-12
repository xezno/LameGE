namespace Engine.Common.FileUtils.FileSystems
{
    public interface IFileSystem
    {
        Asset GetAsset(string path);
        void Init(string contentPath);
    }
}
