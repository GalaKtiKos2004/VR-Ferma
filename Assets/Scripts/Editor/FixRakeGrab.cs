using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Исправляет настройки граблей и тяпки для VR взаимодействия
/// </summary>
public class FixRakeGrab : Editor
{
    [MenuItem("VR-Ferma/Исправить настройки граблей (VR Grab)")]
    public static void FixRake()
    {
        Rake[] rakes = FindObjectsOfType<Rake>();
        if (rakes.Length == 0)
        {
            EditorUtility.DisplayDialog("Не найдено", "Грабли не найдены в сцене!", "OK");
            return;
        }
        
        int fixedCount = 0;
        foreach (Rake rake in rakes)
        {
            if (FixTool(rake.gameObject))
                fixedCount++;
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Исправлено граблей: {fixedCount} из {rakes.Length}\n\nПроверьте настройки в Inspector.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Исправить настройки тяпки (VR Grab)")]
    public static void FixHoe()
    {
        Hoe[] hoes = FindObjectsOfType<Hoe>();
        if (hoes.Length == 0)
        {
            EditorUtility.DisplayDialog("Не найдено", "Тяпка не найдена в сцене!", "OK");
            return;
        }
        
        int fixedCount = 0;
        foreach (Hoe hoe in hoes)
        {
            if (FixTool(hoe.gameObject))
                fixedCount++;
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Исправлено тяпок: {fixedCount} из {hoes.Length}\n\nПроверьте настройки в Inspector.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Исправить все инструменты (Rake + Hoe)")]
    public static void FixAllTools()
    {
        // Проверяем XR Interaction Manager
        UnityEngine.XR.Interaction.Toolkit.XRInteractionManager manager = 
            FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Внимание", 
                "XR Interaction Manager не найден в сцене!\n\n" +
                "Добавьте XR Origin (XR Rig) из XR Interaction Toolkit в сцену.\n" +
                "Он содержит XR Interaction Manager.", 
                "OK");
        }
        
        Rake[] rakes = FindObjectsOfType<Rake>();
        Hoe[] hoes = FindObjectsOfType<Hoe>();
        
        int total = rakes.Length + hoes.Length;
        if (total == 0)
        {
            EditorUtility.DisplayDialog("Не найдено", "Инструменты не найдены в сцене!", "OK");
            return;
        }
        
        int fixedCount = 0;
        foreach (Rake rake in rakes)
        {
            if (FixTool(rake.gameObject))
                fixedCount++;
        }
        foreach (Hoe hoe in hoes)
        {
            if (FixTool(hoe.gameObject))
                fixedCount++;
        }
        
        string message = $"Исправлено инструментов: {fixedCount} из {total}";
        if (manager == null)
        {
            message += "\n\n⚠️ ВАЖНО: Добавьте XR Origin в сцену!";
        }
        message += "\n\nПроверьте настройки в Inspector.";
        
        EditorUtility.DisplayDialog("Готово", message, "OK");
    }
    
    private static bool FixTool(GameObject tool)
    {
        bool changed = false;
        
        // 1. Проверяем Rigidbody
        Rigidbody rb = tool.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = tool.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.drag = 0f;
            rb.angularDrag = 0.05f;
            rb.useGravity = true;
            rb.isKinematic = false;
            changed = true;
            Debug.Log($"[FixRakeGrab] Добавлен Rigidbody к {tool.name}");
        }
        else if (rb.isKinematic)
        {
            rb.isKinematic = false;
            changed = true;
            Debug.Log($"[FixRakeGrab] Rigidbody.isKinematic = false для {tool.name}");
        }
        
        // 2. Проверяем коллайдер
        Collider col = tool.GetComponent<Collider>();
        if (col == null)
        {
            // Ищем коллайдер в дочерних объектах
            col = tool.GetComponentInChildren<Collider>();
            if (col == null)
            {
                // Создаём BoxCollider
                BoxCollider box = tool.AddComponent<BoxCollider>();
                box.size = new Vector3(0.5f, 1f, 0.5f);
                box.center = Vector3.zero;
                changed = true;
                Debug.Log($"[FixRakeGrab] Добавлен BoxCollider к {tool.name}");
            }
        }
        
        // 3. Проверяем XRGrabInteractable
        XRGrabInteractable grab = tool.GetComponent<XRGrabInteractable>();
        if (grab == null)
        {
            grab = tool.AddComponent<XRGrabInteractable>();
            changed = true;
            Debug.Log($"[FixRakeGrab] Добавлен XRGrabInteractable к {tool.name}");
        }
        
        // 4. Настраиваем XRGrabInteractable
        if (grab != null)
        {
            SerializedObject so = new SerializedObject(grab);
            
            // Убеждаемся что interactionType правильный
            SerializedProperty interactionType = so.FindProperty("m_InteractionType");
            if (interactionType != null && interactionType.enumValueIndex != 0) // 0 = General
            {
                interactionType.enumValueIndex = 0;
                changed = true;
            }
            
            // Убеждаемся что selectMode правильный
            SerializedProperty selectMode = so.FindProperty("m_SelectMode");
            if (selectMode != null && selectMode.enumValueIndex != 0) // 0 = Multiple
            {
                selectMode.enumValueIndex = 0;
                changed = true;
            }
            
            // Убеждаемся что colliders настроены
            SerializedProperty colliders = so.FindProperty("m_Colliders");
            if (colliders != null)
            {
                if (col != null)
                {
                    colliders.arraySize = 1;
                    colliders.GetArrayElementAtIndex(0).objectReferenceValue = col;
                    changed = true;
                }
            }
            
            so.ApplyModifiedProperties();
        }
        
        // 5. Проверяем что нет конфликтующих коллайдеров на дочерних объектах
        Collider[] allColliders = tool.GetComponentsInChildren<Collider>();
        bool hasRootCollider = false;
        foreach (Collider c in allColliders)
        {
            if (c.transform == tool.transform)
            {
                hasRootCollider = true;
                break;
            }
        }
        
        if (!hasRootCollider && allColliders.Length > 0)
        {
            // Используем первый дочерний коллайдер
            if (grab != null)
            {
                SerializedObject so = new SerializedObject(grab);
                SerializedProperty colliders = so.FindProperty("m_Colliders");
                if (colliders != null && colliders.arraySize == 0)
                {
                    colliders.arraySize = 1;
                    colliders.GetArrayElementAtIndex(0).objectReferenceValue = allColliders[0];
                    so.ApplyModifiedProperties();
                    changed = true;
                    Debug.Log($"[FixRakeGrab] Настроен коллайдер из дочернего объекта для {tool.name}");
                }
            }
        }
        
        if (changed)
        {
            EditorUtility.SetDirty(tool);
        }
        
        return changed;
    }
}
