using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Health : MonoBehaviour
{
    public Conductor conductor;
    public Score playerScore;
    public Score enemyScore;
    public float recentWindow = 3;
    Slider slider;
    bool isStarted;
    float currentScoreDiff;
    float recentScoreDiff;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        conductor.SubInit(Init);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted) {
            currentScoreDiff = playerScore.score - enemyScore.score;
            float current = slider.value;
            float goal = Mathf.Clamp01(playerScore.score / (playerScore.score + enemyScore.score + 0.001f)) + 0.5f;
            float v = goal - current;
            slider.value = Mathf.SmoothDamp(current, goal, ref v, 1);
        }
    }

    void Init()
    {
        isStarted = true;
        StartCoroutine(RecentScore());
    }

    IEnumerator RecentScore()
    {
        yield return new WaitForSeconds(recentWindow);
        while (true) {
            recentScoreDiff = playerScore.score - enemyScore.score;
            yield return null;
        }
    }
}
