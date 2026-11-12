using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PaddleMovement : MonoBehaviour
{
    private GameObject Paddle;
    private GameObject BorderLeft;
    private GameObject BorderRight;
    private bool freezeRight = false;
    private bool freezeLeft = false;
    private InputAction Movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Paddle = GameObject.Find("Paddle");
        BorderRight = GameObject.Find("BorderRight");
        BorderLeft = GameObject.Find("BorderLeft");
        Movement = InputSystem.actions.FindActionMap("Player").FindAction("Move");

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Wall hit");
        Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

        if (other.gameObject.CompareTag("BarrierRight")) freezeRight = true;
        if (other.gameObject.CompareTag("BarrierLeft")) freezeLeft = true;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Wall hit");
        if (other.gameObject.CompareTag("BarrierRight")) freezeRight = false;
        if (other.gameObject.CompareTag("BarrierLeft")) freezeLeft = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Movement.ReadValue<Vector2>().x != 0)
        {

            if (Movement.ReadValue<Vector2>().x > 0 && !freezeRight)
            {
                Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(2.5f, 0);
            }
            if (Movement.ReadValue<Vector2>().x < 0 && !freezeLeft)
            {
                Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-2.5f, 0);
            }
        }
        else
        {
            Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
        }


}
}
