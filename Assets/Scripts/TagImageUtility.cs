using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TagImageUtility
{
    private static string GetKeyFromTags<T>(List<T> tags) where T : Enum
    {
        tags.Sort();

        return string.Join(",", tags);
    }

    private static Sprite GetCachedImage<T>(List<T> tags, Dictionary<string, Sprite> imageCache) where T : Enum
    {
        string key = GetKeyFromTags(tags);

        if (imageCache.TryGetValue(key, out Sprite cachedImage))
        {
            return cachedImage;
        }

        return null;
    }

    public static Sprite GetImageWithTags<T>(List<T> desiredTags, Dictionary<T, List<Sprite>> tagDictionary, Dictionary<string, Sprite> imageCache) where T : Enum
    {
        Sprite cachedImage = GetCachedImage(desiredTags, imageCache);

        if (cachedImage != null)
        {
            return cachedImage;
        }

        IEnumerable<Sprite> candidateImages = desiredTags.SelectMany(tag => tagDictionary[tag]);
        IEnumerable<IGrouping<Sprite, Sprite>> groupedCandidates = candidateImages.GroupBy(i => i);
        int maxCount = groupedCandidates.Max(g => g.Count());
        List<IGrouping<Sprite, Sprite>> maxGroups = groupedCandidates.Where(g => g.Count() == maxCount).ToList();
        IGrouping<Sprite, Sprite> selectedGroup = maxGroups[UnityEngine.Random.Range(0, maxGroups.Count)];
        string key = GetKeyFromTags(desiredTags);

        imageCache[key] = selectedGroup.Key;

        return selectedGroup.Key;
    }
}
