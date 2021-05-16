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
    private readonly float crouchSpeed = 3.0f;
    private readonly float walkSpeed = 8.0f;
    private readonly float gravityValue = -9.81f;

    private InputManager inputManager;
    private Transform cameraTransform;

    //Height value when crouched.
    private float crouchHeight = 1.0f;
    //Height value when standing.
    private float defaultHeight = 2.0f;

    private static bool actionTriggered = false;

    public static bool ActionTriggered
    {
        get { return actionTriggered; }
    }
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

        bool underCeiling = Physics.Raycast(controller.transform.position, Vector3.up, 2.0f);

        if (inputManager.PlayerCrouched() && !underCeiling)
        {
            if (actionTriggered)
            {
                actionTriggered = false;
            }
            else
            {
                actionTriggered = true;
            }
        }

        if (actionTriggered || underCeiling)
        {
            print("Crouching");
            controller.height = Mathf.MoveTowards(controller.height, crouchHeight, Time.deltaTime / 0.3f);
            playerSpeed = crouchSpeed;
        }
        else if (!actionTriggered)
        {
            print("Standing");
            controller.height = Mathf.MoveTowards(controller.height, defaultHeight, Time.deltaTime / 0.2f);
            playerSpeed = walkSpeed;
        }
        camTransposer.m_FollowOffset.y = actionTriggered || underCeiling ? HeightChange(-1.0f, 0.3f) : HeightChange(0.0f, 0.2f);
        controller.center = Vector3.down * (defaultHeight - controller.height) / 2.0f;
    }

    private float HeightChange(float N, float speed)
    {
        float results = Mathf.MoveTowards(camTransposer.m_FollowOffset.y, N, Time.deltaTime / speed);
        return results;
    }
}
