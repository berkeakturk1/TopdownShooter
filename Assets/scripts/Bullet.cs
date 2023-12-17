using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    public Vector3 hitPosition;
    private float timer = 0f;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet forward (right) based on its local transform
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Increase the timer
        timer += Time.deltaTime;

        // Check if the bullet has reached its lifetime
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100f); // Adjust the ray length as needed

        if (hit.collider != null)
        {
            Debug.Log("Collision detected with: " + hit.collider.gameObject.name);
            Debug.Log("Object tag: " + hit.collider.gameObject.tag);

            // Calculate the direction vector from the player to the hit point
            Vector3 direction = (Vector3)(hit.point) - GameObject.FindWithTag("Player").transform.position;

            // Draw a red ray from the player's position in the calculated direction
            Debug.DrawRay(GameObject.FindWithTag("Player").transform.position, direction, Color.red);

            hitPosition = hit.point; // Use hit.point to get the exact point of the collision
            if (hit.collider.gameObject.tag != "Bullet")
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    Destination Enemy = hit.collider.gameObject.GetComponent<Destination>();
                    HealthPoints hp = Enemy.GetComponentInChildren<HealthPoints>();
                    hp.SetSize(-10);
                    Destroy(gameObject);
                }
                if (hit.collider.gameObject.tag == "Player")
                {

                    GameObject healthbar = GameObject.Find("Player_HP");
                    HealthPoints PlayerHP = healthbar.GetComponentInChildren<HealthPoints>();
                    PlayerHP.SetSize(-10);
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}