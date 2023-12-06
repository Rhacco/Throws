using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    public int Selected { get; private set; }
    private readonly List<string> difficulties = new() { "Easy", "Harder", "Hard", "Hardest" };

    public void OnClick()
    {
        if (Selected++ > difficulties.Count - 2)
            Selected = 0;
        GetComponent<TMP_Text>().text = difficulties[Selected];
    }
}
