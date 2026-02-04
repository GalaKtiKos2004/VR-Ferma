# Инструкция по настройке простой фермерской игры

## Обзор

Простая VR/Non-VR игра про фермерство с механиками:
- 🌱 Полив грядок и выращивание растений
- 🐔 Уход за животными (кормление и поглаживание)
- 💧 Система воды (лейка + бочка)

## Быстрый старт

### 1. Подготовка сцены

1. Откройте Unity Editor
2. Создайте новую сцену или откройте существующую (`Assets/Scenes/SampleScene.unity`)
3. Добавьте плоскость земли: `GameObject → 3D Object → Plane`
   - Увеличьте масштаб до (5, 1, 5)

### 2. Настройка игрока (Non-VR режим)

1. Создайте пустой объект: `GameObject → Create Empty`
2. Переименуйте в "Player"
3. Добавьте компоненты:
   - `Character Controller`
   - Скрипт `NonVRPlayerController`
4. Создайте дочернюю камеру:
   - `GameObject → Camera`
   - Перетащите камеру в Player
   - Установите позицию (0, 1.6, 0)
   - Присвойте камеру в поле "Player Camera" скрипта NonVRPlayerController

### 3. Добавление GameManager

1. Создайте пустой объект: `GameObject → Create Empty`
2. Переименуйте в "GameManager"
3. Добавьте скрипт `SimpleGameManager`
4. Настройте UI (опционально):
   - Создайте Canvas: `GameObject → UI → Canvas`
   - Добавьте TextMeshPro элементы для отображения ресурсов
   - Присвойте их в поля GameManager

### 4. Добавление грядок

1. Перетащите модель `Assets/Models/gryadka.fbx` на сцену
2. Добавьте компоненты:
   - `Box Collider` (если нет)
   - Скрипт `PlantBed`
3. Настройте PlantBed:
   - Создайте префаб растения (см. ниже)
   - Присвойте в поле "Plant Prefab"
4. Установите Layer "Interactable" (создайте если нужно)
5. Дублируйте грядку несколько раз (Ctrl+D)

### 5. Создание префаба растения

1. Создайте пустой объект: `GameObject → Create Empty`
2. Переименуйте в "Plant_Carrot"
3. Добавьте скрипт `Plant`
4. Настройте параметры:
   - Growth Time: 30 (секунд)
   - Max Growth Stage: 3
5. Добавьте визуал (опционально):
   - Создайте 3 дочерних объекта с разными моделями/размерами
   - Присвойте в массив "Growth Stages"
6. Создайте префаб: перетащите объект в папку `Assets/Prefabs/`
7. Удалите из сцены

### 6. Добавление лейки

1. Перетащите модель `Assets/Models/лейка.fbx` на сцену
2. Добавьте компоненты:
   - `Box Collider` или `Mesh Collider`
   - Скрипт `WateringCan`
   - `XR Grab Interactable` (для VR)
3. Настройте WateringCan:
   - Max Water: 100
   - Current Water: 0
   - Watering Range: 2
   - Layer Mask: выберите слой грядок
4. Добавьте Particle System для эффекта воды:
   - Создайте дочерний объект с Particle System
   - Настройте эмиссию воды
   - Присвойте в поле "Water Particles"
5. Создайте Transform "WaterSpout" внизу лейки
6. Установите Layer "Interactable"

### 7. Добавление бочки с водой

1. Перетащите модель `Assets/Models/бочка.fbx` на сцену
2. Добавьте компоненты:
   - `Box Collider` с галочкой "Is Trigger"
   - Скрипт `WaterSource`
3. Установите Tag "Water" (создайте если нужно)
4. Настройте WaterSource:
   - Is Infinite: ✓
   - Water Amount: 1000

### 8. Добавление животных

1. Перетащите модель `Assets/Models/source/Chicken_Rig.fbx` на сцену
2. Добавьте компоненты:
   - `Capsule Collider` (если нет)
   - Скрипт `Animal`
3. Настройте Animal:
   - Animal Name: "Курица"
   - Max Happiness: 100
   - Current Happiness: 50
   - Feed Amount: 30
   - Pet Amount: 20
4. Установите Layer "Interactable"
5. Присвойте Animator если есть анимации

### 9. Добавление кормушки (опционально)

1. Перетащите модель кормушки или создайте простой Box
2. Добавьте компоненты:
   - `Box Collider`
   - Скрипт `FeedingTrough`
3. Настройте FeedingTrough:
   - Food Amount: 100
   - Max Food: 100
   - Feeding Range: 2
   - Animal Layer: выберите слой животных

### 10. Настройка слоёв и тегов

1. Создайте Layers:
   - `Edit → Project Settings → Tags and Layers`
   - Добавьте "Interactable"
   - Добавьте "Animal"
2. Создайте Tags:
   - Добавьте "Water"
3. Примените к объектам

### 11. Настройка освещения

1. Добавьте Directional Light если его нет
2. Настройте:
   - Rotation: (50, -30, 0)
   - Intensity: 1
   - Color: светло-жёлтый

### 12. Тестирование

1. Нажмите Play
2. Управление:
   - **WASD** - движение
   - **Мышь** - поворот камеры
   - **E / ЛКМ** - взаимодействие
   - **Shift** - бег
   - **ESC** - освободить курсор

## Игровой процесс

### Цикл выращивания растений

1. Возьмите лейку (подойдите и нажмите E)
2. Наполните у бочки с водой
3. Полейте грядку
4. Посадите семена (нажмите E на политой грядке)
5. Дождитесь роста (30 секунд)
6. Соберите урожай (нажмите E на выросшем растении)

### Уход за животными

1. Подойдите к животному
2. Погладьте (нажмите E)
3. Покормите через кормушку или вручную
4. Животное будет довольным и даст продукцию

## Советы по настройке

### Материалы

Если модели розовые (нет материалов):
1. Создайте простой материал: `Assets → Create → Material`
2. Выберите Shader: URP/Lit
3. Настройте цвет
4. Примените к модели

### VR режим

Для включения VR:
1. Добавьте XR Origin вместо обычного Player
2. Используйте `XR Interaction Toolkit`
3. Настройте `XR Grab Interactable` на лейке
4. Добавьте `XR Ray Interactor` на контроллеры

### Оптимизация

- Используйте Object Pooling для растений
- Добавьте Occlusion Culling
- Оптимизируйте коллайдеры (упростите mesh colliders)

## Дополнительные функции

### Звуки

Добавьте AudioClip в соответствующие поля:
- PlantBed: wateringSound, plantingSound
- Animal: happySound, hungrySound, eatSound
- WateringCan: wateringSound, refillSound
- WaterSource: waterScoopSound

### Визуальные эффекты

- Добавьте Particle Systems для воды, сердечек
- Используйте материалы с разными цветами для состояний
- Добавьте анимации для животных

### UI

Подключите TextMeshPro элементы в GameManager:
- Coins Text - для отображения монет
- Carrots Text - для моркови
- Tomatoes Text - для помидоров
- Eggs Text - для яиц
- Hint Text - для подсказок

## Решение проблем

### Не работает взаимодействие

- Проверьте, что объекты на правильном слое (Interactable)
- Проверьте, что у объектов есть Collider
- Убедитесь, что Layer Mask в NonVRPlayerController настроен правильно

### Растения не растут

- Проверьте, что грядка полита
- Проверьте настройки Plant (Growth Time)
- Убедитесь, что скрипт Plant добавлен на префаб

### Лейка не работает

- Проверьте, что бочка имеет Tag "Water"
- Проверьте Layer Mask в WateringCan
- Убедитесь, что Water Spout установлен

## Структура файлов

```
Assets/
├── Scripts/
│   ├── PlantBed.cs           # Грядка
│   ├── Plant.cs              # Растение с ростом
│   ├── WateringCan.cs        # Лейка
│   ├── Animal.cs             # Животное
│   ├── WaterSource.cs        # Источник воды
│   ├── FeedingTrough.cs      # Кормушка
│   ├── SimpleGameManager.cs  # Менеджер игры
│   ├── NonVRPlayerController.cs  # Управление игроком
│   └── VRInteractor.cs       # VR взаимодействие
├── Models/
│   ├── gryadka.fbx           # Грядка
│   ├── лейка.fbx             # Лейка
│   ├── бочка.fbx             # Бочка
│   └── source/
│       └── Chicken_Rig.fbx   # Курица
├── Prefabs/
│   └── Plant_Carrot.prefab   # Префаб растения
└── Scenes/
    └── FarmScene.unity       # Игровая сцена
```

## Дальнейшее развитие

- Добавьте больше типов растений (помидоры, тыква, кукуруза)
- Добавьте больше животных (корова, свинья, утка)
- Добавьте систему торговли
- Добавьте смену дня/ночи
- Добавьте погодные эффекты
- Добавьте постройки (сарай, дом)

---

**Приятной игры! 🌾🐔**
