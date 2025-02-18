using UnityEngine;

public class Button_Manager : MonoBehaviour
{
   
    public GameObject[] cubes; // Array of cubes (or objects) to attach components

    
    public AudioClip[] audioClips; // Array of audio clips

    void Start()
    {
        AttachComponentsToObjects();
    }

    void AttachComponentsToObjects()
    {
        if (cubes.Length == 0)
        {
            Debug.LogWarning("No cubes assigned to add components.");
            return;
        }

        if (cubes.Length != audioClips.Length)
        {
            Debug.LogError("The number of cubes and audio clips do not match. Please make sure they are the same length.");
            return;
        }

        for (int i = 0; i < cubes.Length; i++)
        {
            GameObject cube = cubes[i];

            if (cube == null) continue;

            // Add BoxCollider if not already attached
            if (cube.GetComponent<BoxCollider>() == null)
            {
                cube.AddComponent<BoxCollider>();
                Debug.Log($"BoxCollider added to {cube.name}");
            }

            // Automatically add an AudioSource component if not already attached
            AudioSource audioSource = cube.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = cube.AddComponent<AudioSource>();
                audioSource.playOnAwake = false; // Prevent automatic playback

                if (audioClips[i] != null)
                {
                    audioSource.clip = audioClips[i];
                    Debug.Log($"AudioSource added to {cube.name} with clip: {audioClips[i].name}");
                }
                else
                {
                    Debug.LogWarning($"No audio clip assigned for {cube.name}. No sound will play.");
                }
            }
            else
            {
                Debug.Log($"AudioSource already exists on {cube.name}");
            }
        }
    }
}
