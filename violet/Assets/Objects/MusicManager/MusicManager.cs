using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip[] tracks;
    private AudioSource audioSource;

    private float volumeChangeStep = 0.1f;
    private float maxVolume = 1.0f;
    private float minVolume = 0.0f;
    private float previousVolume = 0.5f;
    private float fadeDuration = 1.5f;

    // Reference to the default input actions
    public InputActionReference rightPrimaryButton;
    public InputActionReference rightSecondaryButton;
    public InputActionReference leftSecondaryButton;
    public InputActionReference leftPrimaryButton;

    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.Start)
        {
            StartCoroutine(FadeIn(fadeDuration));
        }
        else if (state == GameState.End)
        {
            previousVolume = audioSource.volume;
            StartCoroutine(FadeOut(fadeDuration));
        }
    }

    IEnumerator FadeIn(float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, previousVolume, currentTime / duration);
            yield return null;
        }
    }

    IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
            yield return null;
        }
    }

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            GameManager.OnGameStateChange += HandleGameStateChange;

            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this object when loading new scenes
            audioSource = GetComponent<AudioSource>();

            // Add listeners to the input actions
            rightPrimaryButton.action.performed += _ => SkipTrack();
            rightSecondaryButton.action.performed += _ => Restart();
            leftSecondaryButton.action.performed += _ => AdjustVolume(volumeChangeStep);
            leftPrimaryButton.action.performed += _ => AdjustVolume(-volumeChangeStep);

            // Enable the input actions
            rightPrimaryButton.action.Enable();
            rightSecondaryButton.action.Enable();
            leftSecondaryButton.action.Enable();
            leftPrimaryButton.action.Enable();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;

        // Disable the input actions and remove listeners
        rightPrimaryButton.action.Disable();
        leftSecondaryButton.action.Disable();
        leftPrimaryButton.action.Disable();

        rightPrimaryButton.action.performed -= _ => SkipTrack();
        leftSecondaryButton.action.performed -= _ => AdjustVolume(volumeChangeStep);
        leftPrimaryButton.action.performed -= _ => AdjustVolume(-volumeChangeStep);
    }

    void Start()
    {
        PlayRandomTrack();
    }

    void Update()
    {
        // Play a new track when the current one ends
        if (!audioSource.isPlaying) PlayRandomTrack();
    }

    void PlayRandomTrack()
    {
        // Select a random track to play
        int randomIndex = Random.Range(0, tracks.Length);
        audioSource.clip = tracks[randomIndex];
        audioSource.Play();
    }

    void Restart()
    {
        GameManager.Instance.UpdateGameState(GameState.End);
    }

    void SkipTrack()
    {
        PlayRandomTrack();
        SendHapticFeedback(XRNode.RightHand, 0.5f, 0.1f);
    }

    void AdjustVolume(float change)
    {
        float previousVolume = audioSource.volume;
        audioSource.volume = Mathf.Clamp(audioSource.volume + change, minVolume, maxVolume);
        
        // Check if the volume was already at max or min and send stronger feedback
        if ((previousVolume == maxVolume && change > 0) || (previousVolume == minVolume && change < 0))
        {
            SendHapticFeedback(XRNode.LeftHand , 1.0f, 0.3f); // Strong haptic feedback
        }
        else
        {
            SendHapticFeedback(XRNode.LeftHand , 0.5f, 0.1f); // Normal haptic feedback
        }
    }

    private void SendHapticFeedback(XRNode node, float intensity, float duration)
    {
        var hapticDevice = InputDevices.GetDeviceAtXRNode(node);
        if (hapticDevice.isValid)
        {
            hapticDevice.SendHapticImpulse(0, intensity, duration);
        }
    }
}
