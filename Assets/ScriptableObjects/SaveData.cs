using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save Data", menuName = "Custom Types/Save Data")]
public class SaveData : ScriptableObject
{
    public List<string> roles;
    public List<string> messages;
}