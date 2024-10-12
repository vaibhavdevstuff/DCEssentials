using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace DCEditor
{
    public class SceneLoaderWindow : EditorWindow
    {
        private string folderPath = "Assets"; // Default folder path
        private List<string> scenePaths = new List<string>(); // Store scene paths
        private Vector2 scrollPosition; // For scrolling the scene list

        private string ScenePathKey { get { return "SCENEPATHKEY_" + GetProjectName(); } }

        // Add a menu item to open this window
        [MenuItem(DCEditorMain.MenuItemPath + "Scene Loader")]
        public static void ShowWindow()
        {
            GetWindow<SceneLoaderWindow>("Scene Loader");
        }

        private void OnEnable()
        {
            folderPath = EditorPrefs.GetString(ScenePathKey, Application.dataPath);

            if (folderPath != Application.dataPath)
            {
                FindScenesInFolder(folderPath);
            }
        }

        private void OnGUI()
        {
            // Folder selection
            GUILayout.Label("Select Folder", EditorStyles.boldLabel);

            if (GUILayout.Button("Select Folder"))
            {

                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to relative Unity path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        Debug.Log(folderPath);
                        EditorPrefs.SetString(ScenePathKey, folderPath);
                    }
                    else
                    {
                        Debug.LogError("Selected folder must be inside the Assets folder.");
                        return;
                    }

                    FindScenesInFolder(folderPath);
                }
            }

            // Display selected folder
            GUILayout.Label("Selected Folder: " + (folderPath == Application.dataPath ? "Assets" : folderPath), EditorStyles.label);

            //if (folderPath == "Assets") return;

            // Refresh button
            RefreshSceneList();

            // Show found scenes in a scroll view
            GUILayout.Label("Found Scenes:", EditorStyles.boldLabel);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (string scenePath in scenePaths)
            {
                // Extract scene name (remove path and extension)
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                GUILayout.BeginHorizontal();
                GUILayout.Label(sceneName, GUILayout.ExpandWidth(true));

                // Focus scene location in the Project window
                if (GUILayout.Button("*L", GUILayout.Width(25)))
                {
                    FocusSceneInProject(scenePath);
                }

                // Load scene button
                if (GUILayout.Button("Load Scene", GUILayout.Width(100)))
                {
                    LoadScene(scenePath);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void RefreshSceneList()
        {
            if (GUILayout.Button("Refresh"))
            {
                FindScenesInFolder(folderPath);
            }
        }

        // Find all scenes in the folder and subfolders
        private void FindScenesInFolder(string path)
        {
            scenePaths.Clear(); // Clear the previous list

            // Get all scene files in the folder and its subfolders
            string[] allFiles = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);

            foreach (string file in allFiles)
            {
                scenePaths.Add(file);
            }
        }

        // Load the scene by path
        private void LoadScene(string scenePath)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot load scene while the game is playing.");
                return;
            }

            // Prompt the user to save changes if there are any unsaved scenes
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // Load the new scene after saving (or discarding) changes
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
            else
            {
                // User pressed cancel, so abort the scene loading
                Debug.Log("Scene loading canceled by the user.");
            }
        }

        // Focus scene location in the Project window
        private void FocusSceneInProject(string scenePath)
        {
            // Find the asset in the Project window
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath);

            if (asset != null)
            {
                // Ping the asset (highlight it in the Project window)
                EditorGUIUtility.PingObject(asset);

                // Focus on the Project window
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogError("Could not find the scene in the Project window.");
            }
        }

        string GetProjectName()
        {
            // Get the full path of the "Assets" folder
            string projectPath = Application.dataPath;

            // Get the project folder name by getting the parent folder of "Assets"
            return Path.GetFileName(Path.GetDirectoryName(projectPath));
        }
    }
}