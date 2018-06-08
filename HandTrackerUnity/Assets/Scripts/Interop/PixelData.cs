using System.Runtime.InteropServices;

namespace Scripts.Interop
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct PixelData
    {
        public byte r, g, b;
    }
}