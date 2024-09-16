using System.Collections;
using XRMUSE.ExampleScene;
using Unity.Mathematics;
using UnityEngine;
#if XRMUSE_XRE
using Wave.Essence.TrackableMarker;
using Wave.Native;
#endif

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Headset calibration based on ArUco code.
    /// </summary>
    public class ArucoAnchor : MonoBehaviour
    {
#if XRMUSE_XRE
        private TrackableMarkerController _trackableMarkerController; 
        private bool _isMarkerServiceRunning, _isMarkerObserverRunning;
        private WVR_MarkerObserverTarget currentMarkerObserverTarget =
            WVR_MarkerObserverTarget.WVR_MarkerObserverTarget_Aruco;
        private WVR_MarkerObserverState _currentMarkerObserverState;

        private bool _waitingForDetectionStable = false;
        private Vector3 _pos; 
        private Quaternion _rot;
    
        public GameObject ground;
        private MeshRenderer _successVisual;
        public GameObject originBaseGameObject;
        public GameObject rigCamera;
        public Vector3 OriginInCameraSpacePos => rigCamera.transform.InverseTransformPoint(originBaseGameObject.transform.position);
        private double _tolerance = 0.000001f;
        private Vector3 _anchorPosBuffer;
        private Vector3 _anchorRotBuffer;
        private Vector3 _posAtStart;
        private Quaternion _rotAtStart;
        private Vector2 _xzPos;
    
        private void Awake()
        {
            _trackableMarkerController = GetComponent<TrackableMarkerController>();
            _successVisual = GetComponentInChildren<MeshRenderer>();
        }
        /// <summary>
        /// Starting marker detection service when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            StartMarkerService();
        }
        /// <summary>
        /// Stop marker-related functionalities when disabled.
        /// </summary>
        private void OnDisable()
        {
            if (!_isMarkerServiceRunning) return;
            StopMarkerObserver();
            _trackableMarkerController.StopMarkerService();
            _isMarkerServiceRunning = false;
        }
        /// <summary>
        /// Start marker detection service.
        /// </summary>
        private void StartMarkerService()
        {
            if ((Interop.WVR_GetSupportedFeatures() & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_Marker) != 0)
            {
                WVR_Result result = _trackableMarkerController.StartMarkerService();
                if (result == WVR_Result.WVR_Success)
                {
                    _isMarkerServiceRunning = true;
                    StartMarkerObserver();
                }
            }
        }
        /// <summary>
        /// Initiate a marker observer once the marker service is started
        /// </summary>
        private void StartMarkerObserver()
        {
            if (_isMarkerServiceRunning && !_isMarkerObserverRunning)
            {
                WVR_Result result = _trackableMarkerController.StartMarkerObserver(currentMarkerObserverTarget);
                if (result == WVR_Result.WVR_Success)
                {
                    _isMarkerObserverRunning = true;
                    StartMarkerDetection();
                }
            }
        }
        /// <summary>
        /// Terminate the marker observer.
        /// </summary>
        private void StopMarkerObserver()
        {
            if (!_isMarkerServiceRunning || !_isMarkerObserverRunning) return;
            WVR_Result result = _trackableMarkerController.StopMarkerObserver(currentMarkerObserverTarget);
            if (result == WVR_Result.WVR_Success)
                _isMarkerObserverRunning = false;
        }
        /// <summary>
        /// Start the marker detection once the marker observer is set up.
        /// </summary>
        private void StartMarkerDetection()
        {
            var result = _trackableMarkerController.StartMarkerDetection(currentMarkerObserverTarget);
        }
        /// <summary>
        /// Stop marker detection.
        /// </summary>
        private void StopMarkerDetection()
        {
            var result = _trackableMarkerController.StopMarkerDetection(currentMarkerObserverTarget);
        }
        /// <summary>
        /// Once the marker is detected, place the anchor to the detected point.
        /// </summary>
        private void Update()
        {
            var result = _trackableMarkerController.GetMarkerObserverState(currentMarkerObserverTarget, out _currentMarkerObserverState);
            if (result == WVR_Result.WVR_Success && _currentMarkerObserverState == WVR_MarkerObserverState.WVR_MarkerObserverState_Detecting)
            {
                _trackableMarkerController.GetArucoMarkers(WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround, out var markers);
                foreach (var marker in markers)
                {
                    // In this example scenario, we specifically detect the id 3.
                    if (marker.trackerId == 3)
                    {
                        _trackableMarkerController.ApplyTrackingOriginCorrectionToMarkerPose(marker.pose, out _pos, out _rot);
                        _rot *= Quaternion.Euler(Vector3.right * 90f);
                        _rot *= Quaternion.Euler(Vector3.up * 180f);
                        transform.position = _pos;
                        transform.rotation = Quaternion.Euler(0f, _rot.eulerAngles.y, 0f);
                        transform.localScale = new Vector3(marker.size, marker.size, marker.size);
                        StartCoroutine(_Align());
                        StopMarkerDetection();
                        StopMarkerObserver();
                    }
                }
            }
        }
        /// <summary>
        /// Calibrate the entire virtual space to the anchor.
        /// </summary>
        private IEnumerator _Align()
        {
            for (int i = 0; i < 3; i++)
            {
                var lapse = 0f;
                while (lapse < 1f)
                {
                    if (lapse < 0.5f)
                    {
                        _successVisual.material.color = new Color(0f, 255, 0f, Mathf.Lerp(0f, 0.7f, lapse * 2));
                    }
                    else
                    {
                        _successVisual.material.color = new Color(0f, 255, 0f, Mathf.Lerp(0.7f, 0f, (lapse - 0.5f) * 2));
                    }
                    lapse = lapse + Time.deltaTime;
                    yield return null;
                }
            }
            var newPos = new Vector2(-transform.position.x, -transform.position.z);
            var radius = Vector2.Distance(newPos, Vector2.zero);
            var absNewPos = new Vector2(math.abs(newPos.x), math.abs(newPos.y));
            var absAngle = Vector2.Angle(Vector2.right, absNewPos);
            var angle = 0f;
            if (newPos.x > 0 && newPos.y > 0) angle = absAngle;
            else if (newPos.x < 0 && newPos.y > 0) angle = 180 - absAngle;
            else if (newPos.x < 0 && newPos.y < 0) angle = absAngle + 180f;
            else if (newPos.x > 0 && newPos.y < 0) angle = 360f - absAngle;
            else if (newPos.x == 0 && newPos.y == 0) angle = 0f;
            else if (newPos.x == 0 && newPos.y > 0) angle = 90f;
            else if (newPos.x == 0 && newPos.y < 0) angle = 270f;
            else if (newPos.x > 0 && newPos.y == 0) angle = 0f;
            else if (newPos.x < 0 && newPos.y == 0) angle = 180f;

            var rotation = transform.rotation;
            newPos = new Vector2(
                radius * math.cos((angle + rotation.eulerAngles.y) / 180f * math.PI),
                radius * math.sin((angle + rotation.eulerAngles.y) / 180f * math.PI)
            );

            var cameraOffset = new Vector2(rigCamera.transform.position.x, rigCamera.transform.position.z);
            cameraOffset = rotate(cameraOffset, rotation.eulerAngles.y);
            MoveCameraToWorldLocation(new Vector3(
                newPos.x + cameraOffset.x,
                rigCamera.transform.position.y - transform.position.y,
                newPos.y + cameraOffset.y
            ));
            RotateAroundCameraUsingOriginUp(-rotation.eulerAngles.y);
            ground.transform.Translate(0f, -transform.position.y, 0f);
            _successVisual.gameObject.SetActive(false);
            SessionManager.AnchorHeight = transform.localPosition.y;
            SessionManager.IsAnchoringFinished = true;
            gameObject.SetActive(false);
    }
    // ********************************************************************
    // The following utils come from Unity.XR.CoreUtils
    // ********************************************************************
        #region Utils
    private Vector2 rotate(Vector2 v, float delta)
        {
            var mDelta = (delta) / 180f * math.PI;
            return new Vector2(
                v.x * Mathf.Cos(mDelta) - v.y * Mathf.Sin(mDelta),
                v.x * Mathf.Sin(mDelta) + v.y * Mathf.Cos(mDelta)
            );
        }

        /// <summary>
        /// Rotates the XR origin object around the camera object by the provided <paramref name="angleDegrees"/>.
        /// This rotation only occurs around the origin's Up vector
        /// </summary>
        /// <param name="angleDegrees">The amount of rotation in degrees.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed. Otherwise, returns <see langword="false"/>.</returns>
        public void RotateAroundCameraUsingOriginUp(float angleDegrees)
        {
            RotateAroundCameraPosition(originBaseGameObject.transform.up, angleDegrees);
        }

        /// <summary>
        /// Rotates the XR origin object around the camera object's position in world space using the provided <paramref name="vector"/>
        /// as the rotation axis. The XR Origin object is rotated by the amount of degrees provided in <paramref name="angleDegrees"/>.
        /// </summary>
        /// <param name="vector">The axis of the rotation.</param>
        /// <param name="angleDegrees">The amount of rotation in degrees.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed. Otherwise, returns <see langword="false"/>.</returns>
        private void RotateAroundCameraPosition(Vector3 vector, float angleDegrees)
        {
            originBaseGameObject.transform.RotateAround(rigCamera.transform.position, vector, angleDegrees);
        }

        /// <summary>
        /// This function moves the camera to the world location provided by <paramref name="desiredWorldLocation"/>.
        /// It does this by moving the XR Origin object so that the camera's world location matches the desiredWorldLocation
        /// </summary>
        /// <param name="desiredWorldLocation">the position in world space that the camera should be moved to</param>
        /// <returns>Returns <see langword="true"/> if the move is performed. Otherwise, returns <see langword="false"/>.</returns>
        private void MoveCameraToWorldLocation(Vector3 desiredWorldLocation)
        {
            var rot = Matrix4x4.Rotate(rigCamera.transform.rotation);
            var delta = rot.MultiplyPoint3x4(OriginInCameraSpacePos);
            originBaseGameObject.transform.position = delta + desiredWorldLocation;
        }

        #endregion
#endif
    }
}
