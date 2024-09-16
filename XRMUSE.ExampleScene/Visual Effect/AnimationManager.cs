using System;
using System.Collections.Generic;
using Photon.Pun;
using XRMUSE.ExampleScene;
using XRMUSE.Networking;
using XRMUSE.Utilities;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Handles the animations of the example scenario, makes uses of the PSValues as pseudo RPCs
    /// </summary>
    public class AnimationManager : MonoBehaviour
    {
        public static AnimationManager instance;
        private bool _isAnimationManagerActive = true;
    
        [Header("Outline Animation Step")]
        public float outlineWidth = 0.02f;
        public float glowingSpeed = 25f;
    
        [Header("Fusing Animation Setup")] 
        public float castDuration = 1f;
        public float fuseDuration = 1f;
    
        [Header("Spawning Animation Setup")]
        public float velocity = 10f;
        public float gravityConstant = -9.8f;
        public GameObject smokingEffectParticleSystem;
    
        [Header("Disappearing Animation Setup")]
        public GameObject disappearingEffectParticleSystem;

        [Header("Illusion Hole")] 
        [HideInInspector] public Material standardMaterial;
        [HideInInspector] public Material stenciledMaterial;

#if UNITY_EDITOR
        [Header("Animation Test")]
        public bool testAnimationFuse = false;
        public ES_Material mat_test1, mat_test2;
        public ES_Tool tool_test;
#endif

        void Awake()
        {
            instance = this;
        }

        bool foundAnimation;


        void Update()
        {
            if (_isAnimationManagerActive)
            {
                foundAnimation = false;
#if UNITY_EDITOR
                foundAnimation = true;
                if (testAnimationFuse)
                {
                    testAnimationFuse = false;
                    FuseIndustrialMaterial(mat_test1, mat_test2, tool_test);
                }
#endif

                foreach (var psv in PlayerSyncedValues.allInstances)
                {
                    DoAnimationsInPSV(psv);
                }

                if (_producedAnimation)
                {
                    if (_lastProducedMC.SpawningAnimationLerp(_producedAnimationTimestampStart))
                    {
                        _producedAnimation = !_producedAnimation;
                    }
                }

            }

        }

        /// <summary>
        /// Pre-allocated cleaning list to avoid declaration and GC calls in used function
        /// </summary>
        List<(int, int, int, long)> toRemove = new List<(int, int, int, long)> ();
    
        /// <summary>
        /// Assumes an available "AnimationFuse" function in the passed PSV and plays its state according to its internal timestamps
        /// </summary>
        /// <param name="psv"></param>
        void DoAnimationsInPSV(PlayerSyncedValues psv)
        {
            PlayerSyncedValues_AnimationFuse anim_fuse = psv.GetFunction("AnimationFuse", 0)(null) as PlayerSyncedValues_AnimationFuse;
            foreach (var items in anim_fuse.animationList)
                DoAnimation(items, psv);
            foreach(var items in anim_fuse.animationListTMP) 
                DoAnimation(items, psv);

            foreach(var items in toRemove)
                anim_fuse.RemoveAnimation(items);
            toRemove.Clear();
        }

        /// <summary>
        /// Cache to quick access the elements necessary of the animation (rigidbody, material controller and transform) from a Photon "viewID"
        /// This is necessary as the "PlayerSyncedValues_AnimationFuse" data only transmits the affected viewIDs to identify the gameObjects to play with
        /// </summary>
        Dictionary<int, (Rigidbody, MaterialController, Transform)> _ViewIDCache = new Dictionary<int, (Rigidbody, MaterialController, Transform)>();

        //Following declarations are pre-allocation for the next function to avoid GC calls
        Rigidbody _rigidbody1, _rigidbody2;
        MaterialController _materialController1, _materialController2;
        Transform _transform1, _transform2;
        GameObject _go;
        void DoAnimation((int,int,int,long) animationItems, PlayerSyncedValues psv)
        {
            foundAnimation = true;
            //Part1 : Gathering all the objects we need to play with
            try {//dirty but efficient way to access the cache and rebuild if not available
                _rigidbody1 = _ViewIDCache[animationItems.Item1].Item1;
                _materialController1 = _ViewIDCache[animationItems.Item1].Item2;
                _transform1 = _ViewIDCache[animationItems.Item1].Item3;
            }
            catch (Exception)
            {
                _go = PhotonNetwork.GetPhotonView(animationItems.Item1).gameObject;
                _ViewIDCache.Add(animationItems.Item1, (_go.GetComponentInChildren<Rigidbody>(), _go.GetComponentInChildren<MaterialController>(), _go.transform));
                _rigidbody1 = _ViewIDCache[animationItems.Item1].Item1;
                _materialController1 = _ViewIDCache[animationItems.Item1].Item2;
                _transform1 = _ViewIDCache[animationItems.Item1].Item3;
            }

            try{//duplicated code for 2nd material in animation, dirty code but sufficient
                _rigidbody2 = _ViewIDCache[animationItems.Item2].Item1;
                _materialController2 = _ViewIDCache[animationItems.Item2].Item2;
                _transform2 = _ViewIDCache[animationItems.Item2].Item3;
            }
            catch (Exception)
            {
                _go = PhotonNetwork.GetPhotonView(animationItems.Item2).gameObject;
                _ViewIDCache.Add(animationItems.Item2, (_go.GetComponentInChildren<Rigidbody>(), _go.GetComponentInChildren<MaterialController>(), _go.transform));
                _rigidbody2 = _ViewIDCache[animationItems.Item2].Item1;
                _materialController2 = _ViewIDCache[animationItems.Item2].Item2;
                _transform2 = _ViewIDCache[animationItems.Item2].Item3;
            }

            //Part2: "animation" through objects values
            try
            {
                bool isDone = _materialController1.FusingAnimationLerp(animationItems.Item4, _materialController2.transform);
                _materialController2.FusingAnimationLerp(animationItems.Item4, _materialController1.transform);

                if (isDone)
                {
                    if (_transform1.GetComponent<PhotonView>().IsMine)
                    {
                        toRemove.Add(animationItems); 
                        TemporaryFusedProcess(0.5f * (_transform1.position + _transform2.position), _transform1.GetComponent<ES_Material>().materialType, _transform2.GetComponent<ES_Material>().materialType, PhotonNetwork.GetPhotonView(animationItems.Item3).gameObject.GetComponent<ES_Tool>().toolType);
                    }
                }
            }
            catch (MissingReferenceException)
            {
                Debug.LogError("Missing material => skip");
            }
        }

        //Pre-allocations fielsds
        GameObject _lastProduced;
        bool _producedAnimation;
        long _producedAnimationTimestampStart;
        private MaterialController _lastProducedMC;
        /// <summary>
        /// Makes the owner of the first material to fuse the items on their machine and synchronize its effect over the networking.
        /// Creates fused material based on the "IS_Recipes" which is a list of "mat1 + mat2 + tool => mat3" recipes
        /// </summary>
        /// <param name="spawnPosition">Position where the 2 material are going to be fused</param>
        /// <param name="mat1">Type of material1 in the recipe</param>
        /// <param name="mat2">Type of material2 in the recipe</param>
        /// <param name="tool1">Type of the used tool in the recipe</param>
        /// <returns></returns>
        bool TemporaryFusedProcess(Vector3 spawnPosition, int mat1, int mat2, int tool1)
        {
            if (!_transform1.GetComponent<PhotonView>().IsMine)
                return false;
            if (!_transform2.GetComponent<PhotonView>().IsMine)
                _transform2.GetComponent<PhotonView>().RequestOwnership();
            GameObject toProduce = ES_Recipes.getRecipe(mat1, mat2, tool1);
            _lastProduced = PoolManager.SpawnPrefab(toProduce, spawnPosition, Quaternion.identity);
            PhotonNetwork.Destroy(_transform1.gameObject);
            PhotonNetwork.Destroy(_transform2.gameObject);
            _producedAnimation = true;
            _producedAnimationTimestampStart = DateTime.UtcNow.Ticks;
            _lastProducedMC = _lastProduced.GetComponentInChildren<MaterialController>();
            return true;
        }


        /// <summary>
        /// Triggered when a tool collides with 2 materials in this example scenario. 
        /// Starts the entire animation fuse sequence over the network through the PSV, triggered by the owner of the tool.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="t1"></param>
        public void FuseIndustrialMaterial(ES_Material m1, ES_Material m2, ES_Tool t1)
        {
            if (!t1.photonView.IsMine)
                return;

            if (!m1.photonView.IsMine)
                m1.photonView.RequestOwnership();
            if (!m2.photonView.IsMine)
                m2.photonView.RequestOwnership();
            m1.material_lock = true;
            m2.material_lock = true;
        
            m1.materialController.GetComponent<GrabInteractable_AdditionalEvents>().ForceDrop();
            m2.materialController.GetComponent<GrabInteractable_AdditionalEvents>().ForceDrop();

            PlayerSyncedValues.localInstance.GetFunction("AnimationFuse", 1)(new object[] { m1.photonView.ViewID, m2.photonView.ViewID, t1.photonView.ViewID, DateTime.UtcNow.Ticks });
        
            ActivateAnimationManager();
        }

        // =====================================================================
        // ===== The code below is specific to the glowing animations ==========
        // =====================================================================

        /// <summary>
        /// Used to see which ProximityCollision to take care of, each TypedCollision registers an "authorization" through EventsTypedCollision_ISMaterialCollision
        /// to add the animations or accept the tool interaction between the 2 materials.
        /// </summary>
        public Dictionary<TypedCollision, TypedCollision> authorizedISMaterialProximityCollision = new Dictionary<TypedCollision, TypedCollision>();

        /// <summary>
        /// Manages the authorizedISMaterialProximityCollision dictionary in the case of tentatively adding a pair
        /// If a pair already exists with one of the TypedCollisions, removes it
        /// </summary>
        /// <param name="tc1"></param>
        /// <param name="tc2"></param>
        public void RegisterISMaterialProximity(TypedCollision tc1, TypedCollision tc2)
        {
            if (authorizedISMaterialProximityCollision.ContainsKey(tc1))
            {
                if (tc2 != authorizedISMaterialProximityCollision[tc1])
                {
                    UnregisterISMaterialProximity(tc1);
                }
                else
                    return;//no changes to dict
            }

            authorizedISMaterialProximityCollision.Add(tc1, tc2);
            try
            {
                if (authorizedISMaterialProximityCollision[tc2]==tc1)
                    startAnimation(tc1 , tc2);

            }catch (Exception) { }
        }

        /// <summary>
        /// Manages the authorizedISMaterialProximityCollision dictionary in the case of tentatively removing a pair
        /// Stops associated animation if needed, removes pairs in a double reference way
        /// </summary>
        /// <param name="tc1"></param>
        public void UnregisterISMaterialProximity(TypedCollision tc1)
        {
            //safety check
            if (!authorizedISMaterialProximityCollision.ContainsKey(tc1))
                return;

            TypedCollision tc2 = authorizedISMaterialProximityCollision[tc1];
            try
            {
                if (authorizedISMaterialProximityCollision[tc2]==tc1)
                    stopAnimation(tc1 , tc2);//an animation must've been playing between the 2 => remove it
            }
            catch (Exception) { }

            authorizedISMaterialProximityCollision.Remove(tc1);
        }

        /// <summary>
        /// Stops the glowing animation of 2 materials that are close to each others and can be fused
        /// </summary>
        /// <param name="tc1"></param>
        /// <param name="tc2"></param>
        public void stopAnimation(TypedCollision tc1, TypedCollision tc2) {
            tc1.GetComponent<ES_Material>().materialController.StopOutlineGlowing();
            tc2.GetComponent<ES_Material>().materialController.StopOutlineGlowing();
        }

        /// <summary>
        /// Stops the glowing animation of 2 materials that are close to each others and can be fused
        /// </summary>
        /// <param name="tc1"></param>
        /// <param name="tc2"></param>
        public void startAnimation(TypedCollision tc1, TypedCollision tc2) {
            /*tc1.gameObject.GetComponentInChildren<MaterialController>().StartOutlineGlowing();
        tc2.gameObject.GetComponentInChildren<MaterialController>().StartOutlineGlowing();*/
            tc1.GetComponent<ES_Material>().materialController.StartOutlineGlowing();
            tc2.GetComponent<ES_Material>().materialController.StartOutlineGlowing();
        }

    
        // =====================================================================
        // ===== End of glowing animation block ================================
        // =====================================================================


        public static void ActivateAnimationManager()
        {
            instance._isAnimationManagerActive = true;
            instance.gameObject.SetActive(true);
        }
    
        public static void DeactivateAnimationManager()
        {
            instance._isAnimationManagerActive = false;
            instance.gameObject.SetActive(false);
        }
    
        public void DismantleAndDisappearAnimation(Vector3 dropPos)
        {
            Instantiate(AnimationManager.instance.disappearingEffectParticleSystem, dropPos, Quaternion.identity).GetComponent<ParticleSystem>().Play();
        }
    
        public void DoAnimationFromNetwork(Vector3 pos)
        {
            Instantiate(AnimationManager.instance.smokingEffectParticleSystem, pos, Quaternion.identity).GetComponent<ParticleSystem>().Play();
        }
    
    }
}
