namespace Engine.Renderer
{
    public interface IScreen
    {
        void LoadContent();
        void UnloadContent();
        void Update(float deltaTime);
        void Render();
        void Enter();
        void Exit();
    }
}