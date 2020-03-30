using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cs_test_TramSystem : MonoBehaviour
{
    [SerializeField] bool ShowCheckpoints = true;
    [SerializeField] GameObject[] TramCheckpoints;
    Vector3[] TramCheckpointPositions;
    Quaternion[] TramCheckpointRotations;

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        for( int i = 0; i < TramCheckpoints.Length; ++i)
        {
            int nextIndex = i + 1;
            if (i == TramCheckpoints.Length - 1) nextIndex = 0;

            Debug.DrawLine(TramCheckpoints[i].transform.position, TramCheckpoints[nextIndex].transform.position, Color.red);
        }   
    }

    public Vector3[] GetTramCheckpointPositions
    {
        get { return TramCheckpointPositions; }
    }
    public Quaternion[] GetTramCheckpointRotations
    {
        get { return TramCheckpointRotations; }
    }

    void Start()
    {
        // Load tram system positions
        LoadTramSystem();

        // Turn off visible positions if required
        if ( !ShowCheckpoints )
        {
            foreach( GameObject checkpoint in TramCheckpoints )
            {
                checkpoint.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }


    void LoadTramSystem()
    {
        TramCheckpointPositions = new Vector3[TramCheckpoints.Length];
        TramCheckpointRotations = new Quaternion[TramCheckpoints.Length];

        // Convert object positions into Vector3 list
        for (int i = 0; i < TramCheckpoints.Length; ++i)
        {
            TramCheckpointPositions[i] = TramCheckpoints[i].transform.position;
            TramCheckpointRotations[i] = TramCheckpoints[i].transform.rotation;
        }
    }
}

