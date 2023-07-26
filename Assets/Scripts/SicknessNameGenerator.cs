using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SicknessNameGenerator
{
    private string[] firstWords = {
        "hardened", "swift", "sluggish", "furious", "gentle", "frantic", "shimmering", "shadowy", "dusky", "fiery",
        "glowing", "whispering", "enigmatic", "luminous", "mysterious", "sparkling", "radiant", "brilliant", "tranquil", "calm",
        "graceful", "beautiful", "majestic", "magnificent", "regal", "wondrous", "dazzling", "resplendent", "ethereal", "serene",
        // Add more words here ...
        // ...
        "golden", "silver", "crimson", "emerald", "azure", "ebony", "diamond", "sapphire", "velvet", "crystal"
    };

    private string[] secondWords = {
        "bean", "tiger", "whisper", "thunder", "flame", "serpent", "ocean", "moon", "storm", "sunset",
        "star", "night", "dream", "river", "shadow", "rain", "sky", "forest", "soul", "dawn",
        "dusk", "horizon", "cascade", "glimmer", "horizon", "cascade", "glimmer", "wanderer", "voyager", "solitude",
        // Add more words here ...
        // ...
        "mist", "star", "night", "dream", "river", "shadow", "rain", "sky", "forest", "soul"
    };

    public string GenerateRandomSicknessName()
    {
        string firstPart = firstWords[Random.Range(0, firstWords.Length)];
        string secondPart = secondWords[Random.Range(0, secondWords.Length)];

        return firstPart + "-" + secondPart;
    }
}