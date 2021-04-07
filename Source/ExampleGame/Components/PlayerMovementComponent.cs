using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils.MathUtils;
using OpenGL.CoreUI;
using System;
using System.Numerics;

namespace ExampleGame.Components
{
    // TODO: Broken af
    class PlayerMovementComponent : Component<PlayerMovementComponent>
    {
        private TransformComponent transformComponent;
        private bool lockRotation;
        private Vector3 currentInput;

        public Vector3 CurrentInput { get => currentInput; set => currentInput = value; }
        public Vector3 CurrentDirection { get; set; }
        public Vector3 HorizontalDirection { get; set; }

        public float VerticalVelocity { get; set; }
        public float ForwardVelocity { get; set; }
        public Vector3 CurrentRotation { get; set; }
        public float Acceleration { get; set; } = 2f;
        public float Deceleration { get; set; } = 1f;
        public float MaxSpeed { get; set; } = 15.0f;
        public float MouseSensitivityMultiplier { get; set; } = 0.001f;
        public float RotationSensitivity { get; set; } = 3f;
        public float MinVelocity { get; set; } = 0.001f;
        public Vector3 CameraOffset { get; set; }

        public override void Update(float deltaTime)
        {
            var sceneCamera = SceneManager.Instance.MainCamera;
            var sceneCameraTransform = sceneCamera.GetComponent<TransformComponent>();

            var newDirection = (transformComponent.Forward * CurrentInput.Z) + (transformComponent.Right * -CurrentInput.X);
            if (newDirection.Length() != 0)
                newDirection = Vector3.Normalize(newDirection); // this returns NaN instead of (0,0,0) if the vector's length is zero... wtf microsoft

            CurrentDirection = EaseLerpVector3(newDirection, CurrentDirection, 0.7f);

            var newHorizontalDirection = (new Vector3(0, 1, 0) * (float)currentInput.Y);
            if (newHorizontalDirection.Length() != 0)
                newHorizontalDirection = Vector3.Normalize(newHorizontalDirection);

            HorizontalDirection = EaseLerpVector3(newHorizontalDirection, HorizontalDirection, 0.7f);

            ForwardVelocity += (float)Math.Abs(CurrentInput.Length()) * Acceleration;
            ForwardVelocity += -Deceleration;

            VerticalVelocity += (float)newHorizontalDirection.Length() * Acceleration;
            VerticalVelocity += -Deceleration;

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

            sceneCameraTransform.Position = transformComponent.Position + CameraOffset;
            sceneCameraTransform.Rotation = QuaternionExtensions.CreateFromEulerAngles(CurrentRotation * RotationSensitivity);
            transformComponent.Rotation = sceneCameraTransform.Rotation;
        }

        public float EaseLerp(float a, float b, float t)
        {
            t = MathF.Sin(t * (MathF.PI / 2.0f));
            return (1.0f - t) * a + t * b;
        }

        public Vector3 EaseLerpVector3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(EaseLerp(a.X, b.X, t), EaseLerp(a.Y, b.Y, t), EaseLerp(a.Z, b.Z, t));
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
                        CurrentRotation += new Vector3(
                            mouseEventArgs.MouseDelta.Y * -MouseSensitivityMultiplier,
                            mouseEventArgs.MouseDelta.X * -MouseSensitivityMultiplier,
                            0);
                        break;
                    }
                case NotifyType.SceneReady:
                    transformComponent = GetComponent<TransformComponent>();
                    break;
            }
        }
    }
}
