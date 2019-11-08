using ECSEngine.Components;

using OpenGL;

namespace ECSEngine.Entities
{
    public class CameraEntity : Entity<CameraEntity>
    {
        public Matrix4x4f projMatrix => GetComponent<CameraComponent>().projMatrix;
        public Matrix4x4f viewMatrix => GetComponent<CameraComponent>().viewMatrix;

        public CameraEntity()
        {
            AddComponent(new CameraComponent());
        }

    }
}
