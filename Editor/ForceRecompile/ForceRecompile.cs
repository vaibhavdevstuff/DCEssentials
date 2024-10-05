using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;

namespace DCEditor
{
    public class ForceRecompile
    {
        // Adds a menu item called "Force Recompile Scripts" in the "Tools" menu with a shortcut.
        [MenuItem(DCEditorMain.MenuItemPath + "Force Recompile Scripts")]  // % = Ctrl (Cmd on Mac), # = Shift, r = R key
        public static void Recompile()
        {
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}
