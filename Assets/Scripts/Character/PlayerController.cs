using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;
    public bool canMove = true;
    private Vector2 input;

    public Animator animator;
    public string initDir; // Direction character faces once the game begins.
    public static string curDir; // Direction character currently faces.

    private static bool playerExists; // Should be false upon game start-up or after cutscene.

    public static string spawnArea; // Determine which set of lists to read from.
    public static int spawnIndex; // Index of lists to get new coordinates from.
    [SerializeField] private GameObject playerSpawn;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (playerExists)
        {
            playerSpawn.GetComponent<SpawnPointList>().SetNewPos();
            ChangeSpriteDirection(curDir);
        } else
        {
            ChangeSpriteDirection(initDir);
            playerExists = true;
        }
    }

    void Update()
    {
        if(!isMoving && canMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input.x != 0) // Prevent diagonal movement.
            {
                input.y = 0;
            }

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x); // Set idle animations
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x; // Determine the new location.
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving); // Set moving animation
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY")); // The player's current direction.
        var targetPos = transform.position + facingDir;
        var collider = Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer);
        if (collider != null)
        {
            StartCoroutine(collider.GetComponent<NPCController>()?.TalkedTo(transform));
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) // Check whether the difference of the two positions is greater than 0.
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // Move the player toward the target position.
            yield return null; // Stop execution of the coroutine.
        }
        transform.position = targetPos; // Set the player's new position.
        isMoving = false;
    }

    public void ChangeSpriteDirection(string dir)
    {
        if (string.IsNullOrEmpty(dir))
        {
            initDir = "South"; // Default should be south.
        }
        if (dir == "West")
        {
            animator.SetFloat("moveX", -1);
        }
        else if (dir == "East")
        {
            animator.SetFloat("moveX", 1);
        }
        else if (dir == "North")
        {
            animator.SetFloat("moveY", 1);
        }
        else if (dir == "South")
        {
            animator.SetFloat("moveY", -1);
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // Check whether something is on the target tile. If there is, then return false.
        if(Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer) != null) {
            return false;
        }
        return true;
    }
}
