using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class EffectCharAttackObj : MonoBehaviour
{
    public Character character;
    public SkeletonAnimation skeAnim;
    public bool isRelease;

    public static void AddEffect(string nameEffect, Vector3 position, Transform parent = null)
    {
        var effect = EffectManager.Instance.effectCharAttackPool.Get();
        effect.transform.position = position;
        effect.skeAnim.AnimationState.SetAnimation(0, nameEffect, false);
        effect.skeAnim.AnimationState.Complete += (trackEntry) =>
        {
            if (effect.isRelease) return;
            EffectManager.Instance.effectCharAttackPool.Release(effect);
        };
    }
    public static void AddEffect(string nameEffect, Vector3 position, Character parent = null)
    {
        var effect = EffectManager.Instance.effectCharAttackPool.Get();
        effect.transform.position = position;
        effect.character = parent;
        effect.skeAnim.AnimationState.SetAnimation(0, nameEffect, false);
        effect.skeAnim.AnimationState.Complete += (trackEntry) =>
        {
            if (effect.isRelease) return;
            EffectManager.Instance.effectCharAttackPool.Release(effect);
            parent.ResetState();
        };
    }

    void Update()
    {
        if (character != null)
        {
            skeAnim.Skeleton.ScaleX = character.dir;
        }
    }
}
