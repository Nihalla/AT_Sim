using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Manager : MonoBehaviour
{
    private GameObject[] temp_array;
    [SerializeField] private List<GameObject> locations = new();
    private List<GameObject> food_locations = new();
    private List<GameObject> sleep_locations = new();
    private List<GameObject> hunt_locations = new();
    private List<GameObject> herb_locations = new();
    private List<GameObject> digging_locations = new();
    private GameObject object_to_add;

    private void Awake()
    {
        LocationType script;
        temp_array = GameObject.FindGameObjectsWithTag("LocationPoint");
        foreach(GameObject element in temp_array)
        {
            locations.Add(element);
        }

        foreach (GameObject unsorted_location in locations)
        {        
            script = unsorted_location.GetComponentInParent<LocationType>();
            object_to_add = unsorted_location;
            switch (script.GetLocationType())
            {
                case LocationType.Location.DEFAULT:
                    Debug.Log("SHOULD NOT HAPPEN!");
                    break;
                case LocationType.Location.HOME:
                    sleep_locations.Add(object_to_add);
                    break;
                case LocationType.Location.FORAGRY:
                    herb_locations.Add(object_to_add);
                    break;
                case LocationType.Location.QUARRY:
                    digging_locations.Add(object_to_add);
                    break;
                case LocationType.Location.FOREST:
                    hunt_locations.Add(object_to_add);
                    break;
                case LocationType.Location.FOODSPOT:
                    food_locations.Add(object_to_add);
                    break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public List<GameObject> GetLocationsOfType(LocationType.Location location_to_find)
    {
        switch (location_to_find)
        { 
            case LocationType.Location.DEFAULT:
                break;
            case LocationType.Location.HOME:
                return sleep_locations;
            case LocationType.Location.FORAGRY:
                return herb_locations;
            case LocationType.Location.QUARRY:
                return digging_locations;
            case LocationType.Location.FOREST:
                return hunt_locations;
            case LocationType.Location.FOODSPOT:
                return food_locations;
        }
        return null;
    }    

    public void AssignLocation(GameObject location)
    {
        LocationType script;
        script = location.GetComponentInParent<LocationType>();
        switch (script.GetLocationType())
        {
            case LocationType.Location.DEFAULT:
                break;
            case LocationType.Location.HOME:
                sleep_locations.Add(location);
                break;
            case LocationType.Location.FORAGRY:
                herb_locations.Add(location);
                break;
            case LocationType.Location.QUARRY:
                digging_locations.Add(location);
                break;
            case LocationType.Location.FOREST:
                hunt_locations.Add(location);
                break;
            case LocationType.Location.FOODSPOT:
                food_locations.Add(location);
                break;
        }
    }
}
