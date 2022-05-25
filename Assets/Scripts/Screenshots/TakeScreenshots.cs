using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakeScreenshots : MonoBehaviour
{
    [SerializeField]
    public Camera screenshotCamera;
    public ExportRoutine exportRoutine;
    public float screenshotChangeAngle = 22.5f;
    public Vector2 screenshotSize = new Vector2(512, 512);
    public int dephtBuffer = 16;
    public float viewPortMargin = 0.5f;

    private int currentIndex = 0;

    private void Start()
    {
        screenshotCamera = GameObject.FindObjectOfType<Camera>();
        if (exportRoutine == null || screenshotCamera == null)
        {
            Debug.LogWarning($"Export Routine or camera missing, add a export reference!");
            return;
        }
        else
        {
            exportRoutine.onModelCreated += TakeScreenshotsToModel;
            StartTakingScreenshots(currentIndex);
        }
    }

    private void OnDisable()
    {
        exportRoutine.onModelCreated -= TakeScreenshotsToModel;
    }

    private void TakeScreenshotsToModel(GameObject model)
    {
        if (model == null)
        {
            Debug.Log($"Model is empty or null in list, check the value and try again! /n Skipping to next model.");
            return;
        }

        StartCoroutine(RotateAndTakeScreenshot(model));
    }

    private void StartTakingScreenshots(int index)
    {
        SpawnItemList listToIterate = exportRoutine.ItemList;

        if(index < listToIterate.AssetReferenceCount)
        {
            Screen.SetResolution((int)screenshotSize.x, (int)screenshotSize.y, true, 30);
            
            exportRoutine.StartScreenshotAtIndex(index);
        }
        else
        {
            Debug.LogWarning($"No more models to load, exiting playing mode!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private IEnumerator RotateAndTakeScreenshot(GameObject model)
    {
        if (screenshotCamera == null)
        {
            screenshotCamera = Camera.main;
        }

        Bounds? bounds = ResizeCameraToFitModel(model);

        screenshotCamera.clearFlags = CameraClearFlags.SolidColor;
        screenshotCamera.backgroundColor = new Color(0, 0, 0, 0);

        Vector2 objCenter = screenshotCamera.WorldToScreenPoint(bounds.Value.center);

        for (int r = 0; r < 360 / screenshotChangeAngle; r++)
        {
            if (bounds == null)
            {
                continue;
            }

            model.transform.eulerAngles = r * screenshotChangeAngle * Vector3.up;
            yield return new WaitForEndOfFrame();
            screenshotCamera.targetTexture = RenderTexture.GetTemporary((int)screenshotSize.x, (int)screenshotSize.y, dephtBuffer);
            RenderTexture renderTexture = screenshotCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            Rect rect = new Rect(objCenter.x - renderTexture.width / 2, objCenter.y - renderTexture.height / 2, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            var parentDirectory = Directory.CreateDirectory($"{Application.dataPath}").Parent;
            var folder = Directory.CreateDirectory($"{parentDirectory}/Output/{model.name.Replace("(Clone)", "")}");
            File.WriteAllBytes($"{parentDirectory}/Output/{model.name.Replace("(Clone)", "")}/frame{GetFrameIndex(r)}.png", byteArray);
            RenderTexture.ReleaseTemporary(renderTexture);
            screenshotCamera.targetTexture = null;
            yield return new WaitForEndOfFrame();
        }

        Destroy(model);

        currentIndex++;
        StartTakingScreenshots(currentIndex);
    }

    private Bounds? ResizeCameraToFitModel(GameObject model)
    {
        Renderer[] modelRenderers = model.GetComponentsInChildren<Renderer>();
        if (modelRenderers == null)
        {
            Debug.LogWarning($"Couldn't find the model renderers skipping model");
            return null;
        }

        Bounds bounds = new Bounds(transform.position, Vector3.one);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        screenshotCamera.orthographicSize = Mathf.Max(Math.Abs(bounds.min.x), Math.Abs(bounds.min.y), Math.Abs(bounds.min.z), bounds.max.x, bounds.max.y, bounds.max.z) 
            * (bounds.max.y > bounds.max.x && bounds.max.y > bounds.max.z ? Screen.height / screenshotSize.y : Screen.width / screenshotSize.x)
            + viewPortMargin;

        screenshotCamera.transform.position = bounds.center + (Vector3.forward * -11.11f);

        return bounds;
    }
    
    private string GetFrameIndex(int r)
    {
        return r < 10 ? "000" + r : r < 100 ? "00" + r : r < 1000 ? "0" + r : r.ToString();
    }
}
