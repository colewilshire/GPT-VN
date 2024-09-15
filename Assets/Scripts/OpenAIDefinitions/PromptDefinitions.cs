public static class PromptDefinitions
{
    public static string GetGenerateCastListPrompt(
        int numberOfCharacters,
        string genre,
        string setting,
        string protagonistName,
        string accessoriesList,
        string hairsList,
        string outfitsList,
        string facesList)
    {
        return $@"
Generate a cast list of {numberOfCharacters} characters for the next scene of a '{genre}' genre visual novel set in the setting '{setting}'.
One of the characters should be named '{protagonistName}', and serve as the protagonist of the story.
One of the characters should be named 'Narrator', and serve as the story's narrator.
The other {numberOfCharacters - 2} characters are up to you to create. Characters other than 'Narrator' should have actual human names for their name, not titles.
Each character in the list should have a name, body type, hair style, face/hair accessory, outfit, and eye color.
Body types must be chosen from the following list: 'none', 'feminine', 'masculine'.
Accessories must be chosen from the following list: {accessoriesList}.
Hairs must be chosen from the following list: {hairsList}.
Outfits must be chosen from the following list: {outfitsList}.
Chosen outfits and accessories should be appropriate for the story's setting, if possible. For example, in a uniformed setting, all characters of the same gender and position should be wearing the same uniform.
Eye colors must be chosen from the following list: {facesList}.
";
    }

    public static string GetGenerateInitialDialoguePrompt(
        string genre,
        string setting,
        int linesPerScene,
        string protagonistName,
        string backgroundsList)
    {
        return $@"
Generate a script for the next scene of a '{genre}' genre visual novel set in the setting '{setting}', consisting of {linesPerScene} lines of dialogue.
Only a few characters from the cast list should appear in every scene. Some characters should be rarely appearing side characters, and the protagonist, {protagonistName}, and Narrator should appear frequently.
The cast of the story should consist of characters from the previously generated cast list.
Each line should include the speaking character's name, the text of the dialogue, the speaker's mood, and the background image to be displayed.
Format the response as a plain JSON object with a top-level key 'DialogueLines'.
Each entry under 'DialogueLines' should be an object with the keys 'CharacterName', 'DialogueText', 'Mood', and 'BackgroundDescription'.
BackgroundDescriptions should be chosen from the following list: {backgroundsList}.
Unless the story calls for a change in location, the BackgroundDescription should not change from one line of dialogue to the next.
Do not include any additional formatting or markers such as markdown code block markers.
";
    }

    public static string GetGenerateChoicePrompt(string protagonistName)
    {
        return $@"
From where the story left off, offer the player 3 choices of dialogue lines for the protagonist, {protagonistName}, to choose.
This choice should impact the trajectory of the story.
Each line should include the speaking character's name, the text of the dialogue, and the speaker's mood.
Format the response as a plain JSON object with a top-level key 'Choices'.
Each entry under 'Choices' should be an object with the keys 'CharacterName', 'DialogueText', and 'Mood'.
Do not include any additional formatting or markers such as markdown code block markers.
";
    }

    public static string GetGenerateAdditionalDialoguePrompt(
        string choiceText,
        int linesPerScene)
    {
        string choicePart = string.IsNullOrEmpty(choiceText) ? "" : $"The player chose the dialogue option \"{choiceText}\".\n";
        return $@"
{choicePart}
From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue.
";
    }
}
