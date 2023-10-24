using System.Collections.Generic;

public class Thesaurus : Singleton<Thesaurus>
{
    private readonly Dictionary<string, Mood> moodThesaurus = new()
    {
        // Happy
        { "happy", Mood.Happy },
        { "cheerful", Mood.Happy },
        { "excited", Mood.Happy },
        { "eager", Mood.Happy },
        { "elated", Mood.Happy },
        { "overjoyed", Mood.Happy },
        { "thrilled", Mood.Happy },
        { "ecstatic", Mood.Happy },
        { "blissful", Mood.Happy },
        { "bubbly", Mood.Happy },
        { "radiant", Mood.Happy },
        { "satisfied", Mood.Happy },
        { "lively", Mood.Happy },
        { "jubilant", Mood.Happy },
        { "hopeful", Mood.Happy },
        { "vibrant", Mood.Happy },
        { "vigorous", Mood.Happy },
        { "zealous", Mood.Happy },
        { "dynamic", Mood.Happy },
        { "enthusiastic", Mood.Happy },
        { "exhilarated", Mood.Happy },
        { "animated", Mood.Happy },
        { "spirited", Mood.Happy },
        { "ebullient", Mood.Happy },
        { "buoyant", Mood.Happy },
        { "zesty", Mood.Happy },

        // Sad
        { "sad", Mood.Sad },
        { "heartbroken", Mood.Sad },
        { "melancholic", Mood.Sad },
        { "gloomy", Mood.Sad },
        { "despondent", Mood.Sad },
        { "depressed", Mood.Sad },
        { "forlorn", Mood.Sad },
        { "woeful", Mood.Sad },
        { "downhearted", Mood.Sad },
        { "disheartened", Mood.Sad },
        { "blue", Mood.Sad },

        // Angry
        { "angry", Mood.Angry },
        { "livid", Mood.Angry },
        { "outraged", Mood.Angry },
        { "heated", Mood.Angry },
        { "annoyed", Mood.Angry },
        { "irritated", Mood.Angry },
        { "cross", Mood.Angry },
        { "infuriated", Mood.Angry },
        { "exasperated", Mood.Angry },
        { "bothered", Mood.Angry },
        { "frustrated", Mood.Angry },

        // Shocked
        { "shocked", Mood.Shocked },
        { "amazed", Mood.Shocked },
        { "flabbergasted", Mood.Shocked },
        { "bewildered", Mood.Shocked },
        { "dazed", Mood.Shocked },
        { "thunderstruck", Mood.Shocked },
        { "baffled", Mood.Shocked },
        { "perplexed", Mood.Shocked },
        { "astounded", Mood.Shocked },
        { "gobsmacked", Mood.Shocked },
        { "awe-struck", Mood.Shocked },
        { "awestruck", Mood.Shocked },
        { "horrified", Mood.Shocked },

        // Awkward
        { "awkward", Mood.Awkward },
        { "ungraceful", Mood.Awkward },
        { "bumbling", Mood.Awkward },
        { "clunky", Mood.Awkward },
        { "gauche", Mood.Awkward },
        { "inelegant", Mood.Awkward },
        { "maladroit", Mood.Awkward },
        { "oafish", Mood.Awkward },
        { "ungainly", Mood.Awkward },
        { "klutzy", Mood.Awkward },
        { "embarassed", Mood.Awkward },
        { "mortified", Mood.Awkward },
        { "ashamed", Mood.Awkward },
        { "humiliated", Mood.Awkward },
        { "chagrined", Mood.Awkward },
        { "discomfited", Mood.Awkward },
        { "abashed", Mood.Awkward },
        { "self-conscious", Mood.Awkward },
        { "sheepish", Mood.Awkward },
        { "disconcerted", Mood.Awkward },
        { "red-faced", Mood.Awkward }
    };

    public Mood GetMoodSynonym(string emotion)
    {
        if (moodThesaurus.TryGetValue(emotion.ToLower(), out Mood mood))
        {
            return mood;
        }

        return Mood.Neutral;
    }
}
