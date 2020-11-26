using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviour
{
    private Camera myCam;
    private List<PhotonView> listOfObjects = new List<PhotonView>();
    private List<int> listOfObjectsIds = new List<int>();
    
    private GameLogic gameLogic;
    // Start is called before the first frame update
    void Start()
    {
        
        gameLogic = FindObjectOfType<GameLogic>();
        gameLogic.ReferenceAll(gameObject);
    }

    public void ReferenceAllobjects(int photonId)
    {
        listOfObjectsIds.Add(photonId);   
    }
    bool playerObjectsSet = false;
    public void SetObjectVisible(int id)
    {
        if (!playerObjectsSet)
        {
            listOfObjectsIds.Add(id);
            if (listOfObjectsIds.Count == 4)
            {
                for (int i = 0; i < listOfObjects.Count; i++)
                {
                    if (!listOfObjectsIds.Contains(listOfObjects[i].ViewID))
                    {
                        listOfObjects[i].gameObject.SetActive(false);
                    }
                }
                playerObjectsSet = true;
            }
        }
    }

    public void FillObjects()
    {
        foreach(PhotonView pv in FindObjectsOfType<PhotonView>())
        {
            if (listOfObjectsIds.Contains(pv.ViewID))
            {
                listOfObjects.Add(pv);
            }
        }
        listOfObjectsIds.Clear();
        gameLogic.SetObjects(gameObject);
    }



}
