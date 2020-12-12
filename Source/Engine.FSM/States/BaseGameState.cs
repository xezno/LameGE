using Engine.ECS.Observer;
using Engine.Common;
using Quincy.Managers;
using System;
using Engine.FSM.Managers;

namespace Engine.FSM.States
{
    public abstract class BaseGameState : IState
    {
        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            SceneManager.Instance.LoadSceneFromAsset(ServiceLocator.FileSystem.GetAsset("/Scenes/testScene.json"));
        }

        public void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            // throw new NotImplementedException();
        }

        public void Render()
        {
            // throw new NotImplementedException();
        }

        public void Update()
        {
            // throw new NotImplementedException();
        }
    }
}
