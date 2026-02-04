using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Исправляет переходы в Animator Controller для животных, чтобы анимация Eat работала правильно
/// </summary>
public class FixAnimalAnimatorTransitions : EditorWindow
{
    [MenuItem("VR-Ferma/Исправить переходы Animator Controller (Eat/Happy)")]
    public static void FixTransitions()
    {
        string controllerPath = "Assets/Animations/AnimalController.controller";
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Ошибка", 
                $"Контроллер не найден по пути:\n{controllerPath}\n\nУбедитесь, что файл существует.", 
                "OK");
            return;
        }
        
        var root = controller.layers[0].stateMachine;
        
        // Находим состояния
        AnimatorState idle = null;
        AnimatorState walk = null;
        AnimatorState eat = null;
        AnimatorState happy = null;
        AnimatorState hungryIdle = null;
        
        foreach (var state in root.states)
        {
            string name = state.state.name;
            if (name == "Idle") idle = state.state;
            else if (name == "Walk") walk = state.state;
            else if (name == "Eat") eat = state.state;
            else if (name == "Happy") happy = state.state;
            else if (name == "HungryIdle") hungryIdle = state.state;
        }
        
        if (eat == null)
        {
            EditorUtility.DisplayDialog("Ошибка", 
                "Состояние 'Eat' не найдено в контроллере!", 
                "OK");
            return;
        }
        
        if (idle == null)
        {
            EditorUtility.DisplayDialog("Ошибка", 
                "Состояние 'Idle' не найдено в контроллере!", 
                "OK");
            return;
        }
        
        bool changesMade = false;
        
        // 1. Проверяем и исправляем переход из AnyState к Eat
        bool hasAnyToEat = false;
        AnimatorStateTransition anyToEatTransition = null;
        
        foreach (var transition in root.anyStateTransitions)
        {
            if (transition.destinationState == eat)
            {
                hasAnyToEat = true;
                anyToEatTransition = transition;
                break;
            }
        }
        
        if (!hasAnyToEat)
        {
            // Создаем переход из AnyState к Eat
            anyToEatTransition = root.AddAnyStateTransition(eat);
            changesMade = true;
            Debug.Log("[FixTransitions] Создан переход из AnyState к Eat");
        }
        
        // Проверяем условия перехода
        bool hasEatCondition = false;
        foreach (var condition in anyToEatTransition.conditions)
        {
            if (condition.parameter == "Eat" && condition.mode == AnimatorConditionMode.If)
            {
                hasEatCondition = true;
                break;
            }
        }
        
        if (!hasEatCondition)
        {
            // Удаляем все старые условия
            anyToEatTransition.conditions = new AnimatorCondition[0];
            // Добавляем правильное условие
            anyToEatTransition.AddCondition(AnimatorConditionMode.If, 0, "Eat");
            anyToEatTransition.hasExitTime = false;
            anyToEatTransition.duration = 0.1f;
            changesMade = true;
            Debug.Log("[FixTransitions] Добавлено условие 'Eat' к переходу из AnyState");
        }
        else
        {
            // Проверяем настройки перехода
            if (anyToEatTransition.hasExitTime)
            {
                anyToEatTransition.hasExitTime = false;
                changesMade = true;
                Debug.Log("[FixTransitions] Отключен ExitTime для перехода AnyState -> Eat");
            }
        }
        
        // 2. Проверяем переход из Eat к Idle
        bool hasEatToIdle = false;
        AnimatorStateTransition eatToIdleTransition = null;
        
        foreach (var transition in eat.transitions)
        {
            if (transition.destinationState == idle)
            {
                hasEatToIdle = true;
                eatToIdleTransition = transition;
                break;
            }
        }
        
        if (!hasEatToIdle)
        {
            eatToIdleTransition = eat.AddTransition(idle);
            eatToIdleTransition.hasExitTime = true;
            eatToIdleTransition.exitTime = 0.9f;
            eatToIdleTransition.duration = 0.2f;
            changesMade = true;
            Debug.Log("[FixTransitions] Создан переход из Eat к Idle");
        }
        
        // 3. Проверяем и исправляем переход из AnyState к Happy
        bool hasAnyToHappy = false;
        AnimatorStateTransition anyToHappyTransition = null;
        
        foreach (var transition in root.anyStateTransitions)
        {
            if (transition.destinationState == happy)
            {
                hasAnyToHappy = true;
                anyToHappyTransition = transition;
                break;
            }
        }
        
        if (happy != null)
        {
            if (!hasAnyToHappy)
            {
                anyToHappyTransition = root.AddAnyStateTransition(happy);
                changesMade = true;
                Debug.Log("[FixTransitions] Создан переход из AnyState к Happy");
            }
            
            // Проверяем условия перехода к Happy
            bool hasHappyCondition = false;
            foreach (var condition in anyToHappyTransition.conditions)
            {
                if (condition.parameter == "Happy" && condition.mode == AnimatorConditionMode.If)
                {
                    hasHappyCondition = true;
                    break;
                }
            }
            
            if (!hasHappyCondition)
            {
                anyToHappyTransition.conditions = new AnimatorCondition[0];
                anyToHappyTransition.AddCondition(AnimatorConditionMode.If, 0, "Happy");
                anyToHappyTransition.hasExitTime = false;
                anyToHappyTransition.duration = 0.1f;
                changesMade = true;
                Debug.Log("[FixTransitions] Добавлено условие 'Happy' к переходу из AnyState");
            }
            
            // Проверяем переход из Happy к Idle
            bool hasHappyToIdle = false;
            foreach (var transition in happy.transitions)
            {
                if (transition.destinationState == idle)
                {
                    hasHappyToIdle = true;
                    break;
                }
            }
            
            if (!hasHappyToIdle)
            {
                var happyToIdle = happy.AddTransition(idle);
                happyToIdle.hasExitTime = true;
                happyToIdle.exitTime = 0.9f;
                happyToIdle.duration = 0.2f;
                changesMade = true;
                Debug.Log("[FixTransitions] Создан переход из Happy к Idle");
            }
        }
        
        // 4. Добавляем переходы из Idle к Eat (на случай если AnyState не работает)
        bool hasIdleToEat = false;
        foreach (var transition in idle.transitions)
        {
            if (transition.destinationState == eat)
            {
                hasIdleToEat = true;
                break;
            }
        }
        
        if (!hasIdleToEat)
        {
            var idleToEat = idle.AddTransition(eat);
            idleToEat.AddCondition(AnimatorConditionMode.If, 0, "Eat");
            idleToEat.hasExitTime = false;
            idleToEat.duration = 0.1f;
            changesMade = true;
            Debug.Log("[FixTransitions] Создан переход из Idle к Eat");
        }
        
        // Сохраняем изменения
        if (changesMade)
        {
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Готово!", 
                "Переходы в Animator Controller исправлены!\n\n" +
                "Добавлены/исправлены:\n" +
                "✓ AnyState -> Eat (триггер Eat)\n" +
                "✓ Eat -> Idle (exit time)\n" +
                "✓ Idle -> Eat (триггер Eat)\n" +
                (happy != null ? "✓ AnyState -> Happy (триггер Happy)\n" : "") +
                (happy != null ? "✓ Happy -> Idle (exit time)\n" : "") +
                "\nТеперь анимация поедания должна работать!", 
                "OK");
            Debug.Log("[FixTransitions] ✓ Все переходы исправлены и сохранены!");
        }
        else
        {
            EditorUtility.DisplayDialog("Информация", 
                "Все переходы уже настроены правильно!\n\n" +
                "Проверьте, что:\n" +
                "1. Параметр 'Eat' существует и является Trigger\n" +
                "2. Состояние 'Eat' существует\n" +
                "3. Переход из AnyState к Eat имеет условие на триггер Eat", 
                "OK");
        }
    }
}
