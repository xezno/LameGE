using ECSEngine.Components;
using ECSEngine.Math;
using OpenGL;

namespace ECSEngine.Entities
{
    public class CameraEntity : Entity<CameraEntity>
    {
        public Matrix4x4f projMatrix => GetComponent<CameraComponent>().projMatrix;
        public Matrix4x4f viewMatrix => GetComponent<CameraComponent>().viewMatrix;

        public CameraEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), ECSEngine.Math.Quaternion.Identity));
            AddComponent(new CameraComponent());
        }

    }
}
