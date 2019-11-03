using ECSEngine.Events;
using ECSEngine.Render;

namespace ECSEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// </summary>
    public class MeshComponent : IComponent
    {
        Mesh mesh;
        public MeshComponent(string path)
        {
            mesh = new Mesh(path);
        }

        public void Draw()
        {
            // TODO: Draw mesh
        }

        public void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            // TODO: Handle event based on event type and arguments
        }

        public void Update()
        {
            // TODO: Handle game update (no logic here?)
        }
    }
}
