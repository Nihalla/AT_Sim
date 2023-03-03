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

    private bool in_build_mode = false; 
    private GameObject selected_build = null;
    private BuildingOptions build_type_script;
    private GameObject building_to_place = null;
    [SerializeField] private LayerMask ignore_layer;
    private Vector3 selection_XY;
    public GameObject ground;
    Plane plane;

    void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;
        controls.Player.Look.performed += ctx => mouse_pos = ctx.ReadValue<Vector2>();
        controls.Player.Select.performed += ctx => Selection();
        controls.Player.Deselect.performed += ctx => Deselect();

        current_pos = gameObject.transform.position;

        build_type_script = FindObjectOfType<BuildingOptions>();
    }

    private void Start()
    {
        plane = new Plane(ground.transform.up, Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        current_pos.Set(current_pos.x + move.x * Time.deltaTime * camera_speed, current_pos.y, current_pos.z + move.y * Time.deltaTime * camera_speed);
        gameObject.transform.position = current_pos;

        var ray = Camera.main.ScreenPointToRay(mouse_pos);
        float ent;
        if(plane.Raycast(ray, out ent))
        {
            selection_XY = ray.GetPoint(ent);
            if (in_build_mode && building_to_place != null)
            {
                building_to_place.transform.position = selection_XY;
            }

        }

        /*if (in_build_mode && building_to_place != null)
        {
            Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mouse_pos);
            selection_XY = Worldpos;
            RaycastHit hit;
            Debug.DrawRay(Camera.main.transform.position, new Vector3(selection_XY.x, 0, selection_XY.y) - Camera.main.transform.position, Color.red);

            //if (Physics.Raycast(Camera.main.transform.position, new Vector3(selection_XY.x, 0, selection_XY.y) - Camera.main.transform.position, out hit))
            //{
                //Transform objectHit = hit.transform;

                //if (objectHit.gameObject.tag == "Floor")
                //{
                    building_to_place.transform.position = new Vector3(selection_XY.x, 0, selection_XY.y);
                //}
            //}
        }*/

        //Ray ray = Camera.main.ScreenPointToRay(mouse_pos);

    }

    private void Selection()
    {

        Ray ray = Camera.main.ScreenPointToRay(mouse_pos);
        RaycastHit hit;

        if (!in_build_mode)
        {
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
        else
        {
            if (!building_to_place.GetComponent<LocationType>().intersects)
            {
                building_to_place.transform.position = selection_XY;
                if(building_to_place.GetComponent<LocationType>().GetLocationType() == LocationType.Location.HOME)
                {
                    FindObjectOfType<ResourcesManage>().UpdateCapacity(4);
                }
                FindObjectOfType<Location_Manager>().AddLocation(building_to_place);

                building_to_place = null;
                selected_build = null;
                in_build_mode = false;
            }
            else
            { Debug.Log("SCREEEEEEEEEEEEEEEEEEEEEEEEE"); }
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

    public void PlaceHomeBuilding()
    {
        Deselect();
        in_build_mode = true;
        selected_build = build_type_script.home_prefab;
        building_to_place = Instantiate(selected_build, Vector3.zero, Quaternion.identity);
        building_to_place.GetComponent<LocationType>().placed_down = false;
    }

    public void PlaceFoodBulding()
    {
        Deselect();
        in_build_mode = true;
        selected_build = build_type_script.food_prefab;
        building_to_place = Instantiate(selected_build, Vector3.zero, Quaternion.identity);
        building_to_place.GetComponent<LocationType>().placed_down = false;
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
