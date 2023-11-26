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

    //public Vector3 walkPoint; //variable for place the AI will walk to
    //public float walkPointRange; //range the AI can walk
    //bool walkPointSet; //is a walk point set

    public float timeBetweenAttacks; //time between AI attacks
    bool hasAttacked; //has enemy attacked
    //public float sightRange; //range the enemy can see
    public float attackRange; //range the enemy can attack
    //public bool playerInSightRange; //is player in sight range
    public bool playerInAttackRange; //is player in attack range

    public float enemyHealth = 100;
    public float enemyDamage = 10;
    public bool isDead;

    public GameObject bulletPrefab;

    private void Awake()
    {
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

    /*
    private void Patroling()
    {
        if (!walkPointSet) //if no walk point has been set
        {
            SearchWalkPoint(); //search for a walk point
        }

        if (walkPointSet) //if a walk point has been set
        {
            nma.SetDestination(walkPoint); //move enemy to the walk point
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint; //new variable to calculate distance to walk point

        if (distanceToWalkPoint.magnitude < 0.1f) //if enemy is less than 1 unit away from the walk point (basically if the enemy has reached the walk point)
        {
            walkPointSet = false; //walk point set is false
        }
    }
    

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange); //gets a random value for X
        float randomZ = Random.Range(-walkPointRange, walkPointRange); //gets a random value for Z
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.z + randomZ); //create walk point with the random X and Z values

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) //if random walk point is on the level
        {
            walkPointSet = true; //set walk point to true
        }
    }

    private void ChasePlayer()
    {
        nma.SetDestination(player.position); //set destination to the player transform
    }

    */

    private void AttackPlayer()
    {
        //nma.SetDestination(transform.position); //stop enemy moving by setting its move destination to under itself
        transform.LookAt(player); //look at the player when attacking

        if (!hasAttacked)
        {
            //attack code goes here --------------------------------------------------------------------------------------------------------------------------------------------------------------------
            GameObject tempBullet = Instantiate(bulletPrefab);
            tempBullet.GetComponent<ProjectileMotion>().Create(transform, (player.position - transform.position).normalized, 200, 10, 1);



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

        if(enemyHealth <= 0)
        {
            isDead = true;
            enemyHealth = 0;
            gameObject.SetActive(false);
        }
    }
}
