using Engine.ECS.Managers;
using Engine.ECS.Observer;
using Engine.FSM.States;

namespace Engine.FSM.Managers
{
    public interface IState
    {
        void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs);
        void Update();
        void Init();
        void Cleanup();
        void Render();
    }

    public class StateManager : Manager<StateManager>
    {
        private IState currentState;

        public StateManager()
        {
            ChangeState(new LoadingState());
        }

        public override void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            currentState.OnNotify(notifyType, notifyArgs);
        }

        public override void Run()
        {
            currentState.Update();
        }

        public void ChangeState(IState newState)
        {
            currentState?.Cleanup();

            currentState = newState;
            newState?.Init();
        }
    }
}
