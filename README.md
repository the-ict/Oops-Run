## 🎨 Graphics & Assets (Vizual qism)

Ushbu loyihaning barcha vizual elementlari **noldan ishlab chiqilgan** bo‘lib, o‘yinning **"Stylized"** (Stumble Guys uslubidagi) umumiy atmosferasini yaratishga qaratilgan. Dizayn jarayonida **sifat, soddalik va performance** o‘rtasidagi muvozanat asosiy mezon sifatida tanlangan.

---

## 🛠 Ishlatilgan dasturlar

- **Blender**  
  Modellashtirish, personaj dizayni va sahna (environment) konstruksiyasi

- **Substance Painter**  
  Teksturalash va materiallar bilan ishlash

---

## 👤 Personaj dizayni (Character)

- **Modellashtirish**  
  Personaj Blender’da **Low-poly** uslubida tayyorlangan. Bu yondashuv o‘yinning yuqori unumdorlikda (performance) ishlashini ta’minlaydi.

- **Teksturalash**  
  Personaj teksturalari **1024×1024 (1K)** o‘lchamda eksport qilingan. Ushbu yechim sifat va optimizatsiya o‘rtasida optimal muvozanatni saqlashga xizmat qiladi.

---

## 🌲 Atrof-muhit (Environment)

- **Modellashtirish**  
  Xarita elementlari — botqoqlik, ko‘l, toshlar va boshqa obyektlar **Blender** dasturida individual ravishda modellashtirilgan.

- **Materiallar**  
  Barcha environment assetlari **Substance Painter** orqali o‘ziga xos materiallar bilan boyitilgan.

- **Texnik ko‘rsatkichlar**  
  Atrof-muhit assetlari uchun ham **1K tekstura hajmi** tanlangan. Bu o‘yinning mobil qurilmalar va past quvvatli tizimlarda ham silliq ishlashini kafolatlaydi.

---

## ⚙️ Optimizatsiya (Graphics)

- Barcha modellar **Unity** o‘yin dvigateli uchun optimallashtirilgan  
- Mesh’lar keraksiz polygonlardan tozalangan  
- Assetlar **"Game-ready"** holatiga keltirilgan  

Bu yondashuv o‘yinning barqaror ishlashi va yuqori FPS ko‘rsatkichlarini ta’minlaydi.

---

## 🕹 Gameplay Mechanics

### Character Controller System

O‘yinchi harakati Unity’ning **CharacterController** komponenti asosida amalga oshirilgan.

**Qo‘llab-quvvatlanadigan harakatlar:**

- Yurish  
- Yugurish  
- Sakrash  
- Gravitatsiya bilan ishlash  
- Silliq burilish (rotation)

### Input System

O‘yin input’lari Unity’ning standart input mexanizmlari orqali boshqariladi va **mobil hamda PC** qurilmalarga moslashtirilgan.

---

## 🎭 Animation System

Personaj animatsiyalari **Unity Animator** orqali boshqariladi.

**Mavjud animatsiya holatlari:**

- Idle  
- Run  
- Jump  

Animatsiya transition’lari **Animator State Machine** orqali silliq va optimallashtirilgan holda ishlaydi.

---

## ⏱ Game Logic & Flow

### Timer System

O‘yinda **1 daqiqadan 0 gacha** sanovchi countdown timer mavjud.

### Lose Condition

Agar o‘yinchi belgilangan vaqt ichida finish nuqtasiga yetib bormasa:

- O‘yin **Game Over** holatiga o‘tadi

### Win Condition

O‘yinchi finish nuqtasiga yetib borganda:

- O‘yin **muvaffaqiyatli yakunlanadi**

---

## 🧩 Level & Scene Setup

Sahna (scene) **modular** uslubda tashkil qilingan.

**Barcha obstacle va environment elementlar:**

- Alohida **prefab** sifatida yaratilgan  
- Qayta foydalanish (**reusability**) uchun moslashtirilgan  

Bu yondashuv yangi level qo‘shishni va mavjud sahnani kengaytirishni osonlashtiradi.

---

## 🔊 Audio System

O‘yinga **background music** qo‘shilgan.

**Audio xususiyatlari:**

- MP3 format  
- Loop qilinadigan tarzda sozlangan  
- Ovoz darajasi gameplay jarayoniga mos ravishda balanslangan  

---

## ⚡ Optimization & Performance

- Barcha skriptlar **Unity** uchun optimallashtirilgan  
- Keraksiz `Update()` chaqiruvlari minimallashtirilgan  
- Physics hisob-kitoblari faqat zarur joylarda ishlatiladi  

**Natijada:**

- Barqaror FPS  
- Past resurs sarfi  
- Mobil qurilmalar uchun qulay ishlash  

---

## 📦 Build & Platform Support

O‘yin quyidagi platformalar uchun moslashtirilgan:

- PC  
- Mobile (**Android / iOS**)  

Resolution va aspect ratio **avtomatik tarzda moslashadi**.
