using UnityEngine;

namespace Scripts.Interop
{
    public static class PixelDataHelpers
    {
        public static Color32 ToColor32(this PixelData pixel)
        {
            return new Color32(pixel.r, pixel.g, pixel.b, 255);
        }
    }
}