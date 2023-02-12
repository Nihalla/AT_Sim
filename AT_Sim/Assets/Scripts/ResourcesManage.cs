using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManage : MonoBehaviour
{
    [SerializeField] private int building_mats = 100;
    [SerializeField] private int food = 100;
    [SerializeField] private int herbs = 5;
    [SerializeField] private int population = 0;
    private float raid_timer;
    void Start()
    {
        population = GameObject.FindGameObjectsWithTag("Villager").Length;
    }

    // Update is called once per frame
    void Update()
    {

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
}
