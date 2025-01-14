using Unity.VisualScripting;
using UnityEngine;

public class VisualsMode : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (tag == "Red")
            GetComponent<Rigidbody2D>().AddForce(Vector2.right * 4f);
        //else
        //    GetComponent<Rigidbody2D>().AddForce(Vector2.right * -4f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3);
        foreach (Collider2D hit in hits) { 
            if (hit.gameObject == this) continue;
            Debug.DrawLine(transform.position, hit.transform.position, Color.red);
        }
    }

}
