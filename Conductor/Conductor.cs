using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))] // NOTE make sure there are enough AudioSources on GO
public class Conductor : MonoBehaviour
{
    public enum ConductorState { // only waiting+play implemented
        Waiting,
        Play,
        Pause,
        Resume
    }

    public Level level;
    public KeyCode[] keys; 
    public AudioClip missedClip; 
    public float secPerBeat;
    public float timePos;
    public float beatPos;
    public float pcPos; // % finished
    public bool isDebug;

    static Conductor instance;
    AudioSource[] musicSources; // NOTE make sure all AudioSources untick 'Play on Awake'
    UnityEvent startEvent;
    UnityEvent<int>[] resultEvents;
    float dspStartTime;
    ConductorState state = ConductorState.Waiting;
    Track.TrackType curTrackType = Track.TrackType.Main;
    Coroutine missedCor;

    // Singleton
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        startEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        // level = Instantiate(level.gameObject).GetComponent<Level>();
        secPerBeat = 60f / (float)level.tracks[0].bpm;
        musicSources = GetComponents<AudioSource>();

        if (isDebug) {
            StartCoroutine(DebugCor(Track.TrackType.Instrumental));
        }

        Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == ConductorState.Play) {
            level.LevelUpdate();

            timePos = (float)AudioSettings.dspTime - dspStartTime - level.tracks[0].startOffset;
            beatPos = timePos / secPerBeat;
            pcPos = timePos / level.tracks[0].durationInSec; // doesn't take into account offset

            List<int> keysPressed = new List<int>();
            for (int i = 0; i < keys.Length; i++) {
                if (Input.GetKeyDown(keys[i])) {
                    keysPressed.Add(i);
                }
            }
            
            Hit.Result? r = null;
            if (keysPressed.Count > 0) {
                // print($"{keys.Length} keys pressed!");
                r = level.OnInput(beatPos, keysPressed.ToArray());
            }

            // if (isDebug && r != null) print($"Result sum: {r.Value.result}, curCombo: {r.Value.curCombo}");

            if (pcPos > 1) SwitchState(ConductorState.Waiting);
        }
    }

    public void Play()
    {
        dspStartTime = (float)AudioSettings.dspTime;
        for (int i = 0; i < musicSources.Length; i++) {
            musicSources[i].clip = level.tracks[i].audioClip;
            if (i == (int)curTrackType) musicSources[i].mute = false;
            else musicSources[i].mute = true;
            musicSources[i].Play();
        }

        state = ConductorState.Play;
        resultEvents = level.Init(dspStartTime, keys.Length);
        SubToResults(MissedShiftTrack, null, null);
        startEvent.Invoke();
    }

    public void SwitchTrackType(Track.TrackType type)
    {
        if (curTrackType != type) curTrackType = type;
        else return;

        for (int i = 0; i < musicSources.Length; i++) {
            if (i == (int)curTrackType) musicSources[i].mute = false;
            else musicSources[i].mute = true;
        }
    }

    public void SwitchState(ConductorState nextState)
    {
        if (nextState == ConductorState.Waiting) {
            timePos = beatPos = pcPos = 0;
            state = ConductorState.Waiting;
        }
    }

    //////////// Callbacks

    public void SubInit(UnityAction func)
    {
        startEvent.AddListener(func);
    }

    public void SubToResults(UnityAction<int> missedCallback, UnityAction<int> okCallback, UnityAction<int> greatCallback)
    {
        if (missedCallback != null) resultEvents[0].AddListener(missedCallback);
        if (okCallback != null) resultEvents[1].AddListener(okCallback);
        if (greatCallback != null) resultEvents[2].AddListener(greatCallback);
        // for (int i=0;i<3;i++) events[i].Invoke(3);
    }

    public void UnsubToResults(UnityAction<int> missedCallback, UnityAction<int> okCallback, UnityAction<int> greatCallback)
    {
        if (missedCallback != null) resultEvents[0].RemoveListener(missedCallback);
        if (okCallback != null) resultEvents[1].RemoveListener(okCallback);
        if (greatCallback != null) resultEvents[2].RemoveListener(greatCallback);
    }

    ///////////// Helpers

    void MissedShiftTrack(int i)
    {
        if (missedCor == null) missedCor = StartCoroutine(MissedCor());
    }

    IEnumerator MissedCor()
    {
        musicSources[0].PlayOneShot(missedClip);
        musicSources[1].PlayOneShot(missedClip);
        SwitchTrackType(Track.TrackType.Instrumental);
        yield return new WaitForSeconds(3);
        SwitchTrackType(Track.TrackType.Main);
        missedCor = null;
    }

    IEnumerator DebugCor(Track.TrackType trackType)
    {
        yield return new WaitForSeconds(1);
        Play();
        yield return new WaitForSeconds(4);
        SwitchTrackType(trackType);
    }
}
