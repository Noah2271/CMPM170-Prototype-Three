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
    // 1. Paddle Trigger : Checks if paddle in deflect mode, if so, return ball with 3x opposite force, add to layer that kills enemies, and change the material and activate trail renderer.
    //    If not, timescale 0, disable paddle, play die sound, and display dead text. This check also checks if ball deflected by player so player cannot accidentally snipe themselves.
    // 2. Barrier Right/Left Trigger : Call bounce wall bounce physics update function
    // 3. Breakable Collision : Left over from Prototype 3.1
    // 4. Outer Boundary Collision : Big brick on the top outside the screen, just deletes balls that aren't in use.
    // 5. GameOverObject Collision : Left over from Prototype 3.1
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Paddle")) {
            if ((other.gameObject == Paddle) && (Paddle.GetComponent<PaddleMovement>().DeflectOn == true)) 
            {
                Audio.PlayReflect();
                PreservedVerticalVelocity = 3*-PreservedVerticalVelocity;
                this.gameObject.layer = LayerMask.NameToLayer("Kill Ball Layer");
                this.gameObject.GetComponent<SpriteRenderer>().material = DeflectedMaterial;
                this.gameObject.GetComponent<TrailRenderer>().enabled = true;
                BounceOffPaddle(); 
            }
            if ((other.gameObject == Paddle) && (Paddle.GetComponent<PaddleMovement>().DeflectOn == false) && (this.gameObject.layer != LayerMask.NameToLayer("Kill Ball Layer")))
            {
                UI.EndScreenText.gameObject.SetActive(true);
                UI.EndSCreenTextOverlay.gameObject.SetActive(true);
                other.gameObject.SetActive(false);
                Time.timeScale = 0;
                Audio.PlayDie();
            }
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
        BallRigidBody.linearVelocityX = (differenceFromCenter * 10f);
    }

    // Update
    // 1. Constantly set ball velocity to preserved value since physics material tends to lose momentum per collision.
    void FixedUpdate()
    {
        BallRigidBody.linearVelocityY = PreservedVerticalVelocity;
    }
}
