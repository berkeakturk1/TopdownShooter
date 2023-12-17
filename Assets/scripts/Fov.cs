using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fov : MonoBehaviour
{
    public float fovAngle = 60f;
    public Transform fovPoint;
    public float range = 10f;
    public bool inRange = false;
    public Vector3 lastLocation = new Vector3(0,0,0);
    public Transform target;

    void Update()
    {
        // Calculate the direction from the NPC to the target
        Vector2 dir = target.position - transform.position;

        // Calculate the angle between the NPC's forward direction and the direction to the target
        float angle = Vector2.Angle(dir, fovPoint.up);
        
        // Cast a ray from the FOV point towards the target
        RaycastHit2D hit = Physics2D.Raycast(fovPoint.position, dir, range);

        if (angle < fovAngle / 2)
        {
            // Check if the ray hits the target and the target has the "Player" tag
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                print("seen");
                inRange = true;
                lastLocation = hit.collider.gameObject.transform.position;
                Debug.DrawRay(fovPoint.position, dir, Color.red);
            }
            else
            {
                inRange = false;
                print("not seen");
            }
        }
    }
}