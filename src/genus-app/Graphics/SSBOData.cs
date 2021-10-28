using System.Runtime.InteropServices;

namespace genus.app.Graphics
{
	[StructLayout(LayoutKind.Sequential)]
    public struct shader_data
    {
        [MarshalAs(UnmanagedType.ByValArray)]
        public int[] pixels;
    }
}