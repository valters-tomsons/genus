using System.Runtime.InteropServices;

namespace genus.app.Graphics
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct shader_data
    {
        [MarshalAs(UnmanagedType.ByValArray)]
        public float[] pixels;
    }
}