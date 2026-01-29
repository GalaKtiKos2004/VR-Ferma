# Реализация механик VR-Ferma

## 🌱 Механика растений

### Компоненты
- **PlantBed.cs** — грядка с состояниями (взрыхлена, полита, посажена, растёт)
- **WateringCan.cs** — лейка для полива
- **WaterSource.cs** — источник воды (колодец, бочка)
- **Hoe.cs** — тяпка для взрыхления
- **Rake.cs** — грабли для разрыхления
- **SeedBag.cs** — пакет с семенами (морковь, помидоры, тыква, лук)

### Процесс выращивания
1. **Взрыхлить** грядку тяпкой (`PlantBed.Till()`)
2. **Разрыхлить** грядку граблями (`PlantBed.Rake()`)
3. **Посадить** семя (`PlantBed.PlantSeed(seedType)`)
4. **Полить** водой (`PlantBed.Water()`)
5. **Ждать** роста (30 сек, `growthTime`)
6. **Собрать** урожай (`PlantBed.Harvest()`)

### Визуализация
- Цвет грядки меняется (коричневый → зелёный → жёлтый)
- Модель растения масштабируется при росте
- Визуальные ростки появляются после посадки

---

## 🐔 Механика животных

### Компоненты
- **Animal.cs** — базовый класс животного (счастье, голод, звуки)
- **AnimalMovement.cs** — случайное перемещение (wandering) с Rigidbody
- **AnimalVRInteraction.cs** — VR-взаимодействие (поглаживание, кормление)
- **FoodBucket.cs** — ведро с зерном
- **FeedingTrough.cs** — кормушка (автоматическое кормление рядом)

### Животные
- **Курицы** (3 шт.): кура.fbx, курочка2.fbx — CapsuleCollider
- **Корова** (1 шт.): COWWW2.fbx — MeshCollider
- **Козы** (2 шт.): goat1.fbx, goat2.fbx, goat3.fbx — MeshCollider
- **Свиньи** (2 шт.): john pork.fbx

### Состояния животного
- **Счастье** (`currentHappiness` 0-100):
  - > 70: довольное 😊
  - 30-70: нормальное 😐
  - < 30: голодное 😢
- Счастье уменьшается со временем (`happinessDecayRate = 5/сек`)

### Взаимодействие
- **Покормить**: +30 счастья, анимация Eat
- **Погладить**: +20 счастья, анимация Happy
- **Автокормление**: FeedingTrough кормит голодных животных в радиусе 2-4м

### Загоны
- **ChickenPen**: (5, 0, 0), размер 8×6м, из моделей забора
- **GoatPen**: (-7, 0, -5), размер 10×8м
- Заборы: scale=1, rotation +90° по Y, коллайдеры axis-aligned

---

## 🎨 Анимации животных

### Система
- **AnimalController.controller** — общий контроллер с параметрами и состояниями
- **AnimatorOverrideController** — для каждого животного свои клипы
- Сохраняется как asset: `Assets/Animations/Overrides/{ИмяЖивотного}_AnimController.overrideController`

### Параметры
- **IsWalking** (bool) — животное идёт
- **IsHungry** (bool) — животное голодное
- **Eat** (trigger) — анимация еды
- **Happy** (trigger) — анимация поглаживания

### Состояния
- **Idle** — стоит (по умолчанию)
- **Walk** — идёт
- **HungryIdle** — стоит голодное
- **Eat** — ест (триггер → играет → возврат в Idle)
- **Happy** — радуется (триггер → играет → возврат в Idle)

### Клипы (из FBX моделей)
- **Курицы**: rig|Idle, rig|Walk, rig|EATING, rig|PETTING
- **Коровы**: rig|idle, rig|walking, rig|eating, rig|petting
- **Козы**: metarig|IDLE, metarig|WALKING, metarig|EATING, metarig|PETTING

### Настройка
- **Avatar**: Generic, создаётся из FBX (ModelImporter.avatarSetup = CreateFromThisModel)
- **Animator**: находится на GameObject с костями (rig root), не на parent
- **Rig Type**: Generic (для всех животных)

### Меню для анимаций
- **Включить Avatar у моделей Norm** — включает Avatar + Import Animation + Rig=Generic
- **Создать контроллер анимаций** — создаёт AnimalController.controller
- **Обновить Animator у животных в сцене** — назначает контроллер и Avatar существующим животным
- **Отладка анимаций** — показывает FBX клипы, Avatar, состояние животных
- **Тест анимации (выбранное животное)** — runtime диагностика в Inspector

---

## 🎮 Управление

### Клавиатура (NonVRPlayerController)
- **WASD** — движение
- **Мышь** — взгляд
- **Shift** — бег (×2 скорость)
- **E / ЛКМ** — взаимодействие (raycast от камеры, дистанция 5м)
- **Q** — выбросить инструмент/зерно
- **ESC** — показать/скрыть курсор

### VR (XR Origin)
- **Левый джойстик** — Continuous Movement (движение)
- **Правый джойстик** — Snap Turn (поворот)
- **Триггеры** — Grab / Select (захват, взаимодействие)
- **XRSimpleInteractable** — взаимодействие с объектами
- **XRGrabInteractable** — захват инструментов

### Переключение режимов
- **VR-Ferma → Переключить на VR (шлем)** — добавляет XR Origin
- **VR-Ferma → Переключить на клавиатуру (ПК)** — добавляет NonVR Player

---

## 🎓 Обучение (TutorialManager)

### Шаги 1-10: Растения
1. Взять тяпку
2. Взрыхлить грядку тяпкой
3. Взять грабли
4. Разрыхлить грядку граблями
5. Взять семена
6. Посадить семя
7. Взять лейку
8. Наполнить лейку у колодца
9. Полить росток
10. Собрать урожай

### Шаги 11-13: Животные (новое!)
11. Взять зерно из ведра
12. Покормить животное
13. Погладить животное

### Подсветка
- **TutorialHighlight.cs** — жёлтое пульсирующее свечение
- Автоматическое назначение объектов через `GameObject.Find()`

---

## 🛠️ Инструменты редактора

### SimpleFarmSetup.cs
- **Создать простую ферму** — полная сцена (земля, игрок, грядки, животные, UI)
- **Добавить животных** — только животные + кормушки + ведро
- **Создать загон для куриц/коз** — заборы из моделей Norm
- **Очистить сцену** — удаляет все объекты фермы

### AnimalAnimatorSetup.cs
- **Включить Avatar у моделей Norm** — настройка FBX импорта
- **Создать контроллер анимаций** — AnimalController.controller
- **Обновить Animator у животных** — применяет контроллер к существующим
- **Отладка анимаций** — диагностика FBX, клипов, Avatar
- **Тест анимации** — runtime проверка параметров

### Другие утилиты
- **Добавить физику животным** — Rigidbody + коллайдеры
- **Удалить индикаторы голода** — убирает AnimalHungerUI
- **Заменить коллайдеры коз/коров** — CapsuleCollider → MeshCollider
- **Заменить шрифты на Christmas** — применяет TMP_FontAsset ко всем текстам
- **Включить отладку кормушек** — логи автокормления
- **Проверить корову** — диагностика Animator

---

## 🎨 UI и визуализация

### Подсказки (SimpleGameManager)
- **HintCanvas** — Canvas внизу экрана
- **HintText** (TextMeshProUGUI) — текст подсказок
- Автоматическое скрытие через 3 сек
- Блокировка при туториале (5 сек после "Шаг пройден")

### Шрифты
- **Christmas On Crack.otf** → TMP_FontAsset
- Загрузка через `LoadAndSetFont()` в SimpleFarmSetup
- Замена всех шрифтов через меню

---

## 🔧 Технические детали

### Физика
- **CharacterController** — для NonVR Player (высота 2м, радиус 0.5м)
- **Rigidbody** — для животных (масса 10кг, drag 5, freeze rotation X/Z)
- **Continuous Collision Detection** — для точных столкновений
- **LayerMask** — Default для всех объектов

### Reflection
Используется для установки private полей через Editor:
- `animalName`, `centerPoint`, `wanderRadius`, `moveSpeed` и т.п.
- Позволяет настраивать компоненты без публичных полей

### Сохранение
- **AnimatorOverrideController** — сохраняется как asset в `Assets/Animations/Overrides/`
- **SerializedObject** — для принудительного сохранения в Edit Mode
- **EditorUtility.SetDirty** + **MarkSceneDirty** — сохранение изменений сцены

---

## 📁 Структура файлов

```
Assets/
├── Scripts/
│   ├── Animal.cs — базовый класс животного
│   ├── AnimalMovement.cs — движение через Rigidbody
│   ├── AnimalVRInteraction.cs — VR взаимодействие
│   ├── FoodBucket.cs — ведро с зерном
│   ├── FeedingTrough.cs — автокормушка
│   ├── PlantBed.cs — грядка
│   ├── NonVRPlayerController.cs — клавиатура + мышь
│   ├── SimpleGameManager.cs — UI подсказок, счётчики
│   ├── TutorialManager.cs — 13 шагов обучения
│   └── Editor/
│       ├── SimpleFarmSetup.cs — главные инструменты
│       ├── AnimalAnimatorSetup.cs — анимации животных
│       └── AnimalDebugEditor.cs — Inspector для отладки
├── Animations/
│   ├── AnimalController.controller — общий контроллер
│   └── Overrides/ — индивидуальные контроллеры
├── Models/Norm/ — 3D модели (FBX/Blend)
│   ├── кура.fbx, курочка2.fbx
│   ├── COWWW2.fbx
│   ├── goat1/2/3.fbx
│   ├── john pork.fbx
│   ├── целый.fbx, обе.fbx (заборы)
│   └── ведро.blend, hay1.fbx
└── Fonts/
    └── Christmas On Crack.otf (+ TMP_FontAsset)
```

---

## 🎯 Ключевые особенности

### Животные
- ✅ Индивидуальные анимации для каждого вида (AnimatorOverrideController)
- ✅ Generic rig с Avatar
- ✅ Физические столкновения (не проходят друг через друга)
- ✅ Загоны с заборами (курицы, козы)
- ✅ Автокормление через кормушки
- ✅ VR и клавиатурное взаимодействие

### Растения
- ✅ Полный цикл (взрыхление → посадка → полив → рост → сбор)
- ✅ Визуальные изменения на каждом этапе
- ✅ Несколько типов растений

### VR
- ✅ XR Interaction Toolkit 3.1.2
- ✅ Continuous Movement + Snap Turn
- ✅ Grab Interaction для всех инструментов
- ✅ Простое взаимодействие с животными (XRSimpleInteractable)

### Обучение
- ✅ 13 шагов (растения + животные)
- ✅ Подсветка объектов (TutorialHighlight)
- ✅ Пошаговые подсказки на экране
- ✅ Автосохранение прогресса (PlayerPrefs)

---

## 🐛 Отладка

### Консольные логи
- `[Animal.Start]` — состояние Animator при запуске
- `[Feed]` / `[Pet]` — триггеры анимаций
- `[Anim]` — состояние анимации каждые 3 сек
- `[FeedingTrough]` — автокормление (если showDebug=true)
- `[SetupAnimator]` — настройка контроллера в Editor

### Меню диагностики
- **Отладка анимаций** — FBX клипы, Avatar, животные в сцене
- **Тест анимации (выбранное животное)** — runtime параметры
- **Проверить корову** — специальная диагностика
- **Включить отладку кормушек** — логи каждые 5 сек

### Inspector расширения
- **AnimalDebugEditor.cs** — кнопки для ручного теста анимаций в Play Mode
- Показывает текущее состояние, параметры, NormalizedTime

---

## ⚙️ Настройка проекта

### Требования
- Unity 2022.3+
- Universal Render Pipeline
- TextMeshPro
- XR Interaction Toolkit 3.1.2

### Импорт моделей
- **Rig Type**: Generic (для животных)
- **Import Animation**: ✓
- **Avatar Setup**: CreateFromThisModel
- Применяется через меню: **VR-Ferma → Включить Avatar у моделей Norm**

### Шрифты
- Создать TMP_FontAsset: Window → TextMeshPro → Font Asset Creator
- Source: Christmas On Crack.otf
- Применить: **VR-Ferma → Заменить шрифты на Christmas**

---

## 📝 Быстрый старт

1. **VR-Ferma → Создать простую ферму** — создаёт полную сцену
2. **VR-Ferma → Переключить на VR** (или на клавиатуру)
3. **Play Mode (▶)** — тестируйте!

Или по отдельности:
- **Добавить животных** — только животные
- **Создать загон для куриц/коз** — заборы
- **Создать UI подсказок** — интерфейс

---

## 🔍 Решение проблем

### Анимации не работают
1. **VR-Ferma → Включить Avatar у моделей Norm**
2. **VR-Ferma → Создать контроллер анимаций**
3. **VR-Ferma → Обновить Animator у животных**
4. **VR-Ferma → Отладка анимаций** — проверить Avatar/Controller

### Животные проходят сквозь друг друга
1. **VR-Ferma → Добавить физику животным** — добавляет Rigidbody

### Подсказки не отображаются
1. **VR-Ferma → Создать UI подсказок** — создаёт Canvas

### Корова не ест сено
1. **VR-Ferma → Включить отладку кормушек**
2. Смотрите консоль: расстояние, голод, слой

---

**Автор**: VR-Ferma Development Team  
**Версия**: 1.0  
**Дата**: 2026
