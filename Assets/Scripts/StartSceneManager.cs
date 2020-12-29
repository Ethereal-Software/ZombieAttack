using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject InfoPanel;
    public GameObject LoadingPanel;
    public GameObject JumpPanel;
    public GameObject GameMenuPanel;

    public GameObject waveManagerObject;
    public GameObject playerObject;
    public GameObject dontDestroyTarget;
    // Start is called before the first frame update
    void Start()
    {
        playerObject.SetActive(false);
        waveManagerObject.SetActive(false);
        GameMenuPanel.SetActive(false);

        GotoStartPanel();
    }

    public void GotoStartPanel() {
        StartPanel.SetActive(true);
        InfoPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        JumpPanel.SetActive(false);
    }

    public void GotoJumpPanel()
    {
        StartPanel.SetActive(false);
        InfoPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        JumpPanel.SetActive(true);
    }
    public void GotoInfoPanel() {
        Debug.Log("Got click");
        StartPanel.SetActive(false);
        JumpPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        InfoPanel.SetActive(true);
        
    }    
    public void GotoGame() {
        JumpPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        waveManagerObject.SetActive(false);

        StartCoroutine("LoadSceneAsync");
        
    }

    public void SetWave(int waveNumber) {
        var wavMan = waveManagerObject.GetComponent<WaveManager>();
        wavMan.ClearZombies();
        wavMan.startAtWave = waveNumber;
        GotoInfoPanel();
    }

    public void ExitGameToMainMenu() {
        
        playerObject.SetActive(false);
        GameMenuPanel.SetActive(false);

        var wavMan = waveManagerObject.GetComponent<WaveManager>();
        wavMan.ClearZombies();

        waveManagerObject.SetActive(true);


        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(dontDestroyTarget);

        SceneManager.LoadSceneAsync("StartScene_fix");
        

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //do stuff
        //StartPanel.SetActive(false);
        //JumpPanel.SetActive(false);
        //LoadingPanel.SetActive(false);
        //InfoPanel.SetActive(false);

        if (scene.name == "LevelNight_preload")
        {
            playerObject.SetActive(true);
            waveManagerObject.SetActive(true);

        } else if (scene.name == "LevelNight_fix") {
            //Destroy(this.gameObject);
        }


    }


    IEnumerator LoadSceneAsync()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        SceneManager.sceneLoaded += OnSceneLoaded;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelNight_preload");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


}
