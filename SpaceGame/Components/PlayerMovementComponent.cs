using System;
using ECSEngine;
using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using OpenGL.CoreUI;

namespace SpaceGame.Components
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
        private TransformComponent transformComponent;
        private Vector2 lastMousePos;

        public override void Update(float deltaTime)
        {
            transformComponent.position += velocity * deltaTime;
            SceneManager.instance.mainCamera.rotationEuler = currentRotation;
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
                currentRotation += new Vector3(0, 
                    (lastMousePos.x - mouseEventArgs.mousePosition.x) * mouseSensitivityMultiplier, 
                    (lastMousePos.y - (mouseEventArgs.mousePosition.y - RenderSettings.Default.GameResolutionY / 2)) * -mouseSensitivityMultiplier);
                lastMousePos = mouseEventArgs.mousePosition;
            }
            else if (eventType == Event.GameStart)
            {
                transformComponent = GetComponent<TransformComponent>();
            }
        }
    }
}
