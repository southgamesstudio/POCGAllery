using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameLogic : MonoBehaviour
{
    public List<PhotonView> ListOfDragingObjects = new List<PhotonView>();
    private List<PhotonView> ListOfObjectsMirror = new List<PhotonView>();

    private void Start()
    {
        ListOfObjectsMirror = ListOfDragingObjects;
    }

    public void ReferenceAll(GameObject _object)
    {
        for (int i = 0; i < ListOfDragingObjects.Count; i++)
        {
            _object.GetComponent<PlayerController>().ReferenceAllobjects(ListOfDragingObjects[i].ViewID);
        }
        _object.GetComponent<PlayerController>().FillObjects();
    }
    public void SetObjects(GameObject _object)
    {
        for(int i = 0; i < 4; i++)
        {
            int randNum = Random.Range(0, ListOfObjectsMirror.Count);
            _object.GetComponent<PlayerController>().SetObjectVisible(ListOfDragingObjects[randNum].ViewID);
            GetComponent<PhotonView>().RPC("RemoveObjectFromList", RpcTarget.All, randNum);
        }
    }

    [PunRPC]
    public void RemoveObjectFromList(int id)
    {
        ListOfObjectsMirror.RemoveAt(id);
    }
    
}
