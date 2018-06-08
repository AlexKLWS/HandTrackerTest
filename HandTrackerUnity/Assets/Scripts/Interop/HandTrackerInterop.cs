using System.Runtime.InteropServices;

namespace Scripts.Interop
{

    internal static class HandTrackerInterop
    {
        [DllImport("SimpleHandTracker")]
        internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

        [DllImport("SimpleHandTracker")]
        internal static extern int Close();

        [DllImport("SimpleHandTracker")]
        internal static extern void SetupForGettingColorSample();

        [DllImport("SimpleHandTracker")]
        internal static extern void StartColorSampling();

        [DllImport("SimpleHandTracker")]
        internal static extern void StartTracking();

        [DllImport("SimpleHandTracker")]
        internal unsafe static extern void GetFrame(PixelData* pixels);
    }
}