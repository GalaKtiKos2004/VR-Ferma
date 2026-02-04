# 🚀 Быстрая справка - Система обучения

## ⚡ Запуск за 3 минуты

```
1. Unity → VR-Ferma → Tutorial Setup Helper
2. "🚀 АВТОМАТИЧЕСКАЯ НАСТРОЙКА ВСЕГО"
3. Добавить XR Grab Interactable к Rake и SeedBag
4. Play!
```

## 🎮 Управление

| Действие | VR | Non-VR |
|----------|-----|--------|
| Взять | Grab | E / ЛКМ |
| Использовать | Trigger | ЛКМ |

## 🔑 Горячие клавиши (отладка)

| Клавиша | Действие |
|---------|----------|
| **R** | Сбросить обучение |
| **N** | Пропустить текущий шаг |
| **I** | Показать информацию |

## 📋 Этапы обучения (8 шагов)

1. 🔧 Взять грабли
2. 🌾 Взрыхлить грядку
3. 📦 Взять семена
4. 🌱 Посадить семя
5. 🚰 Взять лейку
6. 💧 Наполнить лейку
7. 💦 Полить росток
8. 🥕 Собрать урожай (30 сек)

## 📁 Важные файлы

| Что нужно | Файл |
|-----------|------|
| Краткое описание | README_TUTORIAL.md |
| Быстрый старт | TUTORIAL_QUICKSTART.md |
| Полная документация | TUTORIAL_README.md |
| Архитектура | TUTORIAL_ARCHITECTURE.md |
| Список изменений | TUTORIAL_CHANGELOG.md |

## 🔧 Структура сцены

```
TutorialManager (+ TutorialDebug)
├─ Barn → Rake (Rake + XRGrab)
│      └─ SeedBag (SeedBag + XRGrab)
├─ PlantBed (layer: PlantBed)
├─ WateringCan (WateringCan + XRGrab)
└─ Well (tag: Water, TriggerCollider)
```

## 🐛 Быстрые решения

### Обучение не запускается
```
VR-Ferma → Tutorial Setup Helper → "Сбросить обучение"
```

### Подсветка не работает
```
Проверить: объекты имеют Renderer
```

### Грабли не взрыхляют
```
Проверить:
- Слой PlantBed на грядках
- Tilling Range = 2
- PlantBed имеет Collider
```

### Семена не сажаются
```
Проверить:
- Грядка взрыхлена (isTilled = true)
- Planting Range = 2
```

### Лейка не наполняется
```
Проверить:
- Тег "Water" у колодца
- TriggerCollider у колодца
- Лейка касается воды
```

## 💻 Код: Сброс обучения

```csharp
PlayerPrefs.DeleteKey("TutorialCompleted");
PlayerPrefs.Save();
```

## 💻 Код: Проверка статуса

```csharp
bool isActive = TutorialManager.Instance.IsTutorialActive();
int step = TutorialManager.Instance.GetCurrentStep();
```

## 🎨 Настройки

### Цвет подсветки
```
TutorialManager → Highlight Color
```

### Скорость пульсации
```
TutorialManager → Pulse Speed
```

### Время роста растения
```
PlantBed → Growth Time (секунды)
```

## 📦 Созданные компоненты

| Скрипт | Назначение |
|--------|------------|
| TutorialManager | Главный менеджер |
| TutorialHighlight | Подсветка объектов |
| Rake | Грабли |
| SeedBag | Семена |
| TutorialDebug | Отладка |
| TutorialSetupHelper | Editor утилита |

## 🔄 Обновлённые компоненты

| Скрипт | Изменения |
|--------|-----------|
| PlantBed | + Till(), PlantSeed(), интеграция |
| WateringCan | + OnGrabbed(), интеграция |

## ✅ Контрольный список

- [ ] Открыть Tutorial Setup Helper
- [ ] Автоматическая настройка
- [ ] Добавить XRGrabInteractable к Rake
- [ ] Добавить XRGrabInteractable к SeedBag  
- [ ] Настроить события OnGrabbed()
- [ ] Назначить слой PlantBed грядкам
- [ ] Тест: пройти обучение

## 🆘 Помощь

### Проблемы с настройкой?
→ TUTORIAL_QUICKSTART.md

### Нужна полная документация?
→ TUTORIAL_README.md

### Хочу понять как работает?
→ TUTORIAL_ARCHITECTURE.md

## 📞 Context Menu

```
ПКМ на TutorialManager в Inspector:
- Сбросить обучение
- Пропустить шаг
- Показать информацию
- Завершить обучение
```

## 🌟 Быстрые факты

- **8 этапов** обучения
- **30 секунд** время роста
- **~1400 строк** кода
- **6 документов** MD
- **VR + Non-VR** поддержка
- **0 ошибок** компиляции

## 🎯 Цель обучения

> Провести игрока через полный цикл:  
> Взрыхление → Посадка → Полив → Урожай

## 🏆 Награда

```
+1 Морковка
+10 Монет
Достижение: "Первый урожай!"
```

---

**Версия:** 1.0 | **Дата:** 24.01.2026 | **Статус:** ✅ Готово

**Удачи! 🌱**
