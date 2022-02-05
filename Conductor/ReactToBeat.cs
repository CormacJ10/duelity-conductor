using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToBeat : MonoBehaviour
{
    public bool isPrefab;
    public float beatNum = 1; // beats between react
    public float beatEnlargeInterval = 0.2f; // total enlarge duration as fraction of beat
    public float beatEnlargeMulti = 1.25f; // notes enlarge on beat to help player
    Conductor conductor;
    Transform tf;
    Vector2 startSize;
    bool isStarted;
    float nextBeat;

    // Start is called before the first frame update
    void Start()
    {
        tf = transform;
        startSize = tf.localScale;
        conductor = GameObject.Find("Conductor").GetComponent<Conductor>(); // NOTE hard-coded GO find, to allow prefab usage
        if (isPrefab) Init();
        else conductor.SubInit(Init);
    }

    void Update() {
        if (!isStarted) return;

        if (conductor.beatPos > nextBeat) {
            nextBeat += beatNum;
            StartCoroutine(Expand());
        }
    }

    void Init()
    {
        isStarted = true;
        nextBeat = beatNum;
    }

    IEnumerator Expand()
    {
        float startBeat = conductor.beatPos;
        
        while (conductor.beatPos < startBeat + beatEnlargeInterval) {
            transform.localScale = Vector2.Lerp(startSize * beatEnlargeMulti, startSize, (conductor.beatPos - startBeat) / beatEnlargeInterval);
            yield return null;
        }
    }
}
