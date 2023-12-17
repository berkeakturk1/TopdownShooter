using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Button_Script : MonoBehaviour, IPointerClickHandler
{
    public Color originalColor = new Color(85, 68, 52, 256); // Store the original button color
    public Color clickedColor = new Color(188, 156, 127, 256);

    public int index = 0;
    [SerializeField] public CharacterController cc;
    [SerializeField] public ButtonController bc;
    private Button btn;
    // Start is called before the first frame update
    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bc.ActiveIndex == index)
            ChangeButtonColor();
        else
        {
            RevertButtonColor();
        }
        /*
        Image image = gameObject.GetComponent<Image>();
        if (isSelected == true && image != null)
        {
            ChangeButtonColor(Color.blue);
        }
        else if (isSelected == false && image != null)
        {
            ChangeButtonColor(originalColor);
        }*/
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the left mouse button was clicked
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            bc.ActiveIndex = index;
            if (index == 1)
                cc.mag.GetComponent<SpriteRenderer>().sprite = cc.mag_sprites[cc.bullet_count - 1];

            if (index == 2)
            {
                cc.mag.GetComponent<SpriteRenderer>().sprite = cc.mag_sprites[cc.ar_bullet_count - 1];
                Debug.Log(cc.ar_bullet_count);
                
            }
                
            
            
            if (index == 3)
                cc.mag.GetComponent<SpriteRenderer>().sprite = cc.Shotgun_mag_sprites[cc.shell_count - 1];
        }
    }
    
    public void ChangeButtonColor()
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = clickedColor;
        cb.pressedColor = clickedColor;
        cb.selectedColor = clickedColor;
        btn.colors = cb;
    }
    
    public void RevertButtonColor()
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = originalColor;
        cb.pressedColor = originalColor;
        cb.selectedColor = originalColor;
        btn.colors = cb;
    }
}
