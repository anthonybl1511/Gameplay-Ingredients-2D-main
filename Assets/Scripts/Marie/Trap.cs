using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.name == "BearTrap")
        {
            GetComponent<Animator>().SetTrigger("Trap");
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerLife>().Hurt(damage);
        }
    }
}
