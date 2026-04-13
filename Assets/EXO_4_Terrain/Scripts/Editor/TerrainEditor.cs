using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTerrainGenerator))]
public class TerrainEditor : Editor
{
    // ── Serialized Properties - Grid ──────────────────────────────
    private SerializedProperty gridWidth;
    private SerializedProperty gridHeight;
    private SerializedProperty tileSize;

    // ── Serialized properties - Terrain noise ────────────────────────
    private SerializedProperty seed;
    private SerializedProperty noiseScale;
    private SerializedProperty offsetX;
    private SerializedProperty offsetZ;

    // ── Serialized Properties - Noise Decoration ─────────────────────
    private SerializedProperty decoNoiseScale;
    private SerializedProperty decoOffsetX;
    private SerializedProperty decoOffsetZ;

    // ── Serialized Properties - Thresholds ──────────────────────────────
    private SerializedProperty riverThreshold;
    private SerializedProperty pathThreshold;
    private SerializedProperty grassThreshold;

    // ── Serialized Properties - Densities ────────────────────────────
    private SerializedProperty treeDensity;
    private SerializedProperty rockDensity;
    private SerializedProperty plantDensity;

    // ── Serialized Properties - Prefabs ─────────────────────────────
    private SerializedProperty grassPrefabs;
    private SerializedProperty pathPrefabs;
    private SerializedProperty riverPrefabs;
    private SerializedProperty treePrefabs;
    private SerializedProperty rockPrefabs;
    private SerializedProperty plantPrefabs;
    private SerializedProperty stumpPrefabs;

    // ── Serialized Properties - Visuals ─────────────────────────────
    private SerializedProperty randomYRotation;
    private SerializedProperty scaleVariation;

    // ── State of the Wild ────────────────────────────────────────────
    private bool foldGrid = true;
    private bool foldNoise = true;
    private bool foldBiome = true;
    private bool foldDensity = true;
    private bool foldPrefabSol = true;
    private bool foldPrefabDec = true;
    private bool foldVisual = true;

    // ── Styles ───────────────────────────────────────────────────────
    private GUIStyle headerStyle;
    private GUIStyle sectionStyle;
    private bool stylesInitialized = false;

    // ─────────────────────────────────────────────────────────────────
    private void OnEnable()
    {
        // Grid
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");
        tileSize = serializedObject.FindProperty("tileSize");

        // Terrain Noise
        seed = serializedObject.FindProperty("seed");
        noiseScale = serializedObject.FindProperty("noiseScale");
        offsetX = serializedObject.FindProperty("offsetX");
        offsetZ = serializedObject.FindProperty("offsetZ");

        // Noise deco
        decoNoiseScale = serializedObject.FindProperty("decoNoiseScale");
        decoOffsetX = serializedObject.FindProperty("decoOffsetX");
        decoOffsetZ = serializedObject.FindProperty("decoOffsetZ");

        // Threshold
        riverThreshold = serializedObject.FindProperty("riverThreshold");
        pathThreshold = serializedObject.FindProperty("pathThreshold");
        grassThreshold = serializedObject.FindProperty("grassThreshold");

        // Densities
        treeDensity = serializedObject.FindProperty("treeDensity");
        rockDensity = serializedObject.FindProperty("rockDensity");
        plantDensity = serializedObject.FindProperty("plantDensity");

        // Prefabs ground
        grassPrefabs = serializedObject.FindProperty("grassPrefabs");
        pathPrefabs = serializedObject.FindProperty("pathPrefabs");
        riverPrefabs = serializedObject.FindProperty("riverPrefabs");

        // Prefabs decorations
        treePrefabs = serializedObject.FindProperty("treePrefabs");
        rockPrefabs = serializedObject.FindProperty("rockPrefabs");
        plantPrefabs = serializedObject.FindProperty("plantPrefabs");
        stumpPrefabs = serializedObject.FindProperty("stumpPrefabs");

        // Visuals
        randomYRotation = serializedObject.FindProperty("randomYRotation");
        scaleVariation = serializedObject.FindProperty("scaleVariation");
    }

    // ─────────────────────────────────────────────────────────────────
    private void InitStyles()
    {
        if (stylesInitialized) return;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter
        };

        sectionStyle = new GUIStyle(EditorStyles.foldoutHeader)
        {
            fontStyle = FontStyle.Bold
        };

        stylesInitialized = true;
    }

    // ─────────────────────────────────────────────────────────────────
    public override void OnInspectorGUI()
    {
        InitStyles();
        serializedObject.Update();

        var generator = (ProceduralTerrainGenerator)target;

        // ── Title ────────────────────────────────────────────────────
        DrawTitle();

        EditorGUILayout.Space(4);

        // ── Sections ─────────────────────────────────────────────────
        DrawGridSection();
        DrawNoiseSection();
        DrawBiomeSection();
        DrawDensitySection();
        DrawPrefabSolSection();
        DrawPrefabDecoSection();
        DrawVisualSection();

        EditorGUILayout.Space(8);

        // ── Action Button ─────────────────────────────────────────
        DrawActionButtons(generator);

        // ── Infos ────────────────────────────────────────────────────
        DrawInfoBox();

        serializedObject.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────
    //  SECTIONS
    // ─────────────────────────────────────────────────────────────────

    private void DrawTitle()
    {
        EditorGUILayout.Space(6);
        var rect = EditorGUILayout.GetControlRect(false, 32);
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
        EditorGUI.LabelField(rect, "Procedural Terrain Generator", headerStyle);
        EditorGUILayout.Space(2);
    }

    // ── Grid ───────────────────────────────────────────────────────
    private void DrawGridSection()
    {
        foldGrid = DrawFoldout(foldGrid, "Grid", new Color(0.2f, 0.4f, 0.6f));
        if (!foldGrid) return;

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(gridWidth, new GUIContent("Width (X)", "Number of tiles on X"));
        EditorGUILayout.PropertyField(gridHeight, new GUIContent("Height (Z)", "Number of tiles on Z"));
        EditorGUILayout.PropertyField(tileSize, new GUIContent("Tile size", "Unity unit size"));

        // Shows the calculated total size
        float totalX = gridWidth.intValue * tileSize.floatValue;
        float totalZ = gridHeight.intValue * tileSize.floatValue;
        EditorGUILayout.HelpBox(
            $"Total area : {totalX:0.#} × {totalZ:0.#} units  ({gridWidth.intValue * gridHeight.intValue} tiles)",
            MessageType.None);

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Noise ────────────────────────────────────────────────────────
    private void DrawNoiseSection()
    {
        foldNoise = DrawFoldout(foldNoise, "Noise", new Color(0.2f, 0.5f, 0.4f));
        if (!foldNoise) return;

        EditorGUI.indentLevel++;

        // Seed with button dice
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(seed, new GUIContent("Seed"));
        if (GUILayout.Button("Dice", GUILayout.Width(28)))
            seed.intValue = Random.Range(0, 99999);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Terrain", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(noiseScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(offsetX, new GUIContent("Offset X"));
        EditorGUILayout.PropertyField(offsetZ, new GUIContent("Offset Z"));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Decoration", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(decoNoiseScale, new GUIContent("Scale"));
        EditorGUILayout.PropertyField(decoOffsetX, new GUIContent("Offset X"));
        EditorGUILayout.PropertyField(decoOffsetZ, new GUIContent("Offset Z"));

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Biome thresholds ──────────────────────────────────────────────
    private void DrawBiomeSection()
    {
        foldBiome = DrawFoldout(foldBiome, "Biomes", new Color(0.3f, 0.55f, 0.35f));
        if (!foldBiome) return;

        EditorGUI.indentLevel++;

        float river = riverThreshold.floatValue;
        float path = pathThreshold.floatValue;

        EditorGUILayout.PropertyField(riverThreshold,
            new GUIContent("River Sill", " > this value -> water/river"));
        EditorGUILayout.PropertyField(pathThreshold,
            new GUIContent("Threshold Path", "between River and this threshold -> path"));

        // Validation
        if (path <= river)
        {
            EditorGUILayout.HelpBox("The Path threshold must be > River threshold.", MessageType.Warning);
            pathThreshold.floatValue = river + 0.01f;
        }

        EditorGUILayout.Space(4);

        // Biomes bar
        DrawBiomeBar(river, path);

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Decoration densities ───────────────────────────────────────
    private void DrawDensitySection()
    {
        foldDensity = DrawFoldout(foldDensity, "Decoration densities", new Color(0.45f, 0.35f, 0.2f));
        if (!foldDensity) return;

        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(treeDensity, new GUIContent("Trees"));
        EditorGUILayout.PropertyField(rockDensity, new GUIContent("Rocks"));
        EditorGUILayout.PropertyField(plantDensity, new GUIContent("Plants / Strains"));

        float total = treeDensity.floatValue + rockDensity.floatValue + plantDensity.floatValue;

        // Barre de densité
        DrawDensityBar(treeDensity.floatValue, rockDensity.floatValue, plantDensity.floatValue);

        if (total > 1f)
            EditorGUILayout.HelpBox($"Sum of densities = {total:0.00} > 1. The empty areas will be reduced.", MessageType.Warning);
        else
            EditorGUILayout.HelpBox($"Total density : {total:0.00} — Empty zones : {(1f - total):0.00}", MessageType.None);

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Prefabs grounds ─────────────────────────────────────────────────
    private void DrawPrefabSolSection()
    {
        foldPrefabSol = DrawFoldout(foldPrefabSol, "Prefabs – Grounds", new Color(0.25f, 0.45f, 0.25f));
        if (!foldPrefabSol) return;

        EditorGUI.indentLevel++;
        DrawPrefabList(grassPrefabs, "Grass", "ground_grass.prefab");
        DrawPrefabList(pathPrefabs, "Path", "ground_pathOpen, ground_pathTile…");
        DrawPrefabList(riverPrefabs, "River", "ground_riverOpen, ground_riverStraight…");
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Prefabs decorations ──────────────────────────────────────────
    private void DrawPrefabDecoSection()
    {
        foldPrefabDec = DrawFoldout(foldPrefabDec, "Prefabs – Decorations", new Color(0.35f, 0.5f, 0.25f));
        if (!foldPrefabDec) return;

        EditorGUI.indentLevel++;
        DrawPrefabList(treePrefabs, "Trees", "tree_default, tree_oak, tree_pine…");
        DrawPrefabList(rockPrefabs, "Rocks", "rock_largeA, stone_largeA…");
        DrawPrefabList(plantPrefabs, "Plants / Fleurs", "plant_bush, flower_redA, grass…");
        DrawPrefabList(stumpPrefabs, "Stumps / Logs", "stump_round, log…");
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Visuals ──────────────────────────────────────────────────────
    private void DrawVisualSection()
    {
        foldVisual = DrawFoldout(foldVisual, "Visual options", new Color(0.5f, 0.4f, 0.2f));
        if (!foldVisual) return;

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(randomYRotation, new GUIContent("Random Y rotation"));
        EditorGUILayout.PropertyField(scaleVariation, new GUIContent("Scale variation"));
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(4);
    }

    // ── Buttons ──────────────────────────────────────────────────────
    private void DrawActionButtons(ProceduralTerrainGenerator generator)
    {
        EditorGUILayout.BeginHorizontal();

        // Generate
        GUI.backgroundColor = new Color(0.3f, 0.75f, 0.35f);
        if (GUILayout.Button("Generate", GUILayout.Height(36)))
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Launch the game first.", MessageType.Info);
            }
            else
            {
                Undo.RecordObject(generator, "Generate Terrain");
                generator.Generate();
            }
        }

        // Delete
        GUI.backgroundColor = new Color(0.85f, 0.3f, 0.3f);
        if (GUILayout.Button("Delete", GUILayout.Height(36)))
        {
            Undo.RecordObject(generator, "Clear Terrain");
            generator.Clear();
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        // Random seed + regeneration
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.4f, 0.55f, 0.8f);
        if (GUILayout.Button("Random Seed + Generate", GUILayout.Height(28)))
        {
            seed.intValue = Random.Range(0, 99999);
            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying) generator.Generate();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Generate / Clear buttons require Play Mode.", MessageType.Info);
    }

    // ── Info box ─────────────────────────────────────────────────────
    private void DrawInfoBox()
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.HelpBox(
            "Tip: Drag several prefabs of the same type into a list to vary the look. " +
            "The script chooses randomly among the prefabs of each category.",
            MessageType.Info);
    }

    // ─────────────────────────────────────────────────────────────────
    //  HELPERS UI
    // ─────────────────────────────────────────────────────────────────

    // <summary>Draw a flyer with a colored bar on the left. </summary>
    private bool DrawFoldout(bool state, string label, Color accentColor)
    {
        var lineRect = EditorGUILayout.GetControlRect(false, 22);

        // Side color bar
        var accent = new Rect(lineRect.x, lineRect.y + 1, 3, lineRect.height - 2);
        EditorGUI.DrawRect(accent, accentColor);

        // Subtle background
        var bg = new Rect(lineRect.x + 4, lineRect.y, lineRect.width - 4, lineRect.height);
        EditorGUI.DrawRect(bg, new Color(accentColor.r, accentColor.g, accentColor.b, 0.08f));

        // Foldout
        var foldRect = new Rect(lineRect.x + 8, lineRect.y + 2, lineRect.width - 8, lineRect.height - 2);
        return EditorGUI.Foldout(foldRect, state, label, true, sectionStyle);
    }

    // <summary>Draw a list of prefabs with a help label. </summary>
    private void DrawPrefabList(SerializedProperty list, string label, string hint)
    {
        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        EditorGUI.indentLevel++;

        // Count and list
        EditorGUILayout.PropertyField(list, new GUIContent($"{label} ({list.arraySize})"), true);

        if (list.arraySize == 0)
            EditorGUILayout.HelpBox($"Aucun prefab. Ex : {hint}", MessageType.Warning);

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(2);
    }

    // <summary>Draws a horizontal bar representing the distribution of biomes. </summary>
    private void DrawBiomeBar(float river, float path)
    {
        var rect = EditorGUILayout.GetControlRect(false, 20);
        rect = EditorGUI.IndentedRect(rect);

        // River
        var r1 = new Rect(rect.x, rect.y, rect.width * river, rect.height);
        EditorGUI.DrawRect(r1, new Color(0.2f, 0.45f, 0.8f));
        if (r1.width > 30) DrawCenteredLabel(r1, "Water");

        // Path
        var r2 = new Rect(rect.x + r1.width, rect.y, rect.width * (path - river), rect.height);
        EditorGUI.DrawRect(r2, new Color(0.65f, 0.55f, 0.35f));
        if (r2.width > 30) DrawCenteredLabel(r2, "Path");

        // Grass
        var r3 = new Rect(rect.x + r1.width + r2.width, rect.y, rect.width * (1f - path), rect.height);
        EditorGUI.DrawRect(r3, new Color(0.3f, 0.65f, 0.3f));
        if (r3.width > 30) DrawCenteredLabel(r3, "Grass");
    }

    // <summary>Draw a horizontal bar representing the distribution of decorations. </summary>
    private void DrawDensityBar(float trees, float rocks, float plants)
    {
        var rect = EditorGUILayout.GetControlRect(false, 18);
        rect = EditorGUI.IndentedRect(rect);

        float total = Mathf.Min(trees + rocks + plants, 1f);

        DrawSegment(rect, 0f, trees, new Color(0.2f, 0.55f, 0.2f), "Tree");
        DrawSegment(rect, trees, trees + rocks, new Color(0.55f, 0.45f, 0.35f), "Rocks");
        DrawSegment(rect, trees + rocks, total, new Color(0.45f, 0.7f, 0.35f), "Grass");
        DrawSegment(rect, total, 1f, new Color(0.2f, 0.2f, 0.2f), "Empty");
    }

    private void DrawSegment(Rect full, float start, float end, Color color, string lbl)
    {
        float w = (end - start) * full.width;
        if (w <= 0f) return;
        var r = new Rect(full.x + start * full.width, full.y, w, full.height);
        EditorGUI.DrawRect(r, color);
        if (w > 20) DrawCenteredLabel(r, lbl);
    }

    private void DrawCenteredLabel(Rect rect, string text)
    {
        var style = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
        EditorGUI.LabelField(rect, text, style);
    }
}