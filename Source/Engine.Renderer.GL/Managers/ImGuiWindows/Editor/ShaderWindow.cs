﻿using Engine.Assets;
using Engine.Events;
using Engine.Renderer.GL.Components;
using ImGuiNET;
using System;
using System.IO;
using System.Reflection;

namespace Engine.Renderer.GL.Managers.ImGuiWindows.Editor
{
    class ShaderWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Adjust;
        public override string Title { get; } = "Shaders";

        private bool enableWatcher = true;
        private FileSystemWatcher watcher;

        public ShaderWindow() : base()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Content";
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;
            watcher.Changed += new FileSystemEventHandler(OnWatcherChanged);
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

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            if (eventType == Event.GameEnd)
            {
                watcher.Dispose();
            }
            base.HandleEvent(eventType, baseEventArgs);
        }
    }
}