using ECSEngine.Entities;
using System;
using System.Threading;

namespace ECSEngine.Managers
{
    public class UpdateManager : Manager<UpdateManager>
    {
        private DateTime lastUpdate;
        private int minimumUpdateDelay = 8;
        public override void Run()
        {
            int updateTime = (DateTime.Now - lastUpdate).Milliseconds;
            Debug.Log($"ms since update: {updateTime}");
            float deltaTime = Math.Max(updateTime, 0.1f) / 1000.0f;
            foreach (IEntity entity in SceneManager.instance.entities)
            {
                entity.Update(deltaTime); // TODO
            }
            lastUpdate = DateTime.Now;
            Thread.Sleep(Math.Max(minimumUpdateDelay - updateTime, 0));
        }
    }
}
