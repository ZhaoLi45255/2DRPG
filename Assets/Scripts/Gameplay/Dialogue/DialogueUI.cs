using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    public bool hasBegun = false;

    public ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    Action onDialogueFinished;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
    }

    public IEnumerator ShowDialogue(DialogueObject dialogueObject, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        if(!hasBegun) // Make sure not to reset onFinished, since it's needed to reset NPCState.
        {
            onDialogueFinished = onFinished;
        }
        dialogueBox.SetActive(true);
        hasBegun = true;
        Debug.Log(onDialogueFinished == null);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel); // Make the dialogue appear.
            if(i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
            {
                break;
            }
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); // Text will only go to the next line upon pressing the button.
        }
        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            Debug.Log(onDialogueFinished == null);
            onDialogueFinished?.Invoke();
            CloseDialogueBox();
        }
    }

    private void CloseDialogueBox()
    {
        hasBegun = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
