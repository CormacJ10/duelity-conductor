using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Score : MonoBehaviour
{
    public Conductor conductor;
    public int okPts;
    public int greatPts;
    public bool swapX;
    public int score;
    Text textComponent;
    bool isStarted;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<Text>();
        conductor.SubInit(Init);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted) {
            if (swapX) textComponent.text = $"{score} SCORE\n{Hit.comboCounter} COMBO";
            else textComponent.text = $"SCORE {score}\nCOMBO {Hit.comboCounter}";
        }
    }

    void Init()
    {
        isStarted = true;
        conductor.SubToResults(null, OK, Great);
    }

    void OK(int i)
    {
        score += okPts;
    }

    void Great(int i)
    {
        score += greatPts;
        print("GREAT");
    }
}
