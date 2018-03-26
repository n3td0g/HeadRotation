using System.Collections.Generic;
using OpenTK;

namespace HeadRotation.Render
{
    public class Normal
    {
        private Vector3 normal = Vector3.Zero;
        private float count = 0.0f;

        public Vector3 GetNormal()
        {
            return normal / count;
        }

        public void AddNormal(Vector3 n)
        {
            normal += n;
            count += 1.0f;
        }

        public static List<Vector3> CalculateNormals(List<Vector3> vertexPositions, List<uint> vertexIndices)
        {
            var result = new List<Vector3>();
            var normalsDict = new Dictionary<Vector3, Normal>(new VectorEqualityComparer());

            for (var i = 0; i < vertexIndices.Count / 3; i++)
            {
                var index = i * 3;
                var p0 = vertexPositions[(int)vertexIndices[index]];
                var p1 = vertexPositions[(int)vertexIndices[index + 1]];
                var p2 = vertexPositions[(int)vertexIndices[index + 2]];
                var n = GetNormal(ref p0, ref p1, ref p2);

                for (var j = 0; j < 3; j++)
                {
                    var pointIndex = vertexIndices[index + j];
                    var vp = vertexPositions[(int)pointIndex];
                    if (!normalsDict.ContainsKey(vp))
                        normalsDict.Add(vp, new Normal());
                    normalsDict[vp].AddNormal(n);
                }
            }

            foreach (var position in vertexPositions)
            {
                Normal normal;
                if (normalsDict.TryGetValue(position, out normal))
                {
                    result.Add(normalsDict[position].GetNormal());
                }
                else
                {
                    result.Add(Vector3.One);
                }
            }

            return result;
        }

        public static Vector3 GetNormal(ref Vector3 posA, ref Vector3 posB, ref Vector3 posC)
        {
            var edge1 = Vector3.Subtract(posB, posA);
            var edge2 = Vector3.Subtract(posC, posA);
            var normal = Vector3.Cross(edge1, edge2);
            normal.Normalize();
            return normal;
        }
    }
}
