using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    [SerializeField]
    public AudioSource audi;

    [SerializeField]
    public AudioClip menuClick;

    public void sceneChange(string sceneName){
        audi.PlayOneShot(menuClick);
        SceneManager.LoadScene(sceneName);
    }

    public void quitGame(){
        Application.Quit();
    }
}
