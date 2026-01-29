using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor для отладки анимаций животных в runtime
/// </summary>
[CustomEditor(typeof(Animal))]
public class AnimalDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Animal animal = (Animal)target;
        
        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("=== ОТЛАДКА АНИМАЦИИ (Play Mode) ===", EditorStyles.boldLabel);
            
            var animator = animal.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                EditorGUILayout.HelpBox("Animator не найден!", MessageType.Error);
                return;
            }
            
            EditorGUILayout.LabelField($"Animator на: {animator.gameObject.name}");
            EditorGUILayout.LabelField($"Controller: {(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "НЕТ ❌")}");
            EditorGUILayout.LabelField($"Avatar: {(animator.avatar != null ? animator.avatar.name : "НЕТ ❌")}");
            EditorGUILayout.LabelField($"Enabled: {animator.enabled}");
            EditorGUILayout.LabelField($"IsInitialized: {animator.isInitialized}");
            
            if (animator.isInitialized && animator.layerCount > 0)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                int hash = state.shortNameHash;
                string stateName = hash == Animator.StringToHash("Idle") ? "Idle" :
                                   hash == Animator.StringToHash("Walk") ? "Walk" :
                                   hash == Animator.StringToHash("HungryIdle") ? "HungryIdle" :
                                   hash == Animator.StringToHash("Eat") ? "Eat" :
                                   hash == Animator.StringToHash("Happy") ? "Happy" : hash.ToString();
                EditorGUILayout.LabelField($"Текущее состояние: {stateName}");
                EditorGUILayout.LabelField($"NormalizedTime: {state.normalizedTime:F2}");
                EditorGUILayout.LabelField($"Speed: {animator.speed}");
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Параметры:", EditorStyles.boldLabel);
                foreach (var param in animator.parameters)
                {
                    string value = param.type == AnimatorControllerParameterType.Bool ? animator.GetBool(param.name).ToString() :
                                   param.type == AnimatorControllerParameterType.Float ? animator.GetFloat(param.name).ToString("F2") :
                                   param.type == AnimatorControllerParameterType.Int ? animator.GetInteger(param.name).ToString() : "Trigger";
                    EditorGUILayout.LabelField($"  {param.name} ({param.type}): {value}");
                }
                
                EditorGUILayout.Space();
                if (GUILayout.Button("Триггер: Eat (покормить)"))
                {
                    animator.SetTrigger("Eat");
                    Debug.Log($"[Test] Триггер Eat");
                }
                if (GUILayout.Button("Триггер: Happy (погладить)"))
                {
                    animator.SetTrigger("Happy");
                    Debug.Log($"[Test] Триггер Happy");
                }
                if (GUILayout.Button("Toggle IsWalking"))
                {
                    bool current = animator.GetBool("IsWalking");
                    animator.SetBool("IsWalking", !current);
                    Debug.Log($"[Test] IsWalking = {!current}");
                }
                if (GUILayout.Button("Toggle IsHungry"))
                {
                    bool current = animator.GetBool("IsHungry");
                    animator.SetBool("IsHungry", !current);
                    Debug.Log($"[Test] IsHungry = {!current}");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Animator не инициализирован или нет слоёв!", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Запустите Play Mode для отладки анимаций", MessageType.Info);
        }
    }
}
