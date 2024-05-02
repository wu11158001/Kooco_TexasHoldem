using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableObject", menuName = "MyMenu/Create SpriteData", order = 1)]
public sealed class ScriptableObject_SpriteData : ScriptableObject
{
    public SpriteData sprites;
}

[System.Serializable]
public sealed class SpriteData
{
    public Sprite[] spritesList;
}
