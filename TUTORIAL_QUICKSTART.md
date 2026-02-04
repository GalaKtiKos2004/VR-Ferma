# Быстрый старт - Система обучения

## Краткая инструкция по настройке обучения в Unity

### Шаг 1: Создайте Tutorial Manager (5 минут)

1. В Hierarchy создайте пустой GameObject → назовите "TutorialManager"
2. Add Component → `TutorialManager`
3. Перетащите объекты в поля Inspector:

```
Tutorial Manager (Script)
├─ Tutorial Enabled: ✓
├─ Barn: [перетащите объект сарая]
├─ Rake: [перетащите объект граблей - создайте если нет]
├─ Seed Bag: [перетащите пакет семян - создайте если нет]
├─ Tutorial Plant Bed: [перетащите одну грядку PlantBed]
├─ Watering Can: [перетащите лейку]
└─ Well: [перетащите колодец]
```

### Шаг 2: Создайте грабли (3 минуты)

```
1. GameObject → 3D Object → Cube → назовите "Rake"
2. Transform: Position (0, 1, 0), Scale (0.1, 1, 0.1)
3. Add Component → Rake
4. Add Component → XR Grab Interactable
5. В Rake (Script):
   - Tilling Range: 2
   - Plant Bed Layer: PlantBed
6. В XR Grab Interactable → Events:
   - Select Entered → Rake.OnGrabbed()
```

### Шаг 3: Создайте пакет семян (3 минуты)

```
1. GameObject → 3D Object → Cube → назовите "SeedBag"
2. Transform: Position (1, 1, 0), Scale (0.3, 0.2, 0.2)
3. Material: зелёный цвет
4. Add Component → SeedBag
5. Add Component → XR Grab Interactable
6. В SeedBag (Script):
   - Seed Type: "Морковь"
   - Infinite Seeds: ✓
   - Planting Range: 2
   - Plant Bed Layer: PlantBed
7. В XR Grab Interactable → Events:
   - Select Entered → SeedBag.OnGrabbed()
```

### Шаг 4: Настройте слои (2 минуты)

```
1. Edit → Project Settings → Tags and Layers
2. Layers → User Layer 8: "PlantBed"
3. Выберите все грядки в сцене
4. Inspector → Layer → PlantBed
```

### Шаг 5: Проверьте колодец (1 минута)

```
1. Найдите объект колодца в сцене
2. Inspector → Tag: "Water"
3. Убедитесь что есть Trigger Collider
```

### Шаг 6: Запустите! (1 минута)

```
1. Play
2. Должна появиться подсказка: "Добро пожаловать на ферму!"
3. Сарай и грабли должны подсвечиваться жёлтым
4. Следуйте инструкциям на экране
```

## Быстрое решение проблем

### Подсветка не работает
- Проверьте что у объектов есть Renderer
- Проверьте что TutorialManager.Start() вызывается

### Грабли/семена не работают
- Проверьте слой PlantBed у грядок
- Проверьте что есть Collider на грядках
- Проверьте что Tilling Range / Planting Range достаточный

### Лейка не наполняется
- Проверьте тег "Water" у колодца
- Проверьте что у колодца есть Trigger Collider
- Проверьте что лейка касается воды

### Обучение не запускается
- Проверьте что Tutorial Enabled = ✓
- Очистите PlayerPrefs: `PlayerPrefs.DeleteKey("TutorialCompleted")`
- Проверьте Console на ошибки

## Минимальная структура сцены

```
Scene
├─ TutorialManager
├─ Barn (любой GameObject)
│   ├─ Rake (с компонентом Rake + XRGrabInteractable)
│   └─ SeedBag (с компонентом SeedBag + XRGrabInteractable)
├─ PlantBed (с компонентом PlantBed, слой PlantBed)
├─ WateringCan (с компонентом WateringCan + XRGrabInteractable)
├─ Well (тег "Water", TriggerCollider)
├─ SimpleGameManager (для UI подсказок)
└─ Canvas
    └─ HintText (TextMeshProUGUI)
```

## Полная последовательность обучения

1. ✓ Взять грабли из сарая
2. ✓ Взрыхлить грядку
3. ✓ Взять семена из сарая
4. ✓ Посадить семя на грядку
5. ✓ Взять лейку
6. ✓ Наполнить лейку в колодце
7. ✓ Полить росток
8. ✓ Подождать 30 секунд
9. ✓ Собрать морковку
10. ✓ Получить достижение "Первый урожай!"

## Примечания

- Обучение работает как в VR, так и в Non-VR режиме
- VR: используйте триггер на контроллере
- Non-VR: используйте ЛКМ (левая кнопка мыши)
- После завершения обучение сохраняется и больше не запускается

Удачи! 🌱🥕
