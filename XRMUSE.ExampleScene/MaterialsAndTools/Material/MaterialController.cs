using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using XRMUSE.Networking;

namespace XRMUSE.ExampleScene
{
    public class MaterialController : MonoBehaviour
    {
        private IEnumerator _spinning; // Coroutine for spinning animation
        private const float dPI = 2 * math.PI; // Constant value for 2 * PI

        [Header("Outline Drawing")]
        [SerializeField] private MeshRenderer _renderer;  // Renderer used to draw the outline
        private bool _shouldDrawOutline = false; // Flag to control whether the outline should be drawn
        private Material _outlineMaterial; // Material used for drawing the outline

        [Header("Fusing Animation")]
        private float _totalDuration; // Total duration of the fusing animation
        private Vector3 _originalRotation; // Original rotation of the object
        private float _radius, _theta, _phi; // Internal variables for calculating random offsets during the animation
        private Vector3 _originalPosition; // Original position of the object
        private Vector3 _midPos; // Internal variable for storing the midpoint position between the object and another object
        private Vector3 mOffset; // Internal variable for storing the offset applied to the object's position during animation
        private long _lastAnimationStart = -1; // Timestamp of the last animation start
    
        [Header("Spawning Animation")]
        private float _originalY; // Original Y position of the object
        private float _midTick;  // Midpoint tick duration for the spawning animation
        private Quaternion _spawningOriginalRotation;  // Original rotation of the object during spawning
        private Quaternion _spawningTargetRotation; // Target rotation of the object during spawning

        private void Awake()
        {
            _originalPosition = transform.position;
            FindOutlineShaderMaterial();
        }

        private void Start()
        {
            _totalDuration = AnimationManager.instance.fuseDuration + AnimationManager.instance.castDuration;
        }
    
        /// <summary>
        /// Handles the spawning animation by interpolating the position and rotation of the object based on a given timestamp.
        /// </summary>
        /// <param name="mAnimationTimestamp">The timestamp of the animation to be played.</param>
        /// <returns>
        /// True if the animation has completed; false if the animation is still in progress or if the animation cannot start due to an early timestamp.
        /// </returns>
        public bool SpawningAnimationLerp(long mAnimationTimestamp)
        {
            if (mAnimationTimestamp < _lastAnimationStart)
            {
                Debug.LogError("The spawning animation is trying to play on this object too early!");
                return false;
            }

            if (mAnimationTimestamp != _lastAnimationStart)
            {
                _lastAnimationStart = mAnimationTimestamp;
                _originalY = transform.position.y;
                DataRegister_AnimationAtPosition.AddAnimation(transform.position, "SpawnAnimation");
                _midTick = math.abs(AnimationManager.instance.velocity / AnimationManager.instance.gravityConstant);
                _spawningOriginalRotation = transform.rotation;
                _spawningTargetRotation = Quaternion.Euler(
                    _spawningOriginalRotation.x + UnityEngine.Random.Range(-20f, 20f),
                    _spawningOriginalRotation.y + UnityEngine.Random.Range(-60f, 60f),
                    _spawningOriginalRotation.z + UnityEngine.Random.Range(-20f, 20f)
                );
                return false;
            }
            transform.GetComponent<Rigidbody>().isKinematic = true;
            float mLapse = CalculateDuration(_lastAnimationStart, DateTime.UtcNow.Ticks);
        
            if (mLapse <= _midTick)
            {
                transform.parent.rotation = Quaternion.Lerp(_spawningOriginalRotation, _spawningTargetRotation, math.clamp(mLapse / _midTick, 0f, 1f));
            }
            else
            {
                transform.parent.rotation = Quaternion.Lerp(_spawningTargetRotation, _spawningOriginalRotation, math.clamp(mLapse / _midTick - 1, 0f, 1f));
            }

            var nextPos = _originalY + AnimationManager.instance.velocity * mLapse + 0.5f * AnimationManager.instance.gravityConstant * mLapse * mLapse;
            if (nextPos < _originalY || mAnimationTimestamp - _lastAnimationStart>10f)
            {
                var body = transform.GetComponent<Rigidbody>();
                body.isKinematic = false;
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                transform.parent.position = new Vector3(transform.position.x, _originalY, transform.position.z);
                return true;
            }
            transform.parent.position = new Vector3(transform.position.x, nextPos, transform.position.z);
            return false;
        }

        /// <summary>
        /// Handles the fusing animation by interpolating the position and rotation of the object relative to another object based on a given timestamp.
        /// </summary>
        /// <param name="mAnimationTimestamp">The timestamp of the animation to be played.</param>
        /// <param name="mOtherObject">The other object involved in the fusing animation.</param>
        /// <returns>
        /// True if the animation has completed; false if the animation is still in progress or if the animation cannot start due to an early timestamp.
        /// </returns>
        public bool FusingAnimationLerp(long mAnimationTimestamp, Transform mOtherObject)
        {
            if (mAnimationTimestamp < _lastAnimationStart)
            {
                Debug.LogError("An animation less recent is trying to play on this object!");
                return false;
            }
        
            if (mAnimationTimestamp != _lastAnimationStart)
            {
                _lastAnimationStart = mAnimationTimestamp;
                _originalRotation = transform.parent.rotation.eulerAngles;
                _originalPosition = transform.position;
                _midPos = 0.5f*(this.transform.position + mOtherObject.position);
            }
        
            float mLapse = CalculateDuration(_lastAnimationStart, DateTime.UtcNow.Ticks);

            if (mLapse <= AnimationManager.instance.castDuration)
            {
                _radius = math.lerp(0.002f, 0.003f, mLapse / AnimationManager.instance.castDuration);
                _theta = UnityEngine.Random.Range(0f, dPI);
                _phi = UnityEngine.Random.Range(0f, dPI);
                mOffset = new Vector3(_radius * math.sin(_theta) * math.sin(_phi), _radius * math.sin(_theta) * math.cos(_phi),
                    _radius * math.cos(_theta));
                transform.parent.position = _originalPosition + mOffset;
                transform.parent.rotation = Quaternion.Euler(new Vector3(
                    _originalRotation.x + UnityEngine.Random.Range(-5f, 5f),
                    _originalRotation.y + UnityEngine.Random.Range(-5f, 5f),
                    _originalRotation.z + UnityEngine.Random.Range(-5f, 5f))
                );
            }
            else if (mLapse <= _totalDuration)
            {
                transform.parent.position = Vector3.Lerp(_originalPosition, _midPos, (mLapse - AnimationManager.instance.castDuration) / AnimationManager.instance.fuseDuration);
            }
            else if(mLapse > _totalDuration)
            {
                return true;
            }
            return false;
        }
    
        /// <summary>
        /// Enables the outline glow effect by setting the _shouldDrawOutline flag to true.
        /// </summary>
        public void StartOutlineGlowing()
        {
            _shouldDrawOutline = true;
        }

        /// <summary>
        /// Disables the outline glow effect by setting the _shouldDrawOutline flag to false.
        /// </summary>
        public void StopOutlineGlowing()
        {
            _shouldDrawOutline = false;
        }

    
        float mProgress = 0f; // Interval variable
        /// <summary>
        /// Updates the outline glowing animation each frame.
        /// </summary>
        private void Update()
        {
            _outlineMaterial.SetFloat("_OutlineThickness", AnimationManager.instance.outlineWidth * math.sin(mProgress * math.PI / 2f));
            if (_shouldDrawOutline)
            {
                mProgress = math.clamp(mProgress + AnimationManager.instance.glowingSpeed / 1000f, 0f, 1f);
            }
            else
            {
                mProgress = math.clamp(mProgress - AnimationManager.instance.glowingSpeed / 1000f, 0f, 1f);
            }

            if (mProgress > 0)
            {
                _outlineMaterial.SetFloat("_Alpha", 1f);
            }
            else
            {
                _outlineMaterial.SetFloat("_Alpha", 0f);
            }
        }

        /// <summary>
        /// Finds and sets the material for drawing the outline.
        /// </summary>
        private void FindOutlineShaderMaterial()
        {
            foreach (var mat in _renderer.materials)
            {
                if (mat.name.Contains("Outline"))
                {
                    _outlineMaterial = mat;
                }
            }
            if (_outlineMaterial == null)
            {
                throw new Exception("Outline Shader material not found.");
            }
        }

        /// <summary>
        /// A helper method for calculating the time duration between two timestamps.
        /// </summary>
        /// <param name="t1">The start timestamp.</param>
        /// <param name="t2">The stop timestamp.</param>
        /// <returns>Time duration in seconds.</returns>
        float CalculateDuration(long t1, long t2)
        {
            return (t2 - t1) / 10000000f;
        }
    
    }
}