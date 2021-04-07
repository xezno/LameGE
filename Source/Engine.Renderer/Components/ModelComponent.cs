﻿using Assimp;
using Engine.Assets;
using Engine.ECS.Components;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using ImGuiNET;
using OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Engine.Renderer.Components
{
    [Requires(typeof(ShaderComponent))]
    [Requires(typeof(TransformComponent))]
    public class ModelComponent : Component<ModelComponent>
    {
        private string directory;
        private Asset asset;

        public List<Mesh> Meshes { get; set; } = new List<Mesh>();

        public ModelComponent() { }

        public ModelComponent(Asset asset)
        {
            this.asset = asset;
            LoadModel(asset);
        }

        public void Draw(Mesh.DrawInfo drawInfo)
        {
            var matrix = GetComponent<TransformComponent>().Matrix;
            foreach (var mesh in Meshes)
            {
                mesh.Draw(drawInfo);
            }
        }

        public void DrawShadows(LightComponent light, ShaderComponent shader)
        {
            var matrix = GetComponent<TransformComponent>().Matrix;
            foreach (var mesh in Meshes)
            {
                mesh.DrawShadows(light, shader, matrix);
            }
        }

        public override void Update(float deltaTime)
        {
            foreach (var mesh in Meshes)
            {
                // mesh.Update(deltaTime);
            }
        }

        private void LoadModel(Asset asset)
        {
            // TODO: Remove assimp
            var context = new AssimpContext();

            var logStream = new LogStream((msg, userData) =>
            {
                Logging.Log($"{msg}");
            });
            logStream.Attach();

            var extension = Path.GetExtension(asset.MountPath).Substring(1);
            using var memoryStream = new MemoryStream(asset.Data);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var scene = context.ImportFile("Content/" + asset.MountPath,
                PostProcessSteps.Triangulate
                | PostProcessSteps.PreTransformVertices
                | PostProcessSteps.RemoveRedundantMaterials
                | PostProcessSteps.CalculateTangentSpace
                | PostProcessSteps.OptimizeMeshes
                | PostProcessSteps.OptimizeGraph
                | PostProcessSteps.ValidateDataStructure
                | PostProcessSteps.GenerateNormals
                | PostProcessSteps.FlipUVs);

            directory = Path.GetDirectoryName(asset.MountPath);

            ProcessNode(scene.RootNode, scene);
        }

        private void ProcessNode(Node node, Assimp.Scene scene)
        {
            for (int i = 0; i < node.MeshCount; ++i)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                Meshes.Add(ProcessMesh(mesh, scene, node.Transform));
            }

            foreach (var child in node.Children)
            {
                ProcessNode(child, scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene, Assimp.Matrix4x4 transform)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; ++i)
            {
                var vertex = new Vertex()
                {
                    Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z),
                    Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z)
                };

                if (mesh.HasTextureCoords(0))
                {
                    var texCoords = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
                    vertex.TexCoords = texCoords;
                }
                else
                {
                    vertex.TexCoords = new Vector2(0, 0);
                }

                if (mesh.HasTangentBasis)
                {
                    vertex.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                    vertex.BiTangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);
                }

                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh.FaceCount; ++i)
            {
                var face = mesh.Faces[i];
                for (int f = 0; f < face.IndexCount; ++f)
                {
                    indices.Add((uint)face.Indices[f]);
                }
            }

            if (mesh.MaterialIndex >= 0)
            {
                var material = scene.Materials[mesh.MaterialIndex];
                var diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                textures.AddRange(diffuseMaps);

                if (diffuseMaps.Count == 0)
                {
                    textures.Add(Texture.LoadFromData(new[]
                    {
                        (byte)(material.ColorDiffuse.R * 255),
                        (byte)(material.ColorDiffuse.G * 255),
                        (byte)(material.ColorDiffuse.B * 255),
                        (byte)(material.ColorDiffuse.A * 255)
                    }, 1, 1, 4, "texture_diffuse"));
                }

                var specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
                textures.AddRange(specularMaps);

                if (specularMaps.Count == 0)
                {
                    textures.Add(Texture.LoadFromData(new[]
                    {
                        (byte)(material.ColorSpecular.R * 255),
                        (byte)(material.ColorSpecular.G * 255),
                        (byte)(material.ColorSpecular.B * 255),
                        (byte)(material.ColorSpecular.A * 255)
                    }, 1, 1, 4, "texture_specular"));
                }

                var normalMaps = LoadMaterialTextures(material, TextureType.Normals, "texture_normal");
                textures.AddRange(normalMaps);

                var emissiveMaps = LoadMaterialTextures(material, TextureType.Emissive, "texture_emissive");
                textures.AddRange(emissiveMaps);

                if (specularMaps.Count == 0)
                {
                    textures.Add(Texture.LoadFromData(new[]
                    {
                        (byte)(material.ColorEmissive.R * 255),
                        (byte)(material.ColorEmissive.G * 255),
                        (byte)(material.ColorEmissive.B * 255),
                        (byte)(material.ColorEmissive.A * 255)
                    }, 1, 1, 4, "texture_emissive"));
                }

                var unknownMaps = LoadMaterialTextures(material, TextureType.Unknown, "texture_unknown"); // includes roughness
                textures.AddRange(unknownMaps);
            }

            var oglTransform = new System.Numerics.Matrix4x4(
                transform.A1, transform.A2, transform.A3, transform.A4,
                transform.B1, transform.B2, transform.B3, transform.B4,
                transform.C1, transform.C2, transform.C3, transform.C4,
                transform.D1, transform.D2, transform.D3, transform.D4
            );

            return new Mesh(vertices, indices, textures, oglTransform);
        }

        private List<Texture> LoadMaterialTextures(Material material, TextureType textureType, string typeName)
        {
            var textures = new List<Texture>();

            for (int i = 0; i < material.GetMaterialTextureCount(textureType); ++i)
            {
                material.GetMaterialTexture(textureType, i, out var textureSlot);

                if (string.IsNullOrEmpty(textureSlot.FilePath))
                {
                    continue;
                }

                Logging.Log($"Loading {directory}/{textureSlot.FilePath} as {typeName}");

                var texture = Texture.LoadFromAsset(ServiceLocator.FileSystem.GetAsset($"{directory}/{textureSlot.FilePath}"), typeName);
                textures.Add(texture);
            }

            return textures;
        }

        public override void RenderImGui()
        {
            base.RenderImGui();

            ImGui.Columns(2);

            var assetName = asset?.MountPath ?? "No Asset";
            ImGui.Text("File");
            ImGui.NextColumn();
            ImGui.InputText("##hidelabel", ref assetName, (uint)assetName.Length, ImGuiInputTextFlags.ReadOnly);
            ImGui.SameLine();
            ImGui.Button(FontAwesome5.EllipsisH);

            ImGui.Columns(1);
        }
    }
}
