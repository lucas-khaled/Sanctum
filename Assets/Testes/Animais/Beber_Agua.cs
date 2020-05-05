using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beber_Agua : StateMachineBehaviour
{
    float aux = 15;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aux <= 0)
        {
            int random = Random.Range(1, 3);
            if (random == 1)
            {
                animator.SetBool("Bebendo_Agua", false);
                animator.SetTrigger("Caminhar");
            }
            aux = 15;
        }
        else
        {
            aux = aux - Time.deltaTime;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
