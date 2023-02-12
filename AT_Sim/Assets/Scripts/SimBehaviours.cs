using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimBehaviours : MonoBehaviour
{
    [SerializeField] private List<GameObject> locations;
    private GameObject food_location;
    private GameObject sleep_location;
    private GameObject hunt_location;
    private GameObject herb_location;
    private GameObject digging_location;

    private SimTraits traits_script;
    [SerializeField] private Sim_State current_state;
    private NavMeshAgent agent;

    [SerializeField] private float idle_timer_max = 5;
    private float idle_timer;
    private float eating_timer = 5f;
    private float hunting_timer = 5f;
    private float sleeping_timer = 5f;
    private float foraging_timer = 5f;

    private int idle_loops;

    private ResourcesManage resources;

    private float max_dist = 2f;

    public enum Sim_State
    {
        IDLE = 0,
        EATING = 1,
        HUNTING = 2,
        FIGHTING = 3,
        FORAGING = 4,
        SLEEPING = 5,
        DIGGING = 6,
        HERBING = 7,
        UNKNOWN = -1
    }
 

    private void Awake()
    {
        traits_script = gameObject.GetComponent<SimTraits>();
        current_state = Sim_State.IDLE;
        agent = gameObject.GetComponent<NavMeshAgent>();
        resources = GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        idle_timer = idle_timer_max;
        food_location = locations[0];
        sleep_location = locations[1];
        hunt_location = locations[2];
        herb_location = locations[3];
        digging_location = locations[4];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (current_state)
        {
            case Sim_State.IDLE:
                ResolveIdle();
                break;
            case Sim_State.EATING:
                ResolveEating();
                break;
            case Sim_State.HUNTING:
                ResolveHunting();
                break;
            case Sim_State.FIGHTING:
                break;
            case Sim_State.FORAGING:
                ResolveForaging();
                break;
            case Sim_State.SLEEPING:
                ResolveSleeping();
                break;
            case Sim_State.HERBING:
                ResolveHerbing();
                break;
            case Sim_State.DIGGING:
                ResolveDigging();
                break;
            case Sim_State.UNKNOWN:
                break;
        }
    }

    private void ResolveIdle()
    {
        traits_script.SetActivity(false);
        if (idle_timer <= 0)
        {
            idle_loops++;
            idle_timer = idle_timer_max;
            switch (traits_script.CheckDireNeeds())
            {
                case 0:
                    if (traits_script.GetPersonality() == SimTraits.Personality.HUNTER)
                    {
                        idle_loops = 0;
                        current_state = Sim_State.HUNTING;
                    }
                    else if (traits_script.GetPersonality() == SimTraits.Personality.FORAGER)
                    {
                        idle_loops = 0;
                        current_state = Sim_State.FORAGING;
                    }
                    break;
                case 1:
                    idle_loops = 0;
                    current_state = Sim_State.SLEEPING;
                    break;
                case 2:
                    idle_loops = 0;
                    current_state = Sim_State.EATING;
                    break;
                case 3:
                    idle_loops = 0;
                    current_state = Sim_State.SLEEPING;
                    break;
            }
            if(current_state == Sim_State.IDLE && idle_loops >= 3)
            {
                idle_loops = 0;
                if (resources.CheckResources("food") < 100)
                {
                    current_state = Sim_State.HUNTING;
                }
                else
                {
                    current_state = Sim_State.FORAGING;
                }
            }
        }
        else
        {
            agent.destination = transform.position;
            idle_timer -= Time.deltaTime;
        }
    }

    private void ResolveEating()
    {
        if (resources.CheckResources("food") >= 50)
        {
            if (CheckDistance(food_location) < max_dist)
            {
                eating_timer -= Time.deltaTime;
                if (eating_timer <= 0)
                {
                    current_state = Sim_State.IDLE;
                    eating_timer = 5f;
                    traits_script.UpdateNeeds("food", 100);
                    resources.UpdateResources("food", -50);
                }
            }
            else
            {
                agent.destination = food_location.transform.position;
            }
        }
        else
        {
            current_state = Sim_State.HUNTING; 
        }
    }

    private void ResolveSleeping()
    {
        if (CheckDistance(sleep_location) < max_dist)
        {
            sleeping_timer -= Time.deltaTime;
            if (sleeping_timer <= 0)
            {
                current_state = Sim_State.IDLE;
                sleeping_timer = 5f;
                traits_script.UpdateNeeds("sleep", 100);
                traits_script.UpdateNeeds("hp", 10);
            }
        }
        else
        {
            agent.destination = sleep_location.transform.position;
        }
    }

    private void ResolveForaging()
    {
        current_state = (resources.CheckResources("herbs") <= 1) ? Sim_State.HERBING : Sim_State.DIGGING;
    }

    private void ResolveHunting()
    {
        if (CheckDistance(hunt_location) < max_dist)
        {
            traits_script.SetActivity(true);
            hunting_timer -= Time.deltaTime;
            if (hunting_timer <= 0)
            {
                hunting_timer = 5f;
                resources.UpdateResources("food", 100);
                traits_script.UpdateNeeds("sleep", -20);
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            agent.destination = hunt_location.transform.position;
        }
    }

    private void ResolveHerbing()
    {
        if (CheckDistance(herb_location) < max_dist)
        {
            traits_script.SetActivity(true);
            foraging_timer -= Time.deltaTime;
            if (foraging_timer <= 0)
            {
                foraging_timer = 5f;
                resources.UpdateResources("herbs", 5);
                traits_script.UpdateNeeds("sleep", -10);
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            agent.destination = herb_location.transform.position;
        }
    }

    private void ResolveDigging()
    {
        if (CheckDistance(digging_location) < max_dist)
        {
            traits_script.SetActivity(true);
            foraging_timer -= Time.deltaTime;
            if (foraging_timer <= 0)
            {
                foraging_timer = 5f;
                resources.UpdateResources("mats", 100);
                traits_script.UpdateNeeds("sleep", -5);
                traits_script.UpdateNeeds("food", -5);
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            agent.destination = digging_location.transform.position;
        }
    }

    private float CheckDistance(GameObject location)
    {
        return Vector3.Distance(location.transform.position, transform.position);
    }

    public Sim_State GetState()
    {
        return current_state;
    }
}
