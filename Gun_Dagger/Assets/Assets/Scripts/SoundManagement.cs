using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    static SoundManagement instance;
    public AudioClip[] audioClips;
    AudioSource audioSource;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        instance = this;
    }

    public static void PlaySound(int soundNum)=>instance._PlaySound(soundNum);

    public void _PlaySound(int soundNum)
    {
        audioSource.clip = audioClips[soundNum];
        audioSource.Play();
    }
}
