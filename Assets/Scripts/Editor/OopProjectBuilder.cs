using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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

        EditorSceneManager.SaveScene(scene, TitleScenePath);
    }

    private static void CreateGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "CreatureLab";

        CreateCamera(new Vector3(0f, 5.8f, -9f), Quaternion.Euler(34f, 0f, 0f));
        CreateLight();
        CreateGround();

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

    private static void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "CarePlatform";
        ground.transform.position = new Vector3(0f, -0.05f, 0f);
        ground.transform.localScale = new Vector3(9f, 0.1f, 5f);
        ground.GetComponent<Renderer>().sharedMaterial = CreateMaterial("CarePlatform_Material", new Color(0.18f, 0.18f, 0.2f));
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
