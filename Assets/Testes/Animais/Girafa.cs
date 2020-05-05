using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Girafa : StateMachineBehaviour
{
    public NavMeshAgent animal;
    public GameObject[] waypoints;
    private int waypoint_atual;

    private void Awake()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        EscolherWaypoint();
        //  animal.destination = waypoints[waypoint_atual].transform.position;
    }

    void EscolherWaypoint()
    {
        int tempWaypoint = waypoint_atual;
        while (tempWaypoint == waypoint_atual)
        {
            waypoint_atual = Random.Range(0, waypoints.Length);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animal = animator.gameObject.GetComponent<NavMeshAgent>();
        EscolherWaypoint();
        animal.destination = waypoints[waypoint_atual].transform.position;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(waypoints[waypoint_atual].transform.position, animator.gameObject.transform.position) < 5f)
        {
            int aux = Random.Range(1, 4);
            if (aux == 1)
            {
                if (Vector3.Distance(waypoints[waypoint_atual].transform.position, animator.gameObject.transform.position) < 2f)
                    EscolherWaypoint();

                animal.destination = waypoints[waypoint_atual].transform.position;
            }
            else if (aux == 2)
            {
                Collider[] arvore = Physics.OverlapSphere(animal.transform.position, 15f, LayerMask.GetMask("Arvore"));
                if (arvore.Length > 0)
                {
                    int escolha = 0;

                    if (arvore.Length > 1)
                        escolha = Random.Range(0, arvore.Length);

                    animal.destination = arvore[escolha].transform.position;
                }
                Debug.Log("COMEEEEEE");
            }
            else if (aux == 3)
            {
                //ficar parado;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

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
