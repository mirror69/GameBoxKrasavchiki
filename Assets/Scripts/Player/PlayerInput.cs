using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private Vector3 moveDirection;
    private Vector3 rotateDirection;
    private float horizontalInput;
    private float verticalInput;
    private PlayerController playerController;
    

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rotateDirection = Input.mousePosition;
        if(Input.GetButtonDown("Fire1")) playerController.PlayerShoot();
    }

    private void FixedUpdate()
    {
        moveDirection = new Vector3(-horizontalInput, 0, -verticalInput);        

        playerController.MovePlayer(moveDirection);
        playerController.RotatePlayer(rotateDirection);

    }
}
