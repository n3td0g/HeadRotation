using OpenTK;

namespace HeadRotation.Render
{
    public class BlendingInfo
    {
        private float radius = 0.0f;

        public float HalfRadius
        {
            get;
            private set;
        }

        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                HalfRadius = value;
                radius = value;
            }
        }

        public Vector2 Position;
    }
}
