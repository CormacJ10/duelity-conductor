using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHits : MonoBehaviour
{
    public GameObject[] hitGOs;
    public float lookAhead = 5; // in beats
    public Canvas canvas;
    public Transform spawnPosTF;
    public Transform barPosTF;
    public Transform nextArrowTF;
    Conductor conductor;
    Level level;
    List<(int, Hit)> hitList;
    Vector2 notePath;
    float arrowWidth;
    float endBeat;
    int noteToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        conductor = GetComponent<Conductor>();
        conductor.SubInit(Init);
        level = conductor.level;
        notePath = barPosTF.position - spawnPosTF.position;
        arrowWidth = -(nextArrowTF.position - barPosTF.position).magnitude;
    }

    void Init()
    {
        print($"Loading level: {level.gameObject.name}");
        level.GetSpawnList(out hitList);
        endBeat = level.tracks[0].durationInBeats;
        StartCoroutine(SpawnCor());
    }

    IEnumerator SpawnCor()
    {
        while (true) {
            HitUpdate();
            yield return null;
        }
    }

    void HitUpdate()
    {
        while (noteToSpawn < hitList.Count && conductor.beatPos + lookAhead < endBeat) {
            if (hitList[noteToSpawn].Item2.beatPoses.x > conductor.beatPos + lookAhead) break;

            float nextHitDiff = hitList[noteToSpawn].Item2.beatPoses.x - (conductor.beatPos + lookAhead);
            SpawnNote(nextHitDiff);
            noteToSpawn++;
        }
    }

    //////////// Helpers

    void SpawnNote(float nextHitDiff)
    {
        int key = hitList[noteToSpawn].Item1;
        // print($"Spawned note[{hitList[noteToSpawn].Item2}] at {spawnPosTF.position} pathing {notePath} with BTL {lookAhead} and diff: {nextHitDiff:F5}");
        GameObject g = Instantiate(hitGOs[key],
            (Vector2)spawnPosTF.position + Vector2.Perpendicular(notePath.normalized) * key * arrowWidth,
            Quaternion.identity,
            canvas.transform);
        g.GetComponent<HitMove>().Init(lookAhead, notePath, nextHitDiff, conductor);
    }
}
