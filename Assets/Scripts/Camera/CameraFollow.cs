using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 initialPos = new Vector3(10, 15, -10);
    void Update()
    {
        FollowCamera();
    }
    void FollowCamera()
    {
        if(Target!=null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, Target.transform.position + initialPos, 5*Time.deltaTime);
        }
    }
    public void SetFollowTarget(Transform t)
    {
        Target = t;
    }
}
