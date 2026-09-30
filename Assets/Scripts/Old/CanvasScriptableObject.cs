using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName ="CanvasParamwter", menuName="ScriptableObjects/CanvasParameter", order=1)]
public class CanvasScriptableObject : ScriptableObject
{
    [SerializeField] 
    public float TimeLimit;//ŠÔ§ŒÀ

    [SerializeField]
    public float TitleMoveSpeed;//TitleƒƒS‚ÌˆÚ“®‘¬“x
}