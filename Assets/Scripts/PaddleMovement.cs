using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.IO;
using JetBrains.Annotations;

public class PaddleMovement : MonoBehaviour
{
    private UI_Manager UI;
    private GameObject Paddle;
    private GameObject BorderLeft;
    private GameObject BorderRight;
    public Material DefaultMaterial;
    public Material DeflectMaterial;
    private bool Started = false;
    public Material OffMaterial;
    public bool DeflectOn = false;
    public bool DeflectAvailable = false;
    private bool freezeRight = false;
    private bool freezeLeft = false;
    public GameObject[] Indicators = new GameObject[5];
    private int WorkingIndicatorIndex = 0;
    private InputAction Movement;
    private InputAction ToggleDeflect;
    private InputAction StartKey;
    private float paddleWidth;

    void Start()
    {
        Time.timeScale = 0;
        Paddle = GameObject.Find("Paddle");
        UI = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        BorderRight = GameObject.Find("BorderRight");
        BorderLeft = GameObject.Find("BorderLeft");
        Movement = InputSystem.actions.FindActionMap("Player").FindAction("Move");
        ToggleDeflect = InputSystem.actions.FindActionMap("Player").FindAction("ToggleDeflect");
        StartKey = InputSystem.actions.FindActionMap("Player").FindAction("Start");
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

    // Deflection Handling
    // 1. DeflectCooldown is coroutine that reactivates the player ability to use deflect after X amount of time, right now it's 10. If adjusting, please only use multiples of 10.
    // 2. DeflectDuration counts down how long the player stays in the deflect state, turns off deflect state after 5 seconds. If adjusting, please only use multiples of 5.
    // 3. IndicatorHandling handles the visual indicator for the deflect state, the WaitForSeconds() values of both types will have to be manually adjusted if any of the above are adjusted.
    // 4. EnableDeflect just turns on the deflect state and starts the tracking coroutines.
    // 5. StartGame is used for the new start screen and also starts the initial coroutines.
    // 6. Update is used for tracking player input for deflect and start.
    IEnumerator DeflectCooldown(float length)
    {
        yield return new WaitForSeconds(length);
        UI.ChargeText.color = new Color(0f, 1f, 1f, 1f);
        DeflectAvailable = true;
    }

    IEnumerator DeflectDuration()
    {
        yield return new WaitForSeconds(5f);
        this.gameObject.GetComponent<SpriteRenderer>().material = DefaultMaterial;
        StartCoroutine(DeflectCooldown(10));
        StartCoroutine(IndicatorHandling("Charge"));
        UI.ChargeText.color = new Color(0.537f, 0.537f, 0.537f, 1f);
        DeflectOn = false;
    }

    IEnumerator IndicatorHandling(string type)
    {
        if (type == "Use")
        {
            for (int i = Indicators.Length; i >= 0; i--)
            {
                if (i != Indicators.Length) Indicators[i].GetComponent<SpriteRenderer>().material = OffMaterial;

                yield return new WaitForSeconds(1f);
            }
        }
        else if (type == "Charge")
        {
            for (int i = -1; i < Indicators.Length; i++)
            {
                if ( i != -1 ) Indicators[i].GetComponent<SpriteRenderer>().material = DeflectMaterial;

                yield return new WaitForSeconds(2f);
            }
        }
    }

    public void EnableDeflect()
    {
        DeflectOn = true;
        DeflectAvailable = false;
        this.gameObject.GetComponent<SpriteRenderer>().material = DeflectMaterial;
        StartCoroutine(IndicatorHandling("Use"));
        StartCoroutine(DeflectDuration());
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        Started = true;
        UI.StartText.gameObject.SetActive(false);
        StartCoroutine(DeflectCooldown(10));
        StartCoroutine(IndicatorHandling("Charge"));
    }

    public void Update()
    {
        if(ToggleDeflect.IsPressed() && DeflectOn == false && DeflectAvailable == true)
        {
            EnableDeflect();
        }
        if(!Started && StartKey.IsPressed())
        {
            StartGame();
        }
    }
}
