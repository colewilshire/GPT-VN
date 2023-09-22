using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TaggedImageUtility
{
    // Generate a string key for caching based on the provided list of tags.
    private static string GetKeyFromTags<T>(List<T> tags) where T : Enum
    {
        tags.Sort();

        return string.Join(",", tags);
    }

    // Try to retrieve an image from the cache based on the provided list of tags.
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

    // Return a random image from all the available tagged images.
    private static U GetRandomImage<T, U>(Dictionary<T, List<U>> tagDictionary) where T : Enum where U : TaggedImage<T>
    {
        List<U> allImages = tagDictionary.Values.SelectMany(list => list).Distinct().ToList();
        return allImages[UnityEngine.Random.Range(0, allImages.Count)];
    }

    // Get an image that matches the provided tags. If multiple images match, prioritize based on enum index. Retrieve a cached image if tags are identical.
    public static U GetImageWithTags<T, U>(List<T> desiredTags, Dictionary<T, List<U>> tagDictionary, Dictionary<string, U> imageCache = null) where T : Enum where U : TaggedImage<T>
    {
        if (desiredTags.Count == 0)
        {
            if (typeof(T) == typeof(AccessoryTag)) return null;
            return GetRandomImage(tagDictionary);
        }

        U cachedImage = GetCachedImage(desiredTags, imageCache);
        if (cachedImage != null) return cachedImage;

        IEnumerable<U> candidateImages = desiredTags.Where(tag => tagDictionary.ContainsKey(tag)).SelectMany(tag => tagDictionary[tag]);
        IEnumerable<IGrouping<U, U>> groupedCandidates = candidateImages.GroupBy(i => i);

        if (!groupedCandidates.Any()) return GetRandomImage(tagDictionary);

        int maxCount = groupedCandidates.Max(g => g.Count());
        List<IGrouping<U, U>> maxGroups = groupedCandidates.Where(g => g.Count() == maxCount).ToList();

        U selectedImage = maxGroups.OrderBy(g => 
        {
            T primaryTag = g.Key.Tags.Intersect(desiredTags).OrderBy(tag => Convert.ToInt32(tag)).FirstOrDefault();
            return primaryTag != null ? Convert.ToInt32(primaryTag) : int.MaxValue;
        })
        .FirstOrDefault().Key;

        string key = GetKeyFromTags(desiredTags);
        if (imageCache != null)
        {
            imageCache[key] = selectedImage;
        }

        return selectedImage;
    }

    // Organize a collection of tagged images into a dictionary based on their tags.
    public static void OrganizeImagesByTag<T, U>(IEnumerable<TaggedImage<T>> taggedImages, Dictionary<T, List<U>> tagDictionary) where T : Enum where U : TaggedImage<T>
    {
        foreach (TaggedImage<T> taggedImage in taggedImages)
        {
            foreach (T tag in taggedImage.Tags)
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

