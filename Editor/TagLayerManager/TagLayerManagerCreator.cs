using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DCEditor
{
    public class TagLayerManagerCreator
    {
        [MenuItem(DCEditorMain.MenuItemPath + "Update Tag Layer Manager")]
        public static void CreateManagerScripts()
        {
            CreateTagLayerManagerScript();
        }

        public static void CreateTagLayerManagerScript()
        {
            CreateScript("TagLayerManager.cs", WriteTagLayerManager);
        }

        private static void CreateScript(string scriptName, Action<string> writeFunction)
        {
            // Check if the script already exists
            string scriptPath = FindScriptPath(scriptName);

            if (!string.IsNullOrWhiteSpace(scriptPath))
            {
                // If the script already exists, use its path
                writeFunction(scriptPath);
            }
            else
            {
                // If the script doesn't exist, prompt the user to select a folder inside the "Assets" folder
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to relative Unity path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        string defaultScriptPath = Path.Combine(relativePath, scriptName);

                        Debug.Log($"{scriptName} is created with namespace {NamespaceManagerWindow.GetDCProjectDefaultNamespace()} at location: {defaultScriptPath}\n" +
                                  "You can place it wherever you want.");

                        // Write the script to the specified path
                        try
                        {
                            writeFunction(defaultScriptPath);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to write script at {defaultScriptPath}. Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogError("Selected folder must be inside the Assets folder.");
                    }
                }
                else
                {
                    Debug.LogWarning("Script creation canceled. No folder selected.");
                }
            }
        }

        private static string FindScriptPath(string scriptName)
        {
            string[] allScriptPaths = Directory.GetFiles(Application.dataPath, scriptName, SearchOption.AllDirectories);

            if (allScriptPaths.Length > 0)
            {
                return allScriptPaths[0];
            }

            return null; // Script not found
        }

        private static void WriteTagLayerManager(string path)
        {
            string projectDefaultNamespace = NamespaceManagerWindow.GetDCProjectDefaultNamespace();
            Debug.Log("Default Name " +  projectDefaultNamespace);
            // Create the new script using a 'using' statement for automatic disposal
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine("using UnityEngine;");
                    writer.WriteLine("");

                    WriteNamespaceBlock(
                        writer,
                        projectDefaultNamespace,
                        () =>
                        {
                            WriteTagManagerClass(writer);
                            writer.WriteLine("");
                            WriteLayerManagerClass(writer);
                        });
                }

                // Refresh the asset database to make sure the new script is recognized
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write the TagLayerManager script. Error: {ex.Message}");
            }
        }

        private static void WriteNamespaceBlock(StreamWriter writer, string namespaceName, Action writeContent)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                writer.WriteLine($"namespace {namespaceName}");
                writer.WriteLine("{");
                writeContent?.Invoke();
                writer.WriteLine("}");
            }
            else
            {
                writeContent?.Invoke();
            }
        }

        private static void WriteTagManagerClass(StreamWriter writer)
        {
            string[] tagNames = UnityEditorInternal.InternalEditorUtility.tags;

            writer.WriteLine("    public static class TagManager");
            writer.WriteLine("    {");
            writer.WriteLine("        // Constant variables to store all Tag names");
            writer.WriteLine("");

            // Add each tag name to the static variable
            for (int i = 0; i < tagNames.Length; i++)
            {
                string variableName = tagNames[i].Replace(" ", ""); // Remove all spaces
                writer.WriteLine($"        public const string {variableName} = \"{tagNames[i]}\";");
            }

            writer.WriteLine("    }");
        }

        public static void WriteLayerManagerClass(StreamWriter writer)
        {
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;

            writer.WriteLine("    public static class LayerManager");
            writer.WriteLine("    {");
            writer.WriteLine("        // Constant variables to store all Layer indices");
            writer.WriteLine("");

            // Add each layer name to the static variable
            for (int i = 0; i < layerNames.Length; i++)
            {
                int layerIndex = LayerMask.NameToLayer(layerNames[i]);
                string variableName = layerNames[i].Replace(" ", ""); // Remove all spaces
                writer.WriteLine($"        public const int {variableName} = {layerIndex};");
            }

            writer.WriteLine("    }");
        }




    } //class
} //namespace
