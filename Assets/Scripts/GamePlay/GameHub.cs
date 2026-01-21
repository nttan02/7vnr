using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHub : MonoBehaviour
{
    [SerializeField] private SkillAttackPanel skillAttackPanel;
    [SerializeField] private GameObject TopPanelUI;
    [SerializeField] private GameObject LeftPanelUI;
    [SerializeField] private GameObject RightPanelUI;
    [SerializeField] private GameObject BottomPanelUI;
    
    public void SetSkillButtons(List<Skill> skills)
    {
        skillAttackPanel.SetSkillButtons(skills.ToArray());
    }
}
