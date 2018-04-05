using UnityEngine;
using UnityEngine.XR.WSA.Input;

using HoloToolkit.Unity;


public class HandsManager : Singleton<HandsManager>
{
    [Tooltip("Audio clip to play when Finger Pressed.")]
    public AudioClip FingerPressedSound;
    private AudioSource audioSource;

    public bool HandDetected
    {
        get;
        private set;
    }

    public bool FingerDown
    {
        get;
        private set;
    }

    public GameObject FocusedGameObject { get; private set; }

    void Awake()
    {
        //EnableAudioHapticFeedback();

        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;

        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

        FocusedGameObject = null;
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (FingerPressedSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = FingerPressedSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        HandDetected = true;
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        HandDetected = false;
        FingerDown = false;

        if (FocusedGameObject != null) {
            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            FocusedGameObject.GetComponent<Interactible>().Deselect();
        }

        ResetFocusedGameObject();
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs hand)
    {
        FingerDown = true;

        if (InteractibleManager.Instance.FocusedGameObject != null)
        {
            // Play a select sound if we have an audio source and are not targeting an asset with a select sound.
            if (audioSource != null && !audioSource.isPlaying &&
                InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>() != null)
            {
                audioSource.Play();
            }

            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            FocusedGameObject.GetComponent<Interactible>().OnSelect();
        }
    }

    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs hand)
    {
        FingerDown = false;

        if (InteractibleManager.Instance.FocusedGameObject != null) {
            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            FocusedGameObject.GetComponent<Interactible>().OffSelect();
        }
        ResetFocusedGameObject();
    }

    private void ResetFocusedGameObject()
    {
        FocusedGameObject = null;
        GestureManager.Instance.ResetGestureRecognizers();
    }

    void OnDestroy()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
    }
}
