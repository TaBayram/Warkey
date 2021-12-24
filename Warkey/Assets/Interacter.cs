using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player hit something");
    }

}
