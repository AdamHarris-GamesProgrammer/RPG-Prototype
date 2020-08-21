using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioModder : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] float pitchMin = 0.1f;
    [SerializeField] float pitchMax = 0.1f;

    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }


    public void Play()
    {
        source.pitch = 1.0f;

        int index = Random.Range(0, audioClips.Count);

        source.clip = audioClips[index];

        float pitchBeforeMod = source.pitch;

        source.pitch = Random.Range(pitchBeforeMod - pitchMin, pitchBeforeMod + pitchMax);

        Debug.Log("Pitch: " + source.pitch);

        source.Play();
    }
}
