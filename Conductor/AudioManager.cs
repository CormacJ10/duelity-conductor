using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public Conductor conductor;
    public AudioClip[] missClips;
    public AudioClip[] okClips;
    public AudioClip[] greatClips;
    AudioSource src;

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        conductor.SubInit(Init);
    }

    void Init()
    {
        conductor.SubToResults(Miss, OK, Great);
    }

    void Miss(int i)
    {
        PlayRandom(missClips);
    }

    void OK(int i)
    {
        PlayRandom(okClips);
    }

    void Great(int i)
    {
        PlayRandom(greatClips);
    }

    void PlayRandom(AudioClip[] clips)
    {
        if (clips.Length != 0) src.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
