using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    public CinemachineTransposer camTransposer;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    protected float playerSpeed = 0.0f;
    private readonly float crouchSpeed = 1.0f;
    private readonly float walkSpeed = 50.0f;
    private readonly float gravityValue = -9.81f;

    private InputManager inputManager;
    private Transform cameraTransform;

    //Height value when crouched.
    private float crouchHeight = 1.0f;
    //Height value when standing.
    private float defaultHeight = 2.0f;

    public bool actionTriggered = false;
    private void Start()
    {
        camTransposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        playerSpeed = walkSpeed;
        cameraTransform = Camera.main.transform;
        inputManager = InputManager.instance;
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (inputManager.PlayerCrouched())
        {
            if (actionTriggered)
            {
                actionTriggered = false;
            }
            else
                actionTriggered = true;
        }

        if (actionTriggered)
        {
            print("Crouching");
            controller.height = Mathf.MoveTowards(controller.height, crouchHeight, Time.deltaTime / 0.5f);

            playerSpeed = crouchSpeed;
        }
        else if (!actionTriggered)
        {
            print("Standing");
            controller.height = Mathf.MoveTowards(controller.height, defaultHeight, Time.deltaTime / 0.5f); 
            playerSpeed = walkSpeed;
        }
        camTransposer.m_FollowOffset.y = actionTriggered ? HeightChange(-1.0f) : HeightChange(0.0f);
        controller.center = Vector3.down * (defaultHeight - controller.height) / 2.0f;
    }

    private float HeightChange(float N)
    {
        float results = Mathf.MoveTowards(camTransposer.m_FollowOffset.y, N, Time.deltaTime / 0.5f);
        return results;
    }
}
