using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    // Enemy settings
    public GameObject ballPrefab;
    public float shootInterval = 3f;
    public float respawnTime = 5f;

    private SpriteRenderer enemySprite;
    private Collider2D enemyCollider;
    private bool isDead = false;

    void Start()
    {
        // Get the enemy's sprite and collider
        enemySprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        // Start the shooting coroutine
        StartCoroutine(ShootBalls());
    }

    IEnumerator ShootBalls()
    {
        while (true)
        {
            // Wait before shooting another ball
            yield return new WaitForSeconds(shootInterval);

            // Only shoot if the enemy is alive
            if (!isDead)
            {
                // Spawn a ball below the enemy
                Vector3 spawnPosition = transform.position + Vector3.down;

                GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

                // Give the ball a random horizontal speed
                float randomX = Random.Range(-2f, 2f);

                // Set the random direction for the spawned ball
                newBall.GetComponent<BallBehaviour>().horizontalSpeed = randomX;
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the enemy was hit by a ball
        if (collision.gameObject.CompareTag("Ball") && !isDead)
        {
            // Start the respawn coroutine
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        // Mark the enemy as dead
        isDead = true;

        // Hide the enemy and disable its collider
        enemySprite.enabled = false;
        enemyCollider.enabled = false;

        // Wait 5 seconds before respawning
        yield return new WaitForSeconds(respawnTime);

        // Make the enemy visible again
        enemySprite.enabled = true;
        enemyCollider.enabled = true;

        // Allow the enemy to shoot and be hit again
        isDead = false;
    }
}
