using Engine.ECS.Managers;
using Engine.Renderer.GL.Entities;
using Engine.Utils;
using System.Collections.Immutable;

namespace Engine.Renderer.GL.Managers
{
    public sealed class SceneManager : Manager<SceneManager> // TODO: we probably want to read scene data from a file later.
    {
        public ImmutableList<CameraEntity> Cameras { get; private set; }

        /// <summary>
        /// The main world camera used to render all entities. Defined as the first object in the Cameras list.
        /// </summary>
        public CameraEntity MainCamera => Cameras[0];

        public ImmutableList<LightEntity> Lights { get; private set; }

        /// <summary>
        /// Construct a world manager containing any entities required.
        /// </summary>
        public SceneManager()
        {
            AddAllLightEntities();
            AddAllCameraEntities();
        }

        /// <summary>
        /// Add all of the scene's light entities.
        /// </summary>
        private void AddAllLightEntities()
        {
            // TODO: make this modifiable at run-time or by the game itself.
            Lights = ImmutableList.Create(
                new LightEntity()
                {
                    Name = "Spot Light"
                }
            );

            foreach (var light in Lights)
                AddEntity(light);
        }

        private void AddAllCameraEntities()
        {
            // TODO: make this modifiable at run-time or by the game itself.
            Cameras = ImmutableList.Create(
                new CameraEntity(GameSettings.GameResolutionX,GameSettings.GameResolutionY)
                {
                    Name = "Main Camera"
                }
            );

            foreach (var camera in Cameras)
                AddEntity(camera);
        }
    }
}