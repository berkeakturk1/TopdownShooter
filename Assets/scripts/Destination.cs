using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using CodeMonkey.Utils;

public class Destination : MonoBehaviour
{
    [SerializeField] public AIDestinationSetter dest;
    [SerializeField] public AIPath path;
    [SerializeField] public triggerManager A, B;
    public Vector3 searchA, searchB; 
    [SerializeField] public  triggerManager SearchTrigA, SearchTrigB;
    [SerializeField] public GameObject SearchArea;
    public bool spotsCreated = false, isAlerted = false;
    public float moveCheck = 0.7f;
    public bool spottedPlayer = false;
    [SerializeField] public GameObject player;
    [SerializeField] public Fov fov;
    [SerializeField] public float repeatRate = 2f, time = 2f;
    [SerializeField] public Transform Bullet_spawn;
    private HealthPoints health;
    
    [SerializeField] public GameObject Bullet;
    [SerializeField] private Material weaponTraceMaterial;
    [SerializeField] public Sprite Search, Attack, Dead;
    private SpriteRenderer Character;
    
    void Start()
    {
        health = GetComponentInChildren<HealthPoints>();
        dest.target = A.transform;
        Character = GetComponentInChildren<SpriteRenderer>();

    }

    void Update()
    {
        if (health.ded)
        {
            Destroy(gameObject);
        }
        
        if (!isAlerted && spottedPlayer) // search state
        {
            Character.sprite = Search;
            Invoke("resetIsSpotted", 10f);
            CancelInvoke("opportunity");
            
            path.endReachedDistance = 0f;
            
            spotsCreated = true;
            if (!IsInvoking("opportunity_search"))
            {
                InvokeRepeating("opportunity_search", time, repeatRate); 
            }
            
            
            Debug.Log(searchA );

        }
        
        if (!isAlerted && !spottedPlayer) // intitial state
        {
            Character.sprite = Search;
            CancelInvoke("opportunity_search");
            path.endReachedDistance = 0f;
            if (!IsInvoking("opportunity"))
            {
                InvokeRepeating("opportunity", time, repeatRate); 
            }
            
        }
        

        if ((calculateDistance(player) < 10f && fov.inRange) || (fov.inRange) || (calculateDistance(player) < 5f)) // attack mode
        {

            Character.sprite = Attack;
            if (!IsInvoking("WeaponTracer"))
            {
                InvokeRepeating("WeaponTracer", time, repeatRate); 
            }
            spottedPlayer = true;
            isAlerted = true;
            path.endReachedDistance = 5f;
            dest.target = player.transform;
        }
        else
        {
            isAlerted = false;
            
        }
        
        
            
    }
    
    private void WeaponTracer()
    {
        
        Quaternion rotation = Bullet_spawn.rotation; // Get the character's rotation
        Vector3 dir = rotation * Vector3.up; // Calculate the direction based on character's rotation
        Bullet_spawn.position = new Vector3 (Bullet_spawn.position.x, Bullet_spawn.position.y, 119);
        // Create a bullet with the correct rotation
        GameObject bullet = Instantiate(Bullet, Bullet_spawn.position, rotation * Quaternion.Euler(0, 0, 90));

        
        Bullet bullet_ = bullet.GetComponent<Bullet>();
        Vector3 hitPoint = bullet_.hitPosition; // Use Vector3 instead of Vector2

        Vector2 bullet_spawn2D = new Vector2(Bullet_spawn.position.x, Bullet_spawn.position.y);
        Vector2 hitPoint2D = new Vector2(hitPoint.x, hitPoint.y);

        float distance = Vector2.Distance(bullet_spawn2D, hitPoint2D); // Calculate the Vector2 distance

        float eulerZ = UtilsClass.GetAngleFromVector(dir) - 90;

        Vector3 tracerSpawnPosition = Bullet_spawn.transform.position + dir * distance * 0.5f;

        Material tempWeaponmat = new Material(weaponTraceMaterial);
        tempWeaponmat.SetTextureScale("_MainTex", new Vector2(1f, distance / 256f));
        World_Mesh worldMesh = World_Mesh.Create(tracerSpawnPosition, eulerZ, 0.5f, distance, tempWeaponmat, null, 10000);

        float timer = 0.1f;

        worldMesh.SetUVCoords(new World_Mesh.UVCoords(0, 0, 16, 256));

        FunctionUpdater.Create(() =>
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                worldMesh.DestroySelf();
                return true;
            }

            return false;
        }); 
    }

    
    float calculateDistance(GameObject player)
    {
        float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y),new Vector2(player.transform.position.x, player.transform.position.y));

        return distance;
    }

    void opportunity()
    {
        float rand = Random.Range(0f, 1f);
        if (rand > 0.2f)
        {
            changeDestination(A, B);
        }
    }

    void changeDestination(triggerManager A, triggerManager B)
    {
        if (dest.target == A.transform)
            dest.target = B.transform;
        else
            dest.target = A.transform;

    }
    
    void opportunity_search()
    {
        Debug.Log(fov.lastLocation);
        Vector3 pos = RandomPointInCircle(fov.lastLocation);
        SearchTrigA.transform.position = pos;
        float rand = Random.Range(0f, 1f);
        if (rand > 0.2f)
        {
            dest.target = SearchTrigA.transform;
        }
    }

    private Vector3 RandomPointInCircle(Vector3 center, float radius = 5f)
    {
        // Generate a random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        // Use trigonometry to calculate the random point within the circle
        float x = center.x + radius * Mathf.Cos(randomAngle);
        float y = center.y + radius * Mathf.Sin(randomAngle);

        return new Vector3(x, y, 116);
    }
    
    
    void createDestinations(Vector3 A, Vector3 B)
    {
        SearchTrigA.transform.position = A;
        SearchTrigB.transform.position = B;
        
    }

    void resetIsSpotted()
    {
        spottedPlayer = false;
    }
}