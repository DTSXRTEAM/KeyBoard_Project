using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Button_Manager : MonoBehaviour
{
    public GameObject[] Keys; // Array of key objects
    public AudioClip[] KeyTones; // Corresponding audio clips
    public float fadeOutDuration = 0.5f; // Duration of fade-out effect

    void Start()
    {
        AttachComponentsToObjects();
    }

    void AttachComponentsToObjects()
    {
        if (Keys.Length == 0)
        {
            Debug.LogWarning("No keys assigned to add components.");
            return;
        }

        if (Keys.Length != KeyTones.Length)
        {
            Debug.LogError("The number of keys and audio clips do not match. Please make sure they are the same length.");
            return;
        }

        for (int i = 0; i < Keys.Length; i++)
        {
            GameObject Key = Keys[i];

            if (Key == null) continue;

            // Add MeshCollider if not already attached
            MeshCollider meshCollider = Key.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = Key.AddComponent<MeshCollider>();
                meshCollider.convex = true; // Required for interactions
                Debug.Log($"MeshCollider added to {Key.name}");
            }

            // Add or get AudioSource
            AudioSource KeyToneSpeaker = Key.GetComponent<AudioSource>();
            if (KeyToneSpeaker == null)
            {
                KeyToneSpeaker = Key.AddComponent<AudioSource>();
                KeyToneSpeaker.playOnAwake = false;
            }

            // Assign the correct audio clip
            if (KeyTones[i] != null)
            {
                KeyToneSpeaker.clip = KeyTones[i];
                Debug.Log($"AudioSource assigned to {Key.name} with clip: {KeyTones[i].name}");
            }

            // Add XRSimpleInteractable if not already attached
            XRSimpleInteractable interactable = Key.GetComponent<XRSimpleInteractable>();
            if (interactable == null)
            {
                interactable = Key.AddComponent<XRSimpleInteractable>();
                Debug.Log($"XRSimpleInteractable added to {Key.name}");
            }

            // Capture index for delegate
            int index = i;
            interactable.firstHoverEntered.AddListener((_) => PlayTone(index));
            interactable.lastHoverExited.AddListener((_) => StartCoroutine(FadeOutTone(index)));
        }
    }

    void PlayTone(int index)
    {
        if (Keys[index] != null)
        {
            AudioSource audioSource = Keys[index].GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.volume = 1f; // Ensure full volume on play
                audioSource.Play();
            }
        }
    }

    IEnumerator FadeOutTone(int index)
    {
        if (Keys[index] != null)
        {
            AudioSource audioSource = Keys[index].GetComponent<AudioSource>();
            if (audioSource != null)
            {
                float startVolume = audioSource.volume;

                // Gradually decrease volume over fadeOutDuration
                for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
                {
                    audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
                    yield return null;
                }

                // Ensure it's fully silent and stop playback
                audioSource.volume = 0;
                audioSource.Stop();
                audioSource.volume = 1f; // Reset volume for next play
            }
        }
    }
}
