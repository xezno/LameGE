using Engine.ECS.Managers;
using Engine.Renderer.GL.Entities;
using Engine.Utils;
using System.Collections.Generic;

namespace Engine.Renderer.GL.Managers
{
    public sealed class SceneManager : Manager<SceneManager> // TODO: we probably want to read scene data from a file later.
    {
        /// <summary>
        /// The main world camera used to render all entities.
        /// </summary>
        public readonly CameraEntity mainCamera;

        public List<LightEntity> lights;

        /// <summary>
        /// Construct a world manager containing any entities required.
        /// </summary>
        public SceneManager()
        {
            mainCamera = new CameraEntity(GameSettings.GameResolutionX, GameSettings.GameResolutionY)
            {
                Name = "Main Camera"
            };
            AddEntity(mainCamera);

            lights = new List<LightEntity>();
            var mainLight = new LightEntity()
            {
                Name = "Spot Light"
            };

            lights.Add(mainLight);
            AddEntity(mainLight);
        }
    }
}