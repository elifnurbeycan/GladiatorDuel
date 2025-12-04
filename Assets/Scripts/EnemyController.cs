using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Gladiator enemy;
    public Gladiator player;

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
    
        yield return new WaitForSeconds(1.0f);

        bool actionDone = false;
        int safety = 0;

        while (!actionDone && safety < 10)
        {
            safety++;


            int choice = Random.Range(0, 5); 

            switch (choice)
            {
                case 0: actionDone = EnemyMove(); break;
                case 1: actionDone = EnemyRanged(); break;
                case 2: actionDone = EnemyMelee(); break;
                case 3: actionDone = EnemySleep(); break;
                case 4: actionDone = EnemyArmorUp(); break;
            }
            yield return null; 
        }

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.EndEnemyTurn();
    }



    private bool EnemyMove()
    {
        if (!enemy.SpendMana(4)) return false;

       
        GameManager.Instance.uiManager.UpdateBattleLog("Rakip Hareket Etti");

       
        bool forward = Random.value > 0.5f;

        if (forward)
            GameManager.Instance.MoveCloser(false); 
        else
            GameManager.Instance.MoveAway(false);

        return true;
    }

    private bool EnemyRanged()
    {
        if (enemy.currentAmmo <= 0) return false;
        if (!enemy.SpendMana(20)) return false;
        if (GameManager.Instance.currentDistance == DistanceLevel.Close) return false;

   
        GameManager.Instance.uiManager.UpdateBattleLog("Rakip Ok Fırlattı!");

        enemy.currentAmmo--;
        
        int damage = Random.Range(15, 21);
        
      
        enemy.ShootProjectile("Player", damage);

        return true;
    }

    private bool EnemyMelee()
    {
        if (GameManager.Instance.currentDistance != DistanceLevel.Close) return false;

      
        bool power = Random.value > 0.5f;
        int manaCost = power ? 30 : 10;

        if (!enemy.SpendMana(manaCost)) return false;

     
        if(power) GameManager.Instance.uiManager.UpdateBattleLog("Rakip Güçlü Saldırdı!");
        else GameManager.Instance.uiManager.UpdateBattleLog("Rakip Hızlı Saldırdı!");

        enemy.TriggerAttack();

        int dmg = 0;
        if (power)
        {
            if (Random.value > 0.5f) 
            {
                GameManager.Instance.uiManager.UpdateBattleLog("Rakip Iskaladı!");
                return true; 
            }
            dmg = Random.Range(25, 36);
        }
        else
        {
            if (Random.value > 0.85f)
            {
                GameManager.Instance.uiManager.UpdateBattleLog("Rakip Iskaladı!");
                return true; 
            }
            dmg = Random.Range(10, 13);
        }

        player.TakeDamage(dmg);
        return true;
    }

    private bool EnemySleep()
    {
        if (enemy.currentMana >= enemy.maxMana) return false;

       
        GameManager.Instance.uiManager.UpdateBattleLog("Rakip Dinleniyor...");

        enemy.RestoreMana(40);
        enemy.RestoreHP(15);
        return true;
    }

    private bool EnemyArmorUp()
    {
        if (!enemy.SpendMana(25)) return false;

       
        GameManager.Instance.uiManager.UpdateBattleLog("Rakip Savunmaya Geçti!");

        enemy.ActivateArmorUp(2);
        return true;
    }
}
