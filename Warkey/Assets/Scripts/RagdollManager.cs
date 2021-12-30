using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    public GameObject ragdollPrefab;
    public GameObject replace;

    public void CreateRagdoll() {
        Destroy(replace);
        Instantiate(ragdollPrefab, transform.position, transform.rotation, transform);
    }
}
