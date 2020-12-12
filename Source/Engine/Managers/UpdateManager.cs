using Engine.ECS.Managers;
using Engine.FSM.Managers;
using Quincy.Managers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Managers
{
    public class UpdateManager : Manager<UpdateManager>
    {
        private DateTime lastUpdate;
        private int minimumUpdateDelay = 8;
        public override void Run()
        {
            var updateTime = (DateTime.Now - lastUpdate).Milliseconds;
            var deltaTime = Math.Max(updateTime, 0.1f) / 1000.0f;

            // Run both on main thread
            Task.Factory.StartNew(() =>
            {
                SceneManager.Instance.Update();
                StateManager.Instance.Run();
            });

            lastUpdate = DateTime.Now;
            Thread.Sleep(Math.Max(minimumUpdateDelay - updateTime, 0));
        }
    }
}
