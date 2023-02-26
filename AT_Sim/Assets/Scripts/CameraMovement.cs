using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    PlayerInput controls;

    private Vector2 move;
    private Vector3 current_pos = Vector3.zero;
    private float camera_speed = 10f;
    private Vector2 mouse_pos;

    private Transform selected_char = null;
    [SerializeField] private LayerMask ignore_layer;

    void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;
        controls.Player.Look.performed += ctx => mouse_pos = ctx.ReadValue<Vector2>();
        controls.Player.Select.performed += ctx => Selection();
        controls.Player.Deselect.performed += ctx => Deselect();

        current_pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        current_pos.Set(current_pos.x + move.x * Time.deltaTime * camera_speed, current_pos.y, current_pos.z + move.y * Time.deltaTime * camera_speed);
        gameObject.transform.position = current_pos;
    }
    
    private void Selection()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouse_pos);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, 1000f, ~ignore_layer))
        {
            Debug.Log("Raycast hit - " + hit.transform.gameObject.name);

            if (selected_char == null && hit.transform.tag == "Villager")
            {
                AssignActiveChar(hit);
            }
            /*else if (selected_char != null && hit.transform.tag == "Floor")
            {
                Deselect();
            }*/
            else if (selected_char != null && hit.transform.gameObject.layer == 3)
            {
                
                LocationType location_type = hit.transform.GetComponent<LocationType>();
                //Debug.Log(location_type.GetLocationType() + " is selected by - " + selected_char.name + " ?");
                if (!location_type.IsFull())
                {
                    Debug.Log(location_type.GetLocationType() + " is available and now - " + selected_char.name + " is going to it");
                    switch (location_type.GetLocationType())
                    {
                        case LocationType.Location.DEFAULT:
                            break;
                        case LocationType.Location.HOME:
                            selected_char.GetComponent<SimBehaviours>().ForceBehaviour(SimBehaviours.Sim_State.SLEEPING);
                            break;
                        case LocationType.Location.FOODSPOT:
                            selected_char.GetComponent<SimBehaviours>().ForceBehaviour(SimBehaviours.Sim_State.EATING);
                            break;
                        case LocationType.Location.QUARRY:
                            selected_char.GetComponent<SimBehaviours>().ForceBehaviour(SimBehaviours.Sim_State.DIGGING);
                            break;
                        case LocationType.Location.FOREST:
                            selected_char.GetComponent<SimBehaviours>().ForceBehaviour(SimBehaviours.Sim_State.HUNTING);
                            break;
                        case LocationType.Location.FORAGRY:
                            selected_char.GetComponent<SimBehaviours>().ForceBehaviour(SimBehaviours.Sim_State.HERBING);
                            break;
                    }
                }

            }
            else if (selected_char == hit.transform)
            {
                Deselect();
            }
            else if (selected_char != hit.transform && hit.transform.tag == "Villager")
            {
                Deselect();
                AssignActiveChar(hit);
            }
        }
    }

    public Transform GetSelectedChar()
    {
        return selected_char;
    }
    private void AssignActiveChar(RaycastHit hit)
    {
        selected_char = hit.transform;
        selected_char.GetChild(0).gameObject.SetActive(true);
        Debug.Log("Current selected villager - " + hit.transform.gameObject.name);
    }
    private void Deselect()
    {
        if (selected_char != null)
        {
            selected_char.GetChild(0).gameObject.SetActive(false);
            selected_char = null;
        }
    }

    public void MagicCure()
    {
        if (selected_char != null)
        {
            selected_char.GetComponent<SimTraits>().MaxNeeds();
        }
    }

    public void ForceDamage()
    {
        if (selected_char != null)
        {
            selected_char.GetComponent<SimTraits>().HurtHealth();
        }
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
