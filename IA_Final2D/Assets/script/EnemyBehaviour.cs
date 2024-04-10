using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;

public class EnemyBehaviour : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;
    float moveSpeed = 2.0f;
    float avoidanceRadius = 1.0f;
    Color myColor = new Color(200f / 255f, 101f / 255f, 0f, 0f);
    AdventurerBehaveiour adventurerBehaveiour;

    void Start()
    {
        adventurerBehaveiour = GameObject.Find("Adventurer(Clone)").GetComponent<AdventurerBehaveiour>();
        // create our behaviour tree and get it's blackboard
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
        /*
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif*/

        // start the behaviour tree
        behaviorTree.Start();
    }


    private Root CreateBehaviourTree()
    {
        // we always need a root node
        return new Root(

            //update the "playerDistance" and "playerLocalPos" Blackboard values every 125 milliseconds
            new Service(0.125f, UpdatePlayerDistance,

                new Selector(

                    // check the 'playerDistance' blackboard value.
                    // When the condition changes, we want to immediately jump in or out of this path, thus we use IMMEDIATE_RESTART
                    new BlackboardCondition("playerDistance", Operator.IS_SMALLER, 7.5f, Stops.IMMEDIATE_RESTART,

                        // the player is in our range of 7.5f
                        new Sequence(
                            new Action(() => SetColor(Color.red)) { Label = "Change to Red" },

                            // go towards player until playerDistance is greater than 7.5 ( in that case, _shouldCancel will get true )
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
                            }) { Label = "Follow" }
                            //new Action(() => AvoidTrees())
                        )
                    ),

                    // park until playerDistance does change
                    new Sequence(
                        new Action(() => SetColor(myColor)) { Label = "invisible" },
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }

    private void UpdatePlayerDistance()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("adventurer").transform.position);
        behaviorTree.Blackboard["playerLocalPos"] = playerLocalPos;
        behaviorTree.Blackboard["playerDistance"] = playerLocalPos.magnitude;
        
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
        //transform.localPosition += localPosition * 0.5f * Time.deltaTime;
    }

    private void AvoidTrees()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidanceRadius);

        foreach (Collider collider in colliders)
        {
                if (collider.CompareTag("tree"))
            {
                // Get the position of the agent and the tree in the XY plane
                Vector3 agentPositionXY = new Vector3(transform.position.x, transform.position.y, 0);
                Vector3 treePositionXY = new Vector3(collider.transform.position.x, collider.transform.position.y, 0);

                // Calculate a direction away from the tree on the XY plane
                Vector3 avoidDirection = agentPositionXY - treePositionXY;
                avoidDirection.Normalize();

                // Move away from the tree on the XY plane while keeping the Z value fixed
                transform.Translate(avoidDirection * moveSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    private void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }
}
