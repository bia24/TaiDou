using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Basic,
    Skill
}
public enum PosType
{
    Basic,
    One,
    Two,
    Three
}

public class Skill {
   
    #region 字段
    private int id;
    private string name;
    private string icon;
    private CharacterType playerType;
    private SkillType skillType;
    private PosType posType;
    private int coldTime;
    private int damage;
    private int level = 1;
    #endregion

    #region 属性
    public int Id
    {
        get { return id;}
        set { id = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Icon
    {
        get { return icon; }
        set { icon = value; }
    }
    public CharacterType PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
    }
    public SkillType SkiiType
    {
        get { return skillType; }
        set { skillType = value; }
    }
    public PosType PosType
    {
        get { return posType; }
        set { posType = value; }
    }
    public int ColdTime
    {
        get { return coldTime; }
        set { coldTime = value; }
    }
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    #endregion

    #region 逻辑方法
    public void LevelUp()
    {
        Level++;
    }
    #endregion
}
