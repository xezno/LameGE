using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils.MathUtils;
using OpenGL.CoreUI;
using System;

namespace ExampleGame.Components
{
    class PlayerMovementComponent : Component<PlayerMovementComponent>
    {
        private TransformComponent transformComponent;
        private bool lockRotation;
        private Vector3d currentInput;

        public Vector3d CurrentInput { get => currentInput; set => currentInput = value; }
        public Vector3d CurrentDirection { get; set; }
        public Vector3d HorizontalDirection { get; set; }

        public float VerticalVelocity { get; set; }
        public float ForwardVelocity { get; set; }
        public Vector3d CurrentRotation { get; set; }
        public float Acceleration { get; set; } = 2f;
        public float Deceleration { get; set; } = 1f;
        public float MaxSpeed { get; set; } = 15.0f;
        public float MouseSensitivityMultiplier { get; set; } = 0.1f;
        public float RotationSensitivity { get; set; } = 3f;
        public float MinVelocity { get; set; } = 0.001f;

        public override void Update(float deltaTime)
        {
            var sceneCamera = SceneManager.Instance.MainCamera;
            var sceneCameraTransform = sceneCamera.GetComponent<TransformComponent>();

            var newDirection = (transformComponent.Forward * (float)CurrentInput.Z) + (transformComponent.Right * (float)CurrentInput.X);
            newDirection.Normalize();

            CurrentDirection = EaseLerpVector3(newDirection.ToVector3d(), CurrentDirection, 0.7f);

            var newHorizontalDirection = (Vector3d.up * (float)currentInput.Y);
            newHorizontalDirection.Normalize();

            HorizontalDirection = EaseLerpVector3(newHorizontalDirection, HorizontalDirection, 0.7f);

            ForwardVelocity += (float)Math.Abs(CurrentInput.Magnitude) * Acceleration;
            ForwardVelocity += Math.Sign(ForwardVelocity) * -Deceleration;

            VerticalVelocity += (float)newHorizontalDirection.Magnitude * Acceleration;
            VerticalVelocity += Math.Sign(VerticalVelocity) * -Deceleration;

            ForwardVelocity = Math.Clamp(ForwardVelocity, -MaxSpeed, MaxSpeed);
            VerticalVelocity = Math.Clamp(VerticalVelocity, -MaxSpeed, MaxSpeed);

            // Accounts for rounding errors
            if (ForwardVelocity < MinVelocity)
            {
                ForwardVelocity = 0f;
            }

            if (VerticalVelocity < MinVelocity)
            {
                VerticalVelocity = 0f;
            }

            transformComponent.Position += CurrentDirection * ForwardVelocity * deltaTime;
            transformComponent.Position += HorizontalDirection * VerticalVelocity * deltaTime;

            sceneCameraTransform.Position = transformComponent.Position;
            sceneCameraTransform.RotationEuler = CurrentRotation * RotationSensitivity;
            transformComponent.RotationEuler = CurrentRotation * RotationSensitivity;
        }

        public double EaseLerp(double a, double b, double t)
        {
            t = Math.Sin(t * (Math.PI / 2));
            return (1.0f - t) * a + t * b;
        }

        public Vector3d EaseLerpVector3(Vector3d a, Vector3d b, float t)
        {
            return new Vector3d(EaseLerp(a.X, b.X, t), EaseLerp(a.Y, b.Y, t), EaseLerp(a.Z, b.Z, t));
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
                                currentInput.Z = eventType == NotifyType.KeyDown ? -1 : 0;
                                break;
                            case KeyCode.A:
                                currentInput.X = eventType == NotifyType.KeyDown ? -1 : 0;
                                break;
                            case KeyCode.S:
                                currentInput.Z = eventType == NotifyType.KeyDown ? 1 : 0;
                                break;
                            case KeyCode.D:
                                currentInput.X = eventType == NotifyType.KeyDown ? 1 : 0;
                                break;
                            case KeyCode.Space:
                                currentInput.Y = eventType == NotifyType.KeyDown ? 1 : 0;
                                break;
                            case KeyCode.Control:
                                currentInput.Y = eventType == NotifyType.KeyDown ? -1 : 0;
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
                case NotifyType.ContextReady:
                    transformComponent = GetComponent<TransformComponent>();
                    break;
            }
        }
    }
}
