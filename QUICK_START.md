# Быстрый старт - Настройка сцены VR Ферма

## Автоматическая настройка (Рекомендуется)

1. Откройте Unity Editor
2. Убедитесь, что у вас открыта сцена (например, BasicScene.unity)
3. В меню Unity выберите: **VR Ferma → Setup Scene**
4. В открывшемся окне нажмите кнопки по порядку:
   - ✅ **Create All Managers** - создаст все необходимые менеджеры
   - ✅ **Setup Tags and Layers** - настроит теги (Water, Ground, Plant, Animal, NPC)
   - ✅ **Create Basic UI** - создаст Canvas и UI элементы
   - ✅ **Create Ground** - создаст землю для посадки
   - ✅ **Create Water Source** - создаст источник воды

5. После этого нужно вручную:
   - Настроить CropManager - добавить типы культур в список
   - Настроить AnimalManager - добавить типы животных
   - Создать префабы для семян, растений, животных, лейки
   - Разместить объекты в сцене

## Что было создано автоматически

### Менеджеры:
- ✅ GameManager
- ✅ CropManager (с дочерним объектом Crops)
- ✅ AnimalManager (с дочерним объектом Animals)
- ✅ AchievementManager
- ✅ TutorialManager
- ✅ TradingSystem
- ✅ UIManager

### UI:
- ✅ Canvas (World Space, масштаб 0.001)
- ✅ MoneyText (отображение денег)
- ✅ InstructionText (инструкции туториала)
- ✅ HintText (подсказки)
- ✅ LevelText (текущий уровень)
- ✅ TimeText (время игры)

### Окружение:
- ✅ Ground (земля с тегом "Ground")
- ✅ WaterSource (источник воды с тегом "Water")

## Следующие шаги

### 1. Настройка CropManager

1. Выберите GameObject "CropManager" в иерархии
2. В инспекторе найдите компонент CropManager
3. В поле "Crop Types" нажмите "+" чтобы добавить новую культуру
4. Для каждой культуры укажите:
   - **Type**: Carrot, Tomato, Pumpkin или Corn
   - **Name**: название (например, "Морковь")
   - **Growth Time**: время роста в секундах (например, 30)
   - **Sell Price**: цена продажи (например, 10)
   - **Seed Prefab**: (создайте позже)
   - **Plant Prefab**: (создайте позже)

### 2. Настройка AnimalManager

1. Выберите GameObject "AnimalManager"
2. В инспекторе найдите компонент AnimalManager
3. В поле "Animal Types" нажмите "+"
4. Для каждого животного укажите:
   - **Type**: Chicken, Cow, Pig или Sheep
   - **Name**: название (например, "Курица")
   - **Feed Interval**: интервал кормления в секундах (например, 30)
   - **Water Interval**: интервал поения в секундах (например, 20)
   - **Happiness Reward**: награда за уход (например, 5)
   - **Prefab**: (создайте позже)

### 3. Создание префабов

#### Простой префаб семени:

1. **GameObject → 3D Object → Sphere**
2. Назовите "Seed"
3. Добавьте компоненты:
   - **XR Grab Interactable** (из XR Interaction Toolkit)
   - **Seed** (Script) - из VRFerma.VR
4. Настройте Seed:
   - **Crop Type**: выберите тип (будет установлен автоматически при создании из мешка)
5. Сохраните как префаб: **Assets/Prefabs/Seed.prefab**

#### Префаб мешка с семенами:

1. **GameObject → 3D Object → Cube**
2. Назовите "SeedBag_Carrot"
3. Добавьте компоненты:
   - **XR Grab Interactable**
   - **SeedBag** (Script)
   - **Rigidbody**
4. Настройте SeedBag:
   - **Seed Type**: Carrot
   - **Seed Count**: 10
   - **Seed Prefab**: перетащите префаб Seed
5. Сохраните как префаб

#### Префаб растения:

1. Создайте пустой GameObject "Plant_Carrot"
2. Добавьте дочерние объекты:
   - "SeedStage" (может быть пустым или с простой моделью)
   - "GrowingStage" (может быть пустым или с моделью)
   - "ReadyStage" (может быть пустым или с моделью)
3. Добавьте к Plant_Carrot компоненты:
   - **PlantedCrop** (Script)
   - **XR Simple Interactable**
   - **CropInteractable** (Script)
   - **Sphere Collider**
4. Настройте PlantedCrop:
   - **Seed Stage**: перетащите SeedStage
   - **Growing Stage**: перетащите GrowingStage
   - **Ready Stage**: перетащите ReadyStage
5. Сохраните как префаб
6. Назначьте префаб в CropManager в соответствующей записи культуры

#### Префаб лейки:

1. **GameObject → 3D Object → Cube** (или используйте свою модель)
2. Назовите "WateringCan"
3. Добавьте компоненты:
   - **XR Grab Interactable**
   - **WateringCan** (Script)
   - **Rigidbody**
   - **Particle System** (для эффекта воды)
4. Настройте WateringCan:
   - **Water Capacity**: 100
   - **Water Per Second**: 10
   - **Watering Range**: 2
   - **Plant Layer**: Default (или создайте слой "Plant")
5. Сохраните как префаб

#### Префаб животного:

1. Создайте пустой GameObject "Chicken"
2. Добавьте дочерние объекты для индикаторов (опционально):
   - "HappyIndicator"
   - "HungryIndicator"
   - "ThirstyIndicator"
3. Добавьте к Chicken компоненты:
   - **FarmAnimal** (Script)
   - **XR Simple Interactable**
   - **AnimalInteractable** (Script)
   - **Capsule Collider**
4. Настройте AnimalInteractable:
   - **Is Feed Action**: true (для кормления)
5. Сохраните как префаб
6. Назначьте префаб в AnimalManager

#### Префаб торговца:

1. **GameObject → 3D Object → Capsule**
2. Назовите "Trader"
3. Добавьте компоненты:
   - **XR Simple Interactable**
   - **NPCInteractable** (Script)
4. Настройте NPCInteractable:
   - **NPC Name**: "Торговец"
   - **Is Trader**: ✓
5. Сохраните как префаб

### 4. Размещение в сцене

1. **XR Origin** - должен быть уже в сцене
   - Если нет: добавьте префаб из `Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/Prefabs/XR Origin (XR Rig).prefab`

2. Разместите мешки с семенами:
   - Создайте экземпляры SeedBag префабов
   - Позиция: X=-3, Y=1, Z=2

3. Разместите лейку:
   - Создайте экземпляр WateringCan префаба
   - Позиция: рядом с WaterSource (X=4, Y=1, Z=5)

4. Разместите животных:
   - Создайте экземпляры Animal префабов
   - Позиция: X=-5, Y=0, Z=-3

5. Разместите торговца:
   - Создайте экземпляр Trader префаба
   - Позиция: X=0, Y=0, Z=-5

## Проверка работы

1. Нажмите Play
2. Проверьте:
   - ✅ UI отображается (деньги, инструкции)
   - ✅ Можно взять мешок с семенами (XR контроллеры)
   - ✅ Можно активировать мешок для получения семени
   - ✅ Можно бросить семя на землю для посадки
   - ✅ Можно взять лейку
   - ✅ Можно наполнить лейку водой (подойти к WaterSource)
   - ✅ Можно поливать растения (активировать лейку)
   - ✅ Можно собирать урожай (взаимодействовать с готовым растением)
   - ✅ Можно кормить животных (взаимодействовать с животным)
   - ✅ Можно торговать (взаимодействовать с торговцем)

## Решение проблем

**UI не видно:**
- Проверьте масштаб Canvas (должен быть 0.001)
- Проверьте позицию Canvas (Y=2, Z=3)

**Нельзя взять объекты:**
- Убедитесь, что XR Origin есть в сцене
- Проверьте, что XR Grab Interactable добавлен

**Растения не растут:**
- Проверьте, что PlantedCrop компонент есть
- Проверьте, что префабы назначены в CropManager

**Лейка не наполняется:**
- Проверьте тег "Water" на источнике воды
- Проверьте, что Collider на воде - Trigger

## Подробная документация

Для более детальной информации см. **SCENE_SETUP_GUIDE.md**
