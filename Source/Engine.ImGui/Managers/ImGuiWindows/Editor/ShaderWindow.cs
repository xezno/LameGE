using Engine.Assets;
using Engine.ECS.Observer;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers;
using ImGuiNET;
using System.IO;
using System.Reflection;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class ShaderWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Adjust;
        public override string Title { get; } = "Shaders";

        private bool enableWatcher = true;
        private FileSystemWatcher watcher;

        public ShaderWindow()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Content";
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;
            watcher.Changed += OnWatcherChanged;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = enableWatcher;
        }

        private void OnWatcherChanged(object sender, FileSystemEventArgs eventargs)
        {
            // TODO: Determine which shader was modified and only reload that one
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.HasComponent<ShaderComponent>())
                {
                    // We're doing this from another thread so mark the shaders as
                    // dirty, forcing a reload next gl draw
                    entity.GetComponent<ShaderComponent>().ShadersDirty = true;
                }
            }
        }

        public override void Draw()
        {
            if (ImGui.Checkbox("Hot-reload", ref enableWatcher))
            {
                watcher.EnableRaisingEvents = enableWatcher;
            }

            if (ImGui.Button("Reload all shaders"))
            {
                ReloadAllShaders();
            }
        }

        private void ReloadAllShaders()
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.HasComponent<ShaderComponent>())
                {
                    entity.GetComponent<ShaderComponent>().ReloadAll();
                }
            }
        }

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            if (eventType == NotifyType.GameEnd)
            {
                watcher.Dispose();
            }
            base.OnNotify(eventType, notifyArgs);
        }
    }
}
