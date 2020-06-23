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
        private TransformComponent transformComponent;
        private bool lockRotation;
        private Vector3 currentInput;
        private Vector3 currentDirection;

        public Vector3 Velocity { get; set; }
        public Vector3 CurrentRotation { get; set; }
        public float Acceleration { get; set; } = 0.125f;
        public float Deceleration { get; set; } = 0.0625f;
        public float MaxSpeed { get; set; } = 10.0f;
        public float MouseSensitivityMultiplier { get; set; } = 0.1f;
        public float RotationSensitivity { get; set; } = 3f;
        public float MinVelocity { get; set; } = 0.05f;

        public override void Update(float deltaTime)
        {
            transformComponent.Position += Velocity * deltaTime;
            SceneManager.Instance.mainCamera.Position = transformComponent.Position;

            SceneManager.Instance.mainCamera.RotationEuler = CurrentRotation * RotationSensitivity;
            transformComponent.RotationEuler = CurrentRotation * RotationSensitivity;

            var newDirection = (transformComponent.Forward * currentInput.z) + (transformComponent.Right * currentInput.x);
            newDirection.y = currentDirection.y + currentInput.y;
            newDirection.Normalize();

            currentDirection = currentDirection.Normalized;

            Velocity += currentDirection * Acceleration;
            Velocity += new Vector3(
                Math.Sign(Velocity.x) * -Deceleration,
                Math.Sign(Velocity.y) * -Deceleration,
                Math.Sign(Velocity.z) * -Deceleration
            );

            Velocity = new Vector3(
                Math.Max(Math.Min(Velocity.x, MaxSpeed), -MaxSpeed),
                Math.Max(Math.Min(Velocity.y, MaxSpeed), -MaxSpeed),
                Math.Max(Math.Min(Velocity.z, MaxSpeed), -MaxSpeed)
            );

            if (Velocity.Magnitude < MinVelocity)
            {
                Velocity = new Vector3(0, 0, 0);
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
                CurrentRotation += new Vector3(mouseEventArgs.MouseDelta.y * -MouseSensitivityMultiplier,
                    mouseEventArgs.MouseDelta.x * -MouseSensitivityMultiplier,
                    0);
            }
            else if (eventType == NotifyType.GameStart)
            {
                transformComponent = GetComponent<TransformComponent>();
            }
        }
    }
}
