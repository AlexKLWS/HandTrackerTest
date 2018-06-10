using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Core;
using Scripts.Interop;
using Scripts.Core.Config;

namespace Scripts.Objects
{

    public class MovableObjectController : ComponentLinkedController
    {

        protected override ComponentHolder ComponentHolder { get { return _componentHolder; } }

        private MovableObjectComponentHolder _componentHolder;
        private Vector2 _videoFeedCenterCoordinates;

        public override void AssignComponentHolder(ComponentHolder componentHolder)
        {
            _componentHolder = componentHolder as MovableObjectComponentHolder;
        }

        public override void Initialize(GameConfig gameConfig)
        {
            //Camera-relative postion could be used as well
            _componentHolder.MovableObjectTransform.position = new Vector3(0f, 0f, GameConfig.CameraDistance);
             //Coordinates of center of a video feed frame in pixels
            _videoFeedCenterCoordinates = new Vector2(gameConfig.CameraWidth * 0.5f, gameConfig.CameraHeight * 0.5f);
            _componentHolder.MovableObjectTransform.gameObject.SetActive(false);
        }

        public void StartTracking()
        {
            GameController.OnUpdate += Update;
            _componentHolder.MovableObjectTransform.gameObject.SetActive(true);
        }

        private void Update()
        {
            float x = 0f, y = 0f;
            HandTrackerInterop.GetHandCoordinates(ref x, ref y);
            Vector2 relativeCoordinates = new Vector2(x, y) - _videoFeedCenterCoordinates;
            //Some filtering or coordinate adjusting, as well as check for borderline 
            //cases could be done here.
            //The idea is to move the object relative to the plane on which
            //the video feed is projected
            _componentHolder.MovableObjectTransform.position = new Vector3(relativeCoordinates.x / 100f, -relativeCoordinates.y / 100f, GameConfig.CameraDistance);
        }

        public override void Release()
        {
            GameController.OnUpdate -= Update;
        }
    }
}