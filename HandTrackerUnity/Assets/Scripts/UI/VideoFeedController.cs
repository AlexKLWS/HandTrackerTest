using UnityEngine;
using Scripts.Interop;
using Scripts.Core;
using Scripts.Core.Config;

namespace Scripts.UI
{

    public class VideoFeedController : ComponentLinkedController
    {
        protected override ComponentHolder ComponentHolder { get { return _componentHolder; } }

        private VideoFeedComponentHolder _componentHolder;

        private PixelData[] _framePixels;
        private Texture2D _videoTexture;

        public override void AssignComponentHolder(ComponentHolder componentHolder)
        {
            _componentHolder = componentHolder as VideoFeedComponentHolder;
        }

        public override void Initialize(GameConfig gameConfig)
        {
            _framePixels = new PixelData[gameConfig.CameraHeight * gameConfig.CameraWidth];
            _videoTexture = new Texture2D(gameConfig.CameraWidth, gameConfig.CameraHeight, TextureFormat.RGBA32, false);
            //In unity texture coordinates start at the bottom left conrer, while in opencv start is at the top left.
            //That's why the original image appears flipped. To compensate for this, I flip upside down
            //the quad on which the video is projected
            //Also make the scale to match the video feed aspect ratio
            _componentHolder.VideoTransform.position = new Vector3(0f, 0f, GameConfig.CameraDistance);
            _componentHolder.VideoTransform.localScale = new Vector3((float)gameConfig.CameraWidth / 100f, -(float)gameConfig.CameraHeight / 100f, 1f);
            _componentHolder.VideoMaterial.mainTexture = _videoTexture;
            GameController.OnUpdate += Update;
        }

        private void Update()
        {
            //This isn't the most performant way of going about this, but I just
            //wanted to test out the concept quickly. Of course it's better to 
            //avoid using Texture2D
            unsafe
            {
                fixed (PixelData* pixels = _framePixels)
                {
                    HandTrackerInterop.GetFrame(pixels);
                }
            }

            Color32[] colors = new Color32[_framePixels.Length];

            for (int i = 0; i < _framePixels.Length; i++)
            {
                colors[i] = _framePixels[i].ToColor32();
            }

            _videoTexture.SetPixels32(colors);
            _videoTexture.Apply();
        }

        public override void Release()
        {
            GameController.OnUpdate -= Update;
        }

    }
}