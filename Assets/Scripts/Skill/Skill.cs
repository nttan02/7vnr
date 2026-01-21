using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Skill
{
    public int skillId;
    public int dame;
    public int level;
    public int cooldown;
    public int maxFight;
    public int manaCost;
    public int dx;
    public int dy;
    public long lastTimeUseSkill;
    public SkillTemplate template;

    public bool IsCooldown()
    {
        long currentTime = GameManager.GetCurrentMilisecond();
        long timeSinceLastUse = currentTime - lastTimeUseSkill;
        Debug.Log($"Time since last use: {timeSinceLastUse}ms, Cooldown: {cooldown}ms");
        return timeSinceLastUse >= cooldown;
    }
}