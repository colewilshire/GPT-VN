using System.Collections.Generic;
using UnityEngine;

public class DialogueChoiceSubmenu : Submenu
{
    [SerializeField] private DialogueChoiceButton choiceButtonPrefab;

    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.Choice
    };

    protected override void ResetMenu()
    {
        base.ResetMenu();

        DestroyChoices();
        SpawnChoices();
    }

    private void DestroyChoices()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SpawnChoices()
    {
        foreach (DialogueLine dialogueLine in DialogueChoiceController.Instance.CurrentChoice.Choices)
        {
            DialogueChoiceButton choice = Instantiate(choiceButtonPrefab, transform);

            choice.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            choice.SetDialogueLine(dialogueLine);
        }
    }
}
