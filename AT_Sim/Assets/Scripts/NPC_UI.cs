using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC_UI : MonoBehaviour
{
    private bool UI_on = false;
    [SerializeField] private TMP_Text character_info;
    [SerializeField] private TMP_Text resource_info;
    private ResourcesManage resources;
    private CameraMovement cam_script;


    private GameObject villager = null;

    private void Awake()
    {
        cam_script = Camera.main.GetComponent<CameraMovement>();
    }
    // Start is called before the first frame update
    void Start()
    { 
        resources = GameObject.FindGameObjectWithTag("Manager").GetComponent<ResourcesManage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam_script.GetComponent<CameraMovement>().GetSelectedChar() != null)
        {
            villager = cam_script.GetComponent<CameraMovement>().GetSelectedChar().gameObject;
        }
        UI_on = cam_script.GetComponent<CameraMovement>().GetSelectedChar() != null ? true : false;
        if (UI_on)
        {
            character_info.text = "" + villager.name + "\n" +
                "Hunger = " + villager.GetComponent<SimTraits>().CheckNeed("food") + "\n" +
                "Energy = " + villager.GetComponent<SimTraits>().CheckNeed("sleep") + "\n" +
                "Health = " + villager.GetComponent<SimTraits>().CheckNeed("hp") + "\n" +
                "Personality - " + villager.GetComponent<SimTraits>().GetPersonality() + "\n" +
                "Currently - " + villager.GetComponent<SimBehaviours>().GetState() + "\n";       
        }
        else
        {
            character_info.text = "Nobody selected";
        }
        resource_info.text = "V - " + resources.CheckResources("v") + "/" + GetComponent<ResourcesManage>().village_capacity +
                " F - " + resources.CheckResources("food") +
                " M - " + resources.CheckResources("mats") +
                " H - " + resources.CheckResources("herbs");
    }
}
