using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.Rooms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ADoorInsideTheDark.Editor
{
    public static class Wave007BHouseWithTheSwitchesSetup
    {
        private const string MenuPath = "ADITD/Setup/Wave 007B House With the Switches";
        private const string ScenePath = "Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";

        [MenuItem(MenuPath)]
        public static void CreateOrRefreshScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject roomRoot = new("Wave007B_HouseWithTheSwitches");
            EditorSceneManager.MoveGameObjectToScene(roomRoot, scene);

            ConfigureSceneLighting(scene);
            BuildRoom(roomRoot.transform, out HouseWithTheSwitchesController controller);
            BuildPlayer(scene);
            ConfigureController(controller);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log(
                $"[Wave007B Setup] Created or refreshed '{ScenePath}'. This setup only overwrites the dedicated Wave 007B graybox scene.",
                roomRoot);
        }

        private static void ConfigureSceneLighting(Scene scene)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.14f, 0.14f, 0.16f, 1f);

            GameObject directionalLightGo = new("Directional Light");
            EditorSceneManager.MoveGameObjectToScene(directionalLightGo, scene);

            Light directionalLight = directionalLightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 0.18f;
            directionalLight.color = new Color(0.80f, 0.82f, 0.88f, 1f);
            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void BuildRoom(Transform parent, out HouseWithTheSwitchesController controller)
        {
            GameObject floor = CreatePrimitive(parent, "Floor", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(7.5f, 0.25f, 7.5f));
            GameObject ceiling = CreatePrimitive(parent, "Ceiling", PrimitiveType.Cube, new Vector3(0f, 3f, 0f), new Vector3(7.5f, 0.25f, 7.5f));
            GameObject backWall = CreatePrimitive(parent, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, 3.75f), new Vector3(7.5f, 3f, 0.25f));
            GameObject frontWall = CreatePrimitive(parent, "FrontWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, -3.75f), new Vector3(7.5f, 3f, 0.25f));
            GameObject leftWall = CreatePrimitive(parent, "LeftWall", PrimitiveType.Cube, new Vector3(-3.75f, 1.5f, 0f), new Vector3(0.25f, 3f, 7.5f));
            GameObject rightWall = CreatePrimitive(parent, "RightWall", PrimitiveType.Cube, new Vector3(3.75f, 1.5f, 0f), new Vector3(0.25f, 3f, 7.5f));
            GameObject safePocket = CreatePrimitive(parent, "SafePocket", PrimitiveType.Cube, new Vector3(0f, 0.14f, -2.55f), new Vector3(2.2f, 0.03f, 1.6f));

            safePocket.GetComponent<Collider>().enabled = false;

            GameObject switchRoot = new("OrdinarySwitch");
            switchRoot.transform.SetParent(parent, false);
            switchRoot.transform.localPosition = new Vector3(0f, 1.4f, 3.55f);

            GameObject switchPlate = CreatePrimitive(switchRoot.transform, "SwitchPlate", PrimitiveType.Cube, Vector3.zero, new Vector3(0.36f, 0.56f, 0.06f));
            GameObject switchHandle = CreatePrimitive(switchRoot.transform, "SwitchHandle", PrimitiveType.Cube, new Vector3(0f, 0f, -0.05f), new Vector3(0.10f, 0.30f, 0.08f));

            Collider plateCollider = switchPlate.GetComponent<Collider>();
            if (plateCollider != null)
            {
                Object.DestroyImmediate(plateCollider);
            }

            HouseSwitchInteractable interactable = switchHandle.AddComponent<HouseSwitchInteractable>();

            GameObject seamRoot = new("HiddenSeamRoot");
            seamRoot.transform.SetParent(parent, false);
            seamRoot.transform.localPosition = Vector3.zero;
            CreatePrimitive(seamRoot.transform, "SeamVertical", PrimitiveType.Cube, new Vector3(0f, 1.46f, 3.60f), new Vector3(0.04f, 0.90f, 0.04f));
            CreatePrimitive(seamRoot.transform, "SeamTop", PrimitiveType.Cube, new Vector3(0f, 1.88f, 3.60f), new Vector3(0.58f, 0.04f, 0.04f));
            CreatePrimitive(seamRoot.transform, "SeamBottom", PrimitiveType.Cube, new Vector3(0f, 1.03f, 3.60f), new Vector3(0.58f, 0.04f, 0.04f));
            CreatePrimitive(seamRoot.transform, "SeamPath", PrimitiveType.Cube, new Vector3(0f, 0.11f, 1.45f), new Vector3(0.08f, 0.03f, 4.40f));
            SetCollidersEnabled(seamRoot, false);

            GameObject completionMarker = CreatePrimitive(parent, "CompletionMarker", PrimitiveType.Cube, new Vector3(0f, 1.5f, -3.45f), new Vector3(1.4f, 2.2f, 0.12f));
            completionMarker.GetComponent<Collider>().enabled = false;

            GameObject overheadLightGo = new("OverheadLight");
            overheadLightGo.transform.SetParent(parent, false);
            overheadLightGo.transform.localPosition = new Vector3(0f, 2.55f, 0f);
            Light overheadLight = overheadLightGo.AddComponent<Light>();
            overheadLight.type = LightType.Point;
            overheadLight.range = 12f;
            overheadLight.shadows = LightShadows.Soft;

            controller = roomRootAddController(parent.gameObject);

            SerializedObject interactableSo = new(interactable);
            interactableSo.FindProperty("_controller").objectReferenceValue = controller;
            interactableSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new(controller);
            controllerSo.FindProperty("_overheadLight").objectReferenceValue = overheadLight;
            controllerSo.FindProperty("_switchRenderer").objectReferenceValue = switchHandle.GetComponent<Renderer>();
            controllerSo.FindProperty("_switchHandle").objectReferenceValue = switchHandle.transform;
            controllerSo.FindProperty("_hiddenSeamRoot").objectReferenceValue = seamRoot;
            controllerSo.FindProperty("_completionMarker").objectReferenceValue = completionMarker;
            SetObjectArray(
                controllerSo.FindProperty("_roomRenderers"),
                floor.GetComponent<Renderer>(),
                ceiling.GetComponent<Renderer>(),
                backWall.GetComponent<Renderer>(),
                frontWall.GetComponent<Renderer>(),
                leftWall.GetComponent<Renderer>(),
                rightWall.GetComponent<Renderer>(),
                safePocket.GetComponent<Renderer>(),
                switchPlate.GetComponent<Renderer>());
            SetObjectArray(
                controllerSo.FindProperty("_hiddenSeamRenderers"),
                seamRoot.GetComponentsInChildren<Renderer>());
            SetObjectArray(
                controllerSo.FindProperty("_completionRenderers"),
                completionMarker.GetComponent<Renderer>());
            controllerSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static HouseWithTheSwitchesController roomRootAddController(GameObject roomRoot)
        {
            HouseWithTheSwitchesController controller = roomRoot.GetComponent<HouseWithTheSwitchesController>();
            if (controller == null)
            {
                controller = roomRoot.AddComponent<HouseWithTheSwitchesController>();
            }

            return controller;
        }

        private static void BuildPlayer(Scene scene)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null)
            {
                Debug.LogError($"[Wave007B Setup] Missing player prefab at '{PlayerPrefabPath}'.");
                return;
            }

            GameObject playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab, scene) as GameObject;
            if (playerInstance == null)
            {
                Debug.LogError("[Wave007B Setup] Failed to instantiate player prefab.");
                return;
            }

            playerInstance.name = "Player";
            playerInstance.transform.position = new Vector3(0f, 0.05f, -2.6f);
            playerInstance.transform.rotation = Quaternion.identity;
        }

        private static void ConfigureController(HouseWithTheSwitchesController controller)
        {
            if (controller == null)
            {
                return;
            }

            SerializedObject controllerSo = new(controller);
            controllerSo.FindProperty("_showDebugOverlay").boolValue = true;
            controllerSo.FindProperty("_shadowPlaceholderControlLabel").stringValue = "Hold Q to let the glare drop";
            controllerSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreatePrimitive(
            Transform parent,
            string name,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale)
        {
            GameObject go = GameObject.CreatePrimitive(primitiveType);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            return go;
        }

        private static void SetCollidersEnabled(GameObject root, bool enabled)
        {
            foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = enabled;
            }
        }

        private static void SetObjectArray(SerializedProperty arrayProperty, params Object[] values)
        {
            if (arrayProperty == null)
            {
                return;
            }

            arrayProperty.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }
    }
}
