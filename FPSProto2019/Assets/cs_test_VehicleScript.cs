using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cs_test_VehicleScript : MonoBehaviour
{
    [SerializeField] GameObject TramSystem;
    Vector3[] TramCheckpoints;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(LoadTramCheckpoints());

        print("Loaded Tram: " + TramSystem.gameObject.name);
    }

    IEnumerator LoadTramCheckpoints()
    {
        while (TramCheckpoints == null)
        {
            TramCheckpoints = TramSystem.GetComponent<cs_test_TramSystem>().GetTramCheckpointPositions;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
