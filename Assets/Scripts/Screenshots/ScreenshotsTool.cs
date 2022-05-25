using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;

public class ScreenshotsTool : EditorWindow
{
    private Camera screenshotCamera;
    private ExportRoutine exportRoutine;
    private TakeScreenshots takeScreenshots;

    //private SpawnItemList m_itemList = null;
    private float screenshotChangeAngle = 22.5f;
    private Vector2Int screenshotSize = new Vector2Int(512, 512);
    private int dephtBuffer = 16;
    private AssetReferenceGameObject m_assetLoadedAsset;
    private GameObject m_instanceObject = null;
    private float viewPortMargin = 0.5f;
    //public Action<GameObject> onModelCreated;

    private int currentIndex = 0;

    private SpawnItemList spawnItemList;


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

        screenshotChangeAngle = EditorGUILayout.FloatField("Screenshot Change Angle", screenshotChangeAngle);

        screenshotSize = EditorGUILayout.Vector2IntField("Screenshot Size", screenshotSize);

        viewPortMargin = EditorGUILayout.FloatField("Screenshot Margin", viewPortMargin);

        dephtBuffer = EditorGUILayout.IntField("Depth Buffer", dephtBuffer);


        if (GUILayout.Button("Take screenshots"))
        {
            currentIndex = 0;

            exportRoutine = FindObjectOfType<ExportRoutine>();
            takeScreenshots = FindObjectOfType<TakeScreenshots>();

            if (exportRoutine == null)
            {
                exportRoutine = new GameObject("Asset-SpawnPoint", typeof(ExportRoutine)).GetComponent<ExportRoutine>();
                exportRoutine.ItemList = spawnItemList;
            }

            if (takeScreenshots == null)
            {
                takeScreenshots = exportRoutine.gameObject.AddComponent<TakeScreenshots>();
            }

            takeScreenshots.exportRoutine = this.exportRoutine;
            takeScreenshots.screenshotChangeAngle = this.screenshotChangeAngle;
            takeScreenshots.screenshotSize = this.screenshotSize;
            takeScreenshots.dephtBuffer = this.dephtBuffer;
            takeScreenshots.viewPortMargin = this.viewPortMargin;

            if (spawnItemList == null)       
            {
                spawnItemList = exportRoutine.ItemList;
            }

            screenshotCamera = GameObject.FindObjectOfType<Camera>();

            if (spawnItemList == null)
            {
                Debug.LogError($"Missing Item List, please provide a list of objects to be exported!");
                return;
            }



            EditorApplication.isPlaying = true;

            Debug.Log($"Started editor, trying to start taking screenshots.");
        }
    }
}