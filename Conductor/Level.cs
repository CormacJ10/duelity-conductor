using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public Track[] tracks;
    public HitSheet hitSheet;

    // Game info (unimplemented)
    // public Difficulty difficulty;
    // public Theme theme;

    Hit[][] hitArrays; // compact Hit array, doesn't have empty spaces. Also can't serialise multi or jagged arrays by default
    int numKeys;
    int[] curHitIndex;
    float[] nextHitTimes;
    float trackStartTime = 999;
    float secPerBeat = 999;
    UnityEvent<int> missedCallback; // int represents key missed
    UnityEvent<int> okCallback;
    UnityEvent<int> greatCallback;

    //////////// Main public functions

    public void LevelUpdate()
    {
        for (int i = 0; i < numKeys; i++) {
            // print($"key[{i}]: {(float)AudioSettings.dspTime} - {trackStartTime} > {nextHitTimes[i]}");
            if ((float)AudioSettings.dspTime - trackStartTime > nextHitTimes[i]) {
                // print("hit missed");
                Hit.ResetCombo();
                if (missedCallback != null) missedCallback.Invoke(i);

                UpdateNextHit(i);
            }
        }
    }

    // Initialise callback and track
    public UnityEvent<int>[] Init(float trackStartTime, int numKeys)
    {
        if (numKeys > ((List<List<Hit>>)hitSheet).Count) {
            print($"More input keys [{numKeys}] than given HitSheet arrays [{((List<List<Hit>>)hitSheet).Count}]");
            throw new UnityException("Check the HitSheet -> Level -> Conductor setup");
        }

        hitArrays = new Hit[numKeys][]; // jagged arrays are faster than multi-dimensional arrays
        secPerBeat = 60f / (float)tracks[0].bpm;
        ResetLevel(trackStartTime, numKeys);

        missedCallback = new UnityEvent<int>();
        okCallback = new UnityEvent<int>();
        greatCallback = new UnityEvent<int>();
        UnityEvent<int>[] events = new UnityEvent<int>[3];
        events[0] = missedCallback;
        events[1] = okCallback;
        events[2] = greatCallback;

        // StartCoroutine(DebugCor());
        return events;
    }

    public void ResetLevel(float trackStartTime, int numKeys)
    {
        this.trackStartTime = trackStartTime + tracks[0].startOffset;
        this.numKeys = numKeys;

        curHitIndex = new int[numKeys];
        nextHitTimes = new float[numKeys];
        
        for (int i = 0; i < numKeys; i++) {
            Hit[] tempRef = ((List<List<Hit>>)hitSheet)[i].ToArray(); // need to copy array values
            hitArrays[i] = new Hit[tempRef.Length];
            tempRef.CopyTo(hitArrays[i], 0);
            
            Hit nextHit = hitArrays[i][0]; // NOTE Mostly same as UpdateNextHit()
            nextHitTimes[i] = nextHit.GetEndPos() * secPerBeat;
        }
    }

    // Find result of keypress(es) against level. Assumes beatPos only moves forward
    public Hit.Result? OnInput(float beatPos, int[] arrayIndices)
    {
        // Check against relevant Hits
        List<Hit.Result> results = new List<Hit.Result>();
        for (int i = 0; i < arrayIndices.Length; i++) {
            int key = arrayIndices[i];
            Hit curHit = hitArrays[key][curHitIndex[key]];

            if (beatPos > curHit.GetEndPos()) { // skipped beat(s)
                print("skipped hit(s)");
                Hit.ResetCombo();
                UpdateCurHitIndex(beatPos);

                if (curHitIndex[key] < hitArrays[key].Length) {
                    curHit = hitArrays[key][curHitIndex[key]];
                } else continue;
            }
            // print($"beatPos: {beatPos}, curHit: {curHit.ToString()}");
            if (beatPos > curHit.GetStartPos() && beatPos < curHit.GetEndPos()) {
                Hit.Result r = curHit.OnHit(beatPos);
                if (r.result == 1) okCallback.Invoke(key);
                else if (r.result == 10) greatCallback.Invoke(key);

                results.Add(r);
                UpdateNextHit(key);
            }
        }

        return TallyResults(results);
    }

    public void GetSpawnList(out List<(int,Hit)> spawnList)
    {
        spawnList = new List<(int, Hit)>();

        for (int i = 0; i < numKeys; i++) {
            for (int j = 0; j < hitArrays[i].Length; j++) {
                spawnList.Add((i, hitArrays[i][j]));
            }
        }

        spawnList.Sort(delegate ((int, Hit) a, (int, Hit) b)
        {
            if (a.Item2.beatPoses.x != b.Item2.beatPoses.x) {
                return a.Item2.beatPoses.x.CompareTo(b.Item2.beatPoses.x);
            } else return a.Item1.CompareTo(b.Item1);
        });
    }

    //////////// Helpers

    void UpdateNextHit(int key)
    {
        if (curHitIndex[key] + 1 >= hitArrays[key].Length) {
            // print($"Track key {key} finished");
            return;
        }

        Hit nextHit = hitArrays[key][curHitIndex[key] + 1];
        nextHitTimes[key] = nextHit.GetEndPos() * secPerBeat;
        // print($"next hit end for key[{key}]: {nextHitTimes[key]}");
        curHitIndex[key]++;
    }

    void UpdateCurHitIndex(float beatPos)
    {
        Hit nextHit;
        for (int i = 0; i < numKeys; i++) {
            for (int j = curHitIndex[i]; j < hitArrays[i].Length; j++) {
                nextHit = hitArrays[i][j];
                // print($"hitArray[{i}][{j}] < {nextHit.GetStartPos()}? {beatPos < nextHit.GetStartPos()}");
                if (beatPos < nextHit.GetEndPos()) {
                    nextHitTimes[i] = nextHit.GetEndPos() * secPerBeat;
                    curHitIndex[i] = j;
                    // print($"new curHitIndex[{i}]: {j}");
                    break;
                }

                // No more beats for key
                nextHitTimes[i] = 999;
                curHitIndex[i] = hitArrays[i].Length;
            }
        }
    }

    Hit.Result? TallyResults(List<Hit.Result> results)
    {
        if (results.Count == 0) return null;

        // Tally up results over all keys
        int resultSum = 0;
        for (int i = 0; i < results.Count; i++) resultSum += results[i].result;

        Hit.Result overallResult = new Hit.Result();
        overallResult.result = resultSum;
        overallResult.curCombo = Hit.comboCounter;
        return overallResult;
    }

    IEnumerator DebugCor()
    {
        yield return new WaitForSeconds(4);
        missedCallback.Invoke(1);
        okCallback.Invoke(1);
        greatCallback.Invoke(1);
    }
}
