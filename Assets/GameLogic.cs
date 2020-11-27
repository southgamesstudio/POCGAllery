using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
public class GameLogic : MonoBehaviour
{
    public List<PhotonView> ListOfDragingObjects = new List<PhotonView>();

    
    private List<int> IdThatCantHappen = new List<int>();
    int randNum = 0;
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
            while (IdThatCantHappen.Count < 16)
            {
                randNum = Random.Range(0, ListOfDragingObjects.Count);
                if (!IdThatCantHappen.Contains(randNum))
                {
                    break;
                }
            }
            _object.GetComponent<PlayerController>().SetObjectVisible(ListOfDragingObjects[randNum].ViewID);
            GetComponent<PhotonView>().RPC("RemoveObjectFromList", RpcTarget.AllBuffered, randNum);
            IdThatCantHappen.Add(randNum);
        }
    }

    [PunRPC]
    public void RemoveObjectFromList(string myList)
    {
        //    myString = JsonUtility.ToJson(myItem);
        //myItem = JsonUtility.FromJson<DragAndDropItem>(_JSONobject);
    }
}

