using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakeScreenshots : MonoBehaviour
{
    [SerializeField]
    private Camera screenshotCamera;
    [SerializeField]
    private ExportRoutine exportRoutine;
    [SerializeField]
    private float screenshotChangeAngle = 22.5f;
    [SerializeField]
    private Vector2 screenshotSize = new Vector2(500, 500);
    [SerializeField]
    private int dephtBuffer = 16;

    private int currentIndex = 0;

    private void Start()
    {
        if (exportRoutine == null)
        {
            Debug.LogWarning($"Export Routine missing, add a export reference!");
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
        SpawnItemList listToIterate = exportRoutine.GetItemList;

        if(index < listToIterate.AssetReferenceCount)
        {
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
            yield return new WaitForEndOfFrame();
        for(int r = 0; r < 360 / 22.5f; r++)
        {
            screenshotCamera.targetTexture = RenderTexture.GetTemporary((int)screenshotSize.x, (int)screenshotSize.y, dephtBuffer);
            RenderTexture renderTexture = screenshotCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            Rect rect = new Rect(0,0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            var parentDirectory = Directory.CreateDirectory($"{Application.dataPath}").Parent;
            var folder = Directory.CreateDirectory($"{parentDirectory}/Output/{model.name.Replace("(Clone)", "")}");
            File.WriteAllBytes($"{parentDirectory}/Output/{model.name.Replace("(Clone)", "")}/frame{GetFrameIndex(r)}.png", byteArray);
            model.transform.Rotate(r * 22.5f * Vector3.up);
            RenderTexture.ReleaseTemporary(renderTexture);
            screenshotCamera.targetTexture = null;
        }

        Destroy(model);

        currentIndex++;
        StartTakingScreenshots(currentIndex);
    }

    private string GetFrameIndex(int r)
    {
        return r < 10 ? "000" + r : r < 100 ? "00" + r : r < 1000 ? "0" + r : r.ToString();
    }
}
