# 🔧 Исправление проблемы: "Не могу взять грабли"

## Быстрое решение

### Вариант 1: Автоматическое исправление (рекомендуется)

1. Откройте сцену в Unity
2. **VR-Ferma → Исправить все инструменты (Rake + Hoe)**
3. Скрипт автоматически исправит настройки граблей и тяпки

### Вариант 2: Исправить только грабли

1. **VR-Ferma → Исправить настройки граблей (VR Grab)**

### Вариант 3: Исправить только тяпку

1. **VR-Ferma → Исправить настройки тяпки (VR Grab)**

## Что исправляет скрипт

✅ Проверяет наличие **Rigidbody** (добавляет если нет)  
✅ Проверяет что **Rigidbody.isKinematic = false**  
✅ Проверяет наличие **Collider** (добавляет BoxCollider если нет)  
✅ Проверяет наличие **XRGrabInteractable** (добавляет если нет)  
✅ Настраивает **XRGrabInteractable.colliders** (добавляет коллайдер в список)  
✅ Настраивает правильные параметры **XRGrabInteractable**

## Ручная проверка (если автоматическое исправление не помогло)

### 1. Проверьте XR Interaction Manager

В сцене должен быть **XR Origin (XR Rig)** или **XR Interaction Manager**.

**Как проверить:**
- В Hierarchy найдите объект с компонентом `XR Interaction Manager`
- Если нет — добавьте XR Origin из XR Interaction Toolkit

**Как добавить:**
- GameObject → XR → XR Origin (VR)
- Или используйте префаб из Samples

### 2. Проверьте грабли в Inspector

Выберите грабли в сцене и проверьте:

#### Rigidbody
- ✅ Должен быть компонент **Rigidbody**
- ✅ **Is Kinematic** = ❌ (не включено)
- ✅ **Use Gravity** = ✅ (включено)

#### Collider
- ✅ Должен быть компонент **Collider** (BoxCollider, MeshCollider и т.д.)
- ✅ Collider **не должен быть** Is Trigger

#### XR Grab Interactable
- ✅ Должен быть компонент **XR Grab Interactable**
- ✅ В разделе **Colliders** должен быть добавлен ваш Collider
- ✅ **Interaction Type** = General
- ✅ **Select Mode** = Multiple

### 3. Проверьте Interaction Layers

1. Откройте **Edit → Project Settings → XR Plug-in Management → Interaction Layers**
2. Убедитесь что слои настроены
3. В **XR Grab Interactable** на граблях:
   - **Interaction Layers** должен совпадать с **Ray Interactor** на контроллере

### 4. Проверьте контроллеры

В **XR Origin** проверьте:
- ✅ Есть **Ray Interactor** или **Direct Interactor**
- ✅ **Interaction Layers** совпадают с граблями
- ✅ Контроллеры активны

## Частые проблемы

### Проблема: "Грабли не подсвечиваются при наведении"

**Решение:**
- Проверьте что **XR Grab Interactable** есть на граблях
- Проверьте **Interaction Layers** совпадают
- Проверьте что контроллеры активны

### Проблема: "Грабли подсвечиваются, но не берутся"

**Решение:**
- Проверьте **Rigidbody** — не должен быть Kinematic
- Проверьте **Collider** в списке **XR Grab Interactable**
- Используйте **VR-Ferma → Исправить настройки граблей**

### Проблема: "Грабли берутся, но сразу падают"

**Решение:**
- Проверьте **Rigidbody.mass** (должно быть ~0.5-1.0)
- Проверьте что **XR Grab Interactable** правильно настроен
- Проверьте **Attach Transform** в XRGrabInteractable

### Проблема: "В Non-VR режиме не работает"

**Решение:**
- В Non-VR используйте мышь (ЛКМ)
- Проверьте что **XR Grab Interactable** поддерживает Non-VR
- Или используйте **SimpleInteractable** для Non-VR

## Проверка через Console

Откройте **Window → General → Console** и проверьте ошибки:

- ❌ "XR Interaction Manager not found" → Добавьте XR Origin
- ❌ "No colliders found" → Добавьте Collider к граблям
- ❌ "Rigidbody is kinematic" → Отключите Is Kinematic

## После исправления

1. Сохраните сцену (Ctrl+S)
2. Запустите игру
3. Попробуйте взять грабли в VR

## Если ничего не помогло

1. Удалите грабли из сцены
2. **VR-Ferma → Добавить грабли на сцену** (создаст заново с правильными настройками)
3. Или используйте префаб: **VR-Ferma → Создать префабы граблей и тяпки**

---

**Совет:** Всегда используйте **VR-Ferma → Исправить все инструменты** после создания новых инструментов в сцене!
