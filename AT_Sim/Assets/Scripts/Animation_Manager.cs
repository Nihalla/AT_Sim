using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Manager : MonoBehaviour
{

    private SimBehaviours sim_script;

    public Animator anim;
    public int FlyBool;
    public int WalkingBool;
    public int DeadBool;
    public int FightBool;

    private void Awake()
    {
        FlyBool = Animator.StringToHash("Fly");
        WalkingBool = Animator.StringToHash("Walk");
        DeadBool = Animator.StringToHash("Dead");
        FightBool = Animator.StringToHash("Fight");
    }
    // Start is called before the first frame update
    void Start()
    {
        sim_script = GetComponent<SimBehaviours>();
        anim = gameObject.GetComponentInChildren<Animator>();
        anim?.SetLayerWeight(0, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateAnimationState(SimBehaviours.Sim_State effective_state)
    {

        if (effective_state == SimBehaviours.Sim_State.IDLE)
        {
            anim?.SetBool(FlyBool, false);
            anim?.SetBool(WalkingBool, false);
            anim?.SetBool(DeadBool, false);
            anim?.SetBool(FightBool, false);
        }
        else if (effective_state == SimBehaviours.Sim_State.ROAMING)
        {
            anim?.SetBool(FlyBool, false);
            anim?.SetBool(WalkingBool, true);
            anim?.SetBool(DeadBool, false);
            anim?.SetBool(FightBool, false);
        }
        else if (effective_state == SimBehaviours.Sim_State.HUNTING)
        {
            anim?.SetBool(FlyBool, true);
            anim?.SetBool(WalkingBool, false);
            anim?.SetBool(DeadBool, false);
            anim?.SetBool(FightBool, false);
        }
        else
        {
            anim?.SetBool(FlyBool, false);
            anim?.SetBool(WalkingBool, true);
            anim?.SetBool(DeadBool, false);
            anim?.SetBool(FightBool, false);
        }
    }
    public void SetToSleep()
    {
        anim?.SetBool(FlyBool, false);
        anim?.SetBool(WalkingBool, false);
        anim?.SetBool(DeadBool, true);
        anim?.SetBool(FightBool, false);
    }

    public void SetToAttack()
    {
        anim?.SetBool(FlyBool, false);
        anim?.SetBool(WalkingBool, false);
        anim?.SetBool(DeadBool, false);
        anim?.SetBool(FightBool, true);
    }
}
