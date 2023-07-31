using System.Collections.Generic;
using TMPro;

public class NameDisplayController : Singleton<NameDisplayController>
{
    private TextMeshProUGUI textBox;

    protected override void Awake()
    {
        base.Awake();

        textBox = GetComponent<TextMeshProUGUI>();
    }

    public void SetDisplayName(string characterName)
    {
        characterName = characterName.Trim();

        if (characterName == null || characterName.ToLower() == "narrator")
        {
            textBox.text = "";
        }
        else
        {
            textBox.text = characterName;
        }
    }
}
