using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    
    [SerializeField] public Sprite[] sprites = new Sprite[5];
    public int CollisionCount = 0;
    public bool CollisionMade = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeSprite();
        
        
    }

    void changeSprite()
    {
        switch (CollisionCount)
        {
            case 0:
                break;
            case 1:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case 3:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            case 4:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[4];
                break;
        }
    }
}
