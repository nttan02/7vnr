using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class CharacterSaveSystem
{
    private static string Savepath => 
        Path.Combine(Application.persistentDataPath, "player.json");

    public static void SaveCharacter(Character character)
    {
        CharacterData data = new CharacterData
        {
            name = "Player",
            level = character.power,
            hpMax = character.hpMax,
            hp = character.hp,
            mpMax = character.mpMax,
            mp = character.mp,
            moveSpeed = character.moveSpeed,
            x = Mathf.RoundToInt(character.transform.position.x),
            y = Mathf.RoundToInt(character.transform.position.y),
            SkillIds = new List<int>()
        };

        foreach (Skill skill in character.skills)
        {
            data.SkillIds.Add(skill.skillId);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Savepath, json);
        Debug.Log($"Character saved to {Savepath}");
    }

    public static CharacterData LoadCharacter()
    {
        if (!File.Exists(Savepath))
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }

        string json = File.ReadAllText(Savepath);
        CharacterData data = JsonUtility.FromJson<CharacterData>(json);
        Debug.Log($"Character loaded from {Savepath}");
        return data;
    }

    public static bool HasSaveData()
    {
        return File.Exists(Savepath);
    }

    public static void DeleteSaveData()
    {
        if (File.Exists(Savepath))
        {
            File.Delete(Savepath);
            Debug.Log("Save data deleted.");
        }
    }
}