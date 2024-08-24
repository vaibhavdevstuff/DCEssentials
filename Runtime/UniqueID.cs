using UnityEditor;
using UnityEngine;

namespace DC
{
    public class UniqueID : MonoBehaviour
    {
        // GUID as a unique identifier
        [SerializeField] private string uniqueID;

        public string ID { get { return uniqueID; } }

        public void GenerateUniqueID()
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
        }

        public void ClearUniqueID()
        {
            uniqueID = string.Empty;
        }

        public bool CompareID(string uniqueID)
        {
            if (uniqueID == this.uniqueID)
                return true;

            return false;
        }


    }

    public static class UniqueIDExtension
    {
        /// <summary>
        /// Return Unique GUID of GameObject generated by UniqueID component attached to gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static string GetUniqueID(this GameObject gameObject)
        {
            UniqueID uID = gameObject.GetComponent<UniqueID>();

            if (uID == null) return string.Empty;

            return uID.ID;
        }

        /// <summary>
        /// Compare Unique GUID of GameObject generated by UniqueID component attached to gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public static bool CompareUniqueID(this GameObject gameObject, string otherID)
        {
            UniqueID uID = gameObject.GetComponent<UniqueID>();

            if (uID == null) return false;

            return uID.CompareID(otherID);
        }


    }
}