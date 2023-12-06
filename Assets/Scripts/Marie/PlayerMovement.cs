using System;
using System.Runtime.Versioning;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpStrength = 8f;
    [SerializeField] private float upGravity = 1f, downGravity = 5f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform feet;
    [SerializeField] private Checkpoint checkpoint;
    [SerializeField] private GameObject FixedSwitch;
    [SerializeField] private float coyoteTime;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private SoundPlayer _audioPlayer;

    private LayerMask voidLayer;
    private Collider2D _holeCollider;

    private float horizontalMovement = 0;
    private bool jump = false, isGrounded = false;
    public int torchesLit = 0;
    public int keyUsed = 0;

    [SerializeField] private GameObject gameEndPanel; // Référence à un panneau de fin de jeu
    private bool isOnMovingPlatform = false;
    private Transform movingPlatform;
    private Vector2 movingPlatformOffset = Vector2.zero;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioPlayer = GetComponent<SoundPlayer>();
        voidLayer = LayerMask.NameToLayer("Void");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if(isOnMovingPlatform)
            {
                movingPlatformOffset.x -= _speed * Time.deltaTime;
            }
            horizontalMovement = -_speed;
            _spriteRenderer.flipX = true;
        }else if (Input.GetKey(KeyCode.D))
        {
            if (isOnMovingPlatform)
            {
                movingPlatformOffset.x += _speed * Time.deltaTime;
            }
            horizontalMovement = _speed;
            _spriteRenderer.flipX = false;
        }
        else
        {
            horizontalMovement = 0;
        }
        
        _animator.SetBool("Walk", (horizontalMovement != 0 && isGrounded));
        transform.position += new Vector3(horizontalMovement * Time.deltaTime, 0, 0);

        CheckGround();
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            isGrounded = false;
            _rigidbody.isKinematic = false;
            _animator.SetBool("Jump", true);
            _audioPlayer.PlayAudio(SoundFX.Jump);
        }

        if (isOnMovingPlatform)
        {
            transform.position = new Vector3(movingPlatform.position.x + movingPlatformOffset.x, movingPlatform.position.y + movingPlatformOffset.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            _rigidbody.AddForce(new Vector2(0, _jumpStrength), ForceMode2D.Impulse);
            _rigidbody.gravityScale = upGravity;
            isOnMovingPlatform = false;
            jump = false;
        }

        if (!isGrounded && _rigidbody.velocity.y < 0)
        {
            _rigidbody.gravityScale = downGravity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayers)
        {
            isGrounded = true;
            _animator.SetBool("Jump", false);
            _rigidbody.gravityScale = upGravity;
        } 
        else if(collision.gameObject.layer == voidLayer)
        {
            collision.collider.isTrigger = true;
            GetComponent<PlayerLife>().Hurt(1);
            Invoke("Respawn", 1f);
            _holeCollider = collision.collider;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "SwitchBroken" && PlayerInventory.Instance.IsInInventory("LEVERHANDLE"))
        {
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                collision.gameObject.SetActive(false);
                FixedSwitch.SetActive(true);

                PlayerInventory.Instance.RemoveItemFromInventory("LEVERHANDLE");

                GameObject.Find("Bridge").GetComponent<Animator>().SetTrigger("Down");
            }
        }
        else if (collision.gameObject.name == "BombTrigger" && PlayerInventory.Instance.IsInInventory("BOMB"))
        {
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                PlayerInventory.Instance.RemoveItemFromInventory("BOMB");
                GameObject.Find("Rock").SetActive(false);

                collision.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else if (collision.gameObject.name == "StandingTorch")
        {
            if(PlayerInventory.Instance.IsInInventory("TORCH1"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    torchesLit++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("TORCH1");
                    collision.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    collision.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (PlayerInventory.Instance.IsInInventory("TORCH2"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    torchesLit++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("TORCH2");
                    collision.gameObject.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    collision.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (PlayerInventory.Instance.IsInInventory("TORCH3"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    torchesLit++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("TORCH3");
                    collision.gameObject.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    collision.gameObject.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else if (collision.gameObject.name == "DoorTrigger")
        {
            if (PlayerInventory.Instance.IsInInventory("KEY1"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    keyUsed++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("KEY1");
                    GameObject.Find("Door").transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else if (PlayerInventory.Instance.IsInInventory("KEY2"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    keyUsed++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("KEY2");
                    GameObject.Find("Door").transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            else if (PlayerInventory.Instance.IsInInventory("KEY3"))
            {
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    keyUsed++;

                    collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    PlayerInventory.Instance.RemoveItemFromInventory("KEY3");
                    GameObject.Find("Door").transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SwitchBroken" || collision.gameObject.name == "BombTrigger" || collision.gameObject.name == "StandingTorch" || collision.gameObject.name == "DoorTrigger")
        {
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        transform.position = checkpoint.transform.position;
        if(_holeCollider != null)
        {
            _holeCollider.isTrigger = false;
            _holeCollider = null;
        }
    }

    public void PlayWalkingSound()
    {
        _audioPlayer.PlayAudio(SoundFX.Walk);
    }
    
    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(feet.transform.position, Vector2.down, 0.1f, groundLayers);
        if (hit)
        {
            isGrounded = true;
            _animator.SetBool("Jump", false);
            _rigidbody.gravityScale = upGravity;    
            if (!isOnMovingPlatform && hit.transform.CompareTag("Moving"))
            {
                movingPlatform = hit.transform;
                movingPlatformOffset = new Vector2(
                    transform.position.x - movingPlatform.transform.position.x,
                    transform.position.y - movingPlatform.transform.position.y);
                isOnMovingPlatform = true;             
                _rigidbody.isKinematic = true;
            }
        }
        else {
            Invoke(nameof(CoyoteTime), coyoteTime);
            _rigidbody.isKinematic = false;
            _animator.SetBool("Jump", true);
            _rigidbody.gravityScale = downGravity;
        }
    }

    public void ChangeCheckpoint(Checkpoint newCheckpoint)
    {
        if (checkpoint != newCheckpoint)
        {
            Destroy(checkpoint.gameObject);
        }
        checkpoint = newCheckpoint;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "End Flag")
        {
            CheckForRequiredItem(other.gameObject);
        }
    }

    private void CheckForRequiredItem(GameObject player)
    {

        if (PlayerInventory.Instance.IsInInventory("REDGEM"))
        {

            EndGame();
        }
        else
        {
            Debug.Log("Vous avez besoin de l'objet requis pour terminer le jeu.");
        }
    }

    private void EndGame()
    {
        gameEndPanel.SetActive(true);
    }

    private void CoyoteTime()
    {
        isGrounded = false;
    }
}
