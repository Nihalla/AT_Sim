using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimTraits : MonoBehaviour
{
    [SerializeField] private Personality sim_personality;
    private ResourcesManage manager_res_script;
    [SerializeField] private GameObject prefab;
    private List<string> names;
    private Vector3 default_scale;

    // Stats
    private float movement_speed = 5f;
    public int multiplier_bonus = 2;
    private bool in_activity = false;
    public bool in_objective = false;
    private bool can_have_children = true;
    public float attack_range = 1f;
    public int age = 20;

    // Needs
    private int hunger = 100;
    private int health = 100;
    private int exhaustion = 100;
    private int motivation = 100;

    // Timers
    private float timer_multiplier = 1f;
    [SerializeField] private float max_timer = 60f;
    private float needs_timer;
    [SerializeField] private float offspring_timer_max = 100f;
    private float offspring_timer;
    private float age_timer_max = 10f;
    private float age_timer;

    public enum Personality
    {
        BASIC = 0,
        BRAVE = 1,
        HUNTER = 2,
        FORAGER = 3
    };

    private void Awake()
    {
        default_scale = gameObject.transform.localScale;
        needs_timer = max_timer;
        offspring_timer = offspring_timer_max;
        age_timer = age_timer_max;
        manager_res_script = GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>();
        names = FindObjectOfType<Name_List>().name_list;
    }

    void Start()
    {
        
        gameObject.name = names[Random.Range(0 , names.Count)];
        if (sim_personality == Personality.BRAVE)
        {
            GetComponent<SphereCollider>().radius *= 2;
            attack_range *= 2;
        }
        if (Random.Range(1, 5) == 1)
        {
            can_have_children = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(age_timer <= 0)
        {
            age++;
            if (age == 20)
            {
                gameObject.transform.localScale = default_scale;
            }
            if (age == 80)
            {
                GetComponent<NavMeshAgent>().speed /= 2;
            }
            if (age >= 70)
            {
                int roll = Random.Range(1, 10);
                if (age >= 75)
                {
                    roll = Random.Range(1, 5);
                }
                if (age >= 80)
                {
                    roll = Random.Range(1, 3);
                }

                if (roll == 1)
                {
                    KillNPC();
                }
                else
                {
                    Debug.Log("DEATH ROLLED - " + roll);
                }
            }
            age_timer = age_timer_max;
        }
        else
        {
            age_timer -= Time.deltaTime * timer_multiplier;
        }
        if(timer_multiplier != manager_res_script.time_multiplier)
        {
            timer_multiplier = manager_res_script.time_multiplier;
        }
        needs_timer -= Time.deltaTime * timer_multiplier;
        if (needs_timer <= 0)
        {
            hunger -= 2;
            exhaustion -= 2;
            if(in_activity)
            {
                motivation -= 3;
                exhaustion -= 3;
            }

            
            if (hunger <= 0 || exhaustion <= 0)
            {
                health -= 2;
            }

            

            needs_timer = max_timer;
        }
        hunger = Mathf.Clamp(hunger, 0, 100);
        exhaustion = Mathf.Clamp(exhaustion, 0, 100);
        motivation = Mathf.Clamp(motivation, 0, 100);
        health = Mathf.Clamp(health, 0, 100);

        if( (can_have_children) && 
            (manager_res_script.CheckResources("v") < manager_res_script.village_capacity) &&
            (health > 50))
        {
            offspring_timer -= Time.deltaTime * timer_multiplier;
            if(offspring_timer <= 0)
            {
                offspring_timer = offspring_timer_max;
                SpawnCreature();
            }
        }
        
        if (health <= 0)
        {
            KillNPC();
        }
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

    public void MaxNeeds()
    {
        health = 100;
        exhaustion = 100;
        hunger = 100;
    }

    public void HurtHealth()
    {
        health -= 10;
    }

    public void SpawnCreature()
    {
        Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        manager_res_script.FindNewVillager();   
    }

    public int GetAge()
    {
        return age;
    }

    public void RandomizeStats()
    {
        age = 0;
        //Debug.Log(gameObject.transform.localScale / 2);
        gameObject.transform.localScale /= 2;
        int personality_roll;
        personality_roll = Random.Range(1, 4);
        switch(personality_roll)
        {
            case 1:
                sim_personality = Personality.BASIC;
                break;
            case 2:
                sim_personality = Personality.BRAVE;
                break;
            case 3:
                sim_personality = Personality.HUNTER;
                break;
            case 4:
                sim_personality = Personality.FORAGER;
                break;
        }
        needs_timer = max_timer;
        if (sim_personality == Personality.BRAVE)
        {
            GetComponent<SphereCollider>().radius *= 2;
            attack_range *= 2;
        }
        if (Random.Range(1, 5) == 1)
        {
            can_have_children = false;
        }
    }

    private void KillNPC()
    {
        manager_res_script.RemoveVillager(gameObject);
        GetComponent<Animation_Manager>().SetToSleep();
        Destroy(gameObject);
    }
}
