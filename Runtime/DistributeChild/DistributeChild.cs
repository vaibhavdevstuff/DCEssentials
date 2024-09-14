using UnityEngine;

namespace DC
{

    [ExecuteInEditMode]
    public class DistributeChild : MonoBehaviour
    {
        public float xSpacing = 0f; // Space between the objects along the X axis
        public float ySpacing = 0f; // Space between the objects along the Y axis
        public float zSpacing = 0f; // Space between the objects along the Z axis

        [Space]
        public bool reCalculatePositionOnStart = false;

        private void Start()
        {
            if (reCalculatePositionOnStart)
                Distribute();
        }

        void OnValidate()
        {
            Distribute();
        }

        void Distribute()
        {
            int childCount = transform.childCount;
            if (childCount == 0) return;

            // Calculate total length for centering the children on each axis
            float totalXLength = (childCount - 1) * xSpacing;
            float startXOffset = -totalXLength / 2.0f;

            float totalYLength = (childCount - 1) * ySpacing;
            float startYOffset = -totalYLength / 2.0f;

            float totalZLength = (childCount - 1) * zSpacing;
            float startZOffset = -totalZLength / 2.0f;

            // Distribute children relative to the parent
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                Vector3 newPosition = child.localPosition;

                newPosition.x = startXOffset + i * xSpacing;
                newPosition.y = startYOffset + i * ySpacing;
                newPosition.z = startZOffset + i * zSpacing;

                child.localPosition = newPosition;
            }
        }
    }
}
