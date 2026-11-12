using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PaddleMovement : MonoBehaviour
{
    private GameObject Paddle;
    private InputAction Movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Paddle = GameObject.Find("Paddle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
