using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC_UI : MonoBehaviour
{
    [SerializeField] private bool UI_on = true;
    [SerializeField] private TMP_Text character_info;
    [SerializeField] private TMP_Text resource_info;
    private ResourcesManage resources;


    private GameObject villager;
    // Start is called before the first frame update
    void Start()
    {
        villager = GameObject.FindGameObjectWithTag("Villager");
        resources = GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>();
    }

    // Update is called once per frame
    void Update()
    {
        if(UI_on)
        {
            character_info.text = "" + villager.name + "\n" +
                "Hunger = " + villager.GetComponent<SimTraits>().CheckNeed("food") + "\n" +
                "Energy = " + villager.GetComponent<SimTraits>().CheckNeed("sleep") + "\n" +
                "Health = " + villager.GetComponent<SimTraits>().CheckNeed("hp") + "\n" +
                "Personality - " + villager.GetComponent<SimTraits>().GetPersonality() + "\n" +
                "Currently - " + villager.GetComponent<SimBehaviours>().GetState() + "\n";
            resource_info.text = "V - " + resources.CheckResources("v") +
                " F - " + resources.CheckResources("food") +
                " M - " + resources.CheckResources("mats") +
                " H - " + resources.CheckResources("herbs");
        }
    }
}
