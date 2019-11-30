using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Managers;

using OpenGL.CoreUI;

namespace ECSEngine.Entities
{
    public sealed class ShipEntity : Entity<ShipEntity>
    {
        // inb4 "why are these public" - it's for imgui
        public Vector3 velocity;
        public Vector3 currentDirection;
        public Vector3 currentRotation;
        public float acceleration = 0.5f;

        private TransformComponent transformComponent;

        public ShipEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1)));
            //AddComponent(new MeshComponent(""));
            AddComponent(new CameraComponent());

            transformComponent = GetComponent<TransformComponent>();

            ImGuiManager.instance.AddSerializableObject(this, "Ship");
        }

        public override void Update(float deltaTime)
        {
            transformComponent.position += velocity * deltaTime;
            transformComponent.rotationEuler = currentRotation;
            SceneManager.instance.mainCamera.position = transformComponent.position;
            velocity += currentDirection * acceleration;
        }

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            if (eventType == Event.KeyDown || eventType == Event.KeyUp)
            {
                KeyboardEventArgs keyboardEventArgs = (KeyboardEventArgs)baseEventArgs;

                switch ((KeyCode)keyboardEventArgs.keyboardKey)
                {
                    case KeyCode.W:
                        if (eventType == Event.KeyDown)
                            currentDirection.z = -1;
                        else
                            currentDirection.z = 0;
                        break;
                    case KeyCode.A:
                        if (eventType == Event.KeyDown)
                            currentDirection.x = -1;
                        else
                            currentDirection.x = 0;
                        break;
                    case KeyCode.S:
                        if (eventType == Event.KeyDown)
                            currentDirection.z = 1;
                        else
                            currentDirection.z = 0;
                        break;
                    case KeyCode.D:
                        if (eventType == Event.KeyDown)
                            currentDirection.x = 1;
                        else
                            currentDirection.x = 0;
                        break;
                }
            }
            else if (eventType == Event.MouseMove)
            {
                MouseMoveEventArgs mouseEventArgs = (MouseMoveEventArgs)baseEventArgs;
                // TODO: Replace with mouse delta
                // currentRotation += new Vector3(eventArgs.mousePosition.x, 0, eventArgs.mousePosition.y);
            }
        }
    }
}