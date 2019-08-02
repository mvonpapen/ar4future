﻿using UnityEngine;
using UnityEngine.Video;
using Vuforia;
public class SimpleCloudHandler : MonoBehaviour, IObjectRecoEventHandler
{
    // from howto ar video
    public ImageTargetBehaviour behaviour;
    CloudRecoBehaviour cloud;
    public GameObject mainPlayer;

    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;
    private string mTargetMetadata = "";
    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
        // from howto ar video
        cloud = mCloudRecoBehaviour;
        mainPlayer = GameObject.Find("Player");
        Hide (mainPlayer);

    }

    // from howto ar video
    void Hide(GameObject ob)
    {
        Renderer[] rends = ob.GetComponentsInChildren<Renderer>();
        Collider[] cols = ob.GetComponentsInChildren<Collider>();
        foreach (var item in rends)
        {
            item.enabled = false;
        }
        foreach (var item in cols)
            item.enabled = false;
    }


    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
    }
    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }


    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetBehaviour, 
    /// then delete all trackables
    /// This function is from CloudRecoEventHandler.cs from Vuforia Base sample
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
        Debug.Log("<color=blue>OnStateChanged(): </color>" + scanning);

        // Changing CloudRecoBehaviour.CloudRecoEnabled to false will call:
        // 1. TargetFinder.Stop()
        // 2. All registered ICloudRecoEventHandler.OnStateChanged() with false.

        // Changing CloudRecoBehaviour.CloudRecoEnabled to true will call:
        // 1. TargetFinder.StartRecognition()
        // 2. All registered ICloudRecoEventHandler.OnStateChanged() with true.

    }


    //// Here we handle a cloud target recognition event
    //public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    //{
    //    TargetFinder.CloudRecoSearchResult cloudRecoSearchResult =
    //        (TargetFinder.CloudRecoSearchResult)targetSearchResult;
    //    // do something with the target metadata
    //    mTargetMetadata = cloudRecoSearchResult.MetaData;
    //    // stop the target finder (i.e. stop scanning the cloud)
    //    mCloudRecoBehaviour.CloudRecoEnabled = false;
    //}

    // from howto ar video
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        GameObject newImageTarget = Instantiate(behaviour.gameObject) as GameObject;
        mainPlayer = newImageTarget.transform.GetChild(0).gameObject;
        GameObject augmentation = null;
        if (augmentation != null)
        {
            augmentation.transform.SetParent(newImageTarget.transform);
        }
        if (behaviour)
        {
            TargetFinder.CloudRecoSearchResult cloudRecoSearchResult =
            (TargetFinder.CloudRecoSearchResult)targetSearchResult;
            // do something with the target metadata
            mTargetMetadata = cloudRecoSearchResult.MetaData;
        }
        mainPlayer.GetComponent<VideoPlayer>().url = mTargetMetadata.Trim();
        cloud.CloudRecoEnabled = true;
    }


    void OnGUI()
    {
        // Display current 'scanning' status
        GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
            {
                // Restart TargetFinder
                mCloudRecoBehaviour.CloudRecoEnabled = true;
            }
        }
    }
}