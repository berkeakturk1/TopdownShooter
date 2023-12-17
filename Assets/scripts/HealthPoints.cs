using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    [SerializeField] private Transform bar;
    private float hp = 100;
    public bool ded = false;
    private Transform border, bg, bar_sprite;
    private SpriteRenderer b, bgr, bs;
    void Start()
    {
        Transform border = transform.Find("Border");
        Transform bg = transform.Find("Background");
        Transform bar_sprite = bar.transform.Find("BarSprite");

        b = border.GetComponent<SpriteRenderer>();
        bgr = bg.GetComponent<SpriteRenderer>();
        bs = bar_sprite.GetComponent<SpriteRenderer>();
        
        if (tag != "PlayerHP")
        {
            b.enabled = false;
            bgr.enabled = false;
            bs.enabled = false;
                
        }
    }

    public void SetSize(int val)
    {
        b.enabled = true;
        bgr.enabled = true;
        bs.enabled = true;
        
        hp += val;
        float normalized = hp * 0.01f;
        bar.localScale = new Vector3(normalized, 1f);
    }

    public float getHP()
    {
        return hp;
    }

   
    void Update()
    {
        if (hp <= 0)
        {
            ded = true;
        }
    }
}
