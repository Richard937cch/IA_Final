using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;

public class AdventurerBehaveiour : MonoBehaviour
{
    
    private Blackboard blackboard;
    private Root behaviorTree;

    float moveSpeed = 3.0f;
    float avoidanceRadiusE = 7.0f; //enemy avoid range
    float deleteRadius = 1.0f;    //attack range
    float raycastDistance = 20f; //find furthest tree range
    float fleeDistance = 7.5f; //flee distance from enemy
    Vector3 randomPosition; //Adventurer goal position
    Vector3 lastPosition;
    int timer1 = 0;
    int timer2 = 0;
    int timer3 = 0;
    int timer4 = 0;
    int stucktime = 1000; 
    int fleetime = 900;
    int attackCooltime = 16000;
    int attackduration = 1600;
    int KeepStuck = 0;
    bool fleePos = false;
    Color transparent = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0f);
    Color white = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public bool attacking = false;
    bool attackCooled = true;
    bool inRiver = false;
    

    Gridgen gridgen;
    EnemyBehaviour enemyBehaviour;

    void Start()
    {
        gridgen = GameObject.Find("GameController").GetComponent<Gridgen>();
        lastPosition = transform.position;
        attackColor(transparent);
        
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;
        SetRandomPosition(); //initial target position

        #if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
        #endif


        behaviorTree.Start();
    }

    private void UpdateBlackboard() //blackboard
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        int inRange = 0;
        float closest = fleeDistance;
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 enemyLocalPos = this.transform.InverseTransformPoint(enemies[i].transform.position);
            if (enemyLocalPos.magnitude <= fleeDistance)
            {
                inRange++;
            }
            if (enemyLocalPos.magnitude < closest)
            {
                closest = enemyLocalPos.magnitude;
            }

        }

        
        if (inRiver == true) // in river
        {
            moveSpeed = 1.5f;
        }
        else
        {
                if (inRange != 0) // being chase
            {
                behaviorTree.Blackboard["enemyDistance"] = true;
                moveSpeed = 4.0f + (fleeDistance - closest) / 3.0f;
            }
            else //exploring
            {
                behaviorTree.Blackboard["enemyDistance"] = false;
                moveSpeed = 3.0f;
            }
        } 

        


        behaviorTree.Blackboard["stuck"] = false;
        behaviorTree.Blackboard["keepstuck"] = false;
        fleePos = false;
        timer1++;
        timer2++;
        if (timer1 >= stucktime || Vector3.Distance(transform.position, randomPosition) < 0.9f )
        {
            if (Mathf.Abs(transform.position.x - lastPosition.x)<0.05 &&  Mathf.Abs(transform.position.y - lastPosition.y)<0.05 && KeepStuck > 0) //keepstuck
            {
                behaviorTree.Blackboard["stuck"] = true;
                behaviorTree.Blackboard["keepstuck"] = true;
                print("keepstuck="+KeepStuck);
                KeepStuck++;
            }
            else if (Mathf.Abs(transform.position.x - lastPosition.x)<0.05 &&  Mathf.Abs(transform.position.y - lastPosition.y)<0.05 && KeepStuck == 0) //stuck
            {
                behaviorTree.Blackboard["stuck"] = true;
                print("stuck "+KeepStuck);
                KeepStuck++;
            }
            else
            {
                behaviorTree.Blackboard["stuck"] = false;
                behaviorTree.Blackboard["keepstuck"] = false;
                KeepStuck = 0;
            }
            lastPosition = transform.position;
            timer1 = 0;
        }

        if (timer2 >= fleetime || Vector3.Distance(transform.position, randomPosition) < 0.1f)
        {
            if (KeepStuck < 1)
            {
                fleePos = true;
            }
            
            timer2 = 0;
        }

        if (attackCooled == false)
        {
            timer3++;
        }
        if (attackCooled == false && timer3 > attackCooltime )
        {
            timer3 = 0;
            attackCooled = true;
        }
        if (attacking == true)
        {
            timer4++;
        }
        if (attacking == true && timer4 > attackduration )
        {
            timer4 = 0;
            attacking = false;
            attackColor(transparent);
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("river")) 
        {
            inRiver = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("river")) 
        { 
            inRiver = false;
        }
    }

    private Root CreateBehaviourTree()
    {
        return new Root(

            new Service(0.125f, UpdateBlackboard,

                new Selector(

                    //Enemy in ranges
                    new BlackboardCondition("enemyDistance", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,

                        // the enemy is in range of 7.5f
                        new Sequence(
                            new Selector(
                                new BlackboardCondition("stuck", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                    new Sequence( 
                                        new Selector(
                                            new BlackboardCondition("keepstuck", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                                new Sequence( //keep stucking
                                                    new Action(() => SetRandomPosition()) { Label = "Random" },
                                                    new Action(() => MoveRandomly()) { Label = "Move" },
                                                    new Action(() => attack()) { Label = "Attack" }
                                                )
                                                
                                            ),
                                            new Sequence( //first stuck
                                                new Action(() => FindFurthestTree()) { Label = "FindFurthestTree" },
                                                new Action(() => MoveRandomly()) { Label = "Move" },
                                                new Action(() => attack()) { Label = "Attack" }
                                            )
                                        )
                                        
                                    )
                                ),
                                new Sequence( //Not Stuck
                                    new Action(() => AvoidEnemies()) { Label = "Flee" },
                                    new Action(() => MoveRandomly()) { Label = "Move" }
                                )
                            )
                        )
                    ),

                    // Enemy NOT in range
                    new Sequence(
                        new Selector(
                            new BlackboardCondition("stuck", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                new Sequence( 
                                        new Selector( 
                                            new BlackboardCondition("keepstuck", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                                new Sequence( //keep stucking
                                                    new Action(() => SetRandomPosition()) { Label = "Random" },
                                                    new Action(() => MoveRandomly()) { Label = "Move" }
                                                )
                                                
                                            ),
                                            new Sequence( //firststuck
                                                new Action(() => FindFurthestTree()) { Label = "FindFurthestTree" },
                                                new Action(() => MoveRandomly()) { Label = "Move" }
                                            )
                                        )
                                        
                                    )
                            ),
                            new Sequence( //Not Stuck
                                new Action(() => MoveRandomly()) { Label = "Move" }
                            )
                        )
                    )
                )
            )
        );
    }

    

    private void MoveTowards(Vector3 localPosition)
    {
        transform.localPosition += localPosition * 0.5f * Time.deltaTime;
    }

    private void MoveRandomly()
    {
        transform.position = Vector3.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
    }

    private void SetRandomPosition()
    {
        randomPosition = new Vector3(UnityEngine.Random.Range(0, gridgen.width), UnityEngine.Random.Range(0, gridgen.height), 0);
        print("RANDOM:"+randomPosition);
        behaviorTree.Blackboard["stuck"] = false;
        behaviorTree.Blackboard["keepstuck"] = false;
    }

    private void FindFurthestTree()
    {
        Vector3 Ftree = transform.position;
        // Perform a raycast in all directions around the player
        int raycastGap = UnityEngine.Random.Range(5, 20);
        for (float angle = 0; angle < 360; angle += raycastGap) 
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle+UnityEngine.Random.Range(0, raycastGap) ) * this.GetComponent<Collider2D>().transform.right;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, LayerMask.GetMask("tree")))
            {
                
                // Check if the hit collider is a tree
                if (hit.collider.CompareTag("tree"))
                {
                    //Debug.Log("tree collider position: " + hit.collider.transform.position);
                    float distanceCurrent = Vector3.Distance(hit.collider.transform.position, transform.position);
                    float distanceFtree = Vector3.Distance(Ftree, transform.position);
                    if (distanceCurrent > distanceFtree) //compare current raycast pos with furtherest tree pos
                    {
                        Ftree = hit.collider.transform.position;
                    }
                }
            }
        }
        Ftree.z = 0.0f;
        randomPosition = Ftree;
        Debug.Log("furthest tree collider position: " + randomPosition);
        behaviorTree.Blackboard["stuck"] = false;
    }

    void AvoidEnemies()
    {
        // Find all game objects with the tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        if (enemies.Length == 0)
            return; // No enemies to avoid

        // Calculate total avoidance direction
        Vector3 avoidanceDirection = Vector3.zero;

        foreach (GameObject enemy in enemies)
        {
            Vector3 directionToEnemy = transform.position - enemy.transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;

            // If the enemy is within the avoidance radius, calculate avoidance direction
            if (distanceToEnemy < avoidanceRadiusE)
            {
                avoidanceDirection += directionToEnemy.normalized * (avoidanceRadiusE - distanceToEnemy);
            }
        }

        // Normalize the overall avoidance direction
        if (avoidanceDirection != Vector3.zero)
        {
            avoidanceDirection.Normalize();
        }

        avoidanceDirection.z = 0;

        //Set position to escape
        if (fleePos == true)
        {
            randomPosition = (transform.position + avoidanceDirection * 100.0f) ;
            randomPosition.z = 0.0f;
            print("avoidPosition"+randomPosition);
        }
        
    }

    void attack()
    {
        if (attackCooled == true)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

            foreach (GameObject enemy in enemies)
            {
                // Calculate distance between agent and enemy
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                // If the enemy is within the attack radius, destroy it
                if (distance <= deleteRadius)
                {
                    enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
                    enemyBehaviour.dead = true;
                    attackCooled = false;
                    attacking = true;
                    attackColor(white);
                    print("attack");
                }
            }
            
        }
        
    }

    void attackColor(Color color)
    {
        GameObject[] attacks = GameObject.FindGameObjectsWithTag("attack");

        foreach (GameObject attack in attacks)
        {
            Renderer attackRenderer = attack.GetComponent<SpriteRenderer>();
            attackRenderer.material.color = color;
        }
    }
    
}
