using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float movingStoppedTime;
    [SerializeField, Range(0, 1)] private float shootingCullDownTime;
    private Vector3 moveDirection;
    private Vector3 rotateDirection;
    private float horizontalInput;
    private float verticalInput;
    private PlayerController playerController;
    private bool canMoving = true;
    private bool canShooting = true;
    private bool isPushingButton = false;

    //timers
    private float buttonPressedStartTime;

    

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rotateDirection = Input.mousePosition;
        
        if(Input.GetButtonDown("Fire1"))
        {
            isPushingButton = true;
            buttonPressedStartTime = Time.time;
        }

        if (Input.GetButtonUp("Fire1") && isPushingButton)
        {
            Fire();
            isPushingButton = false;
        }

        if ((Time.time - buttonPressedStartTime > 5) && isPushingButton)
        {
            isPushingButton = false;
            Fire();
        } 
    }

    private void Fire()
    {
        canMoving = false;
        if (canShooting)
        {
            playerController.PlayerShoot(Time.time - buttonPressedStartTime);
            
            StartCoroutine(CullDownWaiting());
            StartCoroutine(MovingStopper());
        }
        buttonPressedStartTime = Time.time;
    }

    private void FixedUpdate()
    {
        moveDirection = new Vector3(-horizontalInput, 0, -verticalInput);        
        if (canMoving) playerController.MovePlayer(moveDirection);
        playerController.RotatePlayer(rotateDirection);

    }

    IEnumerator MovingStopper()
    {
        canMoving = false;
        yield return new WaitForSeconds(movingStoppedTime);
        canMoving = true;
    }

    IEnumerator CullDownWaiting()
    {
        canShooting = false;
        yield return new WaitForSeconds(shootingCullDownTime);
        canShooting = true;

    }
}
