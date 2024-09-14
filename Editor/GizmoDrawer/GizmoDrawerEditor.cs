using UnityEditor;
using UnityEngine;
using DC;

namespace DCEditor
{

#if UNITY_EDITOR
    [CustomEditor(typeof(GizmoDrawer))]
    [CanEditMultipleObjects]
    public class GizmoDrawerEditor : Editor
    {
        // Message to display if no collider is attached
        private readonly string colliderMessage = "Attach a Collider to this GameObject to visualize Gizmos or set the GizmoType to Custom for Custom Gizmos";

        public override void OnInspectorGUI()
        {
            // Draw default Inspector properties
            base.OnInspectorGUI();

            GizmoDrawer gizmoDrawer = (GizmoDrawer)target;
            Undo.RecordObject(gizmoDrawer, "GizmoDrawer");

            var centerAlignStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

            if (gizmoDrawer == null) return;

            // Master Settings
            DrawMasterSettings(gizmoDrawer);

            // Collider Gizmo Settings
            if (gizmoDrawer.GizmoType == GizmosType.Collider && gizmoDrawer.ColliderList.Count <= 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(colliderMessage, MessageType.Error, true);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Add Collider", centerAlignStyle);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Box"))
                    gizmoDrawer.AddColliderComponent(AddCollider.Box);
                if (GUILayout.Button("Sphere"))
                    gizmoDrawer.AddColliderComponent(AddCollider.Sphere);
                if (GUILayout.Button("Capsule"))
                    gizmoDrawer.AddColliderComponent(AddCollider.Capsule);
                if (GUILayout.Button("Mesh"))
                    gizmoDrawer.AddColliderComponent(AddCollider.Mesh);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            // Custom Gizmo Settings
            if (gizmoDrawer.GizmoType == GizmosType.Custom)
            {
                centerAlignStyle.fontStyle = FontStyle.Bold;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Custom Gizmo Properties", centerAlignStyle);
                EditorGUILayout.Space();

                DrawCustomSettings(gizmoDrawer);
            }
        }

        // Draws the master settings
        private void DrawMasterSettings(GizmoDrawer gizmoDrawer)
        {
            gizmoDrawer.DrawGizmos = EditorGUILayout.Toggle("Draw Gizmos", gizmoDrawer.DrawGizmos);

            EditorGUILayout.Space(10);

            gizmoDrawer.GizmoType = (GizmosType)EditorGUILayout.EnumPopup("Gizmo Type", gizmoDrawer.GizmoType);
            gizmoDrawer.GizmoColor = EditorGUILayout.ColorField("Gizmo Color", gizmoDrawer.GizmoColor);
            gizmoDrawer.Transparency = EditorGUILayout.Slider("Transparency", gizmoDrawer.Transparency, 0, 1);
        }

        // Draws custom gizmo-related settings
        private void DrawCustomSettings(GizmoDrawer gizmoDrawer)
        {
            gizmoDrawer.GizmoShape = (GizmoShape)EditorGUILayout.EnumPopup("Gizmo Shape", gizmoDrawer.GizmoShape);

            switch (gizmoDrawer.GizmoShape)
            {
                case GizmoShape.Cube:
                case GizmoShape.WireCube:
                    gizmoDrawer.Center =EditorGUILayout.Vector3Field("Center", gizmoDrawer.Center);
                    gizmoDrawer.CubeSize =EditorGUILayout.Vector3Field("Cube Size", gizmoDrawer.CubeSize);
                    break;

                case GizmoShape.Sphere:
                case GizmoShape.WireSphere:
                    gizmoDrawer.Center =EditorGUILayout.Vector3Field("Center", gizmoDrawer.Center);
                    gizmoDrawer.SphereRadius = EditorGUILayout.FloatField("Radius", gizmoDrawer.SphereRadius);
                    break;

                case GizmoShape.Ray:
                    gizmoDrawer.Center = EditorGUILayout.Vector3Field("Center", gizmoDrawer.Center);
                    gizmoDrawer.RayDirection = (RayDirection)EditorGUILayout.EnumPopup("Direction", gizmoDrawer.RayDirection);
                    gizmoDrawer.RayLength = EditorGUILayout.FloatField("Ray Length", gizmoDrawer.RayLength);
                    break;

                case GizmoShape.Line:
                    gizmoDrawer.LineFrom = (Transform)EditorGUILayout.ObjectField("Line From", gizmoDrawer.LineFrom, typeof(Transform), true);
                    gizmoDrawer.LineTo = (Transform)EditorGUILayout.ObjectField("Line To", gizmoDrawer.LineTo, typeof(Transform), true);
                    break;
            }
        }
    }

#endif
}