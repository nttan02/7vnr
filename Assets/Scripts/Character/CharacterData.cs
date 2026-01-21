using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public string name;
    public int level;
    public int hpMax;
    public int hp;
    public int mpMax;
    public int mp;
    // public int attack;
    // public int defense;
    public float moveSpeed;
    public int x;
    public int y;
    public List<int> SkillIds;
    // public List<Skill> skills;
}
