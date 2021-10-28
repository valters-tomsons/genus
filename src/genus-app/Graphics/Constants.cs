namespace genus.app.Graphics
{
	public static class Constants
	{
        public static int VertexSize => Vertices.Length * sizeof(float);

		public static readonly float[] Vertices = {
            -1.0f, 1.0f, 0.0f,
             1.0f, 1.0f, 0.0f,
             -1.0f,  -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
             1.0f, 1.0f, 0.0f,
             -1.0f,  -1.0f, 0.0f
        };
	}
}