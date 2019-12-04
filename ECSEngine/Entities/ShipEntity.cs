using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using OpenGL.CoreUI;

namespace ECSEngine.Entities
{
    public sealed class ShipEntity : Entity<ShipEntity>
    {
        // inb4 "why are these public" - it's for imgui
        public Vector3 velocity;
        public Vector3 currentDirection;
        public Vector3 currentRotation;
        public float acceleration = 0.125f;
        public float deceleration = 0.0625f;
        public float maxSpeed = 10.0f;

        private TransformComponent transformComponent;

        public ShipEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new CameraComponent());

            transformComponent = GetComponent<TransformComponent>();
        }

        public override void Update(float deltaTime)
        {
            transformComponent.position += velocity * deltaTime;
            transformComponent.rotationEuler = currentRotation;
            SceneManager.instance.mainCamera.position = transformComponent.position;

            velocity += currentDirection * acceleration;
            velocity += new Vector3(
                System.Math.Sign(velocity.x) * -deceleration,
                System.Math.Sign(velocity.y) * -deceleration,
                System.Math.Sign(velocity.z) * -deceleration
            );

            velocity = new Vector3(
                System.Math.Max(System.Math.Min(velocity.x, maxSpeed), -maxSpeed),
                System.Math.Max(System.Math.Min(velocity.y, maxSpeed), -maxSpeed),
                System.Math.Max(System.Math.Min(velocity.z, maxSpeed), -maxSpeed)
            );
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
                    case KeyCode.Space:
                        if (eventType == Event.KeyDown)
                            currentDirection.y = 1;
                        else
                            currentDirection.y = 0;
                        break;
                    case KeyCode.Control:
                        if (eventType == Event.KeyDown)
                            currentDirection.y = -1;
                        else
                            currentDirection.y = 0;
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