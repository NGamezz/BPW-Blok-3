using UnityEngine;

[System.Serializable]
public class Skill : MonoBehaviour
{
    public int WhichType = 0;
    public string SkillName;

    [SerializeField] private float damage = 10f;

    private float damageMultiplier = 1;

    private Character owner;

    public void Initialize(Character owner)
    {
        this.owner = owner;
    }

    public void IncreaseDamageMultiplier(float amount)
    {
        damageMultiplier += amount;
        if (damageMultiplier > 2.5f)
        {
            damageMultiplier = 2.5f;
        }
    }

    public void Attack()
    {
        if (owner == null || this == null) { return; }
        owner.MoveText.text = SkillName;
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
        if (GameManager.Instance == null || this == null || owner == null) { return; }
        if (GameManager.Instance.EntitiesInCombat.Count == 0) { return; }

        for (int i = 0; i < GameManager.Instance.EntitiesInCombat.Count; i++)
        {
            if (GameManager.Instance == null || this == null || owner == null) { return; }
            if (GameManager.Instance.EntitiesInCombat[i] != owner)
            {
                GameManager.Instance.EntitiesInCombat[i].ShakePlayer();
                GameManager.Instance.EntitiesInCombat[i].TakeDamage(damage * damageMultiplier);
            }
        }

        if (GameManager.Instance.Player.CurrentTurn == true)
        {
            GameManager.Instance.ChangeTurn();
        }
    }

    private void Staff()
    {
        if (GameManager.Instance == null) { return; }
        owner.Heal(damage * damageMultiplier);

        if (GameManager.Instance.Player.CurrentTurn == true)
        {
            GameManager.Instance.ChangeTurn();
        }
    }

    private void Shield()
    {
        if (GameManager.Instance == null) { return; }
        int randomInt = Random.Range(0, 3);
        if (randomInt == 0)
        {
            owner.Shield = true;
        }
        if (GameManager.Instance.Player.CurrentTurn == true)
        {
            GameManager.Instance.ChangeTurn();
        }
    }
}
