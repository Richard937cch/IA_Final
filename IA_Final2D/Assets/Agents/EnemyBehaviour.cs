using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;

public class EnemyBehaviour : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;
    float moveSpeed = 4.0f;
    float chaseRange = 7.5f;
    Color myColor = new Color(200f / 255f, 101f / 255f, 0f, 0f);
    Vector3 randomPosition;
    public bool dead = false;

    AdventurerBehaveiour adventurerBehaveiour;
    Gridgen gridgen;

    void Start()
    {
        adventurerBehaveiour = GameObject.Find("Adventurer(Clone)").GetComponent<AdventurerBehaveiour>();
        gridgen = GameObject.Find("GameController").GetComponent<Gridgen>();
        
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;
        
        behaviorTree.Start();
    }

    void Update()
    {
        if (dead == true)
        {
            behaviorTree.Stop();
            Destroy(gameObject);
        }
    }

    private void UpdateBlackboard()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("adventurer").transform.position);
        behaviorTree.Blackboard["playerLocalPos"] = playerLocalPos;
        behaviorTree.Blackboard["playerDistance"] = playerLocalPos.magnitude;
        
    }


    private Root CreateBehaviourTree()
    {
        return new Root(

            new Service(0.125f, UpdateBlackboard,

                new Selector(

                    //If adventurer is in range
                    new BlackboardCondition("playerDistance", Operator.IS_SMALLER, chaseRange, Stops.IMMEDIATE_RESTART,

                        new Sequence(
                            new Action(() => SetColor(Color.red)) { Label = "Change to Red" },

                            // go towards Adventurer until Distance is greater than 7.5 ( in that case, _shouldCancel will get true )
                            new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    MoveTowards(blackboard.Get<Vector3>("playerLocalPos"));
                                    return Action.Result.PROGRESS;
                                }
                                else
                                {
                                    return Action.Result.FAILED;
                                }
                            }) { Label = "Chase" }
                            
                        )
                    ),

                    // Adventurer not in range
                    new Sequence(
                        new Action(() => SetColor(myColor)) { Label = "invisible" },
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }


    private void MoveTowards(Vector3 localPosition)
    {
        if (adventurerBehaveiour.attacking == true) //Adventurer attacking move away
        {
            transform.localPosition -= localPosition * 0.5f * Time.deltaTime;
        }
        else //move toward Adventurer
        {
            transform.localPosition += localPosition * 0.5f * Time.deltaTime;
        }
    }

    private void MoveRandomly()
    {
        // Move randomly within the grid
        transform.position = Vector3.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
    }

    private void SetRandomPosition()
    {
        randomPosition = new Vector3(UnityEngine.Random.Range(0, gridgen.width), UnityEngine.Random.Range(0, gridgen.height), 0);
        print("RANDOM:"+randomPosition);
    }

    private void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }
}
