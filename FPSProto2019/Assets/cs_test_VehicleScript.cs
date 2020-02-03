using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cs_test_VehicleScript : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 5f;
    [SerializeField] GameObject TramSystem;
    Vector3[] TramCheckpoints;
    Quaternion[] TramCheckpointRotations;
    [SerializeField] AnimationCurve animCurve;
    Rigidbody this_RigidBody;
    [SerializeField] float WaitTimer = 3.0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine( LoadTramCheckpoints() );

        print("Loaded Tram: " + TramSystem.gameObject.name);
        yield return StartCoroutine(InitRigidBody());

        StartCoroutine(MoveVehicle());
    }

    IEnumerator LoadTramCheckpoints()
    {
        while (TramCheckpoints == null)
        {
            TramCheckpoints = TramSystem.GetComponent<cs_test_TramSystem>().GetTramCheckpointPositions;
            
            yield return new WaitForEndOfFrame();
        }

        while( TramCheckpointRotations == null)
        {
            TramCheckpointRotations = TramSystem.GetComponent<cs_test_TramSystem>().GetTramCheckpointRotations;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    IEnumerator InitRigidBody()
    {
        if (gameObject.GetComponent<Rigidbody>())
            this_RigidBody = gameObject.GetComponent<Rigidbody>();
        else
        {
            int numChildren = gameObject.transform.childCount;
            for( int i = 0; i < numChildren; ++i )
            {
                if(gameObject.transform.GetChild(i).GetComponent<Rigidbody>())
                {
                    this_RigidBody = gameObject.transform.GetChild(i).GetComponent<Rigidbody>();

                    yield break;
                }
            }

            print("NO RIGIDBODY FOUND VEHICLE IN " + gameObject.name);
        }

        yield break;
    }

    IEnumerator MoveVehicle()
    {
        int currentIndex = -1; // Moves to 0 on first run
        int nextIndex = -1; // Matches currentIndex + 1 on first run
        float lerpPerc = 0f;
        float moveRate = 0f;
        bool changeTracks = true;
        float testTimer = 0f;

        yield return new WaitForSeconds(WaitTimer);

        while ( true )
        {
            if( !changeTracks )
            {
                if( lerpPerc < 1.0f )
                {
                    lerpPerc += Time.deltaTime / moveRate;
                    testTimer += Time.deltaTime;

                    if (lerpPerc > 1.0f)
                    {
                        lerpPerc = 1.0f;
                        changeTracks = true;
                    }

                    Vector3 newPos = Vector3.Lerp(TramCheckpoints[currentIndex], TramCheckpoints[nextIndex], lerpPerc);
                    // gameObject.GetComponent<Rigidbody>().MovePosition(newPos);
                    this_RigidBody.MovePosition(newPos);

                    float lerpAnimCurveValue = animCurve.Evaluate(lerpPerc);
                    Quaternion newRot = Quaternion.Lerp(TramCheckpointRotations[currentIndex], TramCheckpointRotations[nextIndex], lerpAnimCurveValue);
                    // gameObject.GetComponent<Rigidbody>().MoveRotation(newRot);
                    this_RigidBody.MoveRotation(newRot);
                }
            }
            
            if( changeTracks )
            {
                #region Increment Indexes
                currentIndex++;
                if (currentIndex == TramCheckpoints.Length) currentIndex = 0;

                nextIndex = currentIndex + 1;
                if (nextIndex == TramCheckpoints.Length) nextIndex = 0;
                #endregion

                #region Reset perc details
                lerpPerc = 0f;

                float distance = Vector3.Distance(TramCheckpoints[currentIndex], TramCheckpoints[nextIndex]);
                moveRate = distance / MoveSpeed;
                #endregion

                changeTracks = false;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
