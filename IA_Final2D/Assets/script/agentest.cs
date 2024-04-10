using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class agentest : MonoBehaviour
{
    float moveSpeed = 2.0f;
    float avoidanceRadius = 1.0f;
    float timer = 0f;

    float interval = 2f;
    Vector3 randomPosition;
    Gridgen gridgen;
    private Vector3 lastPosition;
    float raycastDistance = 10f;


    void Start()
    {
        gridgen = GameObject.Find("GameController").GetComponent<Gridgen>();
        lastPosition = transform.position;
        SetRandomPosition();
        //MoveRandomly();
    }

    void Update()
    {
        timer += Time.deltaTime;

        MoveRandomly();

        if (timer >= interval)
        {
            if (Mathf.Abs(transform.position.x - lastPosition.x)<0.05 &&  Mathf.Abs(transform.position.y - lastPosition.y)<0.05)
            {
                FindFurthestTree();
                print("r");
            }
            lastPosition = transform.position;
            timer = 0f;
        }
        
        AvoidTrees();
        AvoidEnemy();
    }

    private void MoveRandomly()
    {
        // Move randomly within the grid
        transform.position = Vector3.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
    }

    private void SetRandomPosition()
    {
        randomPosition = new Vector3(Random.Range(0, gridgen.width), Random.Range(0, gridgen.height), 0);
        print("RANDOM:"+randomPosition);
    }

    private void FindFurthestTree()
    {
        Vector3 Ftree = transform.position;
        // Perform a raycast in all directions around the player
        for (float angle = 0; angle < 360; angle += 10) // Change the increment to change the number of directions
        {
            // Calculate the direction of the raycast
            Vector3 direction = Quaternion.Euler(0, 0, angle) * this.GetComponent<Collider>().transform.right;//* transform.forward

            // Perform the raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, LayerMask.GetMask("tree")))
            {
                
                // Check if the hit collider is a tree
                if (hit.collider.CompareTag("tree"))
                {
                    //Debug.DrawLine(transform.position, hit.point, Color.red);
                    Debug.Log("tree collider position: " + hit.collider.transform.position);

                    float distanceCurrent = Vector3.Distance(hit.collider.transform.position, transform.position);
                    float distanceFtree = Vector3.Distance(Ftree, transform.position);
                    if (distanceCurrent > distanceFtree) //compare current raycast pos with furtherest tree pos
                    {
                        Ftree = hit.collider.transform.position;
                    }
                }
            }
        }
        randomPosition = Ftree;
        Debug.Log("furthest tree collider position: " + randomPosition);
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

    private void AvoidEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidanceRadius);

        foreach (Collider collider in colliders)
        {
                if (collider.CompareTag("enemy"))
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

    
}
