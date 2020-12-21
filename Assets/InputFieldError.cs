using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldError : MonoBehaviour
{
    public static InputFieldError error;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (InputFieldError.error == null)
        {
            InputFieldError.error = this;
        }
        else
        {
            if (InputFieldError.error != this)
            {
                Object.Destroy(InputFieldError.error);
                // Destroy(PhotonRoom.room.gameObject);
                InputFieldError.error = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);

    }
}
