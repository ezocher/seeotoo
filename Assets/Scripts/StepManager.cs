using UnityEngine;
using System.Collections;


// Super hacky way to manage step transitions in the guided experience prototype

// GameObjects for all steps are part of the main scene, step through narrative sequence by showing/hiding objects for each step
// In Unity:
//  Tag GameObjects with "Clear" to clear them before every step
//  Tag GameObjects with "Step#" to have them show during that step (e.g. "Step2")

public class StepManager : MonoBehaviour
{

    private const int firstStep = 1;
    private const int lastStep = 5;
    private int currentStep;

    // Runs before all Start() methods
    void Awake()
    {
        OnRestart();
    }

    void OnRestart()
    {
        currentStep = firstStep;
        SetToCurrentStep();
    }

    void OnStepForward()
    {
        currentStep++;
   
        // Wrap around after lastStep
        if (currentStep > lastStep)
            currentStep = firstStep;

        SetToCurrentStep();
    }

    void OnStepBack()
    {
        currentStep--;

        // Don't do anything if we're at the firstStep
        if (currentStep < firstStep)
            currentStep = firstStep;
        else
        {
            SetToCurrentStep();
        }
    }
    
    void OnQuitRequested()
    {
        Debug.Log("Quit requested");
        Application.Quit();
    }

    void SetToCurrentStep()
    {
        // hide everything that's cleared every step
        GameObject[] gameObjectsToHide;

        gameObjectsToHide = GameObject.FindGameObjectsWithTag("Clear");

        foreach (GameObject objToHide in gameObjectsToHide)
        {
           objToHide.SetActive(false);
        }

        // show objects for this step
        GameObject[] gameObjectsToShow;

        gameObjectsToShow = GameObject.FindGameObjectsWithTag("Step" + currentStep.ToString());

        foreach (GameObject objToShow in gameObjectsToShow)
        {
            objToShow.SetActive(true);
        }

        // start narration for this step
        // *TBD*

    }

    /*
    // Handle keyboard input - PERHAPS do this in editor for convenience
    public void Update()
    {
        ZUtil.CheckForQuit();

        int jumpToLevel = ZUtil.DigitInput();
        if (jumpToLevel != ZUtil.noDigitInput)
            if ((jumpToLevel >= 1) && (jumpToLevel <= G.lastGameLevel))
                LoadGameLevel(jumpToLevel);

        Powerups.CheckForKeyboardCommands();
    }
    */

}

