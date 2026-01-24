using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Подставляет модели из Assets/Models/Norm в грядки (PlantBed).
/// Состояния: грядка до → грядка после (тяпка/грабли) → семена → ростки → помидоры на грядке.
/// </summary>
public static class PlantBedNormModels
{
    private const string NormFolder = "Assets/Models/Norm";
    private const string BedDo = NormFolder + "/грядки до.fbx";
    private const string BedPosle = NormFolder + "/грядки после.fbx";
    private const string Seeds = NormFolder + "/семена.fbx";
    private const string Rostki = NormFolder + "/ростки.fbx";
    private const string Pomidory = NormFolder + "/помидоры на грядке (2).fbx";
    private const string Pumpkin = NormFolder + "/тыквы 1.fbx";
    private const string Carrot = NormFolder + "/морковка в грядке.fbx";
    private const string Onion = NormFolder + "/лук в грядке2.fbx";

    [MenuItem("VR-Ferma/Подставить модели Norm в грядки")]
    public static void AssignNormModelsToPlantBeds()
    {
        var bedDo = AssetDatabase.LoadAssetAtPath<GameObject>(BedDo);
        var bedPosle = AssetDatabase.LoadAssetAtPath<GameObject>(BedPosle);
        var seeds = AssetDatabase.LoadAssetAtPath<GameObject>(Seeds);
        var rostki = AssetDatabase.LoadAssetAtPath<GameObject>(Rostki);
        var pomidory = AssetDatabase.LoadAssetAtPath<GameObject>(Pomidory);
        var pumpkin = AssetDatabase.LoadAssetAtPath<GameObject>(Pumpkin);
        var carrot = AssetDatabase.LoadAssetAtPath<GameObject>(Carrot);
        var onion = AssetDatabase.LoadAssetAtPath<GameObject>(Onion);

        if (bedDo == null || bedPosle == null || seeds == null || rostki == null || pomidory == null || pumpkin == null || carrot == null || onion == null)
        {
            string missing = "";
            if (bedDo == null) missing += "\n• грядки до.fbx";
            if (bedPosle == null) missing += "\n• грядки после.fbx";
            if (seeds == null) missing += "\n• семена.fbx";
            if (rostki == null) missing += "\n• ростки.fbx";
            if (pomidory == null) missing += "\n• помидоры на грядке (2).fbx";
            if (pumpkin == null) missing += "\n• тыквы 1.fbx";
            if (carrot == null) missing += "\n• морковка в грядке.fbx";
            if (onion == null) missing += "\n• лук в грядке2.fbx";
            EditorUtility.DisplayDialog("Модели не найдены",
                "Не найдены модели в " + NormFolder + ":" + missing, "OK");
            return;
        }

        PlantBed[] beds = Object.FindObjectsOfType<PlantBed>();
        if (beds.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет грядок", "В сцене нет объектов с PlantBed.", "OK");
            return;
        }

        int count = 0;
        foreach (var bed in beds)
        {
            Undo.RecordObject(bed, "PlantBed Norm models");
            var so = new SerializedObject(bed);
            so.FindProperty("modelBedDo").objectReferenceValue = bedDo;
            so.FindProperty("modelBedPosle").objectReferenceValue = bedPosle;
            so.FindProperty("modelSeeds").objectReferenceValue = seeds;
            so.FindProperty("modelRostki").objectReferenceValue = rostki;
            so.FindProperty("modelPomidory").objectReferenceValue = pomidory;
            so.FindProperty("modelPumpkin").objectReferenceValue = pumpkin;
            so.FindProperty("modelCarrot").objectReferenceValue = carrot;
            so.FindProperty("modelOnion").objectReferenceValue = onion;
            so.ApplyModifiedProperties();
            count++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[PlantBed Norm] Назначены модели Norm для {count} грядок.");
        EditorUtility.DisplayDialog("Готово",
            $"Модели Norm назначены для {count} грядок.\n\nГрядка до → после → семена → ростки → помидоры/тыквы/морковка/лук.", "OK");
    }
}
