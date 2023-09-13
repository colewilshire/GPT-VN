using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TaggedImageUtility
{
    private static string GetKeyFromTags<T>(List<T> tags) where T : Enum
    {
        tags.Sort();

        return string.Join(",", tags);
    }

    private static U GetCachedImage<T, U>(List<T> tags, Dictionary<string, U> imageCache) where T : Enum where U : TaggedImage<T>
    {
        if (imageCache == null) return null;

        string key = GetKeyFromTags(tags);

        if (imageCache.TryGetValue(key, out U cachedImage))
        {
            return cachedImage;
        }

        return null;
    }

    public static U GetImageWithTags<T, U>(List<T> desiredTags, Dictionary<T, List<U>> tagDictionary, Dictionary<string, U> imageCache = null) where T : Enum where U : TaggedImage<T>
    {
        if (desiredTags.Count == 0) return null;

        U cachedImage = GetCachedImage(desiredTags, imageCache);

        if (cachedImage != null)
        {
            return cachedImage;
        }

        IEnumerable<U> candidateImages = desiredTags.Where(tag => tagDictionary.ContainsKey(tag)).SelectMany(tag => tagDictionary[tag]);
        IEnumerable<IGrouping<U, U>> groupedCandidates = candidateImages.GroupBy(i => i);

        if (!groupedCandidates.Any()) return null;

        int maxCount = groupedCandidates.Max(g => g.Count());
        List<IGrouping<U, U>> maxGroups = groupedCandidates.Where(g => g.Count() == maxCount).ToList();
        IGrouping<U, U> selectedGroup = maxGroups[UnityEngine.Random.Range(0, maxGroups.Count)];
        string key = GetKeyFromTags(desiredTags);

        if (imageCache != null)
        {
            imageCache[key] = selectedGroup.Key;
        }

        return selectedGroup.Key;
    }

    public static void OrganizeImagesByTag<T, U>(IEnumerable<TaggedImage<T>> taggedImages, Dictionary<T, List<U>> tagDictionary) where T : Enum where U : TaggedImage<T>
    {
        foreach (TaggedImage<T> taggedImage in taggedImages)
        {
            foreach (T tag in taggedImage.tags)
            {
                if (!tagDictionary.ContainsKey(tag))
                {
                    tagDictionary[tag] = new List<U>();
                }
                tagDictionary[tag].Add((U)taggedImage);
            }
        }
    }
}
