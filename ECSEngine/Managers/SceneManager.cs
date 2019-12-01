using ECSEngine.Entities;
using System.Collections.Generic;

namespace ECSEngine.Managers
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
            mainCamera = new CameraEntity();
            AddEntity(mainCamera);

            lights = new List<LightEntity>();
            var mainLight = new LightEntity();
            lights.Add(mainLight);
            AddEntity(mainLight);
        }
    }
}