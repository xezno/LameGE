using Engine.ECS.Managers;
using Engine.Renderer.GL.Render;
using System.Collections.Generic;

namespace Engine.Renderer.GL.Managers
{
    // TODO: Make this a lot nicer, bundle asset types into one base class
    public class AssetManager : Manager<AssetManager>
    {
        public Dictionary<string, Texture2D> Textures { get; set; } = new Dictionary<string, Texture2D>();
        public Dictionary<string, Shader> Shaders { get; set; } = new Dictionary<string, Shader>();
    }
}
