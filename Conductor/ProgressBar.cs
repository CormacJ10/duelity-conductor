using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressBar : MonoBehaviour
{
    public Conductor conductor;
    Slider slider;
    bool isStarted;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        conductor.SubInit(Init);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted) slider.value = conductor.pcPos;
    }

    void Init()
    {
        isStarted = true;
    }
}
