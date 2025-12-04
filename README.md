# ⚔️ Gladiator Duel - Unity Turn-Based Strategy Game

> **Bu proje, Unity Oyun Motoru kullanılarak geliştirilmiş, fizik tabanlı ve sıra tabanlı bir 2D strateji oyunudur.**

🎮 **Tarayıcıda Oyna (WebGL):** https://elifnurbeycan.itch.io/gladiator-duel

---

## 📸 Oyun İçi Görseller

*(Projenizden aldığınız ekran görüntülerini GitHub'a yükleyip linklerini aşağıya ekleyebilirsiniz)*

| Ana Menü | Savaş Ekranı |
| :---: | :---: |
| <img width="957" height="597" alt="Ekran görüntüsü 2025-12-04 230942" src="https://github.com/user-attachments/assets/04f3d776-8472-49c5-9ace-be3454e1ed6d" />
Ana+Menu+Gorseli) | <img width="931" height="520" alt="Ekran görüntüsü 2025-12-04 231832" src="https://github.com/user-attachments/assets/fe257a08-4cb0-45a7-864b-bdb3b1d0b1e9" />
Gorseli) |
| *Oyun Giriş Ekranı* | *Sıra Tabanlı Savaş Anı* |

---

## 🕹️ Oynanış ve Mekanikler

Oyun, oyuncunun ve rakibin sırayla hamle yaptığı taktiksel bir savaş simülasyonudur. Her karakterin **Can (HP)**, **Mana** ve **Mermi (Ammo)** kaynakları vardır. Stratejik kararlar vererek rakibi alt etmeye çalışırsınız.

### 🎮 Aksiyon Listesi (Actions)

Oyuncu ve Rakip aşağıdaki 5 temel aksiyonu kullanabilir:

| Aksiyon Adı | Gereksinim (Cost) | Açıklama |
| :--- | :--- | :--- |
| **🏃 Move (Hareket)** | `4 Mana` | Karakter ileri veya geri adım atar. Mesafe (Far/Mid/Close) dinamik olarak değişir. |
| **🏹 Ranged (Ok Atma)** | `20 Mana` + `1 Ammo` | Fizik tabanlı bir ok fırlatır. Duvarlardan geçer, sadece hedefe çarparsa hasar verir. *(Close mesafede kullanılamaz)* |
| **⚔️ Melee (Kılıç)** | `10` veya `30 Mana` | Yakın dövüş saldırısı. Hızlı (Quick) veya Güçlü (Power) seçenekleri vardır. *(Sadece Close mesafede)* |
| **🛡️ Armor Up** | `25 Mana` | Defansif duruş. 2 tur boyunca alınan tüm hasarı %20 azaltır. |
| **💤 Sleep (Uyku)** | `0 Mana` | Turu pas geçer. Karşılığında `+40 Mana` ve `+15 HP` yeniler. |

---

## 🎨 Animasyon Sistemi

Oyun, karakterlerin durumuna göre **Animator State Machine** kullanarak akıcı geçişler sağlar.

| Animasyon | Tetikleyici (Trigger/Bool) | Durum |
| :--- | :--- | :--- |
| **Idle** | `Default` | Karakterin hareketsiz, nefes aldığı bekleme hali. |
| **Run** | `Bool: IsMoving` | Karakter hareket ederken oynatılır (Loop). |
| **Attack** | `Trigger: Attack` | Ok atarken veya Kılıç vururken oynatılan savurma hareketi. |
| **Hit** | `Trigger: Hit` | Karakter hasar aldığında sarsılma efekti ("Any State" üzerinden çalışır). |
| **Death** | `Trigger: Death` | HP sıfırlandığında karakterin yere yığılması. |

---

## 🔊 Ses ve Müzik (Audio Assets)

Oyun atmosferini güçlendirmek için aşağıdaki ses efektleri ve müzikler kullanılmıştır. Ses seviyeleri Ana Menü üzerinden kontrol edilebilir.

| Ses Dosyası | Kullanım Yeri | Açıklama |
| :--- | :--- | :--- |
| **🎵 Background Music** | Genel | Ana Menü ve Savaş ekranında sürekli çalan döngüsel (loop) atmosfer müziği. |
| **👣 Walk Sound** | Hareket | Karakter `Move` aksiyonunu gerçekleştirirken çalan adım sesleri. |
| **⚔️ Attack Sound** | Saldırı | Ok fırlatma veya Kılıç savurma anında çalan efekt. |
| **💥 Hit/Damage Sound** | Hasar Alma | Karakter veya Rakip hasar aldığında çalan darbe sesi. |

---

## ⚙️ Teknik Özellikler ve Geliştirme Notları

Bu proje eğitim amaçlı geliştirilmiştir ve aşağıdaki teknik yapıları içerir:

* **Fizik Tabanlı Atış (Projectile System):** Oklar `Instantiate` ile oluşturulur, `Rigidbody 2D` ve `Box Collider 2D` kullanılarak hedefe fiziksel olarak iletilir. Çarpışma anında `OnTriggerEnter2D` ile hasar hesaplanır.
* **Dinamik Mesafe Yönetimi:** Karakterler arasındaki mesafe (Close, Mid, Far) matematiksel olarak hesaplanır. `Mathf.Clamp` kullanılarak karakterlerin kamera dışına çıkması engellenmiştir.
* **Rastgele Davranış (Non-AI):** Rakip karakter kural tabanlı bir zeka (If-Else AI) yerine, tamamen **Rastgele (Random)** kararlar vererek oynar (Ödev kuralları gereği).
* **Ses Yönetimi:** `PlayerPrefs` kullanılarak Ana Menü ve Oyun içindeki Müzik/SFX ses seviyeleri kaydedilir ve sahneler arası taşınır.
* **UI & UX:** Savaş günlüğü (Battle Log) ile yapılan hamleler anlık olarak ekrana yazdırılır. Can ve Mana barları dinamik olarak güncellenir.

---

## 🚀 Kurulum (Unity Editör)

Projeyi kendi bilgisayarınızda çalıştırmak için:

1.  Bu repoyu klonlayın: `git clone [REPO LINKI]`
2.  **Unity Hub** üzerinden projeyi açın (Önerilen Sürüm: 2022.3 LTS veya üzeri).
3.  `Scenes` klasöründen **MainMenu** sahnesini açın.
4.  Play tuşuna basın.

---

**Geliştirici:** Elif Nur Beycan
**Ders:** Oyun Programlama
