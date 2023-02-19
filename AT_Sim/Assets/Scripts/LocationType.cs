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
        if(resource_timer <= 0)
        {
            resource_timer = max_timer;
            resource_held += resource_gain;
        }
        else
        {
            resource_timer -= Time.deltaTime;
        }
    }
    private void UpdateIsFull()
    {
        is_full = in_location < capacity ? false : true;
    }
    public bool IsFull()
    {
        return is_full;
    }


    public Location GetLocationType()
    {
        return location_type;
    }

    public void AddUser()
    {
        in_location++;
        UpdateIsFull();
    }

    public void RemoveUser()
    {
        in_location--;
        UpdateIsFull();
    }

    public int GetCapacity()
    {
        return capacity;
    }

 /*   private void OnTriggerEnter(Collider collision)
    {
        if (!is_full)
        {
            if (collision.gameObject.tag == "Villager")
            {
                
                    in_location++;
                    UpdateIsFull();
                
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!is_full)
        {
            if (collision.gameObject.tag == "Villager")
            {
                in_location--;
                UpdateIsFull();
            }
        }
    }*/

}
