using HeadRotation.ObjFile;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;

namespace HeadRotation.Render
{
    public class RenderMesh
    {
        public List<MeshPart> Parts = new List<MeshPart>();
        public delegate void BeforePartDrawHandler(MeshPart part);
        public event BeforePartDrawHandler OnBeforePartDraw;
        public RectangleAABB AABB = new RectangleAABB();
        public Matrix4 RotationMatrix = Matrix4.Identity;

        //Угол поворота головы
        public float HeadAngle
        {
            get;
            set;
        }
        public float FaceCenterX
        {
            get;
            set;
        }
        public float NoseDepth
        {
            get;
            set;
        }

        public RenderMesh()
        {
            HeadAngle = 0.0f;
        }

        public void Destroy()
        {
            foreach (var part in Parts)
            {
                part.Destroy();
            }
        }

        public void UpdateAABB(MeshPart part)
        {
            var a = AABB.A;
            var b = AABB.B;
            foreach (var vertex in part.Vertices)
            {
                a.X = Math.Min(vertex.Position.X, a.X);
                b.X = Math.Max(vertex.Position.X, b.X);

                a.Y = Math.Min(vertex.Position.Y, a.Y);
                b.Y = Math.Max(vertex.Position.Y, b.Y);

                a.Z = Math.Min(vertex.Position.Z, a.Z);
                b.Z = Math.Max(vertex.Position.Z, b.Z);
            }
            AABB.A = a;
            AABB.B = b;
        }

        [Obsolete]
        public void DetectFaceRotation(Vector2 noseTip, Vector2 noseTop, Vector2 noseBottom)
        {
            var noseLength = (noseTop.Y - noseTip.Y) * (float)Math.Tan(35.0 * Math.PI / 180.0);
            var angle = (float)Math.Asin(Math.Abs(noseTip.X - noseTop.X) / noseLength);

            HeadAngle = noseTip.X < noseTop.X ? angle : -angle;
            Matrix4.CreateRotationY(HeadAngle, out RotationMatrix);
        }
        public void DetectFaceRotationEmgu()
        {
            var imagePoints = new List<PointF>();
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[66].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[66].Y));        // уши
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[67].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[67].Y));
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[0].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[0].Y));       // глаза центры
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[1].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[1].Y));
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[3].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[3].Y));       // левый-правый угол рта
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[4].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[4].Y));
            imagePoints.Add(new PointF(ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[2].X, ProgramCore.MainForm.PhotoControl.Recognizer.RealPoints[2].Y));       // центр носа

            var modelPoints = new List<MCvPoint3D32f>();
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[66].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[66].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[66].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[67].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[67].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[67].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[0].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[0].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[0].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[1].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[1].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[1].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[3].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[3].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[3].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[4].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[4].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[4].Z));
            modelPoints.Add(new MCvPoint3D32f(ProgramCore.MainForm.RenderControl.HeadPoints.Points[2].X, ProgramCore.MainForm.RenderControl.HeadPoints.Points[2].Y, ProgramCore.MainForm.RenderControl.HeadPoints.Points[2].Z));

            #region CamMatrix

            var img = CvInvoke.Imread(ProgramCore.MainForm.PhotoControl.TemplateImage);
            var max_d = Math.Max(img.Rows, img.Cols);
            var camMatrix = new Emgu.CV.Matrix<double>(3, 3);
            camMatrix[0, 0] = max_d;
            camMatrix[0, 1] = 0;
            camMatrix[0, 2] = img.Cols / 2.0;
            camMatrix[1, 0] = 0;
            camMatrix[1, 1] = max_d;
            camMatrix[1, 2] = img.Rows / 2.0;
            camMatrix[2, 0] = 0;
            camMatrix[2, 1] = 0;
            camMatrix[2, 2] = 1.0;

            #endregion

            var distArray = new double[] { 0, 0, 0, 0 };
            var distMatrix = new Matrix<double>(distArray);      // не используемый коэф.

            var rv = new double[] { 0, 0, 0 };
            var rvec = new Matrix<double>(rv);

            var tv = new double[] { 0, 0, 1 };
            var tvec = new Matrix<double>(tv);

            Emgu.CV.CvInvoke.SolvePnP(modelPoints.ToArray(), imagePoints.ToArray(), camMatrix, distMatrix, rvec, tvec, false, Emgu.CV.CvEnum.SolvePnpMethod.EPnP);      // решаем проблему PNP
            var rotM = new Matrix<double>(3, 3);
            CvInvoke.Rodrigues(rvec, rotM);

            var eulerVector = MatrixToEuler(rotM);

            HeadAngle = (float)(Math.PI) - eulerVector.Y;
            if(HeadAngle > Math.PI)
            {
                HeadAngle -= (float)(Math.PI * 2.0);
            }
            else
            {
                if (HeadAngle < -Math.PI)
                {
                    HeadAngle += (float)(Math.PI * 2.0);
                }
            }
            Matrix4.CreateRotationY(HeadAngle, out RotationMatrix);
        }
        private static Vector3 MatrixToEuler(Matrix<double> m)
        {
            float x, y, z;
            double cy = Math.Sqrt(m[2, 2] * m[2, 2] + m[2, 0] * m[2, 0]);
            if (cy > 16 * float.Epsilon)
            {
                z = (float)Math.Atan2(m[0, 1], m[1, 1]);
                x = (float)Math.Atan2(-m[2, 1], (float)cy);
                y = (float)Math.Atan2(m[2, 0], m[2, 2]);
            }
            else
            {
                z = (float)Math.Atan2(-m[1, 0], m[0, 0]);
                x = (float)Math.Atan2(-m[2, 1], (float)cy);
                y = 0;
            }

            return new Vector3(x, y, z);
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

            if(debug)
            {
                AABB.Draw();
            }
        }


        public void DrawToTexture(int textureId)
        {
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            //GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            foreach (var part in Parts)
            {
                if (part.Texture != textureId)
                    continue;
                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(2, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes));
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + 2 * Vector4.SizeInBytes));
                //GL.TexCoordPointer(1, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(3 * Vector3.SizeInBytes + Vector2.SizeInBytes + 2 * Vector4.SizeInBytes));
                GL.ColorPointer(4, ColorPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes));

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }                 

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            //GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }

        public static RenderMesh LoadFromFile(string filePath)
        {
            var result = new RenderMesh();
            var objData = ObjLoader.LoadObjFile(filePath);

            var lastTriangle = 0;
            var meshPartsInfo = LoadHeadMeshes(objData, 1.0f, ref lastTriangle);

            Vector3 A = new Vector3(99999.0f, 99999.0f, 99999.0f);
            Vector3 B = new Vector3(-99999.0f, -99999.0f, -99999.0f);

            foreach (var meshPartInfo in meshPartsInfo)
            {
                foreach (var p in meshPartInfo.VertexPositions)
                {
                    A.X = Math.Min(A.X, p.X);
                    A.Y = Math.Min(A.Y, p.Y);
                    A.Z = Math.Min(A.Z, p.Z);

                    B.X = Math.Max(B.X, p.X);
                    B.Y = Math.Max(B.Y, p.Y);
                    B.Z = Math.Max(B.Z, p.Z);
                }
            }

            Vector3 Center = (A + B) * 0.5f;


            foreach (var meshPartInfo in meshPartsInfo)
            {
                var meshPart = new MeshPart();
                if (meshPart.Create(meshPartInfo, -Center))
                {
                    result.UpdateAABB(meshPart);
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

        public void CalculateBlendingWeights(List<BlendingInfo> blendingInfo)
        {
            foreach (var part in Parts)
            {
                part.CalculateBlendingWeights(blendingInfo);
            }
        }

        public void SetMorphPercent(float percent)
        {
            foreach (var part in Parts)
            {
                part.SetMorphPercent(percent);
            }
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
