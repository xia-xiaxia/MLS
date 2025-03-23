#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DataImporter : EditorWindow
{
    private TextAsset ingredientsCSV;
    private TextAsset recipesCSV;
    private Dictionary<string, IngredientConfig> ingredientCache;

    [MenuItem("Tools/Data Importer")]
    public static void ShowWindow()
    {
        GetWindow<DataImporter>("��Ϸ���ݵ��빤��");
    }

    void OnGUI()
    {
        GUILayout.Label("���ݵ�������", EditorStyles.boldLabel);

        ingredientsCSV = EditorGUILayout.ObjectField("ʳ��CSV�ļ�", ingredientsCSV, typeof(TextAsset), false) as TextAsset;
        recipesCSV = EditorGUILayout.ObjectField("�䷽CSV�ļ�", recipesCSV, typeof(TextAsset), false) as TextAsset;

        if (GUILayout.Button("����ʳ������"))
        {
            if (ingredientsCSV != null)
            {
                ImportIngredientsFromCSV(ingredientsCSV);
                BuildIngredientCache();
            }
            else
            {
                Debug.LogError("����ѡ��ʳ��CSV�ļ�");
            }
        }

        if (GUILayout.Button("�����䷽����"))
        {
            if (recipesCSV != null)
            {
                if (ingredientCache == null) BuildIngredientCache();
                ImportRecipesFromCSV(recipesCSV);
            }
            else
            {
                Debug.LogError("����ѡ���䷽CSV�ļ�");
            }
        }
    }

    private void ImportIngredientsFromCSV(TextAsset csvFile)
    {
        string[] lines = csvFile.text.Split('\n');
        int successCount = 0;

        // ����Ŀ¼����������ڣ�
        Directory.CreateDirectory(Application.dataPath + "/Data/Ingredients");

        foreach (string line in lines.Skip(1)) // ����������
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                Debug.LogError($"��Ч������: {line}");
                continue;
            }

            try
            {
                var config = ScriptableObject.CreateInstance<IngredientConfig>();
                config.id = int.Parse(fields[0]);
                config.displayName = fields[1].Trim();

                // ����ϡ�ж�ϵ��
                var rarityValues = fields[2].Split('/');
                config.rarityProfitMultiplier = float.Parse(rarityValues[0]);
                config.rarityCostMultiplier = float.Parse(rarityValues[1]);

                config.baseProfit = int.Parse(fields[3]);
                config.basePurchaseCost = int.Parse(fields[4]);

                // �����ʲ�
                string safeName = config.displayName.Replace(" ", "_");
                string assetPath = $"Assets/Data/Ingredients/{safeName}.asset";

                // ��ֹ�ظ�����
                if (!File.Exists(assetPath))
                {
                    AssetDatabase.CreateAsset(config, assetPath);
                    successCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"������ʧ��: {line}\n����: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"ʳ�ĵ�����ɣ��ɹ����� {successCount}/{lines.Length - 1} ������");
    }

    private void ImportRecipesFromCSV(TextAsset csvFile)
    {
        string[] lines = csvFile.text.Split('\n');
        int successCount = 0;

        // ����Ŀ¼����������ڣ�
        Directory.CreateDirectory(Application.dataPath + "/Data/Recipes");

        foreach (string line in lines.Skip(1)) // ����������
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                Debug.LogError($"��Ч������: {line}");
                continue;
            }

            try
            {
                var config = ScriptableObject.CreateInstance<RecipeConfig>();
                config.id = int.Parse(fields[0]);
                config.recipeName = fields[1].Trim();
                config.rarityBaseProfit = int.Parse(fields[2]);
                config.levelProfitIncrement = int.Parse(fields[3]);
                config.baseProfit = int.Parse(fields[4]);

                // ����ʳ������
                string[] requirements = fields[5].Split(',');
                foreach (string req in requirements)
                {
                    string[] parts = req.Trim().Split(' ');
                    if (parts.Length != 2)
                    {
                        Debug.LogWarning($"������Чʳ������: {req}");
                        continue;
                    }

                    string ingredientName = parts[0].Trim();
                    int amount = int.Parse(parts[1]);

                    if (ingredientCache.TryGetValue(ingredientName, out var ingredient))
                    {
                        config.requirements.Add(new RecipeConfig.IngredientRequirement
                        {
                            ingredient = ingredient,
                            amount = amount
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"�Ҳ���ʳ��[{ingredientName}]���䷽: {config.recipeName}");
                    }
                }

                // �����ʲ�
                string safeName = config.recipeName.Replace(" ", "_");
                string assetPath = $"Assets/Data/Recipes/{safeName}.asset";

                if (!File.Exists(assetPath))
                {
                    AssetDatabase.CreateAsset(config, assetPath);
                    successCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"������ʧ��: {line}\n����: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"�䷽������ɣ��ɹ����� {successCount}/{lines.Length - 1} ������");
    }

    private void BuildIngredientCache()
    {
        ingredientCache = new Dictionary<string, IngredientConfig>();
        string[] guids = AssetDatabase.FindAssets("t:IngredientConfig");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<IngredientConfig>(path);
            if (config != null)
            {
                string key = config.displayName.Trim();
                if (!ingredientCache.ContainsKey(key))
                {
                    ingredientCache.Add(key, config);
                }
                else
                {
                    Debug.LogWarning($"�����ظ�ʳ������: {key}");
                }
            }
        }
        Debug.Log($"�ѻ��� {ingredientCache.Count} ��ʳ������");
    }
}
#endif