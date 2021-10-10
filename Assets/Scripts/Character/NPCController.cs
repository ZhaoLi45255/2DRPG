using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 input;
    private bool isMoving;
    [SerializeField] List<Vector2> movements;
    [SerializeField] float timeBetweenMoves;

    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    [SerializeField] private DialogueObject dialogueObject;

    private Animator animator;

    private Vector3 moveDirection;

    float idleTimer = 0f;
    public NPCState state;
    int currentPattern = 0;
    float counter = 0; // Keep track of how many steps the character will move in Move(). Only one direction is allowed.

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TalkedTo(PlayerController player)
    {
        if (state != NPCState.Walking) // Make sure that NPC cannot be interacted with while walking.
        {
            state = NPCState.Busy;
            FacePlayer(player.transform.position);
            StartCoroutine(DialogueUI.ShowDialogue(dialogueObject, () =>
            {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
            Debug.Log(state);
        }
    }

    public IEnumerator Move(Vector2 moveVec)
    {
        if (counter == 0) // If counter is zero, then add moves to it.
        {
            if (moveVec.x == 0)
            {
                counter = Mathf.Abs(moveVec.y);
            }
            else if (moveVec.y == 0)
            {
                counter = Mathf.Abs(moveVec.x);
            }
        }
        while (counter > 0) // This will check tile one at a time.
        {
            moveVec.x = Mathf.Clamp(moveVec.x, -1f, 1f);
            moveVec.y = Mathf.Clamp(moveVec.y, -1f, 1f);
            animator.SetFloat("moveX", moveVec.x); // Set idle animations
            animator.SetFloat("moveY", moveVec.y);
            var targetPos = transform.position;
            targetPos.x += moveVec.x; // Determine the new location.
            targetPos.y += moveVec.y;

            if (!IsWalkable(targetPos))
            {
                yield break;
            }

            isMoving = true;
            animator.SetBool("isMoving", isMoving); // Set moving animation
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) // Check whether the difference of the two positions is greater than 0.
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // Move the player toward the target position.
                yield return null; // Stop execution of the coroutine.
            }
            transform.position = targetPos; // Set the player's new position.
            isMoving = false;
            animator.SetBool("isMoving", isMoving); // Set moving animation
            counter--;
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // Check whether something is on the target tile. If there is, then return false.
        if (Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.instance.SolidObjectsLayer | GameLayers.instance.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public Animator Animator
    {
        get => animator;
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            if(counter != 0) // Make sure the NPC finishes its movement right after the path is clear.
            {
                StartCoroutine(Walk());
            }
            idleTimer += Time.deltaTime;
            if(idleTimer > timeBetweenMoves)
            {
                idleTimer = 0f;
                if (movements.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }
    }

    IEnumerator Walk()
    {
        if (state != NPCState.Busy)
        {
            state = NPCState.Walking;
            var oldPos = transform.position;
            yield return Move(movements[currentPattern]);
            if (counter == 0) // It should also make sure to complete the step if someone were blocking it earlier.
            {
                currentPattern = (currentPattern + 1) % movements.Count; // Loop through the patterns once they're all done.
            }
            state = NPCState.Idle;
        }
    }

    public void FacePlayer(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);
        if(xDiff == 0 || yDiff == 0) // This means the player is right next to the NPC.
        {
            animator.SetFloat("moveX", xDiff); // Set idle animations
            animator.SetFloat("moveY", yDiff);
        }
    }
}

public enum NPCState { Idle, Walking, Busy }