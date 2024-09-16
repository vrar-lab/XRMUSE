using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.Utilities;

namespace XRMUSE.ExampleScene
{
    [RequireComponent(typeof(TypedCollision))]
    /// <summary>
    /// Additional behaviours of the "materials" in the example scenario, adds the behaviours to the attached TypedCollision at start
    /// </summary>
    public class EventsTypedCollision_ISMaterialCollision : MonoBehaviour
    {
        TypedCollision _tc;
        List<TypedCollision> _collidingISMaterials = new List<TypedCollision>();

        bool _isSent = false;
        
        void Start()
        {
            _tc = GetComponent<TypedCollision>();
            _tc.eventsEnter.AddListener(CheckOnEnter);
            _tc.eventsExit.AddListener(CheckOnExit);
            _tc.eventsClean.AddListener(OnClean);
        }
        
        void CheckOnEnter()
        {
            if (_tc._lastEnterType.collisionType == TypedCollision.TYPED_IS_MATERIAL)
            {
                _collidingISMaterials.Add(_tc._lastEnterType);
                MaterialProximityCheck();
            }
        }

        void CheckOnExit()
        {
            if (_collidingISMaterials.Contains(_tc._lastExitType))
            {
                _collidingISMaterials.Remove(_tc._lastExitType);
                MaterialProximityCheck();
            }
        }

        void OnClean()
        {
            _collidingISMaterials.Clear();
            MaterialProximityCheck();
        }

        void MaterialProximityCheck()
        {
            if (_isSent)
            {
                AnimationManager.instance.UnregisterISMaterialProximity(_tc);
                _isSent = false;
            }

            if (_collidingISMaterials.Count == 1)
            {
                AnimationManager.instance.RegisterISMaterialProximity(_tc, _collidingISMaterials[0]);
                _isSent = true;
            }
        }
    }
}