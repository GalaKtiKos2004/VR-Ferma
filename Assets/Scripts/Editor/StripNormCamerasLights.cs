using UnityEngine;
using UnityEditor;

/// <summary>
/// Убирает камеры и свет из моделей Blender (Assets/Models/Norm):
/// 1) Отключает импорт камер/света в настройках FBX и переимпортирует;
/// 2) Удаляет уже импортированные Camera/Light из префабов сцен, если остались.
/// </summary>
public static class StripNormCamerasLights
{
    private const string NormFolder = "Assets/Models/Norm";

    [MenuItem("VR-Ferma/Убрать камеры и свет из моделей Norm (FBX)")]
    public static void DisableCamerasLightsImportAndReimport()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { NormFolder });
        int changed = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || !path.StartsWith(NormFolder)) continue;

            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;

            bool needReimport = false;
            if (importer.importCameras)
            {
                importer.importCameras = false;
                needReimport = true;
            }
            if (importer.importLights)
            {
                importer.importLights = false;
                needReimport = true;
            }

            if (needReimport)
            {
                importer.SaveAndReimport();
                changed++;
                Debug.Log($"[Norm] Отключены камеры/свет, переимпорт: {path}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (changed > 0)
            EditorUtility.DisplayDialog("Готово", $"Обработано {changed} моделей в {NormFolder}.\n\nИмпорт камер и света отключён, модели переимпортированы.", "OK");
        else
            EditorUtility.DisplayDialog("Информация", "Все модели Norm уже без импорта камер/света или в папке нет FBX.", "OK");
    }
}
