using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class OopProjectBuilder
{
    private const string SceneFolder = "Assets/Scenes";
    private const string MaterialFolder = "Assets/Materials";
    private const string TitleScenePath = SceneFolder + "/Title.unity";
    private const string GameScenePath = SceneFolder + "/CreatureLab.unity";

    [MenuItem("Tools/OOP Project/Rebuild Scenes")]
    public static void BuildProject()
    {
        Directory.CreateDirectory(SceneFolder);
        Directory.CreateDirectory(MaterialFolder);

        CreateTitleScene();
        CreateGameScene();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(TitleScenePath, true),
            new EditorBuildSettingsScene(GameScenePath, true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("OOP Programming Theory project scenes rebuilt.");
    }

    private static void CreateTitleScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Title";

        CreateCamera(new Vector3(0f, 4f, -8f), Quaternion.Euler(24f, 0f, 0f));
        CreateLight();
        CreateTitleSet();

        Canvas canvas = CreateCanvas("TitleCanvas");
        Text title = CreateText(canvas.transform, "TitleText", "Creature Care Lab", 46, TextAnchor.MiddleCenter);
        SetRect(title.rectTransform, new Vector2(0.12f, 0.56f), new Vector2(0.88f, 0.84f), Vector2.zero, Vector2.zero);

        Text subtitle = CreateText(canvas.transform, "SubtitleText", "Junior Programmer - Programming Theory in Action", 22, TextAnchor.MiddleCenter);
        SetRect(subtitle.rectTransform, new Vector2(0.16f, 0.45f), new Vector2(0.84f, 0.58f), Vector2.zero, Vector2.zero);

        TitleScreen controller = new GameObject("TitleScreenController").AddComponent<TitleScreen>();
        Button startButton = CreateButton(canvas.transform, "StartButton", "Start", new Vector2(0.5f, 0.32f), new Vector2(220f, 56f));
        startButton.onClick.AddListener(controller.StartGame);

        Button quitButton = CreateButton(canvas.transform, "QuitButton", "Quit", new Vector2(0.5f, 0.21f), new Vector2(220f, 56f));
        quitButton.onClick.AddListener(controller.QuitGame);

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, TitleScenePath);
    }

    private static void CreateGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "CreatureLab";

        CreateCamera(new Vector3(0f, 5.8f, -9f), Quaternion.Euler(34f, 0f, 0f));
        CreateLight();
        CreateLabSet();

        CreatureSelector forest = CreateCreature<ForestCreature>(PrimitiveType.Capsule, "Mossling", new Vector3(-3f, 1f, 0f), 64, 52);
        CreatureSelector crystal = CreateCreature<CrystalCreature>(PrimitiveType.Cube, "Glint", new Vector3(0f, 1f, 0f), 54, 67);
        CreatureSelector ember = CreateCreature<EmberCreature>(PrimitiveType.Sphere, "Cinder", new Vector3(3f, 1f, 0f), 70, 44);

        Canvas canvas = CreateCanvas("GameCanvas");
        Text title = CreateText(canvas.transform, "TitleText", string.Empty, 26, TextAnchor.UpperLeft);
        SetRect(title.rectTransform, new Vector2(0.03f, 0.82f), new Vector2(0.7f, 0.97f), Vector2.zero, Vector2.zero);

        Text creature = CreateText(canvas.transform, "CreatureText", string.Empty, 22, TextAnchor.UpperLeft);
        SetRect(creature.rectTransform, new Vector2(0.03f, 0.55f), new Vector2(0.38f, 0.8f), Vector2.zero, Vector2.zero);

        Text status = CreateText(canvas.transform, "StatusText", string.Empty, 20, TextAnchor.LowerLeft);
        SetRect(status.rectTransform, new Vector2(0.03f, 0.04f), new Vector2(0.95f, 0.22f), Vector2.zero, Vector2.zero);

        CreatureCareGame game = new GameObject("CreatureCareGame").AddComponent<CreatureCareGame>();
        SerializedObject serialized = new SerializedObject(game);
        SerializedProperty creatures = serialized.FindProperty("creatures");
        creatures.arraySize = 3;
        creatures.GetArrayElementAtIndex(0).objectReferenceValue = forest;
        creatures.GetArrayElementAtIndex(1).objectReferenceValue = crystal;
        creatures.GetArrayElementAtIndex(2).objectReferenceValue = ember;
        serialized.FindProperty("titleText").objectReferenceValue = title;
        serialized.FindProperty("creatureText").objectReferenceValue = creature;
        serialized.FindProperty("statusText").objectReferenceValue = status;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, GameScenePath);
    }

    private static CreatureSelector CreateCreature<T>(PrimitiveType primitive, string creatureName, Vector3 position, int energy, int mood)
        where T : Creature
    {
        GameObject obj = GameObject.CreatePrimitive(primitive);
        obj.name = creatureName;
        obj.transform.position = position;

        T creature = obj.AddComponent<T>();
        creature.Initialize(creatureName, energy, mood);

        CreatureSelector selector = obj.AddComponent<CreatureSelector>();
        SerializedObject serialized = new SerializedObject(selector);
        serialized.FindProperty("creature").objectReferenceValue = creature;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        Renderer renderer = obj.GetComponent<Renderer>();
        renderer.sharedMaterial = CreateMaterial(creatureName + "_Material", creature.DisplayColor);
        AddCreatureVisualDetails(obj, primitive, creature.DisplayColor);
        CreatePedestal(creatureName + "_Pedestal", position + new Vector3(0f, -0.56f, 0f), creature.DisplayColor);
        return selector;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void CreateTitleSet()
    {
        GameObject floor = CreateBox("TitleFloor", new Vector3(0f, -0.08f, 1.2f), new Vector3(10f, 0.12f, 6f), CreateMaterial("Floor_Material", new Color(0.13f, 0.15f, 0.18f)));
        floor.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        CreateBox("BackPanel", new Vector3(0f, 1.8f, 2.9f), new Vector3(8.5f, 3.2f, 0.16f), CreateMaterial("BackPanel_Material", new Color(0.11f, 0.16f, 0.2f)));
        CreateBox("Console", new Vector3(0f, 0.35f, 0.85f), new Vector3(4.8f, 0.26f, 0.65f), CreateMaterial("Console_Material", new Color(0.2f, 0.32f, 0.36f)));
        CreateAccentLight("TitleAccentLeft", new Vector3(-2.1f, 0.62f, 0.45f), new Color(0.2f, 0.75f, 0.32f));
        CreateAccentLight("TitleAccentMiddle", new Vector3(0f, 0.62f, 0.45f), new Color(0.35f, 0.85f, 1f));
        CreateAccentLight("TitleAccentRight", new Vector3(2.1f, 0.62f, 0.45f), new Color(1f, 0.42f, 0.18f));
    }

    private static void CreateLabSet()
    {
        CreateBox("CarePlatform", new Vector3(0f, -0.05f, 0f), new Vector3(9f, 0.1f, 5f), CreateMaterial("CarePlatform_Material", new Color(0.18f, 0.18f, 0.2f)));
        CreateBox("LabBackWall", new Vector3(0f, 1.55f, 2.55f), new Vector3(9f, 3.2f, 0.16f), CreateMaterial("LabBackWall_Material", new Color(0.11f, 0.15f, 0.2f)));
        CreateBox("StatusRail", new Vector3(0f, 0.12f, 2.2f), new Vector3(7.8f, 0.18f, 0.18f), CreateMaterial("StatusRail_Material", new Color(0.18f, 0.35f, 0.38f)));
        CreateAccentLight("ForestStationLight", new Vector3(-3f, 0.14f, -0.9f), new Color(0.2f, 0.75f, 0.32f));
        CreateAccentLight("CrystalStationLight", new Vector3(0f, 0.14f, -0.9f), new Color(0.35f, 0.85f, 1f));
        CreateAccentLight("EmberStationLight", new Vector3(3f, 0.14f, -0.9f), new Color(1f, 0.42f, 0.18f));
    }

    private static void AddCreatureVisualDetails(GameObject root, PrimitiveType primitive, Color color)
    {
        switch (primitive)
        {
            case PrimitiveType.Capsule:
                root.transform.localScale = new Vector3(0.72f, 1.08f, 0.72f);
                AddChildPrimitive(root.transform, "LeafCrown", PrimitiveType.Sphere, new Vector3(0f, 0.78f, 0f), new Vector3(0.62f, 0.22f, 0.62f), CreateMaterial("Mossling_Leaf_Material", new Color(0.12f, 0.48f, 0.2f)));
                AddChildPrimitive(root.transform, "SeedGlow", PrimitiveType.Sphere, new Vector3(0f, 0.2f, -0.42f), new Vector3(0.18f, 0.18f, 0.18f), CreateMaterial("Mossling_Glow_Material", new Color(0.95f, 0.85f, 0.25f)));
                break;
            case PrimitiveType.Cube:
                root.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
                root.transform.localScale = new Vector3(0.92f, 0.92f, 0.92f);
                AddChildPrimitive(root.transform, "CrystalShardLeft", PrimitiveType.Cube, new Vector3(-0.42f, 0.55f, 0f), new Vector3(0.22f, 0.78f, 0.22f), CreateMaterial("Glint_Shard_Material", new Color(0.78f, 0.95f, 1f)));
                AddChildPrimitive(root.transform, "CrystalShardRight", PrimitiveType.Cube, new Vector3(0.42f, 0.42f, 0f), new Vector3(0.2f, 0.62f, 0.2f), CreateMaterial("Glint_Shard_Dark_Material", new Color(0.16f, 0.55f, 0.72f)));
                break;
            case PrimitiveType.Sphere:
                root.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                AddChildPrimitive(root.transform, "EmberCore", PrimitiveType.Sphere, new Vector3(0f, 0f, -0.45f), new Vector3(0.45f, 0.45f, 0.45f), CreateMaterial("Cinder_Core_Material", new Color(1f, 0.78f, 0.2f)));
                AddChildPrimitive(root.transform, "FlameCrest", PrimitiveType.Capsule, new Vector3(0f, 0.68f, 0f), new Vector3(0.3f, 0.52f, 0.3f), CreateMaterial("Cinder_Flame_Material", new Color(1f, 0.22f, 0.08f)));
                break;
        }
    }

    private static void CreatePedestal(string name, Vector3 position, Color accent)
    {
        Material baseMaterial = CreateMaterial(name + "_Base_Material", new Color(0.22f, 0.24f, 0.28f));
        Material accentMaterial = CreateMaterial(name + "_Accent_Material", accent * 0.7f + Color.white * 0.3f);

        GameObject baseObj = CreateBox(name, position, new Vector3(1.55f, 0.18f, 1.55f), baseMaterial);
        GameObject inset = CreateBox(name + "_Inset", position + new Vector3(0f, 0.12f, 0f), new Vector3(1.05f, 0.08f, 1.05f), accentMaterial);
        inset.transform.SetParent(baseObj.transform, true);
    }

    private static GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    private static GameObject AddChildPrimitive(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(primitive);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    private static void CreateAccentLight(string name, Vector3 position, Color color)
    {
        GameObject lightObject = new GameObject(name, typeof(Light));
        lightObject.transform.position = position;
        Light light = lightObject.GetComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = 2.1f;
        light.range = 3f;
    }

    private static Camera CreateCamera(Vector3 position, Quaternion rotation)
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetPositionAndRotation(position, rotation);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.1f, 0.13f);
        return camera;
    }

    private static void CreateLight()
    {
        GameObject lightObject = new GameObject("Directional Light", typeof(Light));
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        Light light = lightObject.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private static Canvas CreateCanvas(string name)
    {
        GameObject canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static Text CreateText(Transform parent, string name, string value, int size, TextAnchor anchor)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(parent, false);
        Text text = obj.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.24f, 0.62f, 0.88f);

        Button button = obj.GetComponent<Button>();
        button.targetGraphic = image;

        Text text = CreateText(obj.transform, "Label", label, 24, TextAnchor.MiddleCenter);
        SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 6f), new Vector2(-12f, -6f));
        return button;
    }

    private static void SetRect(RectTransform rect, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
