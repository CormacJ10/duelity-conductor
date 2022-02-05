using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hit
{
    [System.Serializable]
    public enum HitType {
        Tap,
        Hold // unimplemented
    }

    [System.Serializable]
    public struct Result {
        public int result; // 10 = Great, 1 = OK, 0 = miss
        public int curCombo;
        public override string ToString() { return $"Result[{result},{curCombo}]"; }
    }

    public HitType hitType = HitType.Tap;
    public Vector2 beatPoses = Vector2.zero; // x = start, y = end if hold type (ignore if tap type)
    public static int comboCounter = 0;
    Vector2 timingIntervals = new Vector2(0.3f, 0.5f);  // NOTE edit hit timings (units = beats) here (x = Great, y = OK, else ignore/miss). Total interval with beatPoses as midpoints

    public Hit() : this(Vector2.zero) {} // constructor chaining

    public Hit(Vector2 poses)
    {
        beatPoses = poses;

        switch (hitType) {
            case HitType.Tap:
            default:
                timingIntervals = new Vector2(0.3f, 0.5f); // NOTE edit hit timings (units = beats) here
                break;
        }
    }

    public override string ToString()
    {
        return $"Hit[{hitType.ToString()},{beatPoses},{timingIntervals}]";
    }


    public Result OnHit(float beatPosHitTime, Vector2? customInterval = null)
    {
        float great = (customInterval == null ? timingIntervals.x : customInterval.Value.x) / 2;
        float ok = (customInterval == null ? timingIntervals.y : customInterval.Value.y) / 2;
        float diff = beatPoses.x - beatPosHitTime;

        Result r = new Result();
        if (Mathf.Abs(diff) < great) {
            r.result = 10;
            System.Threading.Interlocked.Increment(ref comboCounter);
        } else if (Mathf.Abs(diff) < ok) {
            r.result = 1;
            System.Threading.Interlocked.Increment(ref comboCounter);
        } else { // should never happen since misses accounted for in Level class
            r.result = 0;
            comboCounter = 0;
            Debug.Log("Hit class had a missed OnHit somehow!");
        }
        
        r.curCombo = comboCounter;
        return r;
    }

    public static void ResetCombo()
    {
        comboCounter = 0;
    }

    public float GetStartPos()
    {
        return beatPoses.x - timingIntervals.y / 2;
    }

    public float GetEndPos()
    {
        return beatPoses.x + timingIntervals.y / 2;
    }
}
