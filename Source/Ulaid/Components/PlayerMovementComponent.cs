using Engine.ECS.Notify;
using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers;
using Engine.Utils.MathUtils;
using OpenGL.CoreUI;
using System;

namespace Ulaid.Components
{
    class PlayerMovementComponent : Component<PlayerMovementComponent>
    {
        // inb4 "why are these public" - it's for imgui
        public Vector3 velocity;
        public Vector3 currentDirection;
        public Vector3 currentInput;
        public Vector3 currentRotation;
        public float acceleration = 0.125f;
        public float deceleration = 0.0625f;
        public float maxSpeed = 10.0f;
        public float mouseSensitivityMultiplier = 0.1f;
        public float rotationSensitivity = 3f;
        public float minVelocity = 0.05f;
        private TransformComponent transformComponent;
        private bool lockRotation;

        public override void Update(float deltaTime)
        {
            transformComponent.Position += velocity * deltaTime;
            SceneManager.Instance.mainCamera.Position = transformComponent.Position;

            if (SceneManager.Instance.mainCamera.RotationEuler.x > 90)
            {
                // currentRotation.x = 90;
            }
            SceneManager.Instance.mainCamera.RotationEuler = currentRotation * rotationSensitivity;
            transformComponent.RotationEuler = currentRotation * rotationSensitivity;

            currentDirection = (transformComponent.Forward * currentInput.z) + (transformComponent.Right * currentInput.x);
            currentDirection.y += currentInput.y;

            currentDirection = currentDirection.Normalized;

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

            if (velocity.Magnitude < minVelocity)
            {
                velocity = new Vector3(0, 0, 0);
            }
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

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            if (eventType == NotifyType.KeyDown || eventType == NotifyType.KeyUp)
            {
                KeyboardNotifyArgs keyboardEventArgs = (KeyboardNotifyArgs)notifyArgs;

                switch ((KeyCode)keyboardEventArgs.KeyboardKey)
                {
                    case KeyCode.W:
                        currentInput.z = eventType == NotifyType.KeyDown ? -1 : 0;
                        break;
                    case KeyCode.A:
                        currentInput.x = eventType == NotifyType.KeyDown ? -1 : 0;
                        break;
                    case KeyCode.S:
                        currentInput.z = eventType == NotifyType.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.D:
                        currentInput.x = eventType == NotifyType.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.Space:
                        currentInput.y = eventType == NotifyType.KeyDown ? 1 : 0;
                        break;
                    case KeyCode.Control:
                        currentInput.y = eventType == NotifyType.KeyDown ? -1 : 0;
                        break;
                    case KeyCode.F1:
                        if (eventType == NotifyType.KeyUp)
                            lockRotation = !lockRotation;
                        break;
                }
            }
            else if (eventType == NotifyType.MouseMove)
            {
                if (lockRotation)
                    return;
                MouseMoveNotifyArgs mouseEventArgs = (MouseMoveNotifyArgs)notifyArgs;
                currentRotation += new Vector3(mouseEventArgs.MouseDelta.y * -mouseSensitivityMultiplier,
                    mouseEventArgs.MouseDelta.x * -mouseSensitivityMultiplier,
                    0);
            }
            else if (eventType == NotifyType.GameStart)
            {
                transformComponent = GetComponent<TransformComponent>();
            }
        }
    }
}
