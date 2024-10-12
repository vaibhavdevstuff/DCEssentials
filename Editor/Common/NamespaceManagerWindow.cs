using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using DCEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class NamespaceManagerWindow : EditorWindow
{
    private string namespaceInput = "";
    private string errorMessage = "";
    private const string ProjectDefaultNamespace = "ProjectDefaultNamespace"; // Default key
    private const string DontShowOnStartKey = "NamespaceManger_DontShowOnStart"; // Unique key for the project
    private bool dontShowOnStart = false; // State of the checkbox

    private string headInfo = "Enter your default project Namespace (For DC Essential Tools)";
    private Regex invalidNamespaceRegex = new Regex(@"[^a-zA-Z0-9_.]"); // Regex for invalid characters

    static NamespaceManagerWindow()
    {
        if (!SessionState.GetBool("FirstInitDone", false))
        {
            SessionState.SetBool("FirstInitDone", true);

            EditorApplication.update += CheckEditorState;
        }
    }

    private static void CheckEditorState()
    {
        // Check if the editor is playing or scripts are compiling
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;

        // Unsubscribe to prevent the method from being called repeatedly
        EditorApplication.update -= CheckEditorState;

        // Check the EditorPrefs on load
        if (string.IsNullOrEmpty(EditorPrefs.GetString(GetDCProjectDefaultNamespace(), "")) &&
            !EditorPrefs.GetBool(DontShowOnStartKey, false))
        {
            ShowWindow();
        }
    }


    public static string GetDCProjectDefaultNamespace()
    {
        return EditorPrefs.GetString(KEY_DCProjectDefaultNamespace(), "");
    }
    
    private static string KEY_DCProjectDefaultNamespace()
    {
        // Get the full path of the "Assets" folder
        string projectPath = Application.dataPath;

        // Get the project folder name by getting the parent folder of "Assets"
        string projectName = Path.GetFileName(Path.GetDirectoryName(projectPath));

        return $"{projectName}_{ProjectDefaultNamespace}";
    }

    [MenuItem(DCEditorMain.MenuItemPath + "Set Project Default Namespace", priority = 10)]
    public static void ShowWindow()
    {
        NamespaceManagerWindow window = (NamespaceManagerWindow)GetWindow(typeof(NamespaceManagerWindow), true, "Namespace Manager");
        window.minSize = new Vector2(600, 200);
        window.maxSize = new Vector2(1200, 400);
        window.Initialize(); // Initialize the window state
        window.Show(); // Show the window
    }

    private void Initialize()
    {
        namespaceInput = GetDCProjectDefaultNamespace();
        dontShowOnStart = EditorPrefs.GetBool(DontShowOnStartKey, false); // Get the checkbox state
        position = new Rect(100, 100, 600, 300); // Set the default position and size
    }

    private void OnGUI()
    {
        // Custom style for the HelpBox
        GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 11, // Set desired font size
            wordWrap = true // Word wrapping like the default HelpBox
        };

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(headInfo, EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        // Input field for namespace
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        namespaceInput = GUILayout.TextField(namespaceInput, GUILayout.Width(300));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Display the error message (if any)
        if (!string.IsNullOrEmpty(errorMessage))
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(errorMessage, helpBoxStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();

        // OK and Keep Empty buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // OK Button
        if (GUILayout.Button("OK", GUILayout.Width(100)))
        {
            ValidateAndSaveNamespace();
        }

        GUILayout.Space(20);

        // Keep Empty Button
        if (GUILayout.Button("Keep Empty", GUILayout.Width(100)))
        {
            ClearNamespace();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        // Checkbox for "Don't Show on Start" at the bottom left corner
        GUILayout.BeginHorizontal();
        bool newDontShowOnStart = GUILayout.Toggle(dontShowOnStart, "Don't Show on Start");
        if (newDontShowOnStart != dontShowOnStart) // If the checkbox value has changed
        {
            dontShowOnStart = newDontShowOnStart;
            SaveDontShowOnStart(); // Save immediately when changed
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    private void ValidateAndSaveNamespace()
    {
        // Check if input is empty
        if (string.IsNullOrEmpty(namespaceInput))
        {
            errorMessage = "Namespace cannot be empty!";
        }
        else if (char.IsDigit(namespaceInput[0])) // Check if input starts with a number
        {
            errorMessage = "Namespace cannot start with a number!";
        }
        else if (invalidNamespaceRegex.IsMatch(namespaceInput)) // Check for invalid characters
        {
            errorMessage = "Namespace cannot contain spaces or special symbols (except dots and underscores)!";
        }
        else
        {
            // Save to EditorPrefs using the specific key
            EditorPrefs.SetString(KEY_DCProjectDefaultNamespace(), namespaceInput);
            SaveDontShowOnStart(); // Save checkbox state
            errorMessage = "";
            Close(); // Close the window after successful save
        }
    }

    private void ClearNamespace()
    {
        namespaceInput = "";
        EditorPrefs.SetString(KEY_DCProjectDefaultNamespace(), "");
        SaveDontShowOnStart(); // Save checkbox state
        Debug.Log("Namespace cleared, set to empty.");
        errorMessage = "";
        Close();
    }

    private void SaveDontShowOnStart()
    {
        EditorPrefs.SetBool(DontShowOnStartKey, dontShowOnStart);
    }

    private void OnDisable() // Save the state when the window is closed
    {
        SaveDontShowOnStart();
    }
}
