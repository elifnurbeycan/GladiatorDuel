using UnityEngine;

public class Gladiator : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator; 

    [Header("Base Stats")]
    public int maxHP = 100;
    public int currentHP;

    public int maxMana = 120;
    public int startMana = 80;
    public int currentMana;

    [Header("Ranged")]
    public int maxAmmo = 10;
    public int currentAmmo;

    [Header("Armor Up")]
    public bool armorUpActive = false;
    public int armorUpTurnsRemaining = 0; 

    [Header("Audio")]
    public AudioSource audioSource;   // Karakterin üzerindeki Audio Source
    public AudioClip attackSound;     // Vuruş Sesi
    public AudioClip hitSound;        // Hasar/Acı Sesi
    public AudioClip walkSound;       // Yürüme Sesi (Loop)

    [Header("Projectile Settings")]
    public GameObject arrowPrefab;    // Fırlatılacak Ok Prefab'ı
    public Transform firePoint;       // Okun çıkacağı nokta

    private void Awake()
    {
        currentHP = maxHP;
        currentMana = Mathf.Clamp(startMana, 0, maxMana);
        currentAmmo = maxAmmo;
    }

    
    private void Start()
    {
        
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        if (audioSource != null)
        {
            audioSource.volume = sfxVol; // Karakterin sesini ayarla
        }
    }

    // --- FIRLATMA SİSTEMİ ---
    
    public void ShootProjectile(string targetTag, int dmg)
    {
        // 1. Animasyon ve Ses
        TriggerAttack(); 

        // 2. Pozisyonu Al
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
        
        
        Quaternion spawnRot;

        // Eğer bu scripti çalıştıran kişi "Enemy" ise;
        if (gameObject.CompareTag("Enemy"))
        {
            // Oku 180 derece döndür (Sola baksın)
            spawnRot = Quaternion.Euler(0, 0, 180f);
        }
        else
        {
            // Player ise düz kalsın (Sağa baksın)
            spawnRot = Quaternion.identity; 
        }

        // 3. Oku Yarat
        if (arrowPrefab != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, spawnPos, spawnRot);
            
            Projectile p = arrow.GetComponent<Projectile>();
            if (p != null)
            {
                p.damage = dmg;
                p.targetTag = targetTag;
            }
        }
        else
        {
            Debug.LogWarning("Arrow Prefab atanmamış!");
        }
    }

    // --- ANİMASYON VE SES FONKSİYONLARI ---

    // 1. Yürüme Animasyonu ve Sesi (Aç/Kapa)
    public void SetMoveAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

    public void ToggleWalkSound(bool isWalking)
    {
        if (audioSource == null || walkSound == null) return;

        if (isWalking)
        {
            
            if (!audioSource.isPlaying || audioSource.clip != walkSound)
            {
                audioSource.clip = walkSound;
                audioSource.loop = true; 
                audioSource.Play();
            }
        }
        else
        {
            // Yürüme bittiyse durdur
            if (audioSource.clip == walkSound)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
        }
    }

    // 2. Saldırı Animasyonu ve Sesi (Tetikleyici)
    
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    // 3. Hasar ve Ölüm Mantığı
    public void TakeDamage(int amount)
    {
        
        if (currentHP <= 0) return;

        float finalDamage = amount;

        if (armorUpActive)
        {
            finalDamage *= 0.8f; 
        }

        currentHP -= Mathf.RoundToInt(finalDamage);
        if (currentHP < 0) currentHP = 0;

        if (animator != null)
        {
            if (currentHP <= 0)
            {
                // ÖLÜM
                animator.SetTrigger("Death");
            }
            else
            {
                // HASAR ALMA
                animator.SetTrigger("Hit");

                if (audioSource != null && hitSound != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
    }

    // --- YARDIMCI FONKSİYONLAR ---

    public bool SpendMana(int amount)
    {
        if (currentMana < amount) return false;
        currentMana -= amount;
        return true;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
    }

    public void RestoreHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    public void ActivateArmorUp(int turns)
    {
        armorUpActive = true;
        armorUpTurnsRemaining = turns;
    }

    public void OnTurnEnd()
    {
        if (armorUpActive)
        {
            armorUpTurnsRemaining--;
            if (armorUpTurnsRemaining <= 0)
            {
                armorUpActive = false;
            }
        }
    }
}
