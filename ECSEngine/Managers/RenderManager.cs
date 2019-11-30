using ECSEngine.Entities;

namespace ECSEngine.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        /// <summary>
        /// Render all the entities within the world manager.
        /// </summary>
        public override void Run()
        {
            foreach (IEntity entity in SceneManager.instance.entities)
            {
                entity.Render();
            }
        }
    }
}
