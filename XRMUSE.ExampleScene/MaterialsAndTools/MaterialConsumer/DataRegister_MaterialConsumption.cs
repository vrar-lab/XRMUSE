using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.Networking;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// DataRegister of the example scenario, registers how many Materials of each type have been put in the "material crates" by the local player on their PSV
    /// Aggregating these across all players let us know how many materials of a type have been produced on the full environment
    /// </summary>
    public class DataRegister_MaterialConsumption : MonoBehaviour, IPlayerSyncedValues_DataRegister
    {
        public string DataName => "MaterialConsume";

        Dictionary<int, int> materialDeleteCount = new Dictionary<int, int>();
        Dictionary<int, int> materialDeleteCountTMP = new Dictionary<int, int>();
        bool _materialDeleteCountNewValues = false;

        public void Read(PhotonStream stream, PhotonMessageInfo info)
        {
            int materialCountType;
            while ((materialCountType = (int)stream.ReceiveNext()) != -1)
            {
                if (materialCountType == -2)
                    materialDeleteCount.Clear();
                else
                {
                    if (!materialDeleteCount.ContainsKey(materialCountType))
                        materialDeleteCount.Add(materialCountType, 0);
                    materialDeleteCount[materialCountType] = (int)stream.ReceiveNext();
                }
            }
        }

        public void ResetValues()
        {
            materialDeleteCount.Clear();
            materialDeleteCountTMP.Clear();
        }

        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info)
        {
            stream.SendNext(-2); //=>clear dict
            foreach (int type in materialDeleteCount.Keys)
            {
                stream.SendNext(type);
                stream.SendNext(materialDeleteCount[type]);
            }

            stream.SendNext(-1);
            _materialDeleteCountNewValues = false;
        }

        public void TemporaryWrite(PhotonStream stream, PhotonMessageInfo info)
        {
            foreach (int type in materialDeleteCountTMP.Keys)
            {
                if (!materialDeleteCount.ContainsKey(type))
                    materialDeleteCount.Add(type, 0);
                if (materialDeleteCount[type] != materialDeleteCountTMP[type])
                {
                    materialDeleteCount[type] = materialDeleteCountTMP[type];
                    stream.SendNext(type);
                    stream.SendNext(materialDeleteCount[type]);
                }
            }

            stream.SendNext((int)-1);
            _materialDeleteCountNewValues = false;
        }

        public bool TemporaryWriteCheck() => _materialDeleteCountNewValues;

        /// <summary>
        /// Custom accessible function, takes an integer as a parameter that represents the type of the consumed material to increment the count of
        /// </summary>
        [DataRegister_CustomFunction(0)]
        public object[] AddMaterialCount(object[] args)
        {
            int type = (int)args[0];
            if (!materialDeleteCountTMP.ContainsKey(type))
                materialDeleteCountTMP.Add(type, 0);
            materialDeleteCountTMP[type]++;
            _materialDeleteCountNewValues = true;
            return null;
        }

        /// <summary>
        /// Custom accessible function, takes an integer as a parameter that represents the type of the consumed material to read the value of
        /// </summary>
        [DataRegister_CustomFunction(1)]
        public object GetMaterialCount(object[] args)
        {
            int type = (int)args[0];
            int count = 0;
            if (materialDeleteCountTMP.ContainsKey(type))
                count = materialDeleteCountTMP[type];
            else if (materialDeleteCount.ContainsKey(type))
                count = materialDeleteCount[type];
            return count;
        }

        /// <summary>
        /// Static function called to register a material consumption on the local PSV
        /// </summary>
        public static void AddCount(int type)
        {
            PlayerSyncedValues.localInstance.GetFunction("MaterialConsume", 0)(new object[] { type });
        }
    }
}