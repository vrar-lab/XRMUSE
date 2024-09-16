using System.Collections;
using System.Collections.Generic;
using XRMUSE.SceneDescription;
using XRMUSE.Utilities;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    public enum CalibrationMethod
{
    Manual,
    Aruco
}

/// <summary>
/// The overall manager of the XR session.
/// </summary>
public class SessionManager : MonoBehaviour
{
    public int userId;
    public static SessionManager Instance;
    public UIPanelController[] ui;
    public static bool IsAnchoringFinished = false;
    public static bool IsPlayerIdLoaded = false;
    public static bool IsPlayerConnected = false;
    public static bool IsAdaptationFinished = false;
    public static float AnchorHeight = 0.8f;
    public static float width = 1.80f;
    public static float height = 0.8f;
    public List<int[]> initialObjectives = new List<int[]>();
    public GameObject createPhoton;
    
    // Headset calibration related
    public GameObject arucoCodeAnchor;
    public GameObject manualAnchor;
    public CalibrationMethod calibrationMethod = CalibrationMethod.Aruco;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(Main());
    }
    
    /// <summary>
    /// Update the UI panel. 
    /// </summary>
    /// <param name="matType">The type of product concerned.</param>
    /// <param name="haveProduced">How many products of this type have been produced.</param>
    /// <param name="count">The total number of the product needs to be produced.</param>
    public void UpdateObjective(int matType, int haveProduced, int count)
    {
        ui[userId].objectivePanelController.AddObjectiveUpdateEvent(matType, haveProduced, count);
    }

    /// <summary>
    /// An asynchronous function for initializing an XR session, which includes headset calibration, server connection, scene operation, and UI panel update.
    /// </summary>
    public IEnumerator Main()
    {
        // Headset calibration
        if (calibrationMethod == CalibrationMethod.Aruco)
        {
            PromptController.Activate.Invoke();
            PromptController.SetText.Invoke("<b>Anchoring...</b><br><br>Please look at the code on the table.");
            PromptController.Activate.Invoke();
            arucoCodeAnchor.SetActive(true);
        }
        else if(calibrationMethod == CalibrationMethod.Manual)
        {
            PromptController.Activate.Invoke();
            PromptController.SetText.Invoke("Please use controllers to adjust the anchor.");
            manualAnchor.SetActive(true);
        }
        #if UNITY_EDITOR
        #else
        yield return new WaitUntil(() => IsAnchoringFinished);
        #endif

        // Arrange the virtual elements based on the location and dimension of the real table.
        Adapter.AdaptDimension.Invoke(width, height);
        Adapter.ActivateSelf.Invoke();
        yield return new WaitUntil(() => IsAdaptationFinished);

        // Connecting to the remote Photon server.
        TS_TimelineNetworked.SendStartLoadLog();
        if(createPhoton == null)
            createPhoton = GameObject.Find("CreatePhoton");
        createPhoton.SetActive(true);
        PromptController.SetText.Invoke("Connecting to the server...");
        yield return new WaitUntil(() => IsPlayerConnected);
        PromptController.Deactivate.Invoke();

        // Scene operation
        TS_TimelineNetworked.StartProcess.Invoke();
        yield return new WaitUntil(() => ES_Recipes.instance.recipes.Count > 0);
        yield return new WaitUntil(() => IsPlayerIdLoaded);
        userId = SD_Settings.playerNum - 1;

        // Updating UI panels
        var mat1 = ES_Recipes.instance.recipesMat1;
        var mat2 = ES_Recipes.instance.recipesMat2;
        var tool = ES_Recipes.instance.recipesTool;
        var products = ES_Recipes.instance.recipesProductions;
        ui[userId].ActivateSelf();
        ui[userId].DisplayCombsAndObjectives(UIPanelController.Convert(mat1, mat2, tool, products), initialObjectives);
        yield return null;
    }
}
}


