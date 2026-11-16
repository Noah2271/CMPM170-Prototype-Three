using TMPro;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private UI_Manager UI;
    private GameObject Paddle;
    private Rigidbody2D BallRigidBody;

    public float horizontalSpeed = 2f;

    void Start()
    {
        UI = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        Paddle = GameObject.Find("Paddle");;
        BallRigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        ApplyDownwardForce(-5f);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Paddle")) BounceOffPaddle();
        if (other.gameObject.CompareTag("BarrierRight") || other.gameObject.CompareTag("BarrierLeft")) BounceOffWall();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Donk");
        if (collision.gameObject.CompareTag("Breakable")) collision.gameObject.SetActive(false);
        if (collision.gameObject.CompareTag("OuterBoundary")) Destroy(this.gameObject);
        if (collision.gameObject.CompareTag("GameOverObject"))
        {
            UI.EndScreenText.gameObject.SetActive(true);
            collision.gameObject.SetActive(false);
            Time.timeScale = 0;

        }
    }
    
    // Force Functions
    // 1. ApplyDownwardForce used for testing just to throw ball down the field, might be useful when spawning the prefab via enemies
    // 2. BounceOffWall has specific trigger volume checks and a wall bounce mechanics since if the velocity is too shallow it just hugs the wall with regular Unity physics
    // 3. BounceOffPaddle takes the difference from the middle X position of the paddle and uses it to allow the player to aim when reflecting the ball
    void ApplyDownwardForce(float amount)
    {
        BallRigidBody.linearVelocity = new Vector2(horizontalSpeed, amount);
    }

    void BounceOffWall()
    {
        BallRigidBody.linearVelocity = new Vector2(-BallRigidBody.linearVelocityX, BallRigidBody.linearVelocityY);
    }

    void BounceOffPaddle()
    {
        Debug.Log("Bounce");
        float differenceFromCenter = transform.position.x - Paddle.transform.position.x;
        BallRigidBody.linearVelocity = new Vector2(differenceFromCenter * 5f, -BallRigidBody.linearVelocityY);
    }
}
