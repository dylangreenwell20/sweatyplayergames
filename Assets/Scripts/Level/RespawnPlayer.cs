using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class RespawnPlayer : MonoBehaviour
{
    public GameObject respawnPoint;
    public GameObject respawnPoint2;
    public GameObject respawnPoint3;
    public GameObject player;
    public GameObject deadMenu;
    public GameObject playerCamera;
    public GameObject effectHolder;
    public SpawnEnemy enemyScript;

    PostProcessVolume volume;
    Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        volume = effectHolder.GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings<Vignette>(out vignette);

        if (!vignette)
        {
            Debug.Log("error, vignette empty");
        }

        else
        {
            vignette.enabled.Override(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("DeadZone"))
        {
            Debug.Log("Player enter");
            player.transform.position = respawnPoint.transform.position;
            enemyScript.spawnEnemy1();
        }

        if (col.gameObject.CompareTag("DeadZone2"))
        {
            Debug.Log("Player enter");
            player.transform.position = respawnPoint2.transform.position;
            enemyScript.spawnEnemy2();
        }

        if (col.gameObject.CompareTag("DeadZone3"))
        {
            Debug.Log("Player enter");
            player.transform.position = respawnPoint3.transform.position;
            enemyScript.spawnEnemy3();
        }

        if (col.gameObject.CompareTag("ToLevel2"))
        {
            Debug.Log("Player enter");
            player.transform.position = respawnPoint2.transform.position;
        }

        if (col.gameObject.CompareTag("ToLevel3"))
        {
            Debug.Log("Player enter");
            player.transform.position = respawnPoint3.transform.position;
        }
    }

    public void DeathMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //vignette.enabled.Override(true);
        //PostProcessVolume ppVolume = playerCamera.GetComponent<PostProcessVolume>();
        //ppVolume.enabled = !ppVolume.enabled;
        deadMenu.SetActive(true);
    }

    public void Respawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //vignette.enabled.Override(false);
        //PostProcessVolume ppVolume = playerCamera.GetComponent<PostProcessVolume>();
        //ppVolume.enabled = !ppVolume.enabled;
        //player.transform.position = respawnPoint.transform.position;
        deadMenu.SetActive(false);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
