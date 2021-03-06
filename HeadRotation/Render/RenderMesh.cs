﻿using HeadRotation.ObjFile;
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Linq;

namespace HeadRotation.Render
{
    public class RenderMesh
    {
        public List<MeshPart> Parts = new List<MeshPart>();
        public delegate void BeforePartDrawHandler(MeshPart part);
        public event BeforePartDrawHandler OnBeforePartDraw;
        public RectangleAABB AABB = new RectangleAABB();
        public Matrix4 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                Matrix4.Invert(ref rotationMatrix, out invRotationMatrix);
            }
        }

        private Matrix4 rotationMatrix = Matrix4.Identity;
        private Matrix4 invRotationMatrix = Matrix4.Identity;

        //Reversed rotation
        public Matrix4 ReverseRotationMatrix
        {
            get { return reverseRotationMatrix; }
            set
            {
                reverseRotationMatrix = value;
                Matrix4.Invert(ref reverseRotationMatrix, out invReverseRotationMatrix);
            }
        }

        private Matrix4 reverseRotationMatrix = Matrix4.Identity;
        private Matrix4 invReverseRotationMatrix = Matrix4.Identity;

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
            Matrix4.CreateRotationY(HeadAngle, out rotationMatrix);
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
            //float imageWidth = img.Cols;
           // float imageHeight = img.Rows;
            float imageWidth = ProgramCore.MainForm.PhotoControl.Recognizer.ImageWidth;
            float imageHeight = ProgramCore.MainForm.PhotoControl.Recognizer.ImageHeight;
            var max_d = Math.Max(imageWidth, imageHeight);
            var camMatrix = new Emgu.CV.Matrix<double>(3, 3);
            camMatrix[0, 0] = max_d;
            camMatrix[0, 1] = 0;
            camMatrix[0, 2] = imageWidth / 2.0;
            camMatrix[1, 0] = 0;
            camMatrix[1, 1] = max_d;
            camMatrix[1, 2] = imageHeight / 2.0;
            camMatrix[2, 0] = 0;
            camMatrix[2, 1] = 0;
            camMatrix[2, 2] = 1.0;

            /*
            float max_d = Mathf.Max (imageHeight, imageWidth);
            camMatrix = new Mat (3, 3, CvType.CV_64FC1);
            camMatrix.put (0, 0, max_d);
            camMatrix.put (0, 1, 0);
            camMatrix.put (0, 2, imageWidth / 2.0f);
            camMatrix.put (1, 0, 0);
            camMatrix.put (1, 1, max_d);
            camMatrix.put (1, 2, imageHeight / 2.0f);
            camMatrix.put (2, 0, 0);
            camMatrix.put (2, 1, 0);
            camMatrix.put (2, 2, 1.0f);
             */

            #endregion

            var distArray = new double[] { 0, 0, 0, 0 };
            var distMatrix = new Matrix<double>(distArray);      // не используемый коэф.

            var rv = new double[] { 0, 0, 0 };
            var rvec = new Matrix<double>(rv);

            var tv = new double[] { 0, 0, 1 };
            var tvec = new Matrix<double>(tv);

            Emgu.CV.CvInvoke.SolvePnP(modelPoints.ToArray(), imagePoints.ToArray(), camMatrix, distMatrix, rvec, tvec, false, Emgu.CV.CvEnum.SolvePnpMethod.EPnP);      // решаем проблему PNP

            double tvec_z = tvec[2, 0];
            var rotM = new Matrix<double>(3, 3);

            if (!double.IsNaN(tvec_z))
            {                
                CvInvoke.Rodrigues(rvec, rotM);
                Matrix4 transformationM = new Matrix4();
                transformationM.Row0 = new Vector4((float)rotM[0, 0], (float)rotM[0, 1], (float)rotM[0, 2], (float)tvec[0, 0]);
                transformationM.Row1 = new Vector4((float)rotM[1, 0], (float)rotM[1, 1], (float)rotM[1, 2], (float)tvec[1, 0]);
                transformationM.Row2 = new Vector4((float)rotM[2, 0], (float)rotM[2, 1], (float)rotM[2, 2], (float)tvec[2, 0]);
                transformationM.Row3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                var quaternion = ExtractRotationFromMatrix(ref transformationM);                

                //quaternion.Y = -quaternion.Y;

                // RotationMatrix = CreateRotationMatrix(quaternion);
                // RotationMatrix = Matrix4.CreateFromQuaternion(quaternion);
                // RotationMatrix = invertYM * RotationMatrix * invertZM;

                quaternion.X = -quaternion.X;
                quaternion.Y = -quaternion.Y;
                quaternion.Z = -quaternion.Z;

                MeshQuaternion = quaternion;

                var angles = ToEulerRad(MeshQuaternion);
                if(angles.X > -6f && angles.X < 3.0f)
                    angles.X = 0.0f;

                HeadAngle = angles.Y;

                MeshQuaternion = quaternion = ToQ(angles);

                RotationMatrix = CreateRotationMatrix(quaternion);

                quaternion.Z = -MeshQuaternion.Z;
                ReverseRotationMatrix = CreateRotationMatrix(quaternion);
            }
            else
            {
                MeshQuaternion = Quaternion.Identity;
                HeadAngle = 0.0f;
            }

        }

        private static Matrix4 CreateRotationMatrix(Quaternion quaternion)
        {
            var invertYM = Matrix4.CreateScale(1.0f, -1.0f, 1.0f);
            var invertZM = Matrix4.CreateScale(1.0f, 1.0f, -1.0f);

            var result = Matrix4.CreateFromQuaternion(quaternion);
            result = invertYM * result * invertZM;

            return result;
        }

        public Vector3 GetWorldPoint(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, RotationMatrix).Xyz;
        }

        public Vector3 GetPositionFromWorld(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, invRotationMatrix).Xyz;
        }

        public Vector3 GetReverseWorldPoint(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, ReverseRotationMatrix).Xyz;
        }

        public Vector3 GetReversePositionFromWorld(Vector3 point)
        {
            var point4 = new Vector4(point);
            return Vector4.Transform(point4, invReverseRotationMatrix).Xyz;
        }

        public Quaternion MeshQuaternion = Quaternion.Identity;

        private static float Rad2Deg = 180.0f / (float)Math.PI;

        public static Quaternion ToQ(Vector3 v)
        {
            return ToQ(v.Y, v.X, v.Z);
        }

        public static Quaternion ToQ(float yaw, float pitch, float roll)
        {
            const float Deg2Rad = (float)Math.PI / 180.0f;

            yaw *= Deg2Rad;
            pitch *= Deg2Rad;
            roll *= Deg2Rad;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)Math.Cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)Math.Cos((double)yawOver2);
            Quaternion result = new Quaternion();
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }

        private static Vector3 ToEulerRad(Quaternion rotation)
        {
            float sqw = rotation.W * rotation.W;
            float sqx = rotation.X * rotation.X;
            float sqy = rotation.Y * rotation.Y;
            float sqz = rotation.Z * rotation.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = rotation.X * rotation.W - rotation.Y * rotation.Z;
            Vector3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.Y = 2f * (float)Math.Atan2(rotation.Y, rotation.X);
                v.X = (float)Math.PI / 2.0f;
                v.Z = 0;
                return NormalizeAngles(v * Rad2Deg);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.Y = -2f * (float)Math.Atan2(rotation.Y, rotation.X);
                v.X = -(float)Math.PI / 2.0f;
                v.Z = 0;
                return NormalizeAngles(v * Rad2Deg);
            }
            Quaternion q = new Quaternion(rotation.W, rotation.Z, rotation.X, rotation.Y);
            v.Y = (float)System.Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v.X = (float)System.Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch
            v.Z = (float)System.Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));      // Roll
            return NormalizeAngles(v * Rad2Deg);
        }

        private static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.X = NormalizeAngle(angles.X);
            angles.Y = NormalizeAngle(angles.Y);
            angles.Z = NormalizeAngle(angles.Z);
            return angles;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180.0f)
                angle -= 360.0f;
            while (angle < -180.0f)
                angle += 360.0f;
            return angle;
        }

        private Quaternion ExtractRotationFromMatrix(ref Matrix4 matrix)
        {
            Vector3 forward;
            forward.X = matrix[0, 2];
            forward.Y = matrix[1, 2];
            forward.Z = matrix[2, 2];

            Vector3 upwards;
            upwards.X = matrix[0, 1];
            upwards.Y = matrix[1, 1];
            upwards.Z = matrix[2, 1];

            return LookRotation(forward, upwards);
        }

        public static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            forward.Normalize();

            Vector3 vector = Vector3.Normalize(forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.X;
            var m01 = vector2.Y;
            var m02 = vector2.Z;
            var m10 = vector3.X;
            var m11 = vector3.Y;
            var m12 = vector3.Z;
            var m20 = vector.X;
            var m21 = vector.Y;
            var m22 = vector.Z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
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
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            foreach (var part in Parts)
            {
                if (part.Texture != textureId)
                    continue;
                GL.BindBuffer(BufferTarget.ArrayBuffer, part.VertexBuffer);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, part.IndexBuffer);

                GL.VertexPointer(2, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes));
                GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + 2 * Vector4.SizeInBytes));
                GL.TexCoordPointer(1, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(3 * Vector3.SizeInBytes + Vector2.SizeInBytes + 2 * Vector4.SizeInBytes));
                GL.ColorPointer(4, ColorPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes));

                GL.DrawRangeElements(PrimitiveType.Triangles, 0, part.CountIndices, part.CountIndices, DrawElementsType.UnsignedInt, new IntPtr(0));
            }                 

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
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

            var indexer = 0;
            foreach (var modelGroup in objModel.Groups) // one group - one mesh
            {
                if (modelGroup.Key.Name == "Tear" || modelGroup.Key.Name == "Cornea" || modelGroup.Key.Name == "EyeReflection")     // очень плохие материалы. ИЗ-за них ломаются глазки.
                    continue;

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
