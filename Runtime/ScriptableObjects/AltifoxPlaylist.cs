using UnityEngine;
using System.Collections.Generic;
using System;
using AltifoxTools;


[Serializable]
public struct AltifoxMusicNameCouple
{
    public string name;
    public AltifoxMusic altifoxMusic;
}

[CreateAssetMenu(fileName = "AltifoxPlaylist", menuName = "AltifoxPlaylist", order = 0)]
public class AltifoxPlaylist : ScriptableObject
{
    public AltifoxMusicNameCouple[] Items;
    public string defaultMusic;
}