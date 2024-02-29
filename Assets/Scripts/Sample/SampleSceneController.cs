using System.Collections.Generic;
using UnityEngine;

namespace SCD.Sample
{
    public class SampleSceneController : MonoBehaviour
    {
        private readonly Stats stats = new();

        private readonly List<Content> contents = new()
        {
            new Content(
                new List<Stats.Record>
                {
                    new() { name = "Game.Begin", value = 1 }
                },
                new List<Stats.Record>
                {
                    new() { name = "Item.Wood", value = 5 }
                },
                new List<Stats.Record>
                {
                    new() { name = "Content.Cleared.1", value = 1 }
                }
            )
        };
    }
}
