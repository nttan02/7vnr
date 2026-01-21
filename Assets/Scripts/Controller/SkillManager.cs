using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTemplateWrapper
{
    public List<SkillTemplate> skillTemplates;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private Dictionary<int, SkillTemplate> SkillTemplates;
    private Dictionary<int, Skill> Skills;

    [SerializeField] private TextAsset skillDataJson;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadSkillTemplates();
    }
    
    private void LoadSkillTemplates()
    {
        if (skillDataJson == null)
        {
            Debug.LogError("Không tìm thấy skillTemplates.json");
            return;
        }

        SkillTemplateWrapper wrapper =
            JsonUtility.FromJson<SkillTemplateWrapper>(skillDataJson.text);

        SkillTemplates = new Dictionary<int, SkillTemplate>();
        Skills = new Dictionary<int, Skill>();

        foreach (var template in wrapper.skillTemplates)
        {
            SkillTemplates[template.id] = template;

            Skill skill = new Skill
            {
                skillId = template.id,
                template = template,
                dame = template.damage,
                cooldown = template.cooldown
            };

            Skills[skill.skillId] = skill;
        }
    }

    public SkillTemplate GetSkillTemplate(int id)
    {
        if (SkillTemplates.TryGetValue(id, out SkillTemplate template))
        {
            return template;
        }
        Debug.LogWarning($"Không tìm thấy skill với ID {id}");
        return null;
    }

    public void UseSkill(int skillId)
    {

        Skill skill = Character.Instance.skills.Find(s => s.skillId == skillId);

        if (skill == null)
        {
            return;
        }

        Enemy target = Character.Instance.targetEnemy;
        if (target == null)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }

        long currentTime = GameManager.GetCurrentMilisecond();
        long timeSinceLastUse = currentTime - skill.lastTimeUseSkill;

        if (timeSinceLastUse < skill.cooldown)
        {
            long timeLeft = skill.cooldown - timeSinceLastUse;
            return;
        }

        string effectName = EffectManager.GetEffectCharAttackObjById(skill.template.id);

        EffectCharAttackObj.AddEffect(effectName, Character.Instance.transform.position, Character.Instance);
        Character.Instance.SetAnimation(effectName);

        skill.lastTimeUseSkill = currentTime;
        Character.Instance.PrepareSkillHit(target, skill.dame);
        Character.Instance.SetAnimation(effectName, false);

        target.TakeDamage(skill.dame);
    }


    public Skill GetSkillById(int skillId)
    {
        if (Skills.TryGetValue(skillId, out Skill skill))
        {
            Skill newSkill = new Skill
            {
                skillId = skill.skillId,
                cooldown = skill.template.cooldown,
                dame = skill.template.damage,
                template = skill.template,
                lastTimeUseSkill = 0
            };
            return newSkill;
        }
        Debug.LogWarning($"Không tìm thấy kỹ năng với ID {skillId}");
        return null;
    }

}
