using BepuPhysics;
using BepuPhysics.Collidables;
using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;

namespace UlaidGame.Entities
{
    public sealed class TestCubeEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        private Material mainMaterial;
        private int physicsIndex;

        public TestCubeEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 0, 0f),
                                                new Vector3(270, 0, 0),
                                                new Vector3(1, 1, 1)));

            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/main.frag", ShaderType.FragmentShader),
                new Shader("Content/Shaders/Standard/main.vert", ShaderType.VertexShader)));

            AddMeshAndMaterialComponents("level01");

            // Add physics
            var box = new Box(1, 1, 1);
            box.ComputeInertia(1.0f, out var inertia);
            var boxIndex = PhysicsManager.Instance.Simulation.Shapes.Add(box);
            physicsIndex = PhysicsManager.Instance.Simulation.Bodies.Add(
                BodyDescription.CreateDynamic(
                    GetComponent<TransformComponent>().Position.ConvertToNumerics(),
                    inertia,
                    new CollidableDescription(boxIndex, 0.1f),
                    new BodyActivityDescription(0.004f)
                )
            );
        }

        public override void Update(float deltaTime)
        {
            GetComponent<TransformComponent>().Position = Vector3.ConvertFromNumerics(PhysicsManager.Instance.Simulation.Bodies.GetBodyReference(physicsIndex).Pose.Position);
            // GetComponent<TransformComponent>().RotationEuler = Vector3.ConvertFromNumerics(PhysicsManager.Instance.Simulation.Bodies.GetBodyReference(physicsIndex).Pose.);
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"Content/Materials/{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"Content/Models/cube.obj"));
        }
    }
}