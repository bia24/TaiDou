using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillManager{

    private List<Skill> skillList;
    public SkillManager()
    {
        Init();
    }


    #region 逻辑方法
    private  void Init()
    {
        skillList = new List<Skill>();
        string skillText = Resources.Load<TextAsset>("Skill").text;
        string[] lines = skillText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach(string line in lines)
        {
            string[] skillInfo = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Skill skill = new Skill();
            skill.Id = int.Parse(skillInfo[0]);
            skill.Name = skillInfo[1];
            skill.Icon = skillInfo[2];
            switch (skillInfo[3])
            {
                case "Man":
                    skill.PlayerType = CharacterType.Man;
                    break;
                case "Woman":
                    skill.PlayerType = CharacterType.Woman;
                    break;
            }
            switch (skillInfo[4])
            {
                case "Basic":
                    skill.SkiiType = SkillType.Basic;
                    break;
                case "Skill":
                    skill.SkiiType = SkillType.Skill;
                    break;
            }
            switch (skillInfo[5])
            {
                case "Basic":
                    skill.PosType = PosType.Basic;
                    break;
                case "One":
                    skill.PosType = PosType.One;
                    break;
                case "Two":
                    skill.PosType = PosType.Two;
                    break;
                case "Three":
                    skill.PosType = PosType.Three;
                    break;
            }
            skill.ColdTime = int.Parse(skillInfo[6]);
            skill.Damage = int.Parse(skillInfo[7]);
            skill.Level = 1;
            skillList.Add(skill);
        }
    }
    public Skill GetSkill(CharacterType playerType,PosType pos)
    {
        foreach(var t in skillList)
        {
            if (t.PlayerType == playerType && t.PosType == pos)
            {
                return t;
            }
        }
        return null;
    }
    #endregion
}
