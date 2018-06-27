using UnityEngine;


public class SphereSounds : MonoBehaviour
{
    [Tooltip("The number of frames without a collision to be considered falling")]
    public int FramesToFall; // EZ

    AudioSource impactAudioSource = null;
    AudioSource rollingAudioSource = null;
    AudioSource fallingAudioSource = null; // EZ

    bool rolling = false;
    bool falling = false; // EZ
    bool frozen = true; // EZ
    int frameFallStarted = -1; // EZ

    void Start()
    {
        // Add an AudioSource component and set up some defaults
        impactAudioSource = gameObject.AddComponent<AudioSource>();
        impactAudioSource.playOnAwake = false;
        impactAudioSource.spatialize = true;
        impactAudioSource.spatialBlend = 1.0f;
        impactAudioSource.dopplerLevel = 0.0f;
        impactAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        impactAudioSource.maxDistance = 20f;

        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.playOnAwake = false;
        rollingAudioSource.spatialize = true;
        rollingAudioSource.spatialBlend = 1.0f;
        rollingAudioSource.dopplerLevel = 0.0f;
        rollingAudioSource.volume = 0.33f;
        rollingAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        rollingAudioSource.maxDistance = 20f;
        rollingAudioSource.loop = true;

        // EZ - added falling sound
        fallingAudioSource = gameObject.AddComponent<AudioSource>();
        fallingAudioSource.playOnAwake = false;
        fallingAudioSource.spatialize = true;
        fallingAudioSource.spatialBlend = 1.0f;
        fallingAudioSource.dopplerLevel = 0.5f;
        fallingAudioSource.volume = 0.66f;
        fallingAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        fallingAudioSource.maxDistance = 20f;
        fallingAudioSource.loop = false;

        // Load the Sphere sounds from the Resources folder
        impactAudioSource.clip = Resources.Load<AudioClip>("Impact");
        rollingAudioSource.clip = Resources.Load<AudioClip>("Rolling");
        fallingAudioSource.clip = Resources.Load<AudioClip>("Falling");
    }

    // Occurs when this object starts colliding with another object
    void OnCollisionEnter(Collision collision)
    {
        // EZ - stop the falling sound (in case it was playing)
        falling = false;
        frozen = false;
        fallingAudioSource.Stop();

        // Play an impact sound if the sphere impacts strongly enough.
        if (collision.relativeVelocity.magnitude >= 0.1f)
        {
            impactAudioSource.Play();
        }
    }

    // Occurs each frame that this object continues to collide with another object
    void OnCollisionStay(Collision collision)
    {
        falling = false;
        frozen = false;

        Rigidbody rigid = gameObject.GetComponent<Rigidbody>();

        // if rigid != null

        // Play a rolling sound if the sphere is rolling fast enough.
        if (!rolling && rigid.velocity.magnitude >= 0.01f)
        {
            rolling = true;
            rollingAudioSource.Play();
        }
        // Stop the rolling sound if rolling slows down.
        else if (rolling && rigid.velocity.magnitude < 0.01f)
        {
            rolling = false;
            rollingAudioSource.Stop();
        }
    }

    // Occurs when this object stops colliding with another object
    void OnCollisionExit(Collision collision)
    {
        // Stop the rolling sound if the object falls off and stops colliding.
        // EZ - always immediately stop all other sounds and stop rolling
        // if (rolling)
        // {
        rolling = false;
        impactAudioSource.Stop();
        rollingAudioSource.Stop();
        //}

        // EZ - Started falling or bounced
        if (!frozen)
        {
            falling = true;
            frameFallStarted = Time.frameCount;
        }
    }

    private void Update()
    {
        // EZ - we only want to play the falling sound on a long fall, not on a short bounce
        if (falling && !frozen)
            if ((Time.frameCount - frameFallStarted) >= FramesToFall)
            {
                // EZ - Play a short falling sound
                fallingAudioSource.Play();
                falling = false; // One sound per fall
            }
    }

    // EZ - TBD: will this get called after the other script on this objext has already responded to OnFreeze?
    // TBD: Maybe add freeze sound
    void OnFreeze()
    {
        frozen = true;
        rolling = false;
        falling = false;
        impactAudioSource.Stop();
        rollingAudioSource.Stop();
        fallingAudioSource.Stop(); 
    }

    // EZ - TBD: will this get called after the other script on this objext has already responded to OnFreeze?
    void OnReset()
    {
        OnFreeze();
    }
}