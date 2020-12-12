using Engine.ECS.Observer;
using Engine.FSM.Managers;
using System;

namespace Engine.FSM.States
{
    class LoadingState : IState
    {
        private bool loadComplete = false;
        public void Cleanup()
        {
            // Get ready to switch to a new game state; since this is a loading screen,
            // we don't really want to *un-load* everything here, otherwise this state
            // has no purpose whatsoever.
        }

        public void Init()
        {
            // Load all game content
            // When finished, change state to base game

            loadComplete = true;
        }

        public void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
        }

        public void Render()
        {
        }

        public void Update()
        {
            if (loadComplete)
                Broadcast.Notify(NotifyType.LoadFinished, new GenericNotifyArgs(this));
        }
    }
}
