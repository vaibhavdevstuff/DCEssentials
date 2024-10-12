using System;
using UnityEngine;
using UnityEngine.Events;

namespace DC
{
    public class TriggerCallbacks : MonoBehaviour
    {
        // Action events for code-based listeners
        public static event Action<Collider> OnTriggerEntered;
        public static event Action<Collider> OnTriggerStayed;
        public static event Action<Collider> OnTriggerExited;

        // UnityEvent fields for inspector-based event handling
        [Tooltip("Called when OnTriggerEnter is invoked.")]
        public UnityEvent<Collider> OnTriggerEnterEvent;

        [Tooltip("Called when OnTriggerStay is invoked.")]
        public UnityEvent<Collider> OnTriggerStayEvent;

        [Tooltip("Called when OnTriggerExit is invoked.")]
        public UnityEvent<Collider> OnTriggerExitEvent;

        // Filtering settings
        [Header("Filtering Settings")]
        [Tooltip("Only triggers for objects in these layers.")]
        [SerializeField] private LayerMask triggerLayerMask = ~0;

        [Tooltip("Objects with these tags will be ignored.")]
        [SerializeField] private string[] ignoreTags;

        private void OnTriggerEnter(Collider other)
        {
            if (ShouldProcessTrigger(other.gameObject))
            {
                OnTriggerEntered?.Invoke(other);  // Code-based listeners
                OnTriggerEnterEvent?.Invoke(other);  // Inspector-based listeners
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (ShouldProcessTrigger(other.gameObject))
            {
                OnTriggerStayed?.Invoke(other);  // Code-based listeners
                OnTriggerStayEvent?.Invoke(other);  // Inspector-based listeners
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ShouldProcessTrigger(other.gameObject))
            {
                OnTriggerExited?.Invoke(other);  // Code-based listeners
                OnTriggerExitEvent?.Invoke(other);  // Inspector-based listeners
            }
        }

        // Filtering logic: check if the object should be processed based on tag or layer
        private bool ShouldProcessTrigger(GameObject obj)
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
            return (triggerLayerMask.value & (1 << layer)) > 0;
        }
    }
}
