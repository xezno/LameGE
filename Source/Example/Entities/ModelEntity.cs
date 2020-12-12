using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Common;
using Engine.Common.FileUtils;
using Engine.Common.MathUtils;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class ModelEntity : Entity<ModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public ModelEntity()
        { 
            // Parameterless ctor for scene serialization
        }

        public string ModelAsset
        {
            get
            {
                if (HasComponent<ModelComponent>())
                {
                    return GetComponent<ModelComponent>().Asset.MountPath;
                }

                return null;
            }
            set
            {
                if (HasComponent<ModelComponent>())
                {
                    RemoveComponent<ModelComponent>();
                }

                AddComponent(new ModelComponent(ServiceLocator.FileSystem.GetAsset(value)));
            }
        }

        public Vector3d Position
        {
            get
            {
                if (HasComponent<TransformComponent>())
                {
                    return GetComponent<TransformComponent>().Position;
                }

                return new Vector3d();
            }
            set
            {
                if (HasComponent<TransformComponent>())
                {
                    GetComponent<TransformComponent>().Position = value;
                    return;
                }

                AddComponent(new TransformComponent(value, new Vector3d(0, 0, 0), new Vector3d(1, 1, 1)));
            }
        }

        public Vector3d Scale
        {
            get
            {
                if (HasComponent<TransformComponent>())
                {
                    return GetComponent<TransformComponent>().Scale;
                }

                return new Vector3d();
            }
            set
            {
                if (HasComponent<TransformComponent>())
                {
                    GetComponent<TransformComponent>().Scale = value;
                    return;
                }

                AddComponent(new TransformComponent(new Vector3d(1, 1, 1), new Vector3d(0, 0, 0), value));
            }
        }

        public ModelEntity(Asset modelAsset, Vector3d position, Vector3d scale)
        {
            AddComponent(new TransformComponent(position,
                                                new Vector3d(0, 0, 0),
                                                scale));

            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/pbr.json")));
            AddComponent(new ModelComponent(modelAsset));
        }
    }
}