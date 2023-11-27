using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class PauseMenu : MonoBehaviour
{
    public bool gamePausing = false;

    public GameObject pauseMenuUI;

    public GameObject playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PostProcessVolume ppVolume = playerCamera.GetComponent<PostProcessVolume>();
            ppVolume.enabled = !ppVolume.enabled;

            if(gamePausing)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        gamePausing = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePausing = true;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
