﻿using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;
using HeadRotation.Morphing;

namespace HeadRotation.Render
{    
    public enum PartType
    {
        Neck,
        Default,
    }

    public class MeshPart
    {
        public List<MorphingPoint> MorphPoints = new List<MorphingPoint>();
        public List<uint> Indices = new List<uint>();
        public List<int> PointIndices = new List<int>();
        public Vertex3d[] Vertices = null;

        public int Texture = 0;
        public int TransparentTexture = 0;
        public Vector4 Color = Vector4.One;

        public PartType Type = PartType.Default;

        public int IndexBuffer, VertexBuffer = 0;
        public int CountIndices
        {
            get;
            set;
        }

        public void UpdateBuffers()
        {
            UpdateVertexBuffer();
            UpdateIndexBuffer();
        }

        public void Destroy()
        {
            if (VertexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref VertexBuffer);
                VertexBuffer = 0;
            }

            if (IndexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref IndexBuffer);
                IndexBuffer = 0;
            }
        }

        private void UpdateVertexBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex3d.Stride), Vertices, BufferUsageHint.StreamDraw);
            OpenGlHelper.CheckErrors();
        }

        public void UpdateIndexBuffer()
        {
            UpdateIndexBuffer(Indices);
        }

        public void UpdateIndexBuffer(List<uint> indices)
        {
            CountIndices = indices.Count;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(CountIndices * sizeof(uint)), indices.ToArray(), BufferUsageHint.DynamicDraw);
            OpenGlHelper.CheckErrors();
        }

        public bool Create(MeshPartInfo info, Vector3 Offset)
        {
            if (info.VertexPositions.Count == 0)
                return false;
            
            Color = info.Color;
            Texture = info.Texture;
            TransparentTexture = info.TransparentTexture;

            Indices.Clear();
            var positions = new List<Vector3>();
            var texCoords = new List<Vector2>();

            var pointnsDict = new Dictionary<VertexInfo, int>(new VectorEqualityComparer());
            var positionsDict = new Dictionary<Vector3, int>(new VectorEqualityComparer());

            for (var i = 0; i < info.VertexPositions.Count; i++)
            {
                var vertexInfo = new VertexInfo
                {
                    Position = info.VertexPositions[i] + Offset,
                    TexCoords = info.TextureCoords[i]
                };
                if (!pointnsDict.ContainsKey(vertexInfo))
                {
                    var index = positions.Count;                    

                    if (!positionsDict.ContainsKey(vertexInfo.Position))
                    {
                        positionsDict.Add(vertexInfo.Position, MorphPoints.Count);
                        PointIndices.Add(MorphPoints.Count);
                        MorphPoints.Add(new MorphingPoint
                        {
                            Indices = new List<int> { index },
                            Position = vertexInfo.Position
                        });
                    }
                    else
                    {
                        var id = positionsDict[vertexInfo.Position];
                        PointIndices.Add(id);
                        MorphPoints[id].Indices.Add(index);
                    }

                    pointnsDict.Add(vertexInfo, index);
                    Indices.Add((uint)index);
                    positions.Add(vertexInfo.Position);
                    texCoords.Add(vertexInfo.TexCoords);
                }
                else
                {
                    PointIndices.Add(positionsDict[vertexInfo.Position]);
                    Indices.Add((uint)pointnsDict[vertexInfo]);
                }                   
            }

            CountIndices = Indices.Count;
            Vertices = new Vertex3d[positions.Count];

            var normals = Normal.CalculateNormals(positions, Indices);
            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].OriginalPosition = Vertices[i].Position = positions[i];
                Vertices[i].Normal = normals[i];
                Vertices[i].TexCoord = texCoords[i];
                Vertices[i].Color = Vector4.One;
            }
            
            Destroy();
            GL.GenBuffers(1, out VertexBuffer);
            GL.GenBuffers(1, out IndexBuffer);

            return true;
        }

        public void SetMorphPercent(float percent)
        {
            foreach(var point in MorphPoints)
            {
                foreach (var index in point.Indices)
                {
                    var vertex = Vertices[index];
                    vertex.Position = point.Position * percent + vertex.OriginalPosition * (1.0f - percent);
                    Vertices[index] = vertex;
                }
            }
            UpdateVertexBuffer();
        }

        public void CalculateBlendingWeights(List<BlendingInfo> blendingInfo)
        {
            for(int i = 0; i < Vertices.Length; ++i)
            {
                var v = Vertices[i];

                var k = 0.0f;
                foreach (var b in blendingInfo)
                {
                    var length = (b.Position - v.OriginalPosition.Xy).Length;
                    if (length < b.Radius)
                        k = 1.0f;
                    else if (length < (b.Radius + b.HalfRadius))
                        k = Math.Max(k, (1.0f - ((length - b.Radius) / b.HalfRadius)));
                }

                v.BlendWeight = k;

                Vertices[i] = v;
            }
            UpdateVertexBuffer();      
        }
    }
}
