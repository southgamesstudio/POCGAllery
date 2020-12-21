using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InputFieldSelector : MonoBehaviour
{
    public List<TMP_InputField> InputFields = new List<TMP_InputField>();
    // Start is called before the first frame update

    int listIndex = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            while (!InputFields[listIndex].gameObject.activeInHierarchy)
            {
                listIndex++;
                if (listIndex == InputFields.Count)
                {
                    listIndex = 0;
                }
            }
            InputFields[listIndex].ActivateInputField();
            InputFields[listIndex].Select();
            listIndex++;
            if (listIndex == InputFields.Count)
            {
                listIndex = 0;
            }
        }
    }
}
