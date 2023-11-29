using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //public NavMeshAgent nma; //nav mesh agent for the enemy
    public Transform player; //player object (for enemy to attack)
    public LayerMask whatIsGround; //layer for what is the ground
    public LayerMask whatIsPlayer; //layer for what is the player

    public EnemyHealthBar healthBar; // Script that manages health

    //public Vector3 walkPoint; //variable for place the AI will walk to
    //public float walkPointRange; //range the AI can walk
    //bool walkPointSet; //is a walk point set

    public float timeBetweenAttacks; //time between AI attacks
    bool hasAttacked; //has enemy attacked
    //public float sightRange; //range the enemy can see
    public float attackRange; //range the enemy can attack
    //public bool playerInSightRange; //is player in sight range
    public bool playerInAttackRange; //is player in attack range

    public float maxEnemyHealth = 100;
    private float enemyHealth;
    public float enemyDamage = 10;
    public bool isDead;

    [Header("Attacking")]
    public GameObject bulletPrefab; // The bullet prefab to load when shooting
    public float bulletSpeed;
    public float bulletDuration;

    private void Awake()
    {
        //healthBar = gameObject.GetComponent<EnemyHealthBar>(); //get EnemyHealthBar component - did not work as UI script is not on enemy
        enemyHealth = maxEnemyHealth; // Setting the max health assigned
        healthBar.updateHealth(enemyHealth, maxEnemyHealth); // Resetting the health bar on load

        player = GameObject.Find("PlayerModel").transform;
        //nma = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //check if player is in sight range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); //check if player is in attack range

        if(!playerInAttackRange) //if player is not in sight or attack range
        {
            //Patroling(); //patrol the area around the enemy
            //Debug.Log("not in attack range");
        }
        /*
        else if(playerInSightRange && !playerInAttackRange) //if player is in sight range but not attack range
        {
            ChasePlayer(); //chase the player
        }
        */
        else if(playerInAttackRange) //if the player is in sight range and also in attack range
        {
            AttackPlayer(); //attack the player
            //Debug.Log("in attack range");
        }
    }

    private void AttackPlayer()
    {
        //nma.SetDestination(transform.position); //stop enemy moving by setting its move destination to under itself
        transform.LookAt(player); //look at the player when attacking

        if (!hasAttacked)
        {
            //attack code goes here --------------------------------------------------------------------------------------------------------------------------------------------------------------------
            GameObject tempBullet = Instantiate(bulletPrefab);
            tempBullet.GetComponent<ProjectileMotion>().Create(transform, (player.position - transform.position).normalized, bulletSpeed, enemyDamage, 1);
            Physics.IgnoreCollision(GetComponent<Collider>(), tempBullet.GetComponent<Collider>());


            hasAttacked = true; //enemy has attacked so set boolean to true
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //call the reset attack function after a cooldown
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false; //set boolean value to false to allow enemy to attack again
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        healthBar.updateHealth(enemyHealth, maxEnemyHealth);

        if(enemyHealth <= 0)
        {
            isDead = true;
            enemyHealth = 0;
            gameObject.SetActive(false);
            healthBar.gameObject.SetActive(false); // Hide health bar
        }
    }
}
