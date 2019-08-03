/*==============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour, IObjectRecoEventHandler
{
    // from howto ar video
    public GameObject mainPlayer;
    private string mTargetMetadata = "";
    public Text ErrorTxt;
    string errorTitle, errorMsg;


    #region PRIVATE_MEMBERS
    CloudRecoBehaviour m_CloudRecoBehaviour;
    ObjectTracker m_ObjectTracker;
    TargetFinder m_TargetFinder;
    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_MEMBERS
    /// <summary>
    /// Can be set in the Unity inspector to reference a ImageTargetBehaviour 
    /// that is used for augmentations of new cloud reco results.
    /// </summary>
    [Tooltip("Here you can set the ImageTargetBehaviour from the scene that will be used to " +
             "augment new cloud reco search results.")]
    public ImageTargetBehaviour m_ImageTargetBehaviour;
    public UnityEngine.UI.Image m_CloudActivityIcon;
    public UnityEngine.UI.Image m_CloudIdleIcon;
    #endregion // PUBLIC_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    /// <summary>
    /// Register for events at the CloudRecoBehaviour
    /// </summary>
	void Start()
    {
        CloudRecoBehaviour cloudReco = GetComponent<CloudRecoBehaviour>();
        if (cloudReco)
        {
            cloudReco.RegisterEventHandler(this);
        }
        m_CloudRecoBehaviour = cloudReco;
        mainPlayer = GameObject.Find("Player");
        // Hide(mainPlayer);
    }

    void Update()
    {
        if (m_CloudRecoBehaviour.CloudRecoInitialized && m_TargetFinder != null)
        {
            SetCloudActivityIconVisible(m_TargetFinder.IsRequesting());
        }

        if (m_CloudIdleIcon)
        {
            m_CloudIdleIcon.color = m_CloudRecoBehaviour.CloudRecoEnabled ? Color.white : Color.gray;
        }
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region INTERFACE_IMPLEMENTATION_ICloudRecoEventHandler
    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized successfully.");

        m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        m_TargetFinder = m_ObjectTracker.GetTargetFinder<ImageTargetFinder>();
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized successfully.");

        m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        m_TargetFinder = targetFinder;
    }

    // Error callback methods implemented in CloudErrorHandler
    public void OnInitError(TargetFinder.InitState initError)
    {
        switch (initError)
        {
            case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                errorTitle = "Network Unavailble";
                errorMsg = "Check internet connection and try again";
                break;
            case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                errorTitle = "Service not availble";
                errorMsg = "Failed to initialize beacause service is unavailble";
                break;
        }
        errorMsg = "<color=red>" + initError.ToString().Replace("_", " ") + "</color>\n\n" + errorMsg;
        ErrorTxt.text = "Cloud Reco - Update Error: " + initError + "\n\n" + errorMsg;
    }
    public void OnUpdateError(TargetFinder.UpdateState updateError) { }


    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetBehaviour, 
    /// then delete all trackables
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

    /// <summary>
    /// Handles new search results
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        GameObject newImageTarget = Instantiate(m_ImageTargetBehaviour.gameObject) as GameObject;
        mainPlayer = newImageTarget.transform.GetChild(0).gameObject;
        GameObject augmentation = null;
        if (augmentation != null)
        {
            augmentation.transform.SetParent(newImageTarget.transform);
        }
        if (m_ImageTargetBehaviour)
        {
            TargetFinder.CloudRecoSearchResult cloudRecoResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
            mTargetMetadata = cloudRecoResult.MetaData;
        }
        string URL = mTargetMetadata;
        mainPlayer.GetComponent<VideoPlayer>().url = URL.Trim();
        m_CloudRecoBehaviour.CloudRecoEnabled = true;
    }
    #endregion // INTERFACE_IMPLEMENTATION_ICloudRecoEventHandler


    #region PRIVATE_METHODS
    void SetCloudActivityIconVisible(bool visible)
    {
        if (!m_CloudActivityIcon) return;

        m_CloudActivityIcon.enabled = visible;
    }
    #endregion // PRIVATE_METHODS

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
}
