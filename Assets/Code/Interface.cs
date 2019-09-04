using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Interface : MonoBehaviour
{

    #region Singleton

    public static Interface instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than instance of Interface found!");
            return;
        }
        instance = this;
    }

    #endregion

    public GameObject player;

    public Text white_cubes;
    public Text colored_cubes;
    public Text all_cubes;

    public Text spawn;

    public GameObject win_panel;
    public GameObject lose_panel;

    Spawner spawner;

    public bool game_done = false;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.Find("GameManager").GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameWin()
    {
        win_panel.SetActive(true);
        game_done = true;
        spawner.Stop();
    }

    public void GameLose()
    {
        lose_panel.SetActive(true);
        game_done = true;
        spawner.Stop();
    }

    public void SetStates(int color_count, int all_count)
    {
        int white_color = all_count - color_count;

        white_cubes.text = " White cube - " + white_color;
        white_cubes.GetComponent<Text>().text = " White cube - " + white_color;
        colored_cubes.GetComponent<Text>().text = " Color cube - " + color_count;
        all_cubes.GetComponent<Text>().text = " All cube - " + all_count;
    }

    public void SpawnInfo(int all_enemy)
    {
        spawn.GetComponent<Text>().text = "Destroy "+ all_enemy + " enemies!";
    }

    public void DoneSpawnInfo(Spawner.Wave wave)
    {
        spawn.GetComponent<Text>().text = "You destroyed all the enemies! Get ready, there is a new wave! " + wave.name;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                            Application.Quit();
        #endif
    }

}
