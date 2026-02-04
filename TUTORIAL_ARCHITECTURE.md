# Архитектура системы обучения

## Схема взаимодействия компонентов

```
┌─────────────────────────────────────────────────────────────────────┐
│                         TUTORIAL MANAGER                             │
│                    (Главный контроллер)                              │
│                                                                      │
│  - Управляет этапами (1-8)                                           │
│  - Отслеживает прогресс                                              │
│  - Включает/выключает подсветку                                      │
│  - Получает уведомления от объектов                                  │
└─────────────────────────────────────────────────────────────────────┘
         │                    │                    │
         │ управляет          │ уведомления        │ показывает
         ▼                    ▲                    ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ TutorialHighlight│  │  Game Objects    │  │ SimpleGameManager│
│   (Подсветка)    │  │                  │  │    (UI)          │
│                  │  │  - Rake          │  │                  │
│ - Пульсация      │  │  - SeedBag       │  │ - ShowHint()     │
│ - Эмиссия        │  │  - WateringCan   │  │ - UpdateUI()     │
│ - Цвет           │  │  - PlantBed      │  │                  │
└──────────────────┘  └──────────────────┘  └──────────────────┘
         │                    │                    │
         │ прикреплён к       │ взаимодействуют    │
         ▼                    ▼                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                        ИГРОВЫЕ ОБЪЕКТЫ                               │
│                                                                      │
│  Barn          Rake          SeedBag       PlantBed     WateringCan │
│  (Сарай)       (Грабли)      (Семена)      (Грядка)     (Лейка)     │
│                                                                      │
│  [визуал]      Till()        PlantSeed()   Till()       Water()     │
│                OnGrabbed()   OnGrabbed()   PlantSeed()  OnGrabbed() │
│                                            Water()       Refill()    │
│                                            Harvest()                 │
└─────────────────────────────────────────────────────────────────────┘
```

## Поток обучения (Flow)

```
START
  │
  ▼
┌─────────────────────────┐
│ TutorialManager.Start() │
│ - InitializeHighlights()│
│ - StartTutorial()       │
└─────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                    ЭТАП 1: Взять грабли                          │
│                                                                  │
│  Подсветка: Barn + Rake                                          │
│  Подсказка: "Возьмите граблю из сарая"                           │
│  Действие: Игрок берёт грабли                                    │
│  Событие:  Rake.OnGrabbed() → TutorialManager.OnRakeTaken()      │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                  ЭТАП 2: Взрыхлить грядку                        │
│                                                                  │
│  Подсветка: PlantBed                                             │
│  Подсказка: "Нажмите и вскопайте грядку"                         │
│  Действие: Игрок использует грабли на грядке                     │
│  Событие:  Rake.TryTillBed() → PlantBed.Till()                   │
│            → TutorialManager.OnBedTilled()                       │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                   ЭТАП 3: Взять семена                           │
│                                                                  │
│  Подсветка: Barn + SeedBag                                       │
│  Подсказка: "Возьмите пакет с семенами"                          │
│  Действие: Игрок берёт семена                                    │
│  Событие:  SeedBag.OnGrabbed() → TutorialManager.OnSeedsTaken()  │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                   ЭТАП 4: Посадить семя                          │
│                                                                  │
│  Подсветка: PlantBed                                             │
│  Подсказка: "Подойдите к грядке и посадите семя"                 │
│  Действие: Игрок использует семена на грядке                     │
│  Событие:  SeedBag.TryPlantSeed() → PlantBed.PlantSeed()         │
│            → TutorialManager.OnSeedPlanted()                     │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                    ЭТАП 5: Взять лейку                           │
│                                                                  │
│  Подсветка: WateringCan                                          │
│  Подсказка: "Нажмите, чтобы взять лейку"                         │
│  Действие: Игрок берёт лейку                                     │
│  Событие:  WateringCan.OnGrabbed() → TutorialManager.OnCanTaken()│
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                  ЭТАП 6: Наполнить лейку                         │
│                                                                  │
│  Подсветка: Well                                                 │
│  Подсказка: "Подойдите к колодцу и наполните лейку"              │
│  Действие: Игрок подносит лейку к воде                           │
│  Событие:  OnTriggerEnter("Water") → WateringCan.Refill()        │
│            → TutorialManager.OnCanFilled()                       │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                   ЭТАП 7: Полить росток                          │
│                                                                  │
│  Подсветка: PlantBed                                             │
│  Подсказка: "Подойдите к ростку и полейте его"                   │
│  Действие: Игрок поливает росток                                 │
│  Событие:  WateringCan.TryWaterPlantBed() → PlantBed.Water()     │
│            → TutorialManager.OnPlantWatered()                    │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                ОЖИДАНИЕ: Рост растения (30 сек)                  │
│                                                                  │
│  PlantBed.Update():                                              │
│    - growthTimer += Time.deltaTime                               │
│    - Увеличение scale растения                                   │
│    - if (growthTimer >= growthTime) → isFullyGrown = true        │
│                                                                  │
│  TutorialManager корутина ждёт PlantBed.IsFullyGrown()           │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌──────────────────────────────────────────────────────────────────┐
│                  ЭТАП 8: Собрать урожай                          │
│                                                                  │
│  Подсветка: PlantBed                                             │
│  Подсказка: "Урожай созрел! Соберите морковку!"                  │
│  Действие: Игрок собирает урожай                                 │
│  Событие:  PlantBed.Harvest()                                    │
│            → SimpleGameManager.AddCarrot(1)                      │
│            → TutorialManager.OnHarvestCollected()                │
└──────────────────────────────────────────────────────────────────┘
  │
  ▼
┌─────────────────────────┐
│ CompleteTutorial()      │
│ - Показать достижение   │
│ - Сохранить в PlayerPrefs│
│ - tutorialCompleted=true│
└─────────────────────────┘
  │
  ▼
END
```

## Система событий (Events)

```
User Action           →  Component Method      →  Tutorial Notification
─────────────────────────────────────────────────────────────────────────
Взять грабли          →  Rake.OnGrabbed()      →  OnRakeTaken()
Использовать грабли   →  PlantBed.Till()       →  OnBedTilled()
Взять семена          →  SeedBag.OnGrabbed()   →  OnSeedsTaken()
Использовать семена   →  PlantBed.PlantSeed()  →  OnSeedPlanted()
Взять лейку           →  WateringCan.OnGrabbed()→  OnCanTaken()
Наполнить лейку       →  WateringCan.Refill()  →  OnCanFilled()
Полить растение       →  PlantBed.Water()      →  OnPlantWatered()
Собрать урожай        →  PlantBed.Harvest()    →  OnHarvestCollected()
```

## Иерархия состояний PlantBed

```
┌──────────────────┐
│   Empty          │  isTilled=false, hasPlant=false
│   (Пустая)       │
└──────────────────┘
        │
        │ Till()
        ▼
┌──────────────────┐
│   Tilled         │  isTilled=true, hasPlant=false
│   (Взрыхлена)    │  Цвет: коричневый
└──────────────────┘
        │
        │ PlantSeed()
        ▼
┌──────────────────┐
│   Planted        │  isTilled=true, hasPlant=true
│   (Посажено)     │  Визуал: маленький росток
└──────────────────┘
        │
        │ Water()
        ▼
┌──────────────────┐
│   Growing        │  isGrowing=true
│   (Растёт)       │  Scale увеличивается
│                  │  30 секунд таймер
└──────────────────┘
        │
        │ timer >= 30s
        ▼
┌──────────────────┐
│   FullyGrown     │  isFullyGrown=true
│   (Созрело)      │  Готово к сбору
└──────────────────┘
        │
        │ Harvest()
        ▼
┌──────────────────┐
│   Empty          │  Цикл начинается заново
│   (Пустая)       │
└──────────────────┘
```

## Взаимодействие с UI

```
TutorialManager                    SimpleGameManager
       │                                  │
       │  ShowHint(message, duration)     │
       ├─────────────────────────────────>│
       │                                  │
       │                         ┌────────┴────────┐
       │                         │  hintText.text  │
       │                         │  Invoke(Clear)  │
       │                         └────────┬────────┘
       │                                  │
       │                         Canvas → TextMeshPro
       │                                  │
       │                         Игрок видит подсказку
```

## Система подсветки

```
TutorialHighlight Component
       │
       ├─ CreateHighlightObject()
       │  │
       │  ├─ if (Renderer exists)
       │  │  └─ Создать копию меша
       │  │     - localScale *= 1.1
       │  │     - Material: прозрачный + эмиссия
       │  │
       │  └─ else
       │     └─ Создать простую сферу
       │
       ├─ Update() (если enabled)
       │  └─ Пульсация через Mathf.Sin()
       │     - Alpha от minIntensity до maxIntensity
       │
       └─ EnableHighlight(bool)
          - Включить/выключить визуал
```

## Отладка

```
TutorialDebug
       │
       ├─ Input.GetKeyDown(R)
       │  └─ ResetTutorial()
       │     - PlayerPrefs.DeleteKey()
       │     - Перезапуск сцены
       │
       ├─ Input.GetKeyDown(N)
       │  └─ SkipCurrentStep()
       │     - switch(currentStep)
       │     - Вызов соответствующего On...()
       │
       ├─ Input.GetKeyDown(I)
       │  └─ ShowTutorialInfo()
       │     - Debug.Log информации
       │
       └─ OnGUI()
          └─ Визуальный UI в углу экрана
```

## Зависимости компонентов

```
TutorialManager
├── depends on: SimpleGameManager (для ShowHint)
├── depends on: TutorialHighlight (автоматически добавляет)
└── references:
    ├── Barn (GameObject)
    ├── Rake (GameObject с компонентом Rake)
    ├── SeedBag (GameObject с компонентом SeedBag)
    ├── PlantBed (компонент PlantBed)
    ├── WateringCan (GameObject с компонентом WateringCan)
    └── Well (GameObject с тегом "Water")

Rake
├── depends on: PlantBed (для Till())
└── depends on: TutorialManager (для OnRakeTaken())

SeedBag
├── depends on: PlantBed (для PlantSeed())
└── depends on: TutorialManager (для OnSeedsTaken())

PlantBed
├── depends on: SimpleGameManager (для ShowHint)
└── depends on: TutorialManager (для уведомлений)

WateringCan
├── depends on: PlantBed (для Water())
├── depends on: WaterSource (тег "Water")
└── depends on: TutorialManager (для уведомлений)

TutorialHighlight
└── depends on: Renderer (опционально)
```

## Паттерны проектирования

### Singleton
```
TutorialManager.Instance
SimpleGameManager.Instance
```

### Observer (Event System)
```
Компоненты → notify → TutorialManager
PlantBed.Water() → OnPlantWatered()
```

### State Machine
```
TutorialManager:
  currentStep (0-8)
  StartStepX() → OnStepCompleted() → StartStepX+1()
```

### Component Pattern
```
TutorialHighlight - независимый компонент
Добавляется динамически к любому GameObject
```

---

**Эта архитектура обеспечивает:**
- ✅ Модульность (легко добавлять новые этапы)
- ✅ Расширяемость (новые объекты легко интегрируются)
- ✅ Отладочность (TutorialDebug для тестирования)
- ✅ Независимость (компоненты слабо связаны)
