using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.IO;

public class PaddleMovement : MonoBehaviour
{
    private GameObject Paddle;
    private GameObject BorderLeft;
    private GameObject BorderRight;
    private bool freezeRight = false;
    private bool freezeLeft = false;
    private InputAction Movement;
    private float paddleWidth;

    void Start()
    {
        Paddle = GameObject.Find("Paddle");
        BorderRight = GameObject.Find("BorderRight");
        BorderLeft = GameObject.Find("BorderLeft");
        Movement = InputSystem.actions.FindActionMap("Player").FindAction("Move");
        Cursor.visible = false;
    }

    // 1. OnTriggerEnter stops velocity and freezes directional movement controls if touch border wall
    // 2. OnTriggerExit unfreezes the controls
    // 3. FixedUpdate just moves the paddle and takes away velocity if controls released
    public void OnTriggerEnter2D(Collider2D other)
    {
        Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
        if (other.gameObject.CompareTag("BarrierRight")) freezeRight = true;
        if (other.gameObject.CompareTag("BarrierLeft")) freezeLeft = true;
        paddleWidth = Paddle.GetComponent<Renderer>().bounds.size.x;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BarrierRight")) freezeRight = false;
        if (other.gameObject.CompareTag("BarrierLeft")) freezeLeft = false;
    }

    void FixedUpdate() {
        //arrow keys
        if (Movement.ReadValue<Vector2>().x != 0)
        {
            if (Movement.ReadValue<Vector2>().x > 0 && !freezeRight)
            {
                Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(6f, 0);
            }
            if (Movement.ReadValue<Vector2>().x < 0 && !freezeLeft)
            {
                Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-6f, 0);
            }
        }
        // mouse movement
        else if (Mouse.current != null)
        {
            Vector2 movement = Mouse.current.delta.ReadValue();
            if ((movement.x < 0 && freezeLeft) || (movement.x > 0 && freezeRight))
            {
                movement = Vector2.zero;
                Paddle.transform.position = new Vector2(Mathf.Clamp(Paddle.transform.position.x, -3.2f + paddleWidth / 2, 3.06f - paddleWidth / 2), Paddle.transform.position.y);
            }
            Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(movement.x, 0);
        }
        else
        {
            Paddle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

        }


}
}
