using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] private GameObject gameEndPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameObject.Find("Character").SetActive(false);
            GetComponent<Animator>().SetTrigger("Finish");

            Invoke(nameof(ActivateMenu), 1f);
        }
    }

    private void ActivateMenu()
    {
        gameEndPanel.SetActive(true);
    }
}
