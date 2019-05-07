using UnityEngine;
using Board;

[CreateAssetMenu(fileName ="BoardSettings",menuName ="Custom Objects/Board Settings",order =0)]
public class BoardScriptableObject : ScriptableObject
{
    public int width;
    public int height;
    public BgTileView backgroundTileView;
}
