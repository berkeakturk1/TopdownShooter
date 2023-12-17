using UnityEngine;

public class DpadController : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float moveSpeed = 5f;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Assuming only one touch at a time

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    GameObject touchedObject = hit.collider.gameObject;

                    // Check which D-pad button was touched
                    if (touchedObject.CompareTag("up"))
                    {
                        player.transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                    }
                    else if (touchedObject.CompareTag("down"))
                    {
                        player.transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                    }
                    else if (touchedObject.CompareTag("left"))
                    {
                        player.transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                    }
                    else if (touchedObject.CompareTag("right"))
                    {
                        player.transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
}