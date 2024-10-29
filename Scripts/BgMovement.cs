using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMovement : MonoBehaviour
{
    
    [SerializeField]Rigidbody2D rb;

    private float SpeedX = 5f;
    private float SpeedY = 5f;
    private Vector3 Movement;

    // Start is called before the first frame update
    void Start()
    {
        Movement = new Vector3(SpeedX, SpeedY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Movement.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "x")
        {
            SpeedX *= -1;
            Movement = new Vector3(SpeedX, SpeedY, 0);
        }
        if (collision.gameObject.tag == "y")
        {
            SpeedY *= -1;
            Movement = new Vector3(SpeedX, SpeedY, 0);
        }
    }
}
