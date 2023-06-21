using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class c_EnemyLogic : MonoBehaviour
{
    NavMeshAgent NavMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        GameObject go_GoalNode = GameObject.Find("TempGoal");

        SetGoalPosition(go_GoalNode.transform.position);

    }

    void SetGoalPosition(Vector3 v3_GoalPosition)
    {
        NavMeshAgent.destination = v3_GoalPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
