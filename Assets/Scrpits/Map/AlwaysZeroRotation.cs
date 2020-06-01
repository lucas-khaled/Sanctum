using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysZeroRotation : MonoBehaviour
{
    [SerializeField]
    bool isFlipped = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 value = (isFlipped) ? new Vector3(90,180,0) : new Vector3(90,0,0);
        transform.rotation = Quaternion.Euler(value);
    }
}
