using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace DC
{
#if UNITY_EDITOR
    public class GizmoDrawer : MonoBehaviour
    {
        #region Variables

        [Tooltip("Enable or disable gizmo drawing")]
        [SerializeField, HideInInspector] private bool drawGizmos = true;

        [Tooltip("Color of the gizmo to draw")]
        [SerializeField, HideInInspector] private Color gizmoColor = Color.cyan;

        [Tooltip("Transparency of the gizmo")]
        [SerializeField, HideInInspector] private float transparency = 0.5f;

        [Tooltip("Center position of custom gizmos")]
        [SerializeField, HideInInspector] private Vector3 center = Vector3.zero;

        [Tooltip("Cube size for custom gizmo")]
        [SerializeField, HideInInspector] private Vector3 cubeSize = Vector3.one;

        [Tooltip("Sphere radius for custom gizmo")]
        [SerializeField, HideInInspector] private float sphereRadius = 0.5f;

        [Tooltip("Length of the ray gizmo")]
        [SerializeField, HideInInspector] private float rayLength = 1f;

        [Tooltip("Start point for line gizmo")]
        [SerializeField, HideInInspector] private Transform lineFrom;

        [Tooltip("End point for line gizmo")]
        [SerializeField, HideInInspector] private Transform lineTo;

        // Enum-based controls (used directly instead of converting to int)
        [Tooltip("Type of gizmo to draw (Collider or Custom)")]
        [SerializeField, HideInInspector] private GizmosType gizmoType = GizmosType.Collider;

        [Tooltip("Shape of the custom gizmo")]
        [SerializeField, HideInInspector] private GizmoShape gizmoShape = GizmoShape.Cube;

        [Tooltip("Direction of the ray for custom gizmo")]
        [SerializeField, HideInInspector] private RayDirection rayDirection = RayDirection.forward;

        [HideInInspector] private List<Collider> colliderList = new List<Collider>();

        #endregion

        #region Getters

        public bool DrawGizmos { get { return drawGizmos; } set { drawGizmos = value; } }
        public GizmosType GizmoType { get { return gizmoType; } set { gizmoType = value; } }
        public Color GizmoColor { get { return gizmoColor; } set { gizmoColor = value; } }
        public float Transparency { get { return transparency; } set { transparency = value; } }

        public GizmoShape GizmoShape { get { return gizmoShape; } set { gizmoShape = value; } }
        public Vector3 Center { get { return center; } set { center = value; } }
        public Vector3 CubeSize { get { return cubeSize; } set { cubeSize = value; } }
        public float SphereRadius { get { return sphereRadius; } set { sphereRadius = value; } }
        public RayDirection RayDirection { get { return rayDirection; } set { rayDirection = value; } }
        public float RayLength { get { return rayLength; } set { rayLength = value; } }
        public Transform LineFrom { get { return lineFrom; } set { lineFrom = value; } }
        public Transform LineTo { get { return lineTo; } set { lineTo = value; } }
        public List<Collider> ColliderList { get { return colliderList; } set { colliderList = value; } }



        #endregion

        // Main Gizmo drawing logic
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            gizmoColor.a = transparency;
            Gizmos.color = gizmoColor;

            if (gizmoType == GizmosType.Collider)
                DrawColliderGizmo();
            else if (gizmoType == GizmosType.Custom)
                DrawCustomGizmo();
        }

        // Draws gizmos for colliders attached to the GameObject
        private void DrawColliderGizmo()
        {
            if (colliderList == null) colliderList = new List<Collider>();

            GetComponents(colliderList);

            if (colliderList == null || colliderList.Count == 0) return;

            var scale = transform.lossyScale;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);

            foreach (var collider in colliderList)
            {
                if (!collider.enabled) continue;

                switch (collider)
                {
                    case BoxCollider boxCollider:
                        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                        break;

                    case SphereCollider sphereCollider:
                        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * scale.x);
                        Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                        break;

                    case MeshCollider meshCollider when meshCollider.convex:
                        Gizmos.DrawMesh(meshCollider.sharedMesh);
                        break;
                }
            }
        }

        // Draws custom gizmos based on user input
        private void DrawCustomGizmo()
        {
            var finalPosition = transform.position + center;

            switch (gizmoShape)
            {
                case GizmoShape.Cube:
                    Gizmos.DrawCube(finalPosition, cubeSize);
                    break;

                case GizmoShape.WireCube:
                    Gizmos.DrawWireCube(finalPosition, cubeSize);
                    break;

                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(finalPosition, sphereRadius);
                    break;

                case GizmoShape.WireSphere:
                    Gizmos.DrawWireSphere(finalPosition, sphereRadius);
                    break;

                case GizmoShape.Ray:
                    Gizmos.DrawRay(finalPosition, GetRayDirection() * rayLength);
                    break;

                case GizmoShape.Line when lineFrom != null && lineTo != null:
                    Gizmos.DrawLine(lineFrom.transform.position, lineTo.transform.position);
                    break;
            }
        }

        // Determines the direction for drawing rays
        private Vector3 GetRayDirection()
        {
            return rayDirection switch
            {
                RayDirection.forward => Vector3.forward,
                RayDirection.backward => Vector3.back,
                RayDirection.left => Vector3.left,
                RayDirection.right => Vector3.right,
                RayDirection.up => Vector3.up,
                RayDirection.down => Vector3.down,
                _ => Vector3.zero
            };
        }

        // Adds a collider component dynamically
        public void AddColliderComponent(AddCollider addCol)
        {
            switch (addCol)
            {
                case AddCollider.Box:
                    gameObject.AddComponent<BoxCollider>();
                    break;
                case AddCollider.Sphere:
                    gameObject.AddComponent<SphereCollider>();
                    break;
                case AddCollider.Capsule:
                    gameObject.AddComponent<CapsuleCollider>();
                    break;
                case AddCollider.Mesh:
                    gameObject.AddComponent<MeshCollider>();
                    break;
            }
        }
    }

    // Enum Definitions
    public enum GizmosType { Collider, Custom }
    public enum GizmoShape { Cube, WireCube, Sphere, WireSphere, Ray, Line }
    public enum RayDirection { forward, backward, left, right, up, down }
    public enum AddCollider { Box, Sphere, Capsule, Mesh }

#endif

}