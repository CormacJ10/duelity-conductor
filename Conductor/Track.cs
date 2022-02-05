using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "Track", order = 51)]
public class Track : ScriptableObject
{
    public enum TrackType {
        Main,
        Instrumental
    }

    // Track info
    public AudioClip audioClip;
    public TrackType type;
    public int bpm = 60; // NOTE if bpm varies, rename to bpmMultiple
    public float startOffset;
    public float durationInSec = 60;
    public float durationInBeats; // unused, purely informational
}
