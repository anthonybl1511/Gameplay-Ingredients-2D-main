using UnityEngine;

public class PlatformGoThough : MonoBehaviour
{
    private GameObject player;
    private Vector2 playerHeight;
    private Vector2 platformHeight;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        playerHeight = GetComponent<SpriteRenderer>().size / 2;
        platformHeight = GetComponent<SpriteRenderer>().size / 2;

        if (player.transform.localPosition.y + playerHeight.y >= transform.position.y + platformHeight.y)
        {
            Invoke(nameof(EnableCollider), 0.1f);
        }
        else if (gameObject.GetComponent<BoxCollider2D>().enabled == true)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void EnableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}