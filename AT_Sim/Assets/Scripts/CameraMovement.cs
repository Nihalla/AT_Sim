using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    PlayerInput controls;

    private Vector2 move;
    private Vector3 current_pos = Vector3.zero;
    private float camera_speed = 10f;

    void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        current_pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        current_pos.Set(current_pos.x + move.x * Time.deltaTime * camera_speed, current_pos.y, current_pos.z + move.y * Time.deltaTime * camera_speed);
        gameObject.transform.position = current_pos;
    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }
    public void EnableInput()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    public void DisableInput()
    {
        controls.Player.Disable();
    }

}
