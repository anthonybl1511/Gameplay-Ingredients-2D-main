using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    private void Update()
    {
        if (GameObject.Find("Character").GetComponent<PlayerMovement>().keyUsed == 3)
        {
            GetComponent<Animator>().SetTrigger("Open");
        }
    }
}
