using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationType : MonoBehaviour
{
    [SerializeField] private Location location_type;
    [SerializeField] private int capacity = 2;
    [SerializeField] private int resource_held = 0;
    private bool harvestable = false;
    private int in_location = 0;
    private bool is_full = false;
    [SerializeField] private float max_timer = 10f;
    private float resource_timer = 10f;
    [SerializeField] private int resource_gain = 100;
    private List<GameObject> user_list = new();
    public bool intersects = false;
    private Collider loc_collider;
    private Location_Manager loc_manager;
    public bool placed_down = true;

    public enum Location
    {
        DEFAULT,
        HOME,
        FORAGRY,
        QUARRY,
        FOREST,
        FOODSPOT
    }

    private void Start()
    {
        loc_manager = FindObjectOfType<Location_Manager>();
        loc_collider = GetComponent<Collider>();
        resource_timer = max_timer;
        switch (location_type)
        {
            case Location.DEFAULT:
                break;
            case Location.HOME:
                break;
            case Location.FORAGRY:
                harvestable = true;
                resource_held = 4;
                break;
            case Location.QUARRY:
                harvestable = true;
                resource_held = 5000;
                break;
            case Location.FOREST:
                harvestable = true;
                resource_held = 10000;
                break;
            case Location.FOODSPOT:
                break;
        }
    }

    private void Update()
    {
        if (!placed_down)
        {
            int intersection_points = 0;
            foreach (GameObject location in loc_manager.GetLocations())
            {
                if (loc_collider.bounds.Intersects(location.GetComponentInParent<Collider>().bounds))
                {
                    intersection_points++;
                }
            }
            if(intersection_points > 0)
            {
                intersects = true;
            }
            else
            {
                intersects = false;
            }
        }
        if (resource_timer <= 0)
        {
            resource_timer = max_timer;
            resource_held += resource_gain;
        }
        else
        {
            resource_timer -= Time.deltaTime * GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>().time_multiplier;
        }
    }
    private void UpdateIsFull()
    {
        is_full = user_list.Count < capacity ? false : true;
        /*Debug.Log("location name - "+gameObject.name);
        Debug.Log("how many users in location - "+user_list.Count);
        Debug.Log("how many users allowed in location - "+capacity);
        Debug.Log("is location full? "+is_full);
        Debug.Log("===================================================");*/
    }
    public bool IsFull()
    {
        return is_full;
    }


    public Location GetLocationType()
    {
        return location_type;
    }

    public int AddUser(GameObject user)
    {
        if (!user_list.Contains(user))
        {
            if (is_full)
            {
                //Debug.Log(gameObject.name + " is full, find another location");
                return -1;
            }
            user_list.Add(user);
            //Debug.Log("Added a user to - " + location_type);
            UpdateIsFull();
            return 1;
        }
        return 1;
    }

    public void RemoveUser(GameObject user)
    {
        if (user_list.Contains(user))
        {
            user_list.Remove(user);
            //Debug.Log("Removed a user from - " + location_type);
            UpdateIsFull();
        }

    }

    public int GetCapacity()
    {
        return capacity;
    }
}
