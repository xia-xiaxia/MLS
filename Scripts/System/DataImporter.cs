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
        GetWindow<DataImporter>("游戏数据导入工具");
    }

    void OnGUI()
    {
        GUILayout.Label("数据导入配置", EditorStyles.boldLabel);

        ingredientsCSV = EditorGUILayout.ObjectField("食材CSV文件", ingredientsCSV, typeof(TextAsset), false) as TextAsset;
        recipesCSV = EditorGUILayout.ObjectField("配方CSV文件", recipesCSV, typeof(TextAsset), false) as TextAsset;

        if (GUILayout.Button("导入食材数据"))
        {
            if (ingredientsCSV != null)
            {
                ImportIngredientsFromCSV(ingredientsCSV);
                BuildIngredientCache();
            }
            else
            {
                Debug.LogError("请先选择食材CSV文件");
            }
        }

        if (GUILayout.Button("导入配方数据"))
        {
            if (recipesCSV != null)
            {
                if (ingredientCache == null) BuildIngredientCache();
                ImportRecipesFromCSV(recipesCSV);
            }
            else
            {
                Debug.LogError("请先选择配方CSV文件");
            }
        }
    }

    private void ImportIngredientsFromCSV(TextAsset csvFile)
    {
        string[] lines = csvFile.text.Split('\n');
        int successCount = 0;

        // 创建目录（如果不存在）
        Directory.CreateDirectory(Application.dataPath + "/Data/Ingredients");

        foreach (string line in lines.Skip(1)) // 跳过标题行
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                Debug.LogError($"无效数据行: {line}");
                continue;
            }

            try
            {
                var config = ScriptableObject.CreateInstance<IngredientConfig>();
                config.id = int.Parse(fields[0]);
                config.displayName = fields[1].Trim();

                // 解析稀有度系数
                var rarityValues = fields[2].Split('/');
                config.rarityProfitMultiplier = float.Parse(rarityValues[0]);
                config.rarityCostMultiplier = float.Parse(rarityValues[1]);

                config.baseProfit = int.Parse(fields[3]);
                config.basePurchaseCost = int.Parse(fields[4]);

                // 保存资产
                string safeName = config.displayName.Replace(" ", "_");
                string assetPath = $"Assets/Data/Ingredients/{safeName}.asset";

                // 防止重复创建
                if (!File.Exists(assetPath))
                {
                    AssetDatabase.CreateAsset(config, assetPath);
                    successCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"处理行失败: {line}\n错误: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"食材导入完成，成功导入 {successCount}/{lines.Length - 1} 项数据");
    }

    private void ImportRecipesFromCSV(TextAsset csvFile)
    {
        string[] lines = csvFile.text.Split('\n');
        int successCount = 0;

        // 创建目录（如果不存在）
        Directory.CreateDirectory(Application.dataPath + "/Data/Recipes");

        foreach (string line in lines.Skip(1)) // 跳过标题行
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                Debug.LogError($"无效数据行: {line}");
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

                // 解析食材需求
                string[] requirements = fields[5].Split(',');
                foreach (string req in requirements)
                {
                    string[] parts = req.Trim().Split(' ');
                    if (parts.Length != 2)
                    {
                        Debug.LogWarning($"跳过无效食材需求: {req}");
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
                        Debug.LogWarning($"找不到食材[{ingredientName}]，配方: {config.recipeName}");
                    }
                }

                // 保存资产
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
                Debug.LogError($"处理行失败: {line}\n错误: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"配方导入完成，成功导入 {successCount}/{lines.Length - 1} 项数据");
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
                    Debug.LogWarning($"发现重复食材名称: {key}");
                }
            }
        }
        Debug.Log($"已缓存 {ingredientCache.Count} 项食材配置");
    }
}
#endif