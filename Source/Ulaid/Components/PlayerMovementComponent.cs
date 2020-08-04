using Engine.ECS.Observer;
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

        public Vector3 CurrentInput { get => currentInput; set => currentInput = value; }
        public Vector3 CurrentDirection { get; set; }

        public Vector3d Velocity { get; set; }
        public Vector3d CurrentRotation { get; set; }
        public float Acceleration { get; set; } = 0.125f;
        public float Deceleration { get; set; } = 0.0625f;
        public float MaxSpeed { get; set; } = 10.0f;
        public float MouseSensitivityMultiplier { get; set; } = 0.1f;
        public float RotationSensitivity { get; set; } = 3f;
        public float MinVelocity { get; set; } = 0.05f;

        public override void Update(float deltaTime)
        {
            var sceneCamera = SceneManager.Instance.mainCamera;
            var sceneCameraTransform = sceneCamera.GetComponent<TransformComponent>();

            transformComponent.Position += Velocity * deltaTime;
            sceneCameraTransform.Position = transformComponent.Position;

            sceneCameraTransform.RotationEuler = CurrentRotation * RotationSensitivity;
            transformComponent.RotationEuler = CurrentRotation * RotationSensitivity;

            var newDirection = (transformComponent.Forward * CurrentInput.z) + (transformComponent.Right * CurrentInput.x) + (transformComponent.Up * CurrentInput.y);
            newDirection.Normalize();

            CurrentDirection = newDirection.Normalized;

            Velocity += CurrentDirection.ToVector3d() * Acceleration;
            Velocity += new Vector3d(
                Math.Sign(Velocity.x) * -Deceleration,
                Math.Sign(Velocity.y) * -Deceleration,
                Math.Sign(Velocity.z) * -Deceleration
            );

            Velocity = new Vector3d(
                Math.Max(Math.Min(Velocity.x, MaxSpeed), -MaxSpeed),
                Math.Max(Math.Min(Velocity.y, MaxSpeed), -MaxSpeed),
                Math.Max(Math.Min(Velocity.z, MaxSpeed), -MaxSpeed)
            );

            if (Velocity.Magnitude < MinVelocity)
            {
                Velocity = new Vector3d(0, 0, 0);
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
            switch (eventType)
            {
                case NotifyType.KeyDown:
                case NotifyType.KeyUp:
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
                        case KeyCode.F3:
                        case KeyCode.F1:
                            if (eventType == NotifyType.KeyUp)
                                lockRotation = !lockRotation;
                            break;
                    }

                    break;
                }
                case NotifyType.MouseMove:
                {
                    if (lockRotation)
                        return;

                    MouseMoveNotifyArgs mouseEventArgs = (MouseMoveNotifyArgs)notifyArgs;
                    CurrentRotation += new Vector3d(mouseEventArgs.MouseDelta.y * -MouseSensitivityMultiplier,
                        mouseEventArgs.MouseDelta.x * -MouseSensitivityMultiplier,
                        0);
                    break;
                }
                case NotifyType.GameStart:
                    transformComponent = GetComponent<TransformComponent>();
                    break;
            }
        }
    }
}
