using System;
using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using OpenGL.CoreUI;

namespace UlaidGame.Components
{
    class PlayerMovementComponent : Component<PlayerMovementComponent>
    {
        // inb4 "why are these public" - it's for imgui
        public Vector3 velocity;
        public Vector3 currentDirection;
        public Vector3 currentRotation;
        public float acceleration = 0.125f;
        public float deceleration = 0.0625f;
        public float maxSpeed = 10.0f;
        public float mouseSensitivityMultiplier = 0.1f;
        public float rotationSensitivity = 0.5f;
        private TransformComponent transformComponent;
        private Vector2 lastMousePos;

        public override void Update(float deltaTime)
        {
            transformComponent.position += velocity * deltaTime;
            SceneManager.Instance.mainCamera.RotationEuler = EaseLerpVector3(SceneManager.Instance.mainCamera.RotationEuler,
                new Vector3(velocity.z + velocity.y, 0, velocity.x) * new Vector3(rotationSensitivity, rotationSensitivity, rotationSensitivity),
                0.01f);
            SceneManager.Instance.mainCamera.Position = transformComponent.position;

            if (currentRotation.Magnitude != 0)
            {
                // Debug.Log($"{currentRotation.normalized}");
                // currentDirection *= currentRotation.normalized;
            }

            velocity += currentDirection * acceleration;
            velocity += new Vector3(
                Math.Sign(velocity.x) * -deceleration,
                Math.Sign(velocity.y) * -deceleration,
                Math.Sign(velocity.z) * -deceleration
            );

            velocity = new Vector3(
                Math.Max(Math.Min(velocity.x, maxSpeed), -maxSpeed),
                Math.Max(Math.Min(velocity.y, maxSpeed), -maxSpeed),
                Math.Max(Math.Min(velocity.z, maxSpeed), -maxSpeed)
            );
        }

        public float EaseLerp(float a, float b, float t)
        {
            t = (float)(Math.Sin(t * (Math.PI / 2)));
            return (1.0f - t) * a + t * b;
        }

        public Vector3 EaseLerpVector3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(EaseLerp(a.x, b.x, t), EaseLerp(a.y, b.y, t), EaseLerp(a.z, b.z, t));
        }

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            if (eventType == Event.KeyDown || eventType == Event.KeyUp)
            {
                KeyboardEventArgs keyboardEventArgs = (KeyboardEventArgs)baseEventArgs;

                switch ((KeyCode)keyboardEventArgs.KeyboardKey)
                {
                    case KeyCode.W:
                        currentDirection.z = eventType == Event.KeyDown ? -1 : 0;
                        break;
                    case KeyCode.A:
                        currentDirection.x = eventType == Event.KeyDown ? -1 : 0;
                        break;
                    case KeyCode.S:
                        currentDirection.z = eventType == Event.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.D:
                        currentDirection.x = eventType == Event.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.Space:
                        currentDirection.y = eventType == Event.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.Control:
                        currentDirection.y = eventType == Event.KeyDown ? -1 : 0;
                        break;
                }
            }
            else if (eventType == Event.MouseMove)
            {
                MouseMoveEventArgs mouseEventArgs = (MouseMoveEventArgs)baseEventArgs;
                // TODO: Replace with mouse delta
                currentRotation += new Vector3((lastMousePos.y - mouseEventArgs.MousePosition.y) * -mouseSensitivityMultiplier,
                    (lastMousePos.x - mouseEventArgs.MousePosition.x) * -mouseSensitivityMultiplier,
                    0);
                lastMousePos = mouseEventArgs.MousePosition;
            }
            else if (eventType == Event.GameStart)
            {
                transformComponent = GetComponent<TransformComponent>();
            }
        }
    }
}
