using UnityEngine;
using UnityEditor;

public class ScreenshotsTool : EditorWindow
{
    SpawnItemList spawnItemList;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Compono/ScreenshotsTool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ScreenshotsTool window = (ScreenshotsTool)EditorWindow.GetWindow(typeof(ScreenshotsTool));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.Label("Screenshot tool settings", EditorStyles.boldLabel);

        spawnItemList = EditorGUILayout.ObjectField("Spawn Item List", spawnItemList, typeof(SpawnItemList), true) as SpawnItemList;

        if(GUILayout.Button("Take screenshots"))
        {   
            if (spawnItemList == null)
            {
                Debug.LogError($"Missing Item List, please provide a list of objects to be exported!");
                return;
            }

            // OPTIONAL: Modify the list instantiation to take screenshots outside play mode.

        }



    }
}