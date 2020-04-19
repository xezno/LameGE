using ECSEngine.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            mainCamera = new CameraEntity()
            {
                Name = "Main Camera"
            };
            AddEntity(mainCamera);

            lights = new List<LightEntity>();
            var mainLight = new LightEntity()
            {
                Name = "Test Light"
            };
            lights.Add(mainLight);
            AddEntity(mainLight);

            
            RconManager.Instance.RegisterCommand("getHierarchy", "List scene hierarchy",
                () => JsonConvert.SerializeObject(Entities));
        }
    }
}