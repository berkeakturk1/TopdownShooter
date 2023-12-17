using System.Collections;
using CodeMonkey.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Animator anim;
    private Animator animController;
    
    private InputAction attackAction;
    [SerializeField] private InputActionReference inputAction;
    
    
    
    private int prevIndex = 0;
    [SerializeField] public ButtonController bc;
    public Sprite[] sprites =  new Sprite[4];
    public RuntimeAnimatorController[] Animators = new RuntimeAnimatorController[4];
    public GameObject[] muzzleflashSprites = new GameObject[4];
    
    [SerializeField] public GameObject mag;
    [SerializeField] public Sprite[] mag_sprites = new Sprite[17];
    public int bullet_count = 0;
    public int ar_bullet_count = 0;
    
    [SerializeField] public Sprite[] Shotgun_mag_sprites = new Sprite[6];
    public int shell_count = 0;
    
    [SerializeField] public Light2D muzzleflash;
    [SerializeField] public GameObject muzzleflashSprite;
    [SerializeField] public Transform bullet_spawn;
    [SerializeField] public Transform shell_spawn;
    [SerializeField] public GameObject Bullet;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private float weaponRange= 10f;
    private Bullet b;
    [SerializeField] private Material weaponTraceMaterial;
    [Range(0.1f, 1f)] [SerializeField] private float firingRate;

    

    private bool isAttacking = false;
    private void Start()
    {
        muzzleflashSprite.GetComponent<SpriteRenderer>().enabled = false;
        muzzleflash.enabled = false;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        animController = GetComponent<Animator>();
        
    }

    private void Update()
    {
        if (isAttacking)
        {
            Debug.Log("yooo");
        }
        
        Debug.Log(sprites[bc.ActiveIndex].name);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[bc.ActiveIndex];
        
        //mag.GetComponent<SpriteRenderer>().sprite = bc.ActiveIndex == 3 ? Shotgun_mag_sprites[0] : mag_sprites[0];
        
        animController.runtimeAnimatorController = Animators[bc.ActiveIndex];
                
        
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, input.y, 0); // Use input.x and input.y for the move vector

        // You might want to normalize the move vector to ensure consistent speed in all directions
        move.Normalize();

        if (move != Vector3.zero)
        {
            anim.SetBool("stop", false);
            anim.SetBool("move", true);

            // Calculate the angle in degrees between the character's current rotation and the move direction
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;

            // Set the character's rotation to the calculated angle
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Move the character forward in its local space
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("move", false);
            anim.SetBool("stop", true);
        }

        
    }
    
    private void OnEnable()
    {
        attackAction = inputAction;
        attackAction.Enable();

        attackAction.started += OnAttackStarted;
        attackAction.canceled += OnAttackCanceled;
        attackAction.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        attackAction.Disable();
        attackAction.started -= OnAttackStarted;
        attackAction.canceled -= OnAttackCanceled;
    }
    
    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        isAttacking = true;
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        anim.SetBool("attack", true);
        
        if (bc.ActiveIndex == 1)
        {
            if (bullet_count == 16)
            {
                bullet_count = 0;
            }
            GetComponent<AudioSource>().Play();
            shoot();
            changeMagSprite();
            Invoke("resetAttack", 0.2f);
        }
        
        if (bc.ActiveIndex == 2)
        {
            if (ar_bullet_count == 16)
            {
                //reload
                ar_bullet_count = 0;
            }
            GetComponent<AudioSource>().Play();
            shoot();
            changeMagSprite();
            Invoke("resetAttack", 0.2f);
        }
        

        if (bc.ActiveIndex == 3)
        {
            GetComponent<AudioSource>().Play();
            Invoke("resetAttack", 0.5f);
            mag.GetComponent<SpriteRenderer>().sprite = Shotgun_mag_sprites[shell_count];
            if (shell_count == 5)
            {
                shell_count = 0;
            }
            
            shoot_shotgun();
            
        }
        
        Invoke("resetAttack", 0.2f);
    }
    
 
    
    public void resetAttack(){ 
        anim.SetBool("attack", false);
        muzzleflash.enabled = false;
        muzzleflashSprites[bc.ActiveIndex].GetComponent<SpriteRenderer>().enabled = false;
    }

    private void shoot()
    {
        
        if (bc.ActiveIndex == 1)
            bullet_count++;
        else
        {
            ar_bullet_count++;
        }
        WeaponTracer(bullet_spawn.transform.position);
        muzzleflash.enabled = true;
        muzzleflashSprites[bc.ActiveIndex].GetComponent<SpriteRenderer>().enabled = true;
    }
    
    private void shoot_shotgun()
    {
        shell_count++;
        
        ShotgunTracer(bullet_spawn.transform.position);
    
        muzzleflash.enabled = true;
        muzzleflashSprites[bc.ActiveIndex].GetComponent<SpriteRenderer>().enabled = true;
    }
    
    

    private void WeaponTracer(Vector3 fromPosition)
    {
        
        Quaternion rotation = bullet_spawn.transform.rotation; // Get the character's rotation
        Vector3 dir = rotation * Vector3.up; // Calculate the direction based on character's rotation

        // Create a bullet with the correct rotation
        GameObject bullet = Instantiate(Bullet, bullet_spawn.position, rotation * Quaternion.Euler(0, 0, 90));

        
        Bullet bullet_ = bullet.GetComponent<Bullet>();
        Vector3 hitPoint = bullet_.hitPosition; // Use Vector3 instead of Vector2

        Vector2 bullet_spawn2D = new Vector2(bullet_spawn.position.x, bullet_spawn.position.y);
        Vector2 hitPoint2D = new Vector2(hitPoint.x, hitPoint.y);

        float distance = Vector2.Distance(bullet_spawn2D, hitPoint2D); // Calculate the Vector2 distance

        float eulerZ = UtilsClass.GetAngleFromVector(dir) - 90;

        Vector3 tracerSpawnPosition = bullet_spawn.transform.position + dir * distance * 0.5f;

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

    
    
   private void ShotgunTracer(Vector3 fromPosition)
{
    int numTracers = 3;
    float spacing = 22f; // Degrees between tracers
    float halfSpacing = spacing / 2f;

    // Calculate the rotation for the second bullet (in the middle)
   

   
    for (int i = 0; i < numTracers; i++)
    {
        // Calculate the rotation for this tracer
        Quaternion rotation = shell_spawn.transform.rotation * Quaternion.Euler(0, 0, i * spacing - halfSpacing);

        // Calculate the direction based on the rotation
        Vector3 dir = rotation * Vector3.up;
        
        GameObject bullet = Instantiate(Bullet, shell_spawn.position, Quaternion.identity);
        Bullet _bullet = bullet.GetComponent<Bullet>();
        Vector3 endpoint = _bullet.hitPosition;
        
        float distance = Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y),new Vector2(endpoint.x, endpoint.y)) / 7.2f;
        Debug.Log(distance);
        
        bullet.transform.rotation = rotation * Quaternion.Euler(0, 0, 90);
        
        
        
        Vector3 tracerSpawnPosition = shell_spawn.transform.position + dir * distance * 0.5f;

        // Create a temporary material for this tracer
        Material tempWeaponmat = new Material(weaponTraceMaterial);
        tempWeaponmat.SetTextureScale("_MainTex", new Vector2(1f, distance / 256f));

        // Create the world mesh for this tracer
        World_Mesh worldMesh = World_Mesh.Create(tracerSpawnPosition, UtilsClass.GetAngleFromVector(dir) - 90, 1f, distance, tempWeaponmat, null, 10000);

        // Set UV coordinates or any other properties for the world mesh as needed

        // Set a timer to destroy this tracer after a delay
        float timer = 0.1f;
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

    
}
    
    private void changeMagSprite()
    {
        if (bc.ActiveIndex == 1)
            mag.GetComponent<SpriteRenderer>().sprite = mag_sprites[bullet_count - 1];
        else
        {
            mag.GetComponent<SpriteRenderer>().sprite = mag_sprites[ar_bullet_count - 1];
        }
    }
    
    
    
    

}