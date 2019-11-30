using ECSEngine.Entities;

namespace ECSEngine.Managers 
{ 
    public class UpdateManager : Manager<UpdateManager>
    {
        public override void Run()
        {
            foreach (IEntity entity in SceneManager.instance.entities)
            {
                entity.Update(0.001f); // TODO
            }
        }
    }
}
