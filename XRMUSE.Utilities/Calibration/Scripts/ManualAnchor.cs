using System;
using System.Collections;
using XRMUSE.ExampleScene;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Headset calibration based on controller inputs.
    /// </summary>
    public class ManualAnchor : MonoBehaviour
    {
        public GameObject ground;
        public GameObject originBaseGameObject;
        public GameObject rigCamera;
        public bool isWaveInput = false;
        private Vector3 OriginInCameraSpacePos => rigCamera.transform.InverseTransformPoint(originBaseGameObject.transform.position);
    
        private float anchoringMovingSpeed = 0.2f;
        private float anchorRotatingSpeed = 10;
        private double TOLERANCE = 0.000001f;
        private float TOLERANCE_THUMBSTICK = 0.02f;
        private Vector3 _anchorPosBuffer;
    
        // Wave Input
        private InputAction _viveLPrimaryButton;
        private InputAction _viveLSecondaryButton;
        private InputAction _viveLGripButtonPressed;
        private InputAction _viveRThumbStick;
        private InputAction _viveRPrimaryButton;
        private InputAction _viveRSecondaryButton;

        /// <summary>
        /// Register the input events when enabled.
        /// </summary>
        private void OnEnable()
        {
            _viveLGripButtonPressed.Enable();
            _viveLPrimaryButton.Enable();
            _viveLSecondaryButton.Enable();
            _viveRThumbStick.Enable();
            _viveRPrimaryButton.Enable();
            _viveRSecondaryButton.Enable();
        }
        /// <summary>
        /// Remove the registrations when disabled.
        /// </summary>
        private void OnDisable()
        {
            _viveLGripButtonPressed.Disable();
            _viveLPrimaryButton.Disable();
            _viveLSecondaryButton.Disable();
            _viveRThumbStick.Disable();
            _viveRPrimaryButton.Disable();
            _viveRSecondaryButton.Disable();
        }
        private void Awake()
        {
            if (isWaveInput)
            {
                _viveLPrimaryButton = new("L_PrimaryButton", InputActionType.Value, "<ViveWaveController>{LeftHand}/primaryButton");
                _viveLSecondaryButton = new("L_SecondaryButton", InputActionType.Value, "<ViveWaveController>{LeftHand}/secondaryButton");
                _viveLGripButtonPressed = new("L_GripButton", InputActionType.Value, "<XRController>{LeftHand}/{GripButton}");
                _viveRThumbStick = new("R_ThumbStick", InputActionType.Value, "<ViveWaveController>{RightHand}/thumbstick");
                _viveRPrimaryButton = new("R_PrimaryButton", InputActionType.Value, "<ViveWaveController>{RightHand}/primaryButton");
                _viveRSecondaryButton = new("R_SecondaryButton", InputActionType.Value, "<ViveWaveController>{RightHand}/secondaryButton");
            }
            else
            {
                _viveLPrimaryButton = new("L_PrimaryButton", InputActionType.Value, "<XRController>{LeftHand}/primaryButton");
                _viveLSecondaryButton = new("L_SecondaryButton", InputActionType.Value, "<OculusTouchController>{LeftHand}/secondaryButton");
                _viveLGripButtonPressed = new("L_GripButton", InputActionType.Value, "<XRController>{LeftHand}/{GripButton}");
                _viveRThumbStick = new("R_ThumbStick", InputActionType.Value, "<XRController>{RightHand}/{Primary2DAxis}");
                _viveRPrimaryButton = new("R_PrimaryButton", InputActionType.Value, "<XRController>{RightHand}/{PrimaryButton}");
                _viveRSecondaryButton = new("R_SecondaryButton", InputActionType.Value, "<XRController>{RightHand}/{SecondaryButton}");
            }
        }
        /// <summary>
        /// Start anchoring when at the beginning.
        /// </summary>
        private void Start()
        {
            StartCoroutine(Anchoring());
        }
        /// <summary>
        /// Manual anchoring based on controller input.
        /// </summary>
        IEnumerator Anchoring()
        {
            while (true)
            {
                yield return null;
                print(_viveRThumbStick.ReadValue<Vector2>());
                print(_viveRThumbStick.ReadValue<Vector2>().x);
                print(Math.Abs(_viveRThumbStick.ReadValue<Vector2>().x - 1));
                print(Math.Abs(_viveRThumbStick.ReadValue<Vector2>().x - 1) < TOLERANCE);

                #region rotation_manipulation

                if (Math.Abs(_viveLPrimaryButton.ReadValue<float>() - 1) < TOLERANCE)
                {
                    transform.Rotate(0f, Time.deltaTime * anchorRotatingSpeed, 0f);
                }
                else if (Math.Abs(_viveLSecondaryButton.ReadValue<float>() - 1) < TOLERANCE)
                {
                    transform.Rotate(0f, -Time.deltaTime * anchorRotatingSpeed, 0f);

                }

                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

                #endregion

                #region position_manipulation

                _anchorPosBuffer = transform.localPosition;

                if (Math.Abs(_viveRThumbStick.ReadValue<Vector2>().x - 1) < TOLERANCE_THUMBSTICK)
                {
                    _anchorPosBuffer.x += Time.deltaTime * anchoringMovingSpeed;
                }
                else if (Math.Abs(_viveRThumbStick.ReadValue<Vector2>().x - (-1)) < TOLERANCE_THUMBSTICK)
                {
                    _anchorPosBuffer.x -= Time.deltaTime * anchoringMovingSpeed;
                }
                    

                if (Math.Abs(_viveRThumbStick.ReadValue<Vector2>().y - 1) < TOLERANCE_THUMBSTICK)
                {
                    _anchorPosBuffer.z += Time.deltaTime * anchoringMovingSpeed;
                }
                    
                else if (Math.Abs(_viveRThumbStick.ReadValue<Vector2>().y - (-1)) < TOLERANCE_THUMBSTICK)
                {
                    _anchorPosBuffer.z -= Time.deltaTime * anchoringMovingSpeed;
                }
                    

                if (Math.Abs(_viveRPrimaryButton.ReadValue<float>() - 1) < TOLERANCE)
                    _anchorPosBuffer.y -= Time.deltaTime * anchoringMovingSpeed;
                else if (Math.Abs(_viveRSecondaryButton.ReadValue<float>() - 1) < TOLERANCE)
                    _anchorPosBuffer.y += Time.deltaTime * anchoringMovingSpeed;

                transform.localPosition = _anchorPosBuffer;

                #endregion
                
                #region align

                if (Math.Abs(_viveLGripButtonPressed.ReadValue<float>() - 1) < TOLERANCE)
                {
                    Debug.LogWarning("Left grip button pressed");
                    StartCoroutine(_Align());
                    yield break;
                }
                #endregion
            }
        }
        /// <summary>
        /// Calibrate the entire virtual space to the anchor.
        /// </summary>
        private IEnumerator _Align()
        {
            Vector2 newPos = new Vector2(-transform.position.x, -transform.position.z);
            float radius = Vector2.Distance(newPos, Vector2.zero);
            var absNewPos = new Vector2(math.abs(newPos.x), math.abs(newPos.y));
            var absAngle = Vector2.Angle(Vector2.right, absNewPos);
            float angle = 0f;
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

            Vector2 cameraOffset = new Vector2(rigCamera.transform.position.x, rigCamera.transform.position.z);
            cameraOffset = rotate(cameraOffset, rotation.eulerAngles.y);
            MoveCameraToWorldLocation(new Vector3(
                newPos.x + cameraOffset.x,
                rigCamera.transform.position.y - transform.position.y,
                newPos.y + cameraOffset.y
            ));
            RotateAroundCameraUsingOriginUp(-rotation.eulerAngles.y);
            ground.transform.Translate(0f, -transform.position.y, 0f);
            SessionManager.AnchorHeight = transform.localPosition.y;
            SessionManager.IsAnchoringFinished = true;
            gameObject.SetActive(false);
            yield return null;
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
    }
}
