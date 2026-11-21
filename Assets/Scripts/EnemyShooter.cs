using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    // Enemy settings
    public GameObject ballPrefab;
    public float shootInterval = 3f;
    public float respawnTime = 5f;
    public EnemyParent.part part;
    public bool isDead = false;

    // Controls the spread and speed of enemy bullets
    public float minShootAngle = 240f;
    public float maxShootAngle = 300f;
    public float bulletSpeed = 5f;

    private Audio_Manager Audio;
    private SpriteRenderer enemySprite;
    private Collider2D enemyCollider;


    void Start()
    {
        // Get the enemy's sprite and collider, along with audio manager
        Audio = GameObject.Find("Audio_Manager").GetComponent<Audio_Manager>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
    }

    public void ShootBall()
    {
        // Only shoot if the enemy is alive
        if (!isDead)
        {
            // Spawn a ball below the enemy
            Vector3 spawnPosition = transform.position + Vector3.down * .6f;

            Audio.PlayFire();
            Debug.Log("Ball");

            GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

            // Generate a random angle within the downward cone
            float randomAngle = Random.Range(minShootAngle, maxShootAngle);

            // make ball larger with slower speed
            if (part == EnemyParent.part.CENTER)
            {
                newBall.transform.localScale = new Vector3(.5f, .5f, .5f);
                newBall.GetComponent<BallBehaviour>().ShootAtAngle(randomAngle, bulletSpeed * 1.2f);
            }
            else
            {
                // Shoot the ball using the random angle and speed
                newBall.GetComponent<BallBehaviour>().ShootAtAngle(randomAngle, bulletSpeed);
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the enemy was hit by a ball
        if (collision.gameObject.CompareTag("Ball") && !isDead)
        {
            collision.gameObject.GetComponent<BallBehaviour>().PreservedVerticalVelocity = -collision.gameObject.GetComponent<BallBehaviour>().PreservedVerticalVelocity;
            Audio.PlayEnemyExplode();

            if (part == EnemyParent.part.CENTER)
            {
                this.transform.GetComponentInParent<EnemyParent>().BreakShip();
            }

            Die();
        }
    }

    public void Die()
    {
        // Mark the enemy as dead
        isDead = true;

        // Hide the enemy and disable its collider
        enemySprite.enabled = false;
        enemyCollider.enabled = false;

        // lets the parent know a part has been broken
        this.GetComponentInParent<EnemyParent>().BreakPart();
    }

    public void Respawn()
    {
        // Make the enemy visible again
        enemySprite.enabled = true;
        enemyCollider.enabled = true;

        // Allow the enemy to shoot and be hit again
        isDead = false;
    }
}
