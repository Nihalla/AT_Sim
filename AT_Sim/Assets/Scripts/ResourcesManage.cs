using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResourcesManage : MonoBehaviour
{
    [SerializeField] private int building_mats = 100;
    [SerializeField] private int food = 100;
    [SerializeField] private int herbs = 5;
    [SerializeField] private int population = 0;
    [SerializeField] private GameObject raid_location;
    [SerializeField] private GameObject enemy_prefab;
    public float time_multiplier = 1;
    private List<GameObject> villager_list = new();
    [SerializeField] private float raid_timer_max = 100f;
    private float raid_timer;
    public int village_capacity = 0;

    void Start()
    {
        raid_timer = raid_timer_max;
        foreach(GameObject villager in GameObject.FindGameObjectsWithTag("Villager"))
        {
            villager_list.Add(villager);
        }
        population = villager_list.Count;
        foreach(GameObject home in gameObject.GetComponent<Location_Manager>().GetLocationsOfType(LocationType.Location.HOME))
        {
            village_capacity += home.GetComponentInParent<LocationType>().GetCapacity();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (raid_timer >= 0)
        {
            raid_timer -= Time.deltaTime * time_multiplier;
        }
        else
        {
            SpawnRaid();
            raid_timer = raid_timer_max;  
        }
    }

    public int CheckResources(string value_to_check)
    {
        switch (value_to_check)
        {
            case "food":
                return food;
            case "mats":
                return building_mats;
            case "herbs":
                return herbs;
            case "v":
                return population;
        }

        return 0;
    }

    public void UpdateResources(string value_to_update, int value_inc)
    {
        switch (value_to_update)
        {
            case "food":
                food += value_inc;
                break;
            case "mats":
                building_mats += value_inc;
                break;
            case "herbs":
                herbs += value_inc;
                break;
        }
    }

    public void AddToPopulation(GameObject new_villager)
    {
        villager_list.Add(new_villager);
        population++;
    }

    public void RemoveVillager(GameObject villager_to_remove)
    {
        villager_list.Remove(villager_to_remove);
        population--;
    }

    public void UpdateCapacity(int spots)
    {
        village_capacity += spots;
    }

    public List<GameObject> GetVillagers()
    {
        return villager_list;
    }

    public void SpawnRaid()
    {
        int number_to_spawn = population / 2;
        for(int i = 0; i < number_to_spawn; i++)
        {
            Vector3 position = new Vector3(
                Random.Range((raid_location.transform.position.x - raid_location.transform.localScale.x / 2), (raid_location.transform.position.x + raid_location.transform.localScale.x / 2)), 
                0.25f,
                Random.Range((raid_location.transform.position.z - raid_location.transform.localScale.z / 2), (raid_location.transform.position.z + raid_location.transform.localScale.z / 2)));
            Instantiate(enemy_prefab, position, Quaternion.identity);
        }
    }

    public void FindNewVillager()
    {
        Debug.Log("Looking for new villager!");
        foreach (GameObject villager in GameObject.FindGameObjectsWithTag("Villager"))
        {
            if(!villager_list.Contains(villager))
            {
                Debug.Log("New villager found, welcome to the family - " + villager.name);
                villager.GetComponent<SimTraits>().RandomizeStats();
                AddToPopulation(villager);
            }
        }
    }

    public void UnlimitedResources()
    {
        building_mats += 999999;
        food += 999999;
    }
    public void BoostTime()
    {
        if(time_multiplier == 1)
        {
            time_multiplier = 2;
            foreach(GameObject villager in villager_list)
            {
                villager.GetComponent<NavMeshAgent>().speed *= 2;
            }
        }
        else
        {
            time_multiplier = 1;
            foreach (GameObject villager in villager_list)
            {
                villager.GetComponent<NavMeshAgent>().speed /= 2;
            }
        }
    }
}
