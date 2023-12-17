using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyMovement : MonoBehaviour
{

    public AIPath AIPath;

    private Vector2 Direction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        faceVelocity();
    }

    void faceVelocity()
    {
        Direction = AIPath.desiredVelocity;

        transform.right = Direction;
    }
}
