using UnityEngine;

public class KeyCrate : MonoBehaviour
{
    private bool crateOpen = false;

    private void Update()
    {
        if(GameObject.Find("Character").GetComponent<PlayerMovement>().torchesLit == 3 && !crateOpen)
        {
            crateOpen = true;

            GetComponent<SpriteRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
