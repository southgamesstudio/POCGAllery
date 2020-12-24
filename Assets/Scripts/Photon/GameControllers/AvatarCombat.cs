using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCombat : MonoBehaviour
{

    private PhotonView PV;
    private AvatarSetup avatarSetup;
    public Transform rayOrigin;
    public Text healthDisplay;

    // Use this for initialization
    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<AvatarSetup>();
        healthDisplay = GameSetup.GS.healthDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;
        if (Input.GetMouseButton(0))
        {
            PV.RPC("RPC_Shooting", RpcTarget.All);
        }
        healthDisplay.text = avatarSetup.playerHealt.ToString();
    }

    [PunRPC]
    private void RPC_Shooting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,
            transform.TransformDirection(Vector3.forward),
            out hit, Mathf.Infinity,
            1000))
        {
            Debug.DrawRay(rayOrigin.position,
                rayOrigin.TransformDirection(Vector3.forward) * hit.distance,
                Color.yellow);
            Debug.Log("Did hit");
            if (hit.transform.tag == "Avatar")
            {
                hit.transform.gameObject.GetComponent<AvatarSetup>().playerHealt -= avatarSetup.playerDamge;
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin.position,
                rayOrigin.TransformDirection(Vector3.forward) * 1000,
                Color.white);
            Debug.Log("Did not hit");
        }
    }
}
