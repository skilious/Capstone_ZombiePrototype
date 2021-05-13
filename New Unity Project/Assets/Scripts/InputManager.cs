using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private static InputManager _instance;

    public static InputManager instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        //Returns the Movement Vector value from PlayerControls script that has been generated when creating the input from the new input system.
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDetla()
    {
        //Returns the Look Vector2 value from PlayerControls script that has been generated when creating the input from the new input system.
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerCrouched()
    {
        return playerControls.Player.Crouch.triggered;
    }
}
