using UnityEngine;
using System.Collections;


// Super super hacky way to manage step transitions in the guided experience prototype

// GameObjects for all steps are part of the main scene, step through narrative sequence by showing/hiding objects for each step
// In Unity:
//  Tag one GameObject with "Step#" to have it's visibility controllable at each step
//      By convention there is a container named with the Step # where it first appears
//      If there is only a single object for a step (e.g Title and End Screens) it can be directly tagged and doesn't need to be in a containing object

// Wrapper class to make array of arrays editable in Unity inspector
[System.Serializable]
public class ActiveObjectsThisStep
{
    public bool[] isActive;
}

public class StepManager : MonoBehaviour
{
    private const int firstStep = 0;
    private const int lastStep = 9;
    private int currentStep = firstStep;

    // Save references to root objects for each Step for setting active/inactive
    private GameObject[] stepObjects;

    public AudioClip[] stepNarrations;
    public float[] stepNarrationsVolumeScale;
    public bool[] stepNarrationsLoop;
    public ActiveObjectsThisStep[] activeObjectsPerStep;

    AudioSource audioSource;

    // Awake Runs before all Start() methods
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Find, save and hide game objects for all steps (except first step)
        stepObjects = new GameObject[lastStep + 1];

        GameObject[] taggedObjects;
        for (int i = firstStep; i <= lastStep; i++)
        {
            taggedObjects = GameObject.FindGameObjectsWithTag("Step" + i.ToString());

            if (taggedObjects.Length > 0)
            {
                // Save _only_ the first object found with each Step# tag
                stepObjects[i] = taggedObjects[0];
                if (i != firstStep)
                    stepObjects[i].SetActive(false);
            }
        }

        currentStep = firstStep;
        SetToCurrentStep();
    }

    void OnRestart()
    {
        currentStep = firstStep;
        SetToCurrentStep();
    }

    void OnStart()
    {
        if (currentStep == firstStep)
            OnStepForward();
        else
            OnRestart();
    }

    void OnStepForward()
    {
        currentStep++;

        // Wrap around after lastStep
        if (currentStep > lastStep)
            OnRestart();

        SetToCurrentStep();
    }

    void OnStepBack()
    {
        currentStep--;

        // Don't do anything if we were already at the first Step
        if (currentStep < firstStep)
            currentStep = firstStep;
        else
            SetToCurrentStep();
    }
    
    void OnQuitRequested()
    {
        Debug.Log("Quit requested");
        Application.Quit();
    }

    void SetToCurrentStep()
    {
        // Stop any audio that may still be playing from previous steps
        audioSource.Stop();

        for (int objectIndex = firstStep; objectIndex <= lastStep; objectIndex++)
        {
            stepObjects[objectIndex].SetActive(activeObjectsPerStep[currentStep].isActive[objectIndex]);
        }

        // start narration or music for this step
        PlayStepNarration();
    }

    // start narration or music for this step
    private void PlayStepNarration()
    {
        float volumeScale;
        const float defaultVolumeScale = 1.0f;
        bool defaultLoop = false;
        bool loop = defaultLoop;

        if (((stepNarrations.Length - 1) >= currentStep) && (stepNarrations[currentStep] != null))
        {
            if ((stepNarrationsVolumeScale.Length - 1) >= currentStep)
                volumeScale = stepNarrationsVolumeScale[currentStep];
            else
                volumeScale = defaultVolumeScale;

            if ((stepNarrationsLoop.Length - 1) >= currentStep)
                loop = stepNarrationsLoop[currentStep];
            else
                loop = defaultLoop;

            audioSource.volume = volumeScale;
            audioSource.loop = loop;
            audioSource.clip = stepNarrations[currentStep];

            audioSource.Play();
        }
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

