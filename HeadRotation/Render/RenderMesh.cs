﻿using HeadRotation.ObjFile;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HeadRotation.Render
{
    public class RenderMesh
    {
        public List<MeshPart> Parts = new List<MeshPart>();
        public delegate void BeforePartDrawHandler(MeshPart part);
        public event BeforePartDrawHandler OnBeforePartDraw;

        public void Destroy()
        {
            foreach (var part in Parts)
            {
                part.Destroy();
            }
        }

        public void Draw(bool debug)
        {
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            foreach (var part in Parts)
            {
                OnBeforePartDraw?.Invoke(part);

                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(3, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(0));
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(Vector3.SizeInBytes));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes));
                GL.ColorPointer(4, ColorPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes));

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
        }

        public static RenderMesh LoadFromFile(string filePath)
        {
            var result = new RenderMesh();
            var objData = ObjLoader.LoadObjFile(filePath);

            var lastTriangle = 0;
            var meshPartsInfo = LoadHeadMeshes(objData, 1.0f, ref lastTriangle);
            
            foreach (var meshPartInfo in meshPartsInfo)
            {
                var meshPart = new MeshPart();
                if (meshPart.Create(meshPartInfo))
                {
                    result.Parts.Add(meshPart);
                }                
            }

            foreach (var part in result.Parts)
            {
                part.UpdateBuffers();
            }

            return result;
        }

        private static List<MeshPartInfo> LoadHeadMeshes(ObjItem objModel, float scale, ref int lastTriangle)
        {
            var result = new List<MeshPartInfo>();
            var vertexPositions = new List<float>();
            var vertexNormals = new List<float>();
            var vertexTextureCoordinates = new List<float>();
            var vertexBoneIndices = new List<float>();
            var vertexBoneWeights = new List<float>();
            var indeces = new List<uint>();

            foreach (var modelGroup in objModel.Groups) // one group - one mesh
            {
                vertexPositions.Clear();
                vertexNormals.Clear();
                vertexTextureCoordinates.Clear();
                vertexBoneIndices.Clear();
                vertexBoneWeights.Clear();
                indeces.Clear();

                foreach (var face in modelGroup.Value.Faces)          //  combine all meshes in group - to one mesh.
                    GetObjFace(face, objModel, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces, ref lastTriangle);

                var positions = new List<Vector3>();
                var texCoords = new List<Vector2>();
                var index = 0;
                for (var i = 0; i < vertexPositions.Count / 3; ++i)
                {
                    index = i * 3;
                    positions.Add(new Vector3(vertexPositions[index], vertexPositions[index + 1], vertexPositions[index + 2]));
                    texCoords.Add(new Vector2(vertexTextureCoordinates[i * 2], 1.0f - vertexTextureCoordinates[i * 2 + 1]));
                }

                var meshPartInfo = new MeshPartInfo
                {
                    VertexPositions = GetScaledVertices(positions, scale),
                    MaterialName = modelGroup.Key.Name,
                    TextureCoords = texCoords,
                    PartName = modelGroup.Key.Name == "default" ? string.Empty : modelGroup.Key.Name,
                    Color =
                                           new Vector4(modelGroup.Key.DiffuseColor.X, modelGroup.Key.DiffuseColor.Y,
                                               modelGroup.Key.DiffuseColor.Z, modelGroup.Key.Transparency),
                    Texture = modelGroup.Key.Texture,
                    TransparentTexture = modelGroup.Key.TransparentTexture,
                    TextureName = modelGroup.Key.DiffuseTextureMap,
                    TransparentTextureName = modelGroup.Key.TransparentTextureMap
                };

                result.Add(meshPartInfo);
            }

            return result;
        }

        private static List<Vector3> GetScaledVertices(List<Vector3> vlist, float scale)
        {
            var result = new List<Vector3>();
            foreach (var v in vlist)
            {
                result.Add(v * scale);
            }
            return result;
        }

        private static void GetObjFace(ObjFace face, ObjItem objModel,
                                            ref List<float> vertexPositions, ref List<float> vertexNormals, ref List<float> vertexTextureCoordinates, ref List<float> vertexBoneWeights, ref List<float> vertexBoneIndices, ref List<uint> indeces, ref int lastTriangle)
        {
            if (face.Count == 3)
            {
                for (var i = 0; i < face.Count; i++)
                {
                    var faceVertex = face[i];
                    ObjLoader.AppendObjTriangle(objModel, faceVertex, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                }
                SetFaceTriangleIndex(face, objModel, ref lastTriangle);
            }
            else if (face.Count == 4)
            {
                var faceVertex0 = face[0];
                var faceVertex1 = face[1];
                var faceVertex2 = face[2];
                var faceVertex3 = face[3];

                ObjLoader.AppendObjTriangle(objModel, faceVertex0, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                ObjLoader.AppendObjTriangle(objModel, faceVertex1, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                ObjLoader.AppendObjTriangle(objModel, faceVertex2, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);

                ObjLoader.AppendObjTriangle(objModel, faceVertex2, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                ObjLoader.AppendObjTriangle(objModel, faceVertex3, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                ObjLoader.AppendObjTriangle(objModel, faceVertex0, ref vertexPositions, ref vertexNormals, ref vertexTextureCoordinates, ref vertexBoneWeights, ref vertexBoneIndices, ref indeces);
                SetFaceTriangleIndex(face, objModel, ref lastTriangle);
            }
        }

        private static void SetFaceTriangleIndex(ObjFace face, ObjItem objModel, ref int lastTriangleIndex)
        {
            if (objModel.ObjExport != null && face.ObjExportIndex > -1)
            {
                objModel.ObjExport.Faces[face.ObjExportIndex].TriangleIndex0 = lastTriangleIndex++;
                if (face.Count == 4)
                    objModel.ObjExport.Faces[face.ObjExportIndex].TriangleIndex1 = lastTriangleIndex++;
            }
        }
    }
}