using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Adds a custom collision system to a gameobject. A TypedCollision have a type of collider which can be used to dictates custom events.
    /// When using TypedCollision on a gameobject, you may add a custom behaviour which will add functions to eventsEnter and eventsExit.
    /// </summary>
    public class TypedCollision : MonoBehaviour
    {
        public const int TYPED_IS_UNDEFINED = -1, TYPED_IS_MATERIAL = 0, TYPED_IS_TOOL = 1, TYPED_IS_MATCONSUME = 2;
        /// <summary>
        /// Identifier of the "type" of collider this is. For instance a type of material, a tool, ...
        /// </summary>
        public int collisionType = TYPED_IS_UNDEFINED;
        /// <summary>
        /// Custom behaviour when another TypedCollision enters the collider
        /// </summary>
        public UnityEvent eventsEnter = new UnityEvent();
        /// <summary>
        /// Custom behaviour when another TypedCollision exits the collider
        /// </summary>
        public UnityEvent eventsExit = new UnityEvent();
        /// <summary>
        /// Custom behaviour when cleaning the TypedCollision
        /// </summary>
        public UnityEvent eventsClean = new UnityEvent();

        public List<TypedCollision> currentlyColliding = new List<TypedCollision>();

        /*[HideInInspector]
        public Collision _lastEnterCollision = null;
        [HideInInspector]
        public Collision _lastExitCollision = null;*/
        [HideInInspector]
        public TypedCollision _lastEnterType = null;
        [HideInInspector]
        public TypedCollision _lastExitType = null;
        private void OnCollisionEnter(Collision collision)
        {
            OnEnter(collision.gameObject);
            /*TypedCollision tc = collision.gameObject.GetComponent<TypedCollision>();
            if (tc == null)
                return;
            //_lastEnterCollision = collision;
            _lastEnterType = tc;
            currentlyColliding.Add(tc);
            eventsEnter.Invoke();*/
        }
        private void OnTriggerEnter(Collider other)
        {
            OnEnter(other.gameObject);
        }

        void OnEnter(GameObject other)
        {
            TypedCollision tc = other.GetComponent<TypedCollision>();
            if (tc == null)
                return;
            if ((!isActiveAndEnabled) || !tc.isActiveAndEnabled)
                return;
            if (currentlyColliding.Contains(tc))
                return;
            //_lastEnterCollision = collision;
            _lastEnterType = tc;
            currentlyColliding.Add(tc);
            eventsEnter.Invoke();
        }

        void OnExit(GameObject other)
        {
            TypedCollision tc = other.GetComponent<TypedCollision>();
            if (tc == null)
                return;
            if (!currentlyColliding.Contains(tc))
                return;
            _lastExitType = tc;
            if (currentlyColliding.Remove(tc))
                eventsExit.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            OnExit(collision.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit(other.gameObject);
        }

        private void OnDisable()
        {
            Clean();
        }

        /// <summary>
        /// Clears all the internal variables related to ongoing collisions. 
        /// In most cases should not be used in custom behaviours affecting a typed collision.
        /// </summary>
        public void Clean()
        {
            _lastEnterType = null;
            _lastExitType = null;
            foreach (TypedCollision tc in currentlyColliding)
                tc.currentlyColliding.Remove(this);
            currentlyColliding.Clear();

            eventsClean.Invoke();
        }
    }
}