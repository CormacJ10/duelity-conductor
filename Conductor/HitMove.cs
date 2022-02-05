using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMove : MonoBehaviour
{
    Conductor conductor;
    Vector2 startPos;
    Vector2 path;
    float startBeat;
    float beatsToLive;


    public void Init(float beatsToLive, Vector2 path, float nextHitDiff, Conductor conductor)
    {
        this.conductor = conductor;
        this.path = path;
        this.beatsToLive = beatsToLive;
        startPos = transform.position;
        startBeat = conductor.beatPos;

        // if hit was supposed to be spawned earlier
        if (nextHitDiff < 0) {
            startBeat += nextHitDiff;
            beatsToLive += nextHitDiff; // NOTE doesn't affect field beatsToLive
        }

        Destroy(gameObject, beatsToLive * conductor.secPerBeat);
    }

    void Update()
    {
        transform.position = Vector2.Lerp(startPos, startPos + path, (conductor.beatPos - startBeat) / beatsToLive);
    }
}
