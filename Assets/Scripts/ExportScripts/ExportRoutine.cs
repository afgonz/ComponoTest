﻿using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ExportRoutine : MonoBehaviour
{   
    [SerializeField] private SpawnItemList m_itemList = null;

    private AssetReferenceGameObject m_assetLoadedAsset;
    private GameObject m_instanceObject = null;
    public Action<GameObject> onModelCreated;

    public SpawnItemList GetItemList { get { return m_itemList; } }

    public void StartScreenshotAtIndex(int index)
    {
        if (m_itemList == null || m_itemList.AssetReferenceCount == 0) 
        {
            Debug.LogError("Spawn list not setup correctly");
        }        
        LoadItemAtIndex(m_itemList, index);
    }

    private void LoadItemAtIndex(SpawnItemList itemList, int index) 
    {
        if (m_instanceObject != null) 
        {
            Destroy(m_instanceObject);
        }       
        
        m_assetLoadedAsset = itemList.GetAssetReferenceAtIndex(index);
        var spawnPosition = new Vector3();
        var spawnRotation = Quaternion.identity;
        var parentTransform = this.transform;


        var loadRoutine = m_assetLoadedAsset.LoadAssetAsync();
        loadRoutine.Completed += LoadRoutine_Completed;

        void LoadRoutine_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
        {
            m_instanceObject = Instantiate(obj.Result, spawnPosition, spawnRotation, parentTransform);

            if(m_instanceObject != null)
            {
                onModelCreated?.Invoke(m_instanceObject);
            }
        }
    }
}
