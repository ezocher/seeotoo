using UnityEngine;
using System.Collections;


// Super hacky way to manage step transitions in the guided experience prototype

// GameObjects for all steps are part of the main scene, step through narrative sequence by showing/hiding objects for each step
// In Unity:
//  Tag GameObjects with "Clear" to clear them before every step
//  Tag GameObjects with "Step#" to have them show during that step (e.g. "Step2")

public class StepManager : MonoBehaviour
{

    // *TBD* Need to replace this with an array of tag numbers to be visible in each step
    private const int firstStep = 0;
    private const int lastStep = 6;
    private const int exclusiveStep = 5;
    private int currentStep = 0;
    private int persistentStep = 1; // Always present except for Step0

    private GameObject[] stepObjects;

    public AudioClip[] stepNarrations;
    public float stepNarrationsVolumeScale = 1.0f;
    AudioSource audioSource;

    // NOTE: Step1 is always showing, never hide it

    // Awake Runs before all Start() methods
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Save and hide game objects for all steps (except first step)
        stepObjects = new GameObject[lastStep + 1];

        GameObject[] taggedObjects;
        for (int i = firstStep; i <= lastStep; i++)
        {
            taggedObjects = GameObject.FindGameObjectsWithTag("Step" + i.ToString());

            if (taggedObjects.Length > 0)
            {
                stepObjects[i] = taggedObjects[0];
                if (i != firstStep)
                    stepObjects[i].SetActive(false);
            }
        }

        currentStep = firstStep;
        SetToCurrentStep(false);
    }

    void OnRestart()
    {
        currentStep = firstStep;
        SetToCurrentStep(true);
        stepObjects[firstStep].SetActive(true);
        stepObjects[persistentStep].SetActive(false);
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

        SetToCurrentStep(true);
    }

    void OnStepBack()
    {
        currentStep--;

        // Don't do anything if we're at the firstStep
        if (currentStep < firstStep)
            currentStep = firstStep;
        else
        {
            SetToCurrentStep(true);
        }
    }
    
    void OnQuitRequested()
    {
        Debug.Log("Quit requested");
        Application.Quit();
    }

    void SetToCurrentStep(bool hideAll)
    {
        // Stop any audio that was previously playing
        audioSource.Stop();

        if (hideAll)
        {
            for (int i = firstStep; i <= lastStep; i++)
            {
                if (i != persistentStep)
                    if (stepObjects[i] != null)
                        stepObjects[i].SetActive(false);
            }
        }

        for (int i = firstStep + 1; i <= currentStep; i++)
        {
            if (!((currentStep == exclusiveStep) && (i > firstStep) && (i < exclusiveStep)))
            {
                if (stepObjects[i] != null)
                    stepObjects[i].SetActive(true);
            }
        }

        // start narration for this step
        if (((stepNarrations.Length - 1) >= currentStep) && (stepNarrations[currentStep] != null))
            audioSource.PlayOneShot(stepNarrations[currentStep], stepNarrationsVolumeScale);
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

