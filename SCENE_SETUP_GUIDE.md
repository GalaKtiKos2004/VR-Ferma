# Руководство по настройке сцены VR Ферма

Это подробное руководство поможет вам настроить сцену для игры VR Ферма.

## Быстрая настройка (Автоматическая)

1. Откройте Unity Editor
2. Перейдите в меню: **VR Ferma → Setup Scene**
3. Нажмите кнопки в следующем порядке:
   - **Create All Managers** - создаст все менеджеры
   - **Setup Tags and Layers** - настроит теги
   - **Create Basic UI** - создаст базовый UI
   - **Create Ground** - создаст землю
   - **Create Water Source** - создаст источник воды

## Ручная настройка (Пошаговая)

### Шаг 1: Настройка тегов и слоев

1. **Edit → Project Settings → Tags and Layers**
2. Добавьте следующие теги:
   - `Water` - для источников воды
   - `Ground` - для земли
   - `Plant` - для растений
   - `Animal` - для животных
   - `NPC` - для NPC персонажей

### Шаг 2: Создание менеджеров

Создайте пустые GameObject'ы и добавьте к ним компоненты:

#### GameManager
```
GameObject: "GameManager"
Компоненты:
  - GameManager (Script)
```

#### CropManager
```
GameObject: "CropManager"
Компоненты:
  - CropManager (Script)
  
Дочерний GameObject: "Crops" (для хранения растений)
```

В инспекторе CropManager:
- **Crop Parent**: перетащите GameObject "Crops"
- **Ground Layer**: выберите слой "Default" (или создайте слой "Ground")
- **Planting Distance**: 0.5

#### AnimalManager
```
GameObject: "AnimalManager"
Компоненты:
  - AnimalManager (Script)
  
Дочерний GameObject: "Animals" (для хранения животных)
```

В инспекторе AnimalManager:
- **Animal Parent**: перетащите GameObject "Animals"

#### AchievementManager
```
GameObject: "AchievementManager"
Компоненты:
  - AchievementManager (Script)
```

#### TutorialManager
```
GameObject: "TutorialManager"
Компоненты:
  - TutorialManager (Script)
```

#### TradingSystem
```
GameObject: "TradingSystem"
Компоненты:
  - TradingSystem (Script)
```

#### UIManager
```
GameObject: "UIManager"
Компоненты:
  - UIManager (Script)
```

### Шаг 3: Настройка GameManager

В инспекторе GameManager перетащите ссылки:
- **Crop Manager**: перетащите GameObject "CropManager"
- **Animal Manager**: перетащите GameObject "AnimalManager"
- **Achievement Manager**: перетащите GameObject "AchievementManager"
- **Tutorial Manager**: перетащите GameObject "TutorialManager"
- **UI Manager**: перетащите GameObject "UIManager"

### Шаг 4: Создание UI

#### Canvas для VR
1. Создайте **GameObject → UI → Canvas**
2. Настройте Canvas:
   - **Render Mode**: World Space
   - **Scale**: X=0.001, Y=0.001, Z=0.001
   - **Position**: X=0, Y=2, Z=3

#### UI Элементы

Создайте дочерние объекты Canvas:

**MoneyText** (TextMeshProUGUI):
- Anchor: Top-Left
- Position: X=20, Y=-20
- Size: Width=300, Height=50
- Font Size: 36
- Color: White
- Text: "Деньги: 0"

**InstructionText** (TextMeshProUGUI):
- Anchor: Center-Top
- Position: Y=0.8 (относительно Canvas)
- Size: Width=800, Height=60
- Font Size: 32
- Color: Yellow
- Alignment: Center
- Text: "Добро пожаловать на ферму!"

**HintText** (TextMeshProUGUI):
- Anchor: Center-Top
- Position: Y=0.7
- Size: Width=800, Height=40
- Font Size: 24
- Color: White
- Alignment: Center

**LevelText** (TextMeshProUGUI):
- Anchor: Top-Right
- Position: X=-20, Y=-20
- Size: Width=200, Height=40
- Font Size: 28
- Color: Cyan
- Text: "Уровень: 0"

**TimeText** (TextMeshProUGUI):
- Anchor: Top-Right
- Position: X=-20, Y=-70
- Size: Width=200, Height=40
- Font Size: 24
- Color: White
- Text: "Время: 00:00"

#### Связывание UI с UIManager

В инспекторе UIManager перетащите:
- **Money Text**: MoneyText
- **Instruction Text**: InstructionText
- **Hint Text**: HintText
- **Level Text**: LevelText
- **Time Text**: TimeText

### Шаг 5: Создание земли

1. **GameObject → 3D Object → Plane**
2. Назовите "Ground"
3. Настройки:
   - **Position**: X=0, Y=0, Z=0
   - **Scale**: X=10, Y=1, Z=10
   - **Tag**: Ground
   - **Layer**: Default

4. Создайте материал для земли:
   - **Create → Material** → назовите "GroundMaterial"
   - Цвет: зеленый (R=0.4, G=0.6, B=0.2)
   - Примените к Ground

### Шаг 6: Создание источника воды

1. **GameObject → 3D Object → Cube**
2. Назовите "WaterSource"
3. Настройки:
   - **Position**: X=5, Y=0.5, Z=5
   - **Scale**: X=2, Y=1, Z=2
   - **Tag**: Water
   - **Layer**: Default

4. Создайте материал для воды:
   - **Create → Material** → назовите "WaterMaterial"
   - **Rendering Mode**: Transparent
   - Цвет: синий с прозрачностью (R=0.2, G=0.4, B=0.8, A=0.7)
   - Примените к WaterSource

5. Настройте Collider:
   - **Is Trigger**: ✓ (включено)

### Шаг 7: Настройка CropManager

В инспекторе CropManager добавьте типы культур:

#### Морковь (Carrot)
- **Type**: Carrot
- **Name**: "Морковь"
- **Growth Time**: 30 (секунды)
- **Sell Price**: 10
- **Seed Prefab**: (создайте позже)
- **Plant Prefab**: (создайте позже)

#### Помидор (Tomato)
- **Type**: Tomato
- **Name**: "Помидор"
- **Growth Time**: 45
- **Sell Price**: 15
- **Seed Prefab**: (создайте позже)
- **Plant Prefab**: (создайте позже)

#### Тыква (Pumpkin)
- **Type**: Pumpkin
- **Name**: "Тыква"
- **Growth Time**: 60
- **Sell Price**: 25
- **Seed Prefab**: (создайте позже)
- **Plant Prefab**: (создайте позже)

#### Кукуруза (Corn)
- **Type**: Corn
- **Name**: "Кукуруза"
- **Growth Time**: 40
- **Sell Price**: 20
- **Seed Prefab**: (создайте позже)
- **Plant Prefab**: (создайте позже)

### Шаг 8: Настройка AnimalManager

В инспекторе AnimalManager добавьте типы животных:

#### Курица (Chicken)
- **Type**: Chicken
- **Name**: "Курица"
- **Feed Interval**: 30 (секунды)
- **Water Interval**: 20
- **Happiness Reward**: 5
- **Prefab**: (создайте позже)

### Шаг 9: Создание префабов

#### Префаб семени (Seed)

1. **GameObject → 3D Object → Sphere** (или используйте свою модель)
2. Назовите "Seed"
3. Добавьте компоненты:
   - **XR Grab Interactable**
   - **Seed** (Script)
   - **Rigidbody** (если нужна физика)
4. Настройте XR Grab Interactable:
   - **Interaction Layer Mask**: Default
5. Сохраните как префаб: **Assets/Prefabs/Seed.prefab**

#### Префаб мешка с семенами (SeedBag)

1. **GameObject → 3D Object → Cube** (или используйте свою модель)
2. Назовите "SeedBag_Carrot"
3. Добавьте компоненты:
   - **XR Grab Interactable**
   - **SeedBag** (Script)
   - **Rigidbody**
4. Настройте SeedBag:
   - **Seed Type**: Carrot
   - **Seed Count**: 10
   - **Seed Prefab**: перетащите префаб Seed
5. Сохраните как префаб: **Assets/Prefabs/SeedBag_Carrot.prefab**
6. Повторите для других типов семян

#### Префаб растения (Plant)

1. Создайте иерархию:
```
Plant
├── SeedStage (GameObject - визуальная стадия семени)
├── GrowingStage (GameObject - стадия роста)
└── ReadyStage (GameObject - стадия готовности)
```

2. Добавьте к Plant компоненты:
   - **PlantedCrop** (Script)
   - **XR Simple Interactable**
   - **CropInteractable** (Script)
   - **Collider** (Sphere или Box)

3. Настройте PlantedCrop:
   - **Seed Stage**: перетащите SeedStage
   - **Growing Stage**: перетащите GrowingStage
   - **Ready Stage**: перетащите ReadyStage

4. Настройте XR Simple Interactable:
   - **Interaction Layer Mask**: Default

5. Сохраните как префаб: **Assets/Prefabs/Plant_Carrot.prefab**

6. В CropManager назначьте префаб в соответствующую запись

#### Префаб лейки (WateringCan)

1. **GameObject → 3D Object → Cube** (или используйте свою модель)
2. Назовите "WateringCan"
3. Добавьте компоненты:
   - **XR Grab Interactable**
   - **WateringCan** (Script)
   - **Rigidbody**
   - **Particle System** (для эффекта воды)
   - **Audio Source** (для звука полива)

4. Настройте WateringCan:
   - **Water Capacity**: 100
   - **Water Per Second**: 10
   - **Watering Range**: 2
   - **Plant Layer**: создайте слой "Plant" или используйте "Default"
   - **Water Particles**: перетащите Particle System
   - **Water Sound**: перетащите Audio Source

5. Настройте Particle System:
   - **Start Lifetime**: 0.5
   - **Start Speed**: 5
   - **Start Size**: 0.1
   - **Simulation Space**: World
   - **Play On Awake**: false

6. Сохраните как префаб: **Assets/Prefabs/WateringCan.prefab**

#### Префаб животного (Animal)

1. Создайте иерархию:
```
Chicken
├── Model (ваша модель курицы)
├── HappyIndicator (GameObject - индикатор счастья)
├── HungryIndicator (GameObject - индикатор голода)
└── ThirstyIndicator (GameObject - индикатор жажды)
```

2. Добавьте к Chicken компоненты:
   - **FarmAnimal** (Script)
   - **XR Simple Interactable**
   - **AnimalInteractable** (Script)
   - **Collider** (Capsule или Box)

3. Настройте FarmAnimal:
   - **Happy Indicator**: перетащите HappyIndicator
   - **Hungry Indicator**: перетащите HungryIndicator
   - **Thirsty Indicator**: перетащите ThirstyIndicator

4. Настройте AnimalInteractable:
   - **Is Feed Action**: true (для кормления) или false (для поения)

5. Сохраните как префаб: **Assets/Prefabs/Chicken.prefab**

6. В AnimalManager назначьте префаб в соответствующую запись

#### Префаб NPC/Торговца

1. **GameObject → 3D Object → Capsule** (или используйте свою модель)
2. Назовите "Trader"
3. Добавьте компоненты:
   - **XR Simple Interactable**
   - **NPCInteractable** (Script)

4. Настройте NPCInteractable:
   - **NPC Name**: "Торговец"
   - **Is Trader**: ✓

5. Сохраните как префаб: **Assets/Prefabs/Trader.prefab**

### Шаг 10: Размещение объектов в сцене

1. **XR Origin (XR Rig)** - должен быть уже в сцене
   - Если нет, добавьте префаб из: `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/Prefabs/XR Origin (XR Rig).prefab`

2. Разместите мешки с семенами:
   - Создайте несколько экземпляров SeedBag префабов
   - Разместите их на столе или полке
   - Позиция: например, X=-3, Y=1, Z=2

3. Разместите лейку:
   - Создайте экземпляр WateringCan префаба
   - Позиция: рядом с источником воды

4. Разместите животных:
   - Создайте экземпляры Animal префабов
   - Позиция: например, X=-5, Y=0, Z=-3

5. Разместите торговца:
   - Создайте экземпляр Trader префаба
   - Позиция: например, X=0, Y=0, Z=-5

### Шаг 11: Настройка слоев для взаимодействий

1. **Edit → Project Settings → Physics**
2. Убедитесь, что слои настроены правильно:
   - **Ground Layer**: для земли (используется в Seed.cs)
   - **Plant Layer**: для растений (используется в WateringCan.cs)

### Шаг 12: Финальная проверка

Проверьте следующие пункты:

- [ ] Все менеджеры созданы и связаны
- [ ] UI настроен и связан с UIManager
- [ ] Земля создана с тегом "Ground"
- [ ] Источник воды создан с тегом "Water"
- [ ] Префабы созданы и назначены в менеджерах
- [ ] XR Origin присутствует в сцене
- [ ] Все объекты размещены в сцене
- [ ] Теги и слои настроены

## Тестирование

1. Запустите игру в Play Mode
2. Проверьте:
   - Появляется ли UI с деньгами и инструкциями
   - Можно ли взять мешок с семенами
   - Можно ли посадить семена
   - Можно ли взять лейку и наполнить её водой
   - Можно ли поливать растения
   - Можно ли собирать урожай
   - Можно ли кормить животных
   - Можно ли взаимодействовать с торговцем

## Решение проблем

### UI не отображается
- Проверьте, что Canvas настроен как World Space
- Проверьте масштаб Canvas (должен быть 0.001)
- Проверьте, что камера назначена в Canvas

### Нельзя взять объекты
- Проверьте, что XR Origin присутствует в сцене
- Проверьте, что XR Grab Interactable добавлен к объектам
- Проверьте настройки XR Interaction Manager

### Растения не растут
- Проверьте, что PlantedCrop компонент добавлен
- Проверьте, что CropManager правильно настроен
- Проверьте, что префабы назначены в CropManager

### Лейка не наполняется
- Проверьте, что источник воды имеет тег "Water"
- Проверьте, что Collider на воде настроен как Trigger

## Дополнительные ресурсы

- [XR Interaction Toolkit Documentation](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest)
- [Unity VR Best Practices](https://docs.unity3d.com/Manual/VROverview.html)
