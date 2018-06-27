using UnityEngine;
using System.Collections;


// Super super hacky way to manage step transitions in the guided experience prototype

// GameObjects for all steps are part of the main scene, step through narrative sequence by showing/hiding objects for each step
// In Unity:
//  Tag one GameObject with "Step#" to have it's visibility controllable at each step
//      By convention there is a container named with the Step # where it first appears
//      If there is only a single object for a step (e.g Title and End Screens) it can be directly tagged and doesn't need to be in a containing object
// TBD: Rename all Step# Tags in Unity to Group#
    
// Wrapper class to make array of arrays editable in Unity inspector
[System.Serializable]
public class ActiveGroupsThisStep
{
    public bool[] isActive;
}

public class StepManager : MonoBehaviour
{

    // TBD: Derive lastStep and lastGroup from the lengths of the appropriate arrays
    // TBD: Verify at Start() that all the arrays are the correct length
    private const int firstStep = 0;
    private const int lastStep = 10;
    private int currentStep = firstStep;

    private const int firstGroup = 0;
    private const int lastGroup = 14;

    // Save references to root objects for each Step for setting active/inactive
    private GameObject[] objectGroups;

    public AudioClip[] stepNarrations;
    public float[] stepNarrationsVolumeScale;
    public bool[] stepNarrationsLoop;
    public ActiveGroupsThisStep[] activeGroupsPerStep;

    AudioSource audioSource;

    // Awake Runs before all Start() methods
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Find, save and hide game objects for all groups
        objectGroups = new GameObject[lastGroup + 1];

        GameObject[] taggedObjects;
        for (int i = firstGroup; i <= lastGroup; i++)
        {
            taggedObjects = GameObject.FindGameObjectsWithTag("Step" + i.ToString());

            if (taggedObjects.Length > 0)
            {
                // Save _only_ the first object found with each Step# tag
                objectGroups[i] = taggedObjects[0];

                // SetToCurrentStep() should take care of this
                /* if (i != firstStep)
                    objectGroups[i].SetActive(false);
                    */
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

        for (int objectIndex = firstGroup; objectIndex <= lastGroup; objectIndex++)
        {
            objectGroups[objectIndex].SetActive(activeGroupsPerStep[currentStep].isActive[objectIndex]);
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

