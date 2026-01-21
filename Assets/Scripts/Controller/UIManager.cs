using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    [SerializeField] private SkillAttackPanel skillAttackPanel;
    [SerializeField] private UIPanel uiPanel;
  
    public void SetSkillButtons(List<Skill> skills)
    {
        skillAttackPanel.SetSkillButtons(skills.ToArray());
    }

    
    void Update()
    {

    }
}
