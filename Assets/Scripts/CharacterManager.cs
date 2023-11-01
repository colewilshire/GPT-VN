using System.Collections.Generic;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterPortrait activePortrait;
    public Dictionary<string, CharacterPortrait> Characters {get; private set;} = new Dictionary<string, CharacterPortrait>();
    public CharacterPortrait MainCharacter;

    private void Start()
    {
        StateController.Instance.OnMenuStateChange += ResetMainCharacter;
    }

    private void OnDestroy()
    {
        StateController.Instance.OnMenuStateChange -= ResetMainCharacter;
    }

    private void ResetMainCharacter(GameState state)
    {
        if (state != GameState.MainMenu) return;
        MainCharacter = null;
    }

    // Get specified character and replace their display name and appearance with that from Character Creation.
    // In the off chance a bad name was given, search for some of ChatGPT's favorite names for Protag-kun.
    public void SetMainCharacter(string characterName)
    {
        if (Characters.TryGetValue(characterName, out CharacterPortrait characterPortrait)
            || Characters.TryGetValue("Main Character", out characterPortrait) 
            || Characters.TryGetValue("Protagonist", out characterPortrait))
        {
            if (!CharacterCreationController.Instance.MainCharacterPortait) return;

            MainCharacter = characterPortrait;
            characterPortrait.DisplayName = CharacterCreationController.Instance.MainCharacterPortait.DisplayName;
            characterPortrait.Appearance = CharacterCreationController.Instance.MainCharacterPortait.Appearance;
            characterPortrait.SetAppearance(characterPortrait.Appearance);
        }
    }

    public void CacheCharacterPortrait(CharacterPortrait characterPortrait)
    {
        Characters[characterPortrait.name] = characterPortrait;
    }

    public CharacterPortrait ShowPortrait(string characterName, Mood mood)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (Characters.TryGetValue(characterName, out CharacterPortrait characterPortrait))
        {
            activePortrait = characterPortrait;

            //Mood expression = Thesaurus.Instance.GetMoodSynonym(mood);
            //Enum.TryParse(expressionName, out Mood expression);
            characterPortrait.ShowPortrait(mood);

            return characterPortrait;
        }

        return null;
    }

    public void ClearCharacters()
    {
        foreach (KeyValuePair<string, CharacterPortrait> characterEntry in Characters)
        {
            Destroy(characterEntry.Value.gameObject);
        }

        Characters = new Dictionary<string, CharacterPortrait>();
    }
}
