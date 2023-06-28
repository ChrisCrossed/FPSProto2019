using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStance
{
    Passive,
    Investigating,
    Hunting,
    Fighting,
    Dead
}



[RequireComponent(typeof(NavMeshAgent))]


public class c_EnemyLogic : MonoBehaviour
{
    NavMeshAgent NavMeshAgent;

    EnemyStance EnemyStance;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        // GameObject go_GoalNode = GameObject.Find("TempGoal");
        List<GameObject> go_GoalNodes = new List<GameObject>();

        foreach(GameObject pathnode in GameObject.FindGameObjectsWithTag("PathNode"))
        {
            go_GoalNodes.Add(pathnode);
        }

        SetGoalPosition(go_GoalNodes[0]);
    }

    void SetEnemyStance(GameObject target)
    {
        switch (EnemyStance)
        {
            case EnemyStance.Passive:
                break;
            case EnemyStance.Investigating:
                SetGoalPosition(target.transform.position);
                break;
            case EnemyStance.Hunting:
                SetGoalPosition(target.transform.position);
                break;
            case EnemyStance.Fighting:
                SetGoalPosition(target.transform.position);
                break;
            case EnemyStance.Dead:
                break;
            default:
                break;
        }
    }

    void SetEnemyStance()
    {
        switch (EnemyStance)
        {
            case EnemyStance.Passive:
                break;
            case EnemyStance.Investigating:
                break;
            case EnemyStance.Hunting:
                break;
            case EnemyStance.Fighting:
                break;
            case EnemyStance.Dead:
                break;
            default:
                break;
        }
    }

    void SetGoalPosition(GameObject go_GoalPosition)
    {
        SetGoalPosition(go_GoalPosition.transform.position);
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
