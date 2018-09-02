using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeadRotation.Helpers
{
    public static class VectorEx
    {
        public static void ExportVector(List<Vector3> data, bool isSmile)
        {
            var vectorPath = Path.Combine(Application.StartupPath, "Models", isSmile ? "ChildWithSmile" : "ChildWithoutSmile", "headRotater_vectors.txt");
            using (var writer = new StreamWriter(vectorPath, false, Encoding.Default))
            {
                foreach (var vector in data)
                {
                    writer.WriteLine(VectorEx.ToString(vector));
                }
            }
        }
        public static List<Vector3> ImportVector(bool isSmile)
        {
            var vectorPath = Path.Combine(Application.StartupPath, "Models", isSmile ? "ChildWithSmile" : "ChildWithoutSmile", "headRotater_vectors.txt");
            var result = new List<Vector3>();
            using (var reader = new StreamReader(vectorPath))
            {
                while (!reader.EndOfStream)
                {
                    var str = reader.ReadLine();
                    var vector = FromString(str);
                    result.Add(vector);
                }
            }
            return result;
        }

        public static string ToString(Vector3 vector)
        {
            return vector.X + "/" + vector.Y + "/" + vector.Z;
        }

        public static Vector3 FromString(string str)
        {
            var strs = str.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 3)
                return Vector3.Zero;
            return new Vector3(StringConverter.ToFloat(strs[0]), StringConverter.ToFloat(strs[1]), StringConverter.ToFloat(strs[2]));
        }
    }
}
