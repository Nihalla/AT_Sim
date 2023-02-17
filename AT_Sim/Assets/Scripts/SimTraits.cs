using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimTraits : MonoBehaviour
{
    [SerializeField] private Personality sim_personality;

    // Stats
    private float movement_speed = 5f;
    private float multiplier_bonus = 1f;
    private bool in_activity = false;
    public bool in_objective = false;

    // Needs
    private int hunger = 100;
    private int health = 100;
    private int exhaustion = 100;
    private int motivation = 100;

    // Timers
    private float timer_multiplier = 1f;
    [SerializeField] private float max_timer = 60f;
    private float needs_timer;
    
    public enum Personality
    {
        BASIC = 0,
        BRAVE = 1,
        HUNTER = 2,
        FORAGER = 3
    };

    // Start is called before the first frame update
    void Start()
    {
        needs_timer = max_timer;
    }

    // Update is called once per frame
    void Update()
    {
        needs_timer -= Time.deltaTime * timer_multiplier;
        if (needs_timer <= 0)
        {
            hunger -= 5;
            exhaustion -= 5;
            if(in_activity)
            {
                motivation -= 5;
                exhaustion -= 5;
            }

            
            if (hunger <= 0 || exhaustion <= 0)
            {
                health -= 5;
            }

            

            needs_timer = max_timer;
        }
        hunger = Mathf.Clamp(hunger, 0, 100);
        exhaustion = Mathf.Clamp(exhaustion, 0, 100);
        motivation = Mathf.Clamp(motivation, 0, 100);
        health = Mathf.Clamp(health, 0, 100);
    }

    public void SetActivity(bool value)
    {
        in_activity = value;
    }

    public float GetSpeed()
    {
        return movement_speed;
    }

    public Personality GetPersonality()
    {
        return sim_personality;
    }

    public int CheckDireNeeds()
    {
        if(health <= 20)
        {
            return 1;
        }
        if(hunger <= 50)
        {
            return 2;
        }
        if(exhaustion <= 30)
        {
            return 3;
        }

        return 0;
    }

    public int CheckNeed(string value_to_check)
    {
        switch (value_to_check)
        {
            case "food":
                return hunger;
            case "hp":
                return health;
            case "sleep":
                return exhaustion;
        }

        return 0;
    }    

    public void UpdateNeeds(string value_to_update, int value_inc)
    {
        switch (value_to_update)
        {
            case "hp":
                health += value_inc;
                break;
            case "sleep":
                exhaustion += value_inc;
                break;
            case "food":
                hunger += value_inc;
                break;
        }
    }
}
