using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{

    private ResourcesManage resource_script;
    private NavMeshAgent agent;

    [SerializeField] private float attack_timer_max = 1f;
    private float attack_timer;
    private bool has_target = false;
    private GameObject target;
    private int hp = 3;
    // Start is called before the first frame update
    void Start()
    {
        resource_script = GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        attack_timer = attack_timer_max;
    }

    // Update is called once per frame
    void Update()
    {
        float closest_distance = 9999f;
        foreach(GameObject villager in resource_script.GetVillagers())
        {
           if(Vector3.Distance(villager.transform.position, transform.position) < closest_distance)
            {
                closest_distance = Vector3.Distance(villager.transform.position, transform.position);
                target = villager;
            }
        }

        if(target != null)
        {
            agent.destination = target.transform.position;
        }

        if(has_target)
        {
            if(attack_timer >= 0)
            {
                attack_timer -= Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);
        if (has_target == false && (other.gameObject.tag == "Villager"))
        {
            has_target = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (has_target == false && (other.gameObject.tag == "Villager"))
        {
            target = other.gameObject;
            has_target = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            has_target = false;
            attack_timer = attack_timer_max;
        }
    }

    private void Attack()
    {
        target.GetComponent<SimTraits>().UpdateNeeds("hp", -10);
        attack_timer = attack_timer_max;
    }

    public void TakeDamage()
    {
        hp--;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
