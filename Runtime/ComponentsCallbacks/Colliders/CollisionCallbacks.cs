using System;
using UnityEngine;
using UnityEngine.Events;

namespace DC
{
    public class CollisionCallbacks : MonoBehaviour
    {
        // Action events for code-based listeners
        public static event Action<Collision> OnCollisionEntered;
        public static event Action<Collision> OnCollisionStayed;
        public static event Action<Collision> OnCollisionExited;

        // UnityEvent fields for inspector-based event handling
        [Tooltip("Called when OnCollisionEnter is invoked.")]
        public UnityEvent<Collision> OnCollisionEnterEvent;

        [Tooltip("Called when OnCollisionStay is invoked.")]
        public UnityEvent<Collision> OnCollisionStayEvent;

        [Tooltip("Called when OnCollisionExit is invoked.")]
        public UnityEvent<Collision> OnCollisionExitEvent;

        // Filtering settings
        [Header("Filtering Settings")]
        [Tooltip("Only triggers for objects in these layers.")]
        [SerializeField] private LayerMask collisionLayerMask = ~0;

        [Tooltip("Objects with these tags will be ignored.")]
        [SerializeField] private string[] ignoreTags;

        private void OnCollisionEnter(Collision collision)
        {
            if (ShouldProcessCollision(collision.gameObject))
            {
                OnCollisionEntered?.Invoke(collision);  // Code-based listeners
                OnCollisionEnterEvent?.Invoke(collision);  // Inspector-based listeners
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (ShouldProcessCollision(collision.gameObject))
            {
                OnCollisionStayed?.Invoke(collision);  // Code-based listeners
                OnCollisionStayEvent?.Invoke(collision);  // Inspector-based listeners
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (ShouldProcessCollision(collision.gameObject))
            {
                OnCollisionExited?.Invoke(collision);  // Code-based listeners
                OnCollisionExitEvent?.Invoke(collision);  // Inspector-based listeners
            }
        }

        // Filtering logic: check if the object should be processed based on tag or layer
        private bool ShouldProcessCollision(GameObject obj)
        {
            return !IsTagIgnored(obj.tag) && IsLayerValid(obj.layer);
        }

        // Check if the object's tag matches the ignored tags
        private bool IsTagIgnored(string tag)
        {
            if (ignoreTags == null || ignoreTags.Length == 0) return false;  // No tag filtering
            foreach (string ignoredTag in ignoreTags)
            {
                if (tag == ignoredTag) return true;
            }
            return false;
        }

        // Check if the object's layer matches the valid layers
        private bool IsLayerValid(int layer)
        {
            return (collisionLayerMask.value & (1 << layer)) > 0;
        }
    }
}
