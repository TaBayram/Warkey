using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class sacma : MonoBehaviour
{
    public Material[] material;
    public SkinnedMeshRenderer game;
    public PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        Debug.Log(view.ViewID);
        if (view.ViewID == 1001)
        {
            setMaterial(0);
        }
        else
        {
            setMaterial(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMaterial(int index)
    {
        game.material = material[index];
    }

}
