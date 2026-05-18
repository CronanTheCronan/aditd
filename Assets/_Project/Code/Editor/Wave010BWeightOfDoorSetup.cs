using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.Rooms;
using ADoorInsideTheDark.Shadow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ADoorInsideTheDark.Editor
{
    public static class Wave010BWeightOfDoorSetup
    {
        private const string MenuPath = "ADITD/Setup/Recreate Wave 010B Weight of the Door";
        private const string ScenePath = "Assets/_Project/Scenes/Wave010B_WeightOfTheDoor.unity";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";

        [MenuItem(MenuPath)]
        public static void CreateOrRecreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("[Wave010B Setup] Scene recreation canceled by user before replacing the active scene.");
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject roomRoot = new("Wave010B_WeightOfTheDoor");
            EditorSceneManager.MoveGameObjectToScene(roomRoot, scene);

            ConfigureSceneLighting(scene);
            GameObject player = BuildPlayer(scene);
            ShadowPerceptionController shadowPerception = AddOrGetShadowPerceptionController(player);
            BuildRoom(roomRoot.transform, shadowPerception, out WeightOfDoorController controller);
            WireController(controller);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log(
                $"[Wave010B Setup] Created or recreated '{ScenePath}'. Graybox for room.main_floor.weight_of_door.",
                roomRoot);
        }

        private static void ConfigureSceneLighting(Scene scene)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.12f, 0.12f, 0.14f, 1f);

            GameObject directionalLightGo = new("Directional Light");
            EditorSceneManager.MoveGameObjectToScene(directionalLightGo, scene);

            Light directionalLight = directionalLightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 0.16f;
            directionalLight.color = new Color(0.78f, 0.80f, 0.86f, 1f);
            directionalLight.transform.rotation = Quaternion.Euler(58f, -25f, 0f);
        }

        private static void BuildRoom(
            Transform parent,
            ShadowPerceptionController shadowPerception,
            out WeightOfDoorController controller)
        {
            GameObject floor = CreatePrimitive(parent, "Floor", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(3f, 0.25f, 8f));
            GameObject ceiling = CreatePrimitive(parent, "Ceiling", PrimitiveType.Cube, new Vector3(0f, 3f, 0f), new Vector3(3f, 0.25f, 8f));
            GameObject backWall = CreatePrimitive(parent, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, -4f), new Vector3(3f, 3f, 0.25f));
            GameObject frontWall = CreatePrimitive(parent, "FrontWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, 4f), new Vector3(3f, 3f, 0.25f));
            GameObject leftWall = CreatePrimitive(parent, "LeftWall", PrimitiveType.Cube, new Vector3(-1.5f, 1.5f, 0f), new Vector3(0.25f, 3f, 8f));
            GameObject rightWall = CreatePrimitive(parent, "RightWall", PrimitiveType.Cube, new Vector3(1.5f, 1.5f, 0f), new Vector3(0.25f, 3f, 8f));

            GameObject runner = CreatePrimitive(parent, "RunnerRug", PrimitiveType.Cube, new Vector3(0f, 0.04f, 0f), new Vector3(1.15f, 0.02f, 6.5f));
            SetRendererColor(runner.GetComponent<Renderer>(), new Color(0.32f, 0.30f, 0.28f, 1f));

            GameObject pocketRug = CreatePrimitive(parent, "SafePocketMat", PrimitiveType.Cube, new Vector3(-1.05f, 0.03f, -3.35f), new Vector3(1.4f, 0.02f, 1.5f));
            SetRendererColor(pocketRug.GetComponent<Renderer>(), new Color(0.26f, 0.28f, 0.30f, 1f));
            pocketRug.GetComponent<Collider>().enabled = false;

            GameObject sideTable = CreatePrimitive(parent, "SideTable", PrimitiveType.Cube, new Vector3(-1.1f, 0.35f, -3.2f), new Vector3(0.55f, 0.35f, 0.45f));
            SetRendererColor(sideTable.GetComponent<Renderer>(), new Color(0.36f, 0.33f, 0.30f, 1f));

            GameObject doorPanel = CreatePrimitive(parent, "HeavyDoor", PrimitiveType.Cube, new Vector3(0f, 1.1f, 3.74f), new Vector3(1.15f, 2.15f, 0.12f));
            SetRendererColor(doorPanel.GetComponent<Renderer>(), new Color(0.38f, 0.36f, 0.34f, 1f));
            WeightOfDoorInteractable doorInteractable = doorPanel.AddComponent<WeightOfDoorInteractable>();

            GameObject bindingsRoot = new("DoorBindingsRoot");
            bindingsRoot.transform.SetParent(parent, false);
            bindingsRoot.transform.localPosition = new Vector3(0f, 1.1f, 3.8f);
            CreatePrimitive(bindingsRoot.transform, "BindingArcA", PrimitiveType.Cube, new Vector3(0.48f, 0.32f, -0.06f), new Vector3(0.12f, 0.12f, 0.28f));
            CreatePrimitive(bindingsRoot.transform, "BindingArcB", PrimitiveType.Cube, new Vector3(-0.42f, -0.25f, -0.04f), new Vector3(0.14f, 0.10f, 0.35f));
            CreatePrimitive(bindingsRoot.transform, "BindingArcC", PrimitiveType.Cube, new Vector3(0.1f, 0.58f, -0.02f), new Vector3(0.45f, 0.08f, 0.12f));
            CreatePrimitive(bindingsRoot.transform, "BindingKnot", PrimitiveType.Sphere, new Vector3(0f, 0f, 0.08f), new Vector3(0.22f, 0.22f, 0.18f));
            CreatePrimitive(bindingsRoot.transform, "FloorStrap", PrimitiveType.Cube, new Vector3(0.15f, -0.78f, -0.08f), new Vector3(0.08f, 0.05f, 0.65f));
            SetCollidersEnabled(bindingsRoot, false);

            ShadowRevealable revealable = bindingsRoot.AddComponent<ShadowRevealable>();
            AudioSource bindingsAudio = bindingsRoot.AddComponent<AudioSource>();
            ConfigureRevealAudioSource(bindingsAudio);

            GameObject thermos = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thermos.name = "GreenThermos";
            thermos.transform.SetParent(parent, false);
            thermos.transform.localPosition = new Vector3(-1.05f, 0.58f, -3.18f);
            thermos.transform.localScale = new Vector3(0.12f, 0.14f, 0.12f);
            thermos.transform.localRotation = Quaternion.Euler(8f, 0f, 2f);
            SetRendererColor(thermos.GetComponent<Renderer>(), new Color(0.22f, 0.56f, 0.38f, 1f));

            GameObject snowOwl = GameObject.CreatePrimitive(PrimitiveType.Cube);
            snowOwl.name = "SnowOwlFeatherShimmer";
            snowOwl.transform.SetParent(parent, false);
            snowOwl.transform.localPosition = new Vector3(-0.78f, 0.82f, -3.38f);
            snowOwl.transform.localScale = new Vector3(0.08f, 0.02f, 0.26f);
            snowOwl.transform.localRotation = Quaternion.Euler(12f, 20f, -5f);
            snowOwl.GetComponent<Collider>().enabled = false;
            SetRendererColor(snowOwl.GetComponent<Renderer>(), new Color(0.92f, 0.93f, 0.95f, 1f));

            GameObject overheadLightGo = new("OverheadLight");
            overheadLightGo.transform.SetParent(parent, false);
            overheadLightGo.transform.localPosition = new Vector3(0f, 2.65f, 0f);
            Light overheadLight = overheadLightGo.AddComponent<Light>();
            overheadLight.type = LightType.Point;
            overheadLight.range = 14f;
            overheadLight.shadows = LightShadows.Soft;

            AudioSource roomClarityAudio = parent.gameObject.AddComponent<AudioSource>();
            ConfigureClarityAudioSource(roomClarityAudio);

            controller = AddOrGetController(parent.gameObject);

            SerializedObject interactableSo = new(doorInteractable);
            interactableSo.FindProperty("_controller").objectReferenceValue = controller;
            interactableSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject controllerSo = new(controller);
            controllerSo.FindProperty("_overheadLight").objectReferenceValue = overheadLight;
            controllerSo.FindProperty("_doorPanelRenderer").objectReferenceValue = doorPanel.GetComponent<Renderer>();
            controllerSo.FindProperty("_doorPanelTransform").objectReferenceValue = doorPanel.transform;
            controllerSo.FindProperty("_bindingsRoot").objectReferenceValue = bindingsRoot;
            controllerSo.FindProperty("_bindingsRevealable").objectReferenceValue = revealable;
            controllerSo.FindProperty("_perceptionSource").objectReferenceValue = shadowPerception;
            controllerSo.FindProperty("_clarityAudioSource").objectReferenceValue = roomClarityAudio;
            controllerSo.FindProperty("_thermosRoot").objectReferenceValue = thermos.transform;
            controllerSo.FindProperty("_thermosCompletedLocalPosition").vector3Value = new Vector3(-1.15f, 0.06f, -3.45f);
            controllerSo.FindProperty("_snowOwlMarker").objectReferenceValue = snowOwl;

            SetObjectArray(
                controllerSo.FindProperty("_hallRenderers"),
                floor.GetComponent<Renderer>(),
                ceiling.GetComponent<Renderer>(),
                backWall.GetComponent<Renderer>(),
                frontWall.GetComponent<Renderer>(),
                leftWall.GetComponent<Renderer>(),
                rightWall.GetComponent<Renderer>(),
                runner.GetComponent<Renderer>(),
                pocketRug.GetComponent<Renderer>(),
                sideTable.GetComponent<Renderer>());
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject revealableSo = new(revealable);
            revealableSo.FindProperty("_perceptionSource").objectReferenceValue = shadowPerception;
            SetObjectArray(revealableSo.FindProperty("_objectsToToggle"), System.Array.Empty<Object>());
            SetObjectArray(
                revealableSo.FindProperty("_renderersToToggleVisibility"),
                bindingsRoot.GetComponentsInChildren<Renderer>());
            SetObjectArray(revealableSo.FindProperty("_renderersToTint"), System.Array.Empty<Object>());
            revealableSo.FindProperty("_revealAudioSource").objectReferenceValue = bindingsAudio;
            revealableSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireController(WeightOfDoorController controller)
        {
            if (controller == null)
            {
                return;
            }

            SerializedObject controllerSo = new(controller);
            controllerSo.FindProperty("_showDebugOverlay").boolValue = true;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static WeightOfDoorController AddOrGetController(GameObject roomRoot)
        {
            WeightOfDoorController controller = roomRoot.GetComponent<WeightOfDoorController>();
            if (controller == null)
            {
                controller = roomRoot.AddComponent<WeightOfDoorController>();
            }

            return controller;
        }

        private static GameObject BuildPlayer(Scene scene)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null)
            {
                Debug.LogError($"[Wave010B Setup] Missing player prefab at '{PlayerPrefabPath}'.");
                return null;
            }

            GameObject playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab, scene) as GameObject;
            if (playerInstance == null)
            {
                Debug.LogError("[Wave010B Setup] Failed to instantiate player prefab.");
                return null;
            }

            playerInstance.name = "Player";
            playerInstance.transform.position = new Vector3(0f, 0.2f, -3.1f);
            playerInstance.transform.rotation = Quaternion.identity;
            return playerInstance;
        }

        private static ShadowPerceptionController AddOrGetShadowPerceptionController(GameObject player)
        {
            if (player == null)
            {
                return null;
            }

            ShadowPerceptionController perception = player.GetComponent<ShadowPerceptionController>();
            if (perception == null)
            {
                perception = player.AddComponent<ShadowPerceptionController>();
            }

            AudioSource perceptionAudioSource = player.GetComponent<AudioSource>();
            if (perceptionAudioSource == null)
            {
                perceptionAudioSource = player.AddComponent<AudioSource>();
            }

            ConfigurePerceptionAudioSource(perceptionAudioSource);

            SerializedObject controllerSo = new(perception);
            controllerSo.FindProperty("_perceptionAudioSource").objectReferenceValue = perceptionAudioSource;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            return perception;
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

        private static void ConfigurePerceptionAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }

        private static void ConfigureRevealAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0f;
        }

        private static void ConfigureClarityAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }

        private static void SetRendererColor(Renderer renderer, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            Material material = new Material(renderer.sharedMaterial);
            material.color = color;
            renderer.sharedMaterial = material;
        }
    }
}
