using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ADDITIONALTYPE
{
    附加武力,
    附加内力,
    附加伤害,
    附加暴击,
    武力,
    内力,
    生命,

    无
}


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 2)]
public class Skill : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public int skillLevel = 1;
    [Range(0, 5)] public float skillSpeed = 2;


    public Ability[] abilities;

    public AnimationCurve skillLevelRaise;

    public AnimationCurve skillLevelUpCost;



}


[System.Serializable]
public class Ability
{
    public ADDITIONALTYPE additionalType = ADDITIONALTYPE.无;
    public int value = 0;
}