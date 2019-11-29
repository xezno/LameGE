namespace ECSEngine
{
    // TODO: wow... is this even necessary? Seems to just add more inheritance complexity.
    public interface IBase
    {
        IBase parent { get; set; }

        void Render();

        void Update(float deltaTime);
    }
}
