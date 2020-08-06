using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers;

namespace Engine.Entities
{
    // TODO: Should this be a standard part of all TransformComponents?
    public class MoveWithParentComponent : Component<MoveWithParentComponent>
    {
        public override void Update(float deltaTime)
        {
            var transform = GetComponent<TransformComponent>();
            var sceneCamera = SceneManager.Instance.mainCamera;
            var sceneCameraTransform = sceneCamera.GetComponent<TransformComponent>();

            transform.Position = sceneCameraTransform.Position; // big brain probably
        }
    }
}
