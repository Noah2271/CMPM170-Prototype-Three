using TMPro;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private UI_Manager UI;
    private Audio_Manager Audio;
    private GameObject Paddle;
    private Rigidbody2D BallRigidBody;
    public Material DeflectedMaterial;
    public Material EnemyProjectileMaterial;
    public float PreservedVerticalVelocity;

    void Start()
    {
        UI = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        Audio = GameObject.Find("Audio_Manager").GetComponent<Audio_Manager>();
        Paddle = GameObject.Find("Paddle");;
        BallRigidBody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Trigger and Collision Checks
    // 1. Paddle Trigger : Play reflect sound, update preserved vertical velocity, enable the ball to kill the enemy, and switch the material. Finally call paddle bounce physics update function
    // 2. Barrier Right/Left Trigger : Call bounce wall bounce physics update function
    // 3. Breakable Collision : Play digital break sound, disable the breakable, and update preserved vertical velocity (Unity physics engine is handling bounces off barriers since they're more complex and I couldn't figure out the code)
    // 4. Outer Boundary Collision : Big brick on the top outside the screen, just deletes balls that aren't in use.
    // 5. GameOverObject Collision : The earth, plays d eath sound and updates screen to reflect end of game. Stops the game via timescale.
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Paddle")) {
            Audio.PlayReflect();
            PreservedVerticalVelocity = -PreservedVerticalVelocity;
            this.gameObject.layer = LayerMask.NameToLayer("Kill Ball Layer");
            this.gameObject.GetComponent<SpriteRenderer>().material = DeflectedMaterial;
            BounceOffPaddle();
        }
        if (other.gameObject.CompareTag("BarrierRight") || other.gameObject.CompareTag("BarrierLeft")) BounceOffWall();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Donk");
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Audio.PlayDigitalBreak();
            collision.gameObject.SetActive(false);
            // Attempt to mitigate issue where rebound collision on barriers while moving upwards basically turns ball into a drill
            if(PreservedVerticalVelocity < 0) PreservedVerticalVelocity = -PreservedVerticalVelocity;

        }
        if (collision.gameObject.CompareTag("OuterBoundary")) Destroy(this.gameObject);
        if (collision.gameObject.CompareTag("GameOverObject"))
        {
            Audio.PlayDie();
            UI.EndScreenText.gameObject.SetActive(true);
            collision.gameObject.SetActive(false);
            Time.timeScale = 0;

        }
    }
    
    // Force Functions
    // 1. ShooAtAngle takes a angle and velocity argument and rotates the ball by the angle given, then fires at the given velocity
    // 2. ApplyDownwardForce used for testing just to throw ball down the field, might be useful when spawning the prefab via enemies
    // 3. BounceOffWall has specific trigger volume checks and a wall bounce mechanics since if the velocity is too shallow it just hugs the wall with regular Unity physics
    // 4. BounceOffPaddle takes the difference from the middle X position of the paddle and uses it to allow the player to aim when reflecting the ball
    public void ShootAtAngle(float angle, float velocity)
    {
        Vector2 direction = (Vector2)(Quaternion.Euler(0, 0, angle) * Vector3.right);
        GetComponent<Rigidbody2D>().linearVelocity = direction * velocity;
        PreservedVerticalVelocity = GetComponent<Rigidbody2D>().linearVelocityY;
    }

    void BounceOffWall()
    {
        BallRigidBody.linearVelocity = new Vector2(-BallRigidBody.linearVelocityX, BallRigidBody.linearVelocityY);
    }

    void BounceOffPaddle()
    {
        float differenceFromCenter = transform.position.x - Paddle.transform.position.x;
        BallRigidBody.linearVelocity = new Vector2(differenceFromCenter * 5f, -BallRigidBody.linearVelocityY);
    }

    // Update
    // 1. Constantly set ball velocity to preserved value since physics material tends to lose momentum per collision.
    void FixedUpdate()
    {
        BallRigidBody.linearVelocityY = PreservedVerticalVelocity;
    }
}
