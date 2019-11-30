using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Systems;

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

        private float topSpeedOnPlanet = 100f;
        private float topSpeedOffPlanet = 150f;
        private float topSpeedOffSystem = 1500f;

        private TransformComponent transformComponent;

        public ShipEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), ECSEngine.Math.Quaternion.identity, new Vector3(1, 1, 1)));
            //AddComponent(new MeshComponent(""));
            AddComponent(new CameraComponent());

            transformComponent = GetComponent<TransformComponent>();

            ImGuiSystem.instance.AddSerializableObject(this, "Ship");
        }

        public override void Update(float deltaTime)
        {
            transformComponent.position += velocity * deltaTime;
            transformComponent.rotationEuler = currentRotation;
            SceneSystem.instance.mainCamera.position = transformComponent.position;
            velocity += currentDirection * acceleration;
        }

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs_)
        {
            if (eventType == Event.KeyDown || eventType == Event.KeyUp)
            {
                KeyboardEventArgs eventArgs = (KeyboardEventArgs)baseEventArgs_;

                switch ((KeyCode)eventArgs.keyboardKey)
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
                MouseMoveEventArgs eventArgs = (MouseMoveEventArgs)baseEventArgs_;
                // TODO: Replace with mouse delta
                // currentRotation += new Vector3(eventArgs.mousePosition.x, 0, eventArgs.mousePosition.y);
            }
        }
    }
}