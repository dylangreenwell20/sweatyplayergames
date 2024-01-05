using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyObj;

    //public GameObject spawnBox1;
    //public GameObject spawnBox2;
    //public GameObject spawnBox3;
    //public GameObject spawnBox4;
    //public GameObject spawnBox5;

    public GameObject[] spawnBoxArea1;
    public GameObject[] spawnBoxArea2;
    public GameObject[] spawnBoxArea3;
    public GameObject[] spawnBoxArea4;
    public GameObject[] spawnBoxArea5;

    public EnemyAI[] enemyArea1;
    public EnemyAI[] enemyArea2;
    public EnemyAI[] enemyArea3;
    public EnemyAI[] enemyArea4;
    public EnemyAI[] enemyArea5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void SpawnEnemy1()
    {
        Debug.Log("Spawned");

        for(int i = 0; i < spawnBoxArea1.Length; i++)
        {
            if (enemyArea1[i].isDead)
            {
                Instantiate(enemyObj, spawnBoxArea1[i].transform.position, Quaternion.identity);
            }
        }
    }

    public void SpawnEnemy2()
    {
        Debug.Log("Spawned");

        for (int i = 0; i < spawnBoxArea2.Length; i++)
        {
            if (enemyArea2[i].isDead)
            {
                Instantiate(enemyObj, spawnBoxArea2[i].transform.position, Quaternion.identity);
            }
        }
    }

    public void SpawnEnemy3()
    {
        Debug.Log("Spawned");
        for (int i = 0; i < spawnBoxArea3.Length; i++)
        {
            if (enemyArea3[i].isDead)
            {
                Instantiate(enemyObj, spawnBoxArea3[i].transform.position, Quaternion.identity);
            }
        }
    }

    public void SpawnEnemy4()
    {
        Debug.Log("Spawned");
        for (int i = 0; i < spawnBoxArea4.Length; i++)
        {
            if (enemyArea4[i].isDead)
            {
                Instantiate(enemyObj, spawnBoxArea4[i].transform.position, Quaternion.identity);
            }
        }
    }

    public void SpawnEnemy5()
    {
        Debug.Log("Spawned");
        for (int i = 0; i < spawnBoxArea5.Length; i++)
        {
            if (enemyArea5[i].isDead)
            {
                Instantiate(enemyObj, spawnBoxArea5[i].transform.position, Quaternion.identity);
            }
        }
    }

    /*public IEnumerator EnemySpawn()
    {
        Debug.Log("Spawned");
        GameObject newEnemy = Instantiate(enemyObj, spawnBox.transform.position, Quaternion.identity);

        yield return null;
    }*/
}
