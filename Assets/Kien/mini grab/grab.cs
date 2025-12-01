using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grab : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 20f;
    bool isFacingRing = true;

    public event Action PlayerDied;

    Rigidbody2D rb;
    Animator ani;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        ani.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
    }

    void Flip()
    {
        if(isFacingRing && horizontalInput < 0f|| !isFacingRing && horizontalInput > 0f)
        {
            isFacingRing = !isFacingRing;
            Vector3 localScales = transform.localScale;
            localScales.x *= -1f;
            transform.localScale = localScales;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            PlayerDied.Invoke();
            Destroy(this.gameObject);
        }
    }
}
