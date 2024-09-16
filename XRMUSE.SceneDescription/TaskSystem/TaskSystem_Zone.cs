using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Represents a scene-designer defined "Zone".
    /// A TaskSystem_Zone is a location within the environment where events may happen depending on the zone's type
    /// </summary>
    public class TaskSystem_Zone : MonoBehaviour
    {
        /// <summary>
        /// Enum representing what type of zone we are facing with
        /// </summary>
        public enum Type { PRODUCTION_ZONE = 0, IDENTIFICATION_ZONE, PURE_LOCATION };
        /// <summary>
        /// color representation in-editor of the zone depending on its type, Colors are in order of the enum
        /// </summary>
        public static Color[] colors = { Color.blue, Color.green, Color.yellow };
        public Type m_type;
        /// <summary>
        /// Name of the zone, used as an identifier by other scripts
        /// </summary>
        public string zone_id = "???";
        /// <summary>
        /// Always accessible list of all the zones in the current Scene
        /// </summary>
        public static List<TaskSystem_Zone> allZones = new List<TaskSystem_Zone>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Zone of the current scene that have the corresponding id</returns>
        public static TaskSystem_Zone GetZoneById(string id)
        {
            foreach (var zone in allZones)
                if (id.ToLower().Equals(zone.zone_id.ToLower()))
                    return zone;

            //We don't have the id=> might be load order issue
            foreach (var zone in GameObject.FindObjectsOfType<TaskSystem_Zone>())
            {
                if(!allZones.Contains(zone))
                    allZones.Add(zone);

                if (id.ToLower().Equals(zone.zone_id.ToLower()))
                    return zone;
            }
            return null;
        }
        public void Awake()
        {
            if(!allZones.Contains(this))
                allZones.Add(this);
        }
        /// <summary>
        /// Can be anything in any form (gameObject, whatever), considers the accesser knows what it is.
        /// Usually the type of currentLinked will depend on the type of the zone (for instance, PRODUCTION_ZONE will either be linked to a null object or a GameObject that have in its hierarchy a "Plant_Base")
        /// </summary>
        public object currentLinked = null;
        /// <summary>
        /// local xxyyzz representation of the zone
        /// </summary>
        public float[] xxyyzz = { 0, 1, 0, 1, 0, 1 };
        /// <summary>
        /// world position of the ground in this zone (y axis)
        /// </summary>
        public float y_ground = 0;
        public float X(int i)
        {
            return xxyyzz[i];
        }
        public float Y(int i)
        {
            return xxyyzz[i + posY];
        }
        public float Z(int i)
        {
            return xxyyzz[i + posZ];
        }
        public const int posX = 0, posY = 2, posZ = 4;

        public float X_World(int i)
        {
            return X(i) * transform.lossyScale.x - (transform.lossyScale.x / 2f) + transform.position.x;
        }
        public float Y_World(int i)
        {
            return Y(i) * transform.lossyScale.y - (transform.lossyScale.y / 2f) + transform.position.y;
        }
        public float Z_World(int i)
        {
            return Z(i) * transform.lossyScale.z - (transform.lossyScale.z / 2f) + transform.position.z;
        }
        public Vector3 Center_World()
        {
            return new Vector3(0.5f * X_World(0) + 0.5f * X_World(1), 0.5f * Y_World(0) + 0.5f * Y_World(1), 0.5f * Z_World(0) + 0.5f * Z_World(1));
        }
        public Vector3 Size()
        {
            return new Vector3((X(1) - X(0)) * transform.lossyScale.x, (Y(1) - Y(0)) * transform.lossyScale.y, (Z(1) - Z(0)) * transform.lossyScale.z);
        }

        public bool CollisionAABB(Vector3 centerWorldCollider, float distance)
        {
            return (((centerWorldCollider.x - distance < Mathf.Max(X_World(0), X_World(1))) && (centerWorldCollider.x - distance > Mathf.Min(X_World(0), X_World(1)))) ||
                ((centerWorldCollider.x + distance < Mathf.Max(X_World(0), X_World(1))) && (centerWorldCollider.x + distance > Mathf.Min(X_World(0), X_World(1)))) ||
                ((X_World(0) < centerWorldCollider.x + distance) && (X_World(0) > centerWorldCollider.x - distance)) ||
                ((X_World(1) < centerWorldCollider.x + distance) && (X_World(1) > centerWorldCollider.x - distance))) &&
                (((centerWorldCollider.y - distance < Mathf.Max(Y_World(0), Y_World(1))) && (centerWorldCollider.y - distance > Mathf.Min(Y_World(0), Y_World(1)))) ||
                ((centerWorldCollider.y + distance < Mathf.Max(Y_World(0), Y_World(1))) && (centerWorldCollider.y + distance > Mathf.Min(Y_World(0), Y_World(1)))) ||
                ((Y_World(0) < centerWorldCollider.y + distance) && (Y_World(0) > centerWorldCollider.y - distance)) ||
                ((Y_World(1) < centerWorldCollider.y + distance) && (Y_World(1) > centerWorldCollider.y - distance))) &&
                (((centerWorldCollider.z - distance < Mathf.Max(Z_World(0), Z_World(1))) && (centerWorldCollider.z - distance > Mathf.Min(Z_World(0), Z_World(1)))) ||
                ((centerWorldCollider.z + distance < Mathf.Max(Z_World(0), Z_World(1))) && (centerWorldCollider.z + distance > Mathf.Min(Z_World(0), Z_World(1)))) ||
                ((Z_World(0) < centerWorldCollider.z + distance) && (Z_World(0) > centerWorldCollider.z - distance)) ||
                ((Z_World(1) < centerWorldCollider.z + distance) && (Z_World(1) > centerWorldCollider.z - distance)));// dirty, but proper AABBCC collision check, could be less dirty if we consider sizes to be ordered
        }

#if UNITY_EDITOR
        GUIStyle gui = new GUIStyle();
        public float sphere_size = 0.1f;
        public int font_size = 30;
        private void OnDrawGizmos()
        {
            //draw boundaries
            Vector3 toSubstract = new Vector3(transform.lossyScale.x / 2f, transform.lossyScale.y / 2f, transform.lossyScale.z / 2f);
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                    for (int z = 0; z < 2; z++)
                    {
                        Gizmos.color = colors[(int)m_type];
                        Gizmos.DrawSphere((new Vector3(X(x) * transform.lossyScale.x, Y(y) * transform.lossyScale.y, Z(z) * transform.lossyScale.z)) - toSubstract + transform.position, sphere_size);
                    }
            //draw display locations
            foreach (var pos in displayLocations)
            {
                Gizmos.color = colors[(int)m_type];
                Gizmos.DrawCube(Vector3_World(pos), new Vector3(sphere_size, sphere_size, sphere_size));
            }

            gui.fontSize = (int)(font_size / HandleUtility.GetHandleSize(Center_World()));
            gui.alignment = TextAnchor.MiddleCenter;
            gui.normal.textColor = Gizmos.color;
            Handles.Label(Center_World(), zone_id, gui);

            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
            Gizmos.DrawCube(Center_World(), Size());

        }
#endif

        public float CoordinateX_World(float x)
        {
            return x * transform.lossyScale.x + Center_World().x;
        }
        public float CoordinateY_World(float y)
        {
            return y * transform.lossyScale.y + Center_World().y;
        }
        public float CoordinateZ_World(float z)
        {
            return z * transform.lossyScale.z + Center_World().z;
        }

        public Vector3 Vector3_World(Vector3 v)
        {
            return new Vector3(CoordinateX_World(v.x), CoordinateY_World(v.y), CoordinateZ_World(v.z));
        }
        /// <summary>
        /// Local display locations of the zone that can be used by other elements such as the proximity UI
        /// </summary>
        public Vector3[] displayLocations = { new Vector3(0.2f, 0f, -0.1f), new Vector3(-0.2f, 0f, -0.1f) };

        /// <summary>
        /// Retrieves the world position of the desired displayLocation
        /// </summary>
        /// <param name="i">the index of the displayLocation we need</param>
        /// <param name="worldLocation">out to pass the worldLocation of the desired display location</param>
        /// <returns>true if the display location is available and its worldLocation is in "wordLocation", false otherwise</returns>
        public bool getDisplayLocationWorld(int i, out Vector3 worldLocation)
        {
            if (displayLocations == null || displayLocations.Length <= i)
            {
                worldLocation = new Vector3();
                return false;
            }
            worldLocation = Vector3_World(displayLocations[i]);
            return true;

        }
    }
}