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
        for (int i = 0; i < Keys.Length; i++)
        {
            // Duplicate the key (Instantiate creates a new copy)
            GameObject Visual = Instantiate(Keys[i], Keys[i].transform.position, Keys[i].transform.rotation);
            Visual.name = "Visual";

            // Make it a child of the original key
            Visual.transform.SetParent(Keys[i].transform);
            Visual.transform.localScale = Vector3.one;

            // Remove unwanted components from the duplicate
            if (Visual.TryGetComponent(out BoxCollider boxCollider))
            {
                Destroy(boxCollider);
            }

            if (Keys[i].TryGetComponent(out MeshFilter meshFilter))
            {
                Destroy(meshFilter);
            }

            if (Keys[i].TryGetComponent(out MeshRenderer meshRenderer))
            {
                Destroy(meshRenderer);
            }
        }

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
                Debug.Log($"Audio clip '{KeyTones[i].name}' assigned to {Key.name}");
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

            var vis = Keys[index].gameObject.transform.Find("Visual");

            Vector3 originalPosition = vis.transform.position;  // Save original position of the key
            Vector3 newPosition = originalPosition;
            newPosition.y -= 0.01f; // Move key down slightly
            vis.transform.position = newPosition;

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
            var vis = Keys[index].gameObject.transform.Find("Visual");

            Vector3 originalPosition = vis.transform.position;  // Save original position of the key
            Vector3 newPosition = originalPosition;
            newPosition.y += 0.01f; // Move key down slightly
            vis.transform.position = newPosition;
        }
    }
}
