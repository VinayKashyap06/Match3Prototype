using UnityEngine;
using System.Collections.Generic;
using Board;

[CreateAssetMenu(fileName ="BoardSettings",menuName ="Custom Objects/Board Settings",order =0)]
public class BoardScriptableObject : ScriptableObject
{
    public int width;
    public int height;
    public BgTileView backgroundTileView;
    public List<BlockDataStruct> blockList= new List<BlockDataStruct>();
}
