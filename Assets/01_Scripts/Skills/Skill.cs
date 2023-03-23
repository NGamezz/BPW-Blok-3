using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill : MonoBehaviour
{
    public int WhichType = 0;
    public string SkillName;
    [SerializeField] private float damage = 10f;
    private float damageMultiplier = 1;

    private List<Character> entities = new();

    public void Initialize()
    {
        entities.Clear();
        entities.AddRange(TurnManager.Instance.Entities);

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].inCombat)
            {
                entities.RemoveAt(i);
            }
        }
    }

    public void Attack()
    {
        switch (WhichType)
        {
            case 0:
                Sword();
                break;
            case 1:
                Staff();
                break;
            case 2:
                Shield();
                break;
        }
    }

    private void Sword()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].CurrentTurn == false)
            {
                entities[i].TakeDamage((damage * damageMultiplier));
                entities[i].CurrentTurn = true;
            }
            else
            {
                entities[i].CurrentTurn = false;
            }
        }
        Debug.Log("Sword");
    }

    private void Staff()
    {

    }

    private void Shield()
    {

    }
}
