using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManagerScript : MonoBehaviour
{

    [SerializeField]
    private GameObject[] catPoses;

    [SerializeField]
    private GameObject[] dogPoses;
    public KeyCode[] keys;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animateChar();
    }

    void animateChar(){
        if(Input.GetKey("up")){
            catPoses[0].SetActive(false);
            catPoses[2].SetActive(false);
            catPoses[3].SetActive(false);
            catPoses[4].SetActive(false);
            catPoses[1].SetActive(true);

            dogPoses[0].SetActive(false);
            dogPoses[1].SetActive(false);
            dogPoses[2].SetActive(false);
            dogPoses[4].SetActive(false);
            dogPoses[3].SetActive(true);
        }

        if(Input.GetKey("down")){
            catPoses[0].SetActive(false);
            catPoses[1].SetActive(false);
            catPoses[3].SetActive(false);
            catPoses[4].SetActive(false);
            catPoses[2].SetActive(true);

            dogPoses[0].SetActive(false);
            dogPoses[1].SetActive(false);
            dogPoses[3].SetActive(false);
            dogPoses[4].SetActive(false);
            dogPoses[2].SetActive(true);
        }

        if(Input.GetKey("left")){
            catPoses[0].SetActive(false);
            catPoses[1].SetActive(false);
            catPoses[2].SetActive(false);
            catPoses[4].SetActive(false);
            catPoses[3].SetActive(true);

            dogPoses[0].SetActive(false);
            dogPoses[1].SetActive(false);
            dogPoses[2].SetActive(false);
            dogPoses[3].SetActive(false);
            dogPoses[4].SetActive(true);
        }

        if(Input.GetKey("right")){
            catPoses[0].SetActive(false);
            catPoses[1].SetActive(false);
            catPoses[2].SetActive(false);
            catPoses[3].SetActive(false);
            catPoses[4].SetActive(true);

            dogPoses[0].SetActive(false);
            dogPoses[2].SetActive(false);
            dogPoses[3].SetActive(false);
            dogPoses[4].SetActive(false);
            dogPoses[1].SetActive(true);
        }
    }

}
