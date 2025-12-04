using UnityEngine;
using System.Collections;

public enum DistanceLevel
{
    Close,
    Mid,
    Far
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public Gladiator player;
    public Gladiator enemy;
    public UIManager uiManager;
    public EnemyController enemyController;

    [Header("Transforms")]
    public Transform playerTransform;
    public Transform enemyTransform;

    [Header("Turn / State")]
    public bool isPlayerTurn = true;
    public DistanceLevel currentDistance = DistanceLevel.Far;

    [Header("Audio Settings")]
    public AudioSource musicSource; 

    // 🔥 YENİ AYARLAR: ADIM VE SINIRLAR 🔥
    
    // 1. Bir adımda kaç birim gidilsin? (Küçük adım)
    private float stepSize = 2.0f; 

    // 2. Sahnenin en ucu neresi? (Kırmızı Çarpılar)
    // Karakterler -7.5 ile 7.5 arasında hapsolacak.
    private float mapBoundary = 7.5f;

    // 3. Karakterler birbirine en fazla ne kadar yaklaşabilir? (İç içe girmesinler)
    private float minDistanceBetween = 1.5f; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        isPlayerTurn = true;
        
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioListener.volume = musicVol;

        if (musicSource != null)
        {
            musicSource.loop = true;
            if (!musicSource.isPlaying) musicSource.Play();
        }

        // Başlangıçta en uçlara (Duvarlara) yerleştir
        InitPositions();
        
        // Mesafeyi hesapla ve UI güncelle
        UpdateDistanceState();
        uiManager.UpdateAllUI();
        uiManager.SetTurnText("Oyuncu Sırası");
        uiManager.UpdateActionButtonsInteractable(true);
    }

    private void InitPositions()
    {
        if (playerTransform == null || enemyTransform == null) return;

        // Player En Sola (-7.5), Enemy En Sağa (7.5)
        playerTransform.position = new Vector3(-mapBoundary, playerTransform.position.y, playerTransform.position.z);
        enemyTransform.position  = new Vector3(mapBoundary, enemyTransform.position.y, enemyTransform.position.z);
    }

    // =======================================================
    // BAĞIMSIZ HAREKET MANTIĞI (Sadece Oynayan Hareket Eder)
    // =======================================================

    public void MoveCloser(bool actorIsPlayer)
    {
        // Hedef pozisyonu hesapla
        float currentX = actorIsPlayer ? playerTransform.position.x : enemyTransform.position.x;
        float targetX;

        if (actorIsPlayer)
        {
            // Player SAĞA (+) gider
            targetX = currentX + stepSize;
            
            // Düşmana çarpmamak için sınırla (Düşmanın biraz solunda durmalı)
            float limit = enemyTransform.position.x - minDistanceBetween;
            if (targetX > limit) targetX = limit;
        }
        else
        {
            // Enemy SOLA (-) gider
            targetX = currentX - stepSize;

            // Oyuncuya çarpmamak için sınırla (Oyuncunun biraz sağında durmalı)
            float limit = playerTransform.position.x + minDistanceBetween;
            if (targetX < limit) targetX = limit;
        }

        // Hareketi Başlat
        StartCoroutine(SmoothMoveRoutine(actorIsPlayer, targetX));
    }

    public void MoveAway(bool actorIsPlayer)
    {
        float currentX = actorIsPlayer ? playerTransform.position.x : enemyTransform.position.x;
        float targetX;

        if (actorIsPlayer)
        {
            // Player SOLA (-) kaçar
            targetX = currentX - stepSize;
            
            // Duvar Sınırı (-7.5)
            if (targetX < -mapBoundary) targetX = -mapBoundary;
        }
        else
        {
            // Enemy SAĞA (+) kaçar
            targetX = currentX + stepSize;

            // Duvar Sınırı (7.5)
            if (targetX > mapBoundary) targetX = mapBoundary;
        }

        // Hareketi Başlat
        StartCoroutine(SmoothMoveRoutine(actorIsPlayer, targetX));
    }

    // =======================================================
    // GÖRSEL GÜNCELLEME
    // =======================================================

    private IEnumerator SmoothMoveRoutine(bool actorIsPlayer, float targetX)
    {
        // 1. Sadece hareket eden kişinin animasyonunu aç
        if (actorIsPlayer) { player.SetMoveAnimation(true); player.ToggleWalkSound(true); }
        else { enemy.SetMoveAnimation(true); enemy.ToggleWalkSound(true); }

        Transform movingTransform = actorIsPlayer ? playerTransform : enemyTransform;
        Vector3 startPos = movingTransform.position;
        Vector3 endPos = new Vector3(targetX, startPos.y, startPos.z);

        float duration = 0.8f; // Hareket süresi (Biraz hızlandırdım)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smooth step

            movingTransform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        movingTransform.position = endPos;

        // 2. Animasyonu kapat
        if (actorIsPlayer) { player.SetMoveAnimation(false); player.ToggleWalkSound(false); }
        else { enemy.SetMoveAnimation(false); enemy.ToggleWalkSound(false); }

        // 3. Mesafeyi Ölç ve Durumu Güncelle (Far/Mid/Close)
        UpdateDistanceState();
    }

    // 🔥 MESAFEYİ ÖLÇÜP DURUMU GÜNCELLEYEN FONKSİYON 🔥
    private void UpdateDistanceState()
    {
        float dist = Vector3.Distance(playerTransform.position, enemyTransform.position);

        // Mesafe eşikleri (Senin isteğine göre ayarladım)
        // Close: < 2.5 birim
        // Mid:   2.5 ile 7.0 arası
        // Far:   > 7.0 birim
        
        if (dist <= 2.5f)
        {
            currentDistance = DistanceLevel.Close;
        }
        else if (dist > 2.5f && dist <= 7.0f)
        {
            currentDistance = DistanceLevel.Mid;
        }
        else
        {
            currentDistance = DistanceLevel.Far;
        }

        uiManager.UpdateDistanceText(currentDistance);
    }

    // --- (Diğer yardımcı fonksiyonlar aynen kalıyor) ---
    public void EndPlayerTurn() { player.OnTurnEnd(); uiManager.UpdateAllUI(); CheckGameEnd(); if (IsGameOver()) return; isPlayerTurn = false; uiManager.SetTurnText("Rakip Sırası"); uiManager.UpdateActionButtonsInteractable(false); enemyController.StartEnemyTurn(); }
    public void EndEnemyTurn() { enemy.OnTurnEnd(); uiManager.UpdateAllUI(); CheckGameEnd(); if (IsGameOver()) return; isPlayerTurn = true; uiManager.SetTurnText("Oyuncu Sırası"); uiManager.UpdateActionButtonsInteractable(true); }
    private void CheckGameEnd() { if (player.currentHP <= 0) { uiManager.SetTurnText("Kaybettin!"); uiManager.UpdateActionButtonsInteractable(false); } else if (enemy.currentHP <= 0) { uiManager.SetTurnText("Kazandın!"); uiManager.UpdateActionButtonsInteractable(false); } }
    private bool IsGameOver() { return player.currentHP <= 0 || enemy.currentHP <= 0; }
}