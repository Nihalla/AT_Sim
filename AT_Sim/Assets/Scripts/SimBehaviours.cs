using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimBehaviours : MonoBehaviour
{
    private GameObject nearest_food_location;
    private GameObject nearest_sleep_location;
    private GameObject nearest_hunt_location;
    private GameObject nearest_herb_location;
    private GameObject nearest_digging_location;

    private SimTraits traits_script;
    [SerializeField] private Sim_State current_state;
    private NavMeshAgent agent;

    private float timer_multiplier = 1f;

    [SerializeField] private float idle_timer_max = 5;
    private float idle_timer;
    private float eating_timer = 5f;
    private float hunting_timer = 5f;
    private float sleeping_timer = 5f;
    private float foraging_timer = 5f;

    private int idle_loops;

    private ResourcesManage resources;
    private Location_Manager location_manager;

    private GameObject target;
    private float attack_timer = 1f;

    private float max_dist = 2.5f;

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
        location_manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Location_Manager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        /* idle_timer = idle_timer_max;
         food_location = locations[0];
         sleep_location = locations[1];
         hunt_location = locations[2];
         herb_location = locations[3];
         digging_location = locations[4];*/
    }

    // Update is called once per frame

    private void Update()
    {
        if(timer_multiplier != resources.time_multiplier)
        {
            timer_multiplier = resources.time_multiplier;
        }
    }
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
                ResolveFighting();
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
        traits_script.in_objective = false;
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
            if (current_state == Sim_State.IDLE && idle_loops >= 3)
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
            idle_timer -= Time.deltaTime * timer_multiplier;
        }
    }

    private void ResolveEating()
    {

        if (resources.CheckResources("food") >= 50)
        {
            if (nearest_food_location == null)
            {
                float closest_dist = 999f;
                foreach (GameObject point in location_manager.GetLocationsOfType(LocationType.Location.FOODSPOT))
                {
                    if (!point.GetComponentInParent<LocationType>().IsFull())
                    {
                        float dist = CheckDistance(point);
                        if (dist < closest_dist)
                        {
                            closest_dist = dist;
                            nearest_food_location = point;
                        }
                    }
                }
            }
            else
            {
                if (nearest_food_location.GetComponentInParent<LocationType>().AddUser(gameObject) == 1)
                {
                    if (CheckDistance(nearest_food_location) < max_dist)
                    {
                        if (!traits_script.in_objective)
                        {

                            traits_script.in_objective = true;
                        }

                        eating_timer -= Time.deltaTime * timer_multiplier;
                        if (eating_timer <= 0)
                        {
                            current_state = Sim_State.IDLE;
                            eating_timer = 5f;
                            traits_script.UpdateNeeds("food", 100);
                            resources.UpdateResources("food", -50);
                            nearest_food_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
                            nearest_food_location = null;
                        }
                    }
                    else
                    {
                        agent.destination = nearest_food_location.transform.position;
                    }
                }
                else
                {
                    nearest_food_location = null;
                }
            }
        }
        else
        {
            current_state = Sim_State.HUNTING;
        }
    }

    private void ResolveSleeping()
    {
        if (nearest_sleep_location == null)
        {
            float closest_dist = 999f;
            foreach (GameObject point in location_manager.GetLocationsOfType(LocationType.Location.HOME))
            {
                if (!point.GetComponentInParent<LocationType>().IsFull())
                {
                    float dist = CheckDistance(point);
                    if (dist < closest_dist)
                    {
                        closest_dist = dist;
                        nearest_sleep_location = point;
                    }
                }
            }
        }
        else
        {
            if (nearest_sleep_location.GetComponentInParent<LocationType>().AddUser(gameObject) == 1)
            {
                if (CheckDistance(nearest_sleep_location) < max_dist)
                {
                    if (!traits_script.in_objective)
                    {

                        traits_script.in_objective = true;
                    }
                    sleeping_timer -= Time.deltaTime * timer_multiplier;
                    if (sleeping_timer <= 0)
                    {
                        if(traits_script.CheckNeed("hp") <= 50)
                        {
                            traits_script.UpdateNeeds("hp", 10);
                            resources.UpdateResources("herbs", -2);
                        }
                        current_state = Sim_State.IDLE;
                        sleeping_timer = 5f;
                        traits_script.UpdateNeeds("sleep", 100);
                        traits_script.UpdateNeeds("hp", 10);
                        nearest_sleep_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
                        nearest_sleep_location = null;
                    }
                }
                else
                {
                    agent.destination = nearest_sleep_location.transform.position;
                }
            }
            else
            {
                nearest_sleep_location = null;
            }
        }
    }

    private void ResolveForaging()
    {
        current_state = (resources.CheckResources("herbs") <= 1) ? Sim_State.HERBING : Sim_State.DIGGING;
    }

    private void ResolveHunting()
    {
        if (nearest_hunt_location == null)
        {//Debug.Log("Trying to resolve hunting state");
            float closest_dist = 999f;
            foreach (GameObject point in location_manager.GetLocationsOfType(LocationType.Location.FOREST))
            {
                //Debug.Log("Checking if available");
                if (!point.GetComponentInParent<LocationType>().IsFull() && (point.GetComponentInParent<LocationType>().CheckResourceAmount() >= 200))
                {
                    //Debug.Log("Checking if closest");
                    float dist = CheckDistance(point);
                    if (dist < closest_dist)
                    {
                        //Debug.Log("Closest!");
                        closest_dist = dist;
                        nearest_hunt_location = point;
                    }
                }
            }
            if(nearest_hunt_location == null)
            {
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            if (nearest_hunt_location.GetComponentInParent<LocationType>().AddUser(gameObject) == 1)
            {
                //Debug.Log("Closest exists so if not in range should move to place!");
                if (CheckDistance(nearest_hunt_location) < max_dist)
                {
                    if (!traits_script.in_objective)
                    {
                        traits_script.in_objective = true;

                        traits_script.SetActivity(true);
                    }
                    hunting_timer -= Time.deltaTime * timer_multiplier;
                    if (hunting_timer <= 0)
                    {
                        hunting_timer = 5f;
                        if (traits_script.GetPersonality() == SimTraits.Personality.HUNTER)
                        {
                            nearest_hunt_location.GetComponentInParent<LocationType>().RemoveResourceHeld(100 * traits_script.multiplier_bonus);
                            resources.UpdateResources("food", 100 * traits_script.multiplier_bonus);
                        }
                        else
                        {
                            nearest_hunt_location.GetComponentInParent<LocationType>().RemoveResourceHeld(100);
                            resources.UpdateResources("food", 100);
                        }

                        traits_script.UpdateNeeds("sleep", -20);
                        current_state = Sim_State.IDLE;
                        nearest_hunt_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
                        nearest_hunt_location = null;
                    }
                }
                else
                {
                    agent.destination = nearest_hunt_location.transform.position;
                }
            }
            else
            {
                nearest_hunt_location = null;
            }
        }
    }
    private void ResolveHerbing()
    {
        if (nearest_herb_location == null)
        {
            float closest_dist = 999f;
            foreach (GameObject point in location_manager.GetLocationsOfType(LocationType.Location.FORAGRY))
            {
                if (!point.GetComponentInParent<LocationType>().IsFull() && (point.GetComponentInParent<LocationType>().CheckResourceAmount() >= 2) )
                {
                    float dist = CheckDistance(point);
                    if (dist < closest_dist)
                    {
                        closest_dist = dist;
                        nearest_herb_location = point;
                    }
                }
            }
            if (nearest_herb_location == null)
            {
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            if (nearest_herb_location.GetComponentInParent<LocationType>().AddUser(gameObject) == 1)
            {
                if (CheckDistance(nearest_herb_location) < max_dist)
                {
                    if (!traits_script.in_objective)
                    {

                        traits_script.in_objective = true;
                        traits_script.SetActivity(true);
                    }
                    foraging_timer -= Time.deltaTime * timer_multiplier;
                    if (foraging_timer <= 0)
                    {
                        foraging_timer = 5f;
                        if (traits_script.GetPersonality() == SimTraits.Personality.FORAGER)
                        {
                            nearest_herb_location.GetComponentInParent<LocationType>().RemoveResourceHeld(1 * traits_script.multiplier_bonus);
                            resources.UpdateResources("herbs", 1 * traits_script.multiplier_bonus);
                        }
                        else
                        {
                            nearest_herb_location.GetComponentInParent<LocationType>().RemoveResourceHeld(1);
                            resources.UpdateResources("herbs", 1);
                        }

                        traits_script.UpdateNeeds("sleep", -10);
                        current_state = Sim_State.IDLE;
                        nearest_herb_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
                        nearest_herb_location = null;
                    }
                }
                else
                {
                    agent.destination = nearest_herb_location.transform.position;
                }
            }
            else
            {
                nearest_herb_location = null;
            }
        }
    }

    private void ResolveDigging()
    {
        if (nearest_digging_location == null)
        {
            float closest_dist = 999f;
            foreach (GameObject point in location_manager.GetLocationsOfType(LocationType.Location.QUARRY))
            {
                if (!point.GetComponentInParent<LocationType>().IsFull() && (point.GetComponentInParent<LocationType>().CheckResourceAmount() >= 200))
                {
                    float dist = CheckDistance(point);
                    if (dist < closest_dist)
                    {
                        closest_dist = dist;
                        nearest_digging_location = point;
                    }
                }
            }
            if(nearest_digging_location == null)
            {
                current_state = Sim_State.IDLE;
            }
        }
        else
        {
            if (nearest_digging_location.GetComponentInParent<LocationType>().AddUser(gameObject) == 1)
            {
                if (CheckDistance(nearest_digging_location) < max_dist)
                {
                    if (!traits_script.in_objective)
                    {

                        traits_script.in_objective = true;
                        traits_script.SetActivity(true);
                    }
                    foraging_timer -= Time.deltaTime * timer_multiplier;
                    if (foraging_timer <= 0)
                    {
                        foraging_timer = 5f;
                        if (traits_script.GetPersonality() == SimTraits.Personality.FORAGER)
                        {
                            nearest_digging_location.GetComponentInParent<LocationType>().RemoveResourceHeld(100 * traits_script.multiplier_bonus);
                            resources.UpdateResources("mats", 100 * traits_script.multiplier_bonus);
                        }
                        else
                        {
                            nearest_digging_location.GetComponentInParent<LocationType>().RemoveResourceHeld(100);
                            resources.UpdateResources("mats", 100);
                        }
                        traits_script.UpdateNeeds("sleep", -5);
                        traits_script.UpdateNeeds("food", -5);
                        current_state = Sim_State.IDLE;
                        nearest_digging_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
                        nearest_digging_location = null;
                    }
                }
                else
                {
                    agent.destination = nearest_digging_location.transform.position;
                }
            }
            else
            {
                nearest_digging_location = null;
            }
        }
    }

    private void ResolveFighting()
    {
        if (target != null)
        {
            if (CheckDistance(target) <= GetComponent<SimTraits>().attack_range)
            {
                if (attack_timer <= 0)
                {
                    target.GetComponent<Enemy_AI>().TakeDamage();
                    if (traits_script.GetPersonality() == SimTraits.Personality.BRAVE)
                    {
                        target.GetComponent<Enemy_AI>().TakeDamage();
                    }
                    attack_timer = 1f;
                }
                else
                {
                    attack_timer -= Time.deltaTime * timer_multiplier;
                }
            }
            else
            {
                agent.destination = target.transform.position;
            }
        }
        else
        {
            target = null;
            current_state = Sim_State.IDLE;
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

    public void ForceBehaviour(Sim_State new_state)
    {
        //Debug.Log("should change to new state - " + new_state);
        ResetAllTimers();
        ResetAllLocation();
        idle_loops = 0;
        current_state = new_state;
    }

    public void ResetAllTimers()
    {
        eating_timer = 5f;
        foraging_timer = 5f;
        hunting_timer = 5f;
        idle_timer = idle_timer_max;
    }

    private void ResetAllLocation()
    {
        if (nearest_digging_location != null)
        {
            nearest_digging_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
        }
        if (nearest_food_location != null)
        {
            nearest_food_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
        }
        if (nearest_herb_location != null)
        {
            nearest_herb_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
        }
        if (nearest_hunt_location != null)
        {
            nearest_hunt_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
        }
        if (nearest_sleep_location != null)
        {
            nearest_sleep_location.GetComponentInParent<LocationType>().RemoveUser(gameObject);
        }
        nearest_digging_location = null;
        nearest_food_location = null;
        nearest_herb_location = null;
        nearest_hunt_location = null;
        nearest_sleep_location = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            target = other.gameObject;
            ForceBehaviour(Sim_State.FIGHTING);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Enemy") && target == null)
        {
            target = other.gameObject;
            ForceBehaviour(Sim_State.FIGHTING);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            target = null;
            ForceBehaviour(Sim_State.IDLE);
        }
    }
}
