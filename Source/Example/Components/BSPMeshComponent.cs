//using Engine.ECS.Components;
//using Engine.ECS.Entities;
//using Engine.ECS.Observer;
//using Engine.Utils.DebugUtils;
//using Engine.Utils.MathUtils;
//using System;
//using Example.Assets.BSP;
//using Example.Assets.BSP.Lumps;
//using Example.Assets.BSP.Types;
//using Quincy.Components;

//namespace Example.Components
//{
//    public class BSPMeshComponent : Component<BSPMeshComponent>
//    {
//        private readonly BSPLoader bspLoader;
//        private readonly float bspScaleFactor = 0.0254000508f; // ????

//        public BSPMeshComponent(string bspFileName)
//        {
//            bspLoader = new BSPLoader(bspFileName);
//        }

//        private void GenerateBSPMesh()
//        {
//            var modelComponent = new ModelComponent();
//            var vertexLump = (Lump<Vector3f>)bspLoader.Lumps[(int)BspLumpType.LumpVertexes];
//            var planeLump = (Lump<Plane>)bspLoader.Lumps[(int)BspLumpType.LumpPlanes];
//            var edgeLump = (Lump<Edge>)bspLoader.Lumps[(int)BspLumpType.LumpEdges];
//            var surfEdgeLump = (Lump<int>)bspLoader.Lumps[(int)BspLumpType.LumpSurfEdges];
//            var faceLump = (Lump<Face>)bspLoader.Lumps[(int)BspLumpType.LumpFaces];
//            var texInfoLump = (Lump<TexInfo>)bspLoader.Lumps[(int)BspLumpType.LumpTexInfo];

//            int triCount = 0;
//            foreach (var face in faceLump.Contents)
//            {
//                int rootPoint = 0, firstPoint = 0, secondPoint = 0;
//                for (int surfEdgeNum = 0; surfEdgeNum < face.numSurfEdges; surfEdgeNum++)
//                {
//                    var surfEdgeIndex = face.firstSurfEdge + surfEdgeNum;
//                    var edgeIndex = surfEdgeLump.Contents[surfEdgeIndex];

//                    bool reversed = edgeIndex < 0;
//                    Edge edge = edgeLump.Contents[Math.Abs(edgeIndex)];

//                    int vertPoint;
//                    if (surfEdgeNum == 0)
//                    {
//                        rootPoint = edge.vertexIndices[reversed ? 0 : 1];
//                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];
//                    }
//                    else
//                    {
//                        vertPoint = edge.vertexIndices[reversed ? 0 : 1];

//                        if (vertPoint == rootPoint)
//                            continue;
//                        firstPoint = vertPoint;

//                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];

//                        if (vertPoint == rootPoint)
//                            continue;
//                        secondPoint = vertPoint;
//                    }

//                    triCount++;
//                }
//            }
//            Logging.Log($"BSP has {triCount} tris");

//            // setup vertices
//            foreach (var vertex in vertexLump.Contents)
//            {
//                // Multiply by bsp scale factor here, so that we don't have to fuck around with transform scale
//                modelComponent.RenderMesh.vertices.Add(vertex * bspScaleFactor);
//            }

//            // setup uv coords
//            foreach (var texInfo in texInfoLump.Contents)
//            {
//                string logStr = "";
//                for (int x = 0; x < 2; ++x)
//                {
//                    for (int y = 0; y < 4; ++y)
//                    {
//                        logStr += $"{texInfo.textureVecs[x, y]}\t";
//                    }

//                    logStr += "\n\t";
//                }
//            }

//            // setup normals
//            foreach (var plane in planeLump.Contents)
//            {
//                modelComponent.RenderMesh.normals.Add(plane.normal);
//            }

//            foreach (var face in faceLump.Contents)
//            {
//                if (!face.onNode) continue;
//                int rootPoint = 0, firstPoint = 0, secondPoint = 0;
//                for (int surfEdgeNum = 0; surfEdgeNum < face.numSurfEdges; surfEdgeNum++)
//                {
//                    var surfEdgeIndex = face.firstSurfEdge + surfEdgeNum;
//                    var edgeIndex = surfEdgeLump.Contents[surfEdgeIndex];

//                    // big thanks to https://github.com/ajkhoury/OpenBSP-MinGW for this bit - the vbsp docs aren't great

//                    bool reversed = edgeIndex < 0;
//                    Edge edge = edgeLump.Contents[Math.Abs(edgeIndex)];

//                    int vertPoint;
//                    if (surfEdgeNum == 0)
//                    {
//                        rootPoint = edge.vertexIndices[reversed ? 0 : 1];
//                    }
//                    else
//                    {
//                        vertPoint = edge.vertexIndices[reversed ? 0 : 1];

//                        if (vertPoint == rootPoint)
//                            continue;
//                        firstPoint = vertPoint;

//                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];

//                        if (vertPoint == rootPoint)
//                            continue;
//                        secondPoint = vertPoint;
//                    }

//                    var texInfo = texInfoLump.Contents[face.texInfo];
//                    // u = tv(0,0) * x + tv(0,1) * y + tv(0,2) * z + tv(0,3)
//                    // v = tv(1,0) * x + tv(1,1) * y + tv(1,2) * z + tv(1,3)

//                    var rootPointCoords = vertexLump.Contents[rootPoint];
//                    var firstPointCoords = vertexLump.Contents[firstPoint];
//                    var secondPointCoords = vertexLump.Contents[secondPoint];

//                    modelComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, rootPointCoords));
//                    modelComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, firstPointCoords));
//                    modelComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, secondPointCoords));

//                    modelComponent.RenderMesh.faceElements.Add(new Vertex(
//                        (uint)rootPoint,
//                        (uint)modelComponent.RenderMesh.uvCoords.Count - 3,
//                        face.planeNumber
//                    ));
//                    modelComponent.RenderMesh.faceElements.Add(new Vertex(
//                        (uint)firstPoint,
//                        (uint)modelComponent.RenderMesh.uvCoords.Count - 2,
//                        face.planeNumber
//                    ));
//                    modelComponent.RenderMesh.faceElements.Add(new Vertex(
//                        (uint)secondPoint,
//                        (uint)modelComponent.RenderMesh.uvCoords.Count - 1,
//                        face.planeNumber
//                    ));
//                }
//            }

//            modelComponent.RenderMesh.GenerateBuffers();

//            // TODO: Derive from MeshComponent instead
//            ((IEntity)Parent).AddComponent(modelComponent);
//        }

//        private Vector2f GetUVCoords(TexInfo texInfo, Vector3f coords)
//        {
//            var uCoord = texInfo.textureVecs[0, 0] * coords.x + texInfo.textureVecs[0, 1] * coords.y + texInfo.textureVecs[0, 2] * coords.z +
//                             texInfo.textureVecs[0, 3];
//            var vCoord = texInfo.textureVecs[1, 0] * coords.x + texInfo.textureVecs[1, 1] * coords.y + texInfo.textureVecs[1, 2] * coords.z +
//                             texInfo.textureVecs[1, 3];

//            vCoord = 1.0f - vCoord; // Flip for opengl

//            return new Vector2f(uCoord, vCoord) / 1000.0f;
//        }

//        public override void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
//        {
//            base.OnNotify(notifyType, notifyArgs);
//            // TODO: please no
//            switch (notifyType)
//            {
//                case NotifyType.GameStart:
//                    GenerateBSPMesh();
//                    break;
//            }
//        }
//    }
//}
