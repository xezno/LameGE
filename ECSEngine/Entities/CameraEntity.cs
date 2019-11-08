using ECSEngine.Components;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine.Entities
{
    public class CameraEntity : Entity<CameraEntity>
    {
        public Matrix4x4f projMatrix => GetComponent<CameraComponent>().projMatrix;
        public Matrix4x4f viewMatrix => GetComponent<CameraComponent>().viewMatrix;

        public CameraEntity()
        {
            AddComponent(new CameraComponent());
        }

    }
}
