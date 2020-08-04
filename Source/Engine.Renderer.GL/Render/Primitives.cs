using Engine.Utils.MathUtils;

namespace Engine.Renderer.GL.Render
{
    public class Primitives
    {
        private static Primitives instance;
        private Mesh plane;

        public static Primitives Instance => instance ??= new Primitives();

        public static Mesh Plane => Instance.plane;

        public Primitives()
        {
            SetupPlane();
        }

        private void SetupPlane()
        {
            float[] vertices = new[]
            {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 0.0f
            };

            float[] uvCoords = new[]
            {
                1.0f, 0.0f,
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 1.0f
            };

            float[] normals = new[]
            {
                0.0f, 0.0f, 1.0f
            };

            MeshFaceElement[] meshFaceElements = new[]
            {
                new MeshFaceElement(2, 1, 1),
                new MeshFaceElement(3, 2, 1),
                new MeshFaceElement(1, 3, 1),
                new MeshFaceElement(2, 1, 1),
                new MeshFaceElement(4, 4, 1),
                new MeshFaceElement(3, 2, 1)
            };

            plane = new Mesh();
            SetupMesh(ref plane, vertices, uvCoords, normals, meshFaceElements);
        }

        private void SetupMesh(ref Mesh mesh, float[] vertices, float[] uvCoords, float[] normals,
            MeshFaceElement[] meshFaceElements)
        {
            for (int i = 0; i < vertices.Length; i += 3)
            {
                mesh.vertices.Add(new Vector3f(vertices[i], vertices[i + 1], vertices[i + 2]));
            }
            for (int i = 0; i < uvCoords.Length; i += 2)
            {
                mesh.uvCoords.Add(new Vector2f(uvCoords[i], uvCoords[i + 1]));
            }
            for (int i = 0; i < normals.Length; i += 3)
            {
                mesh.normals.Add(new Vector3f(normals[i], normals[i + 1], normals[i + 2]));
            }

            foreach (var meshFaceElement in meshFaceElements)
            {
                // Fix - allows for obj files to be directly "baked" here instead
                // of having to manually modify each index.
                var meshFaceElementModified = meshFaceElement;
                meshFaceElementModified.normalIndex--;
                meshFaceElementModified.uvIndex--;
                meshFaceElementModified.vertexIndex--;
                mesh.faceElements.Add(meshFaceElementModified);
            }

            mesh.GenerateBuffers();
        }
    }
}
