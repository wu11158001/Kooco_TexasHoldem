using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableObject", menuName = "MyMenu/Create SpriteAlbum", order = 1)]
[System.Serializable]
public sealed class SpriteAlbum : ScriptableObject
{
    public Sprite[] album;
}