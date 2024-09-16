using System.Collections.Generic;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// A SD_EventLogID is an ID used for both SD Events and Logs.
    /// It uses a main string as well as optional int or a non typed List of objects.
    /// </summary>
    public class SD_EventLogID
    {
        public string ID_name;
        public int ID_count;
        public List<object> ID_other = null;
        public SD_EventLogID() { }
        public SD_EventLogID(string ID_name, int ID_count, List<object> ID_other = null) { this.ID_name = ID_name; this.ID_count = ID_count; this.ID_other = ID_other; }
        public override bool Equals(object obj)
        {
            if (!(obj is SD_EventLogID))
                return false;
            SD_EventLogID o = obj as SD_EventLogID;
            if (!(o.ID_name.Equals(ID_name) && o.ID_count == ID_count))
                return false;
            if ((ID_other == null && o.ID_other != null) || (o.ID_other == null && ID_other != null))
                return false;
            if (ID_other == o.ID_other)
                return true;
            foreach (var o1 in o.ID_other)
            {
                if (!ID_other.Contains(o1))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return ID_name.GetHashCode() * 31 + ID_count * 17;
        }

        public override string ToString()
        {
            string tmp = ID_name + ';' + ID_count;
            if (ID_other != null)
                foreach (var a in ID_other)
                    tmp += a.ToString();
            return tmp;
        }
    }
}