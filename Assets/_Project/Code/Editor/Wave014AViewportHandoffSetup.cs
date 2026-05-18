using ADoorInsideTheDark.Rooms;
using ADoorInsideTheDark.Shadow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ADoorInsideTheDark.Editor
{
    public static class Wave014AViewportHandoffSetup
    {
        private const string MenuPath = "ADITD/Setup/Recreate Wave 014A Viewport Handoff";
        private const string ScenePath = "Assets/_Project/Scenes/Wave014A_ViewportHandoff.unity";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";
        private const string ControlsAssetPath = "Assets/_Project/Settings/InputActions/Resources/ADITDControls.inputactions";

        private static readonly Color AmberProxyColor = new(0.92f, 0.62f, 0.18f, 1f);
        private static readonly Color ShadowProxyColor = new(0.10f, 0.12f, 0.22f, 1f);

        [MenuItem(MenuPath)]
        public static void CreateOrRecreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("[Wave014A Setup] Scene recreation canceled by user before replacing the active scene.");
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject roomRoot = new("Wave014A_ViewportHandoff");
            EditorSceneManager.MoveGameObjectToScene(roomRoot, scene);

            ConfigureSceneLighting(scene);
            GameObject player = BuildPlayer(scene);
            DisableShadowPerceptionController(player);
            BuildRoom(roomRoot.transform, out GameObject shadowAnchor, out GameObject egoProxy, out GameObject shadowProxy);
            LocalViewportHandoff handoff = AddOrGetHandoff(roomRoot);
            WireHandoff(handoff, shadowAnchor.transform, egoProxy, shadowProxy);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log(
                $"[Wave014A Setup] Created or recreated '{ScenePath}'. Graybox for room.main_floor.house_with_the_switches viewport handoff prototype.",
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

        private static void BuildRoom(
            Transform parent,
            out GameObject shadowAnchor,
            out GameObject egoProxy,
            out GameObject shadowProxy)
        {
            CreatePrimitive(parent, "Floor", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(7.5f, 0.25f, 7.5f));
            CreatePrimitive(parent, "Ceiling", PrimitiveType.Cube, new Vector3(0f, 3f, 0f), new Vector3(7.5f, 0.25f, 7.5f));
            CreatePrimitive(parent, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, 3.75f), new Vector3(7.5f, 3f, 0.25f));
            CreatePrimitive(parent, "FrontWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, -3.75f), new Vector3(7.5f, 3f, 0.25f));
            CreatePrimitive(parent, "LeftWall", PrimitiveType.Cube, new Vector3(-3.75f, 1.5f, 0f), new Vector3(0.25f, 3f, 7.5f));
            CreatePrimitive(parent, "RightWall", PrimitiveType.Cube, new Vector3(3.75f, 1.5f, 0f), new Vector3(0.25f, 3f, 7.5f));

            GameObject seamMarker = CreatePrimitive(parent, "HiddenSeamMarker", PrimitiveType.Cube, new Vector3(0f, 1.46f, 3.60f), new Vector3(0.04f, 0.90f, 0.04f));
            seamMarker.GetComponent<Collider>().enabled = false;
            SetRendererColor(seamMarker, new Color(0.44f, 0.81f, 0.89f, 1f));

            GameObject overheadLightGo = new("OverheadLight");
            overheadLightGo.transform.SetParent(parent, false);
            overheadLightGo.transform.localPosition = new Vector3(0f, 2.55f, 0f);
            Light overheadLight = overheadLightGo.AddComponent<Light>();
            overheadLight.type = LightType.Point;
            overheadLight.range = 12f;
            overheadLight.shadows = LightShadows.Soft;

            shadowAnchor = new GameObject("InitialShadowAnchor");
            shadowAnchor.transform.SetParent(parent, false);
            shadowAnchor.transform.position = new Vector3(0f, 0.2f, 3.1f);
            shadowAnchor.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            egoProxy = CreateProxy(parent, "EgoProxy", AmberProxyColor, new Vector3(0f, 1f, -2.6f));
            shadowProxy = CreateProxy(parent, "ShadowProxy", ShadowProxyColor, shadowAnchor.transform.position);
            egoProxy.SetActive(false);
        }

        private static LocalViewportHandoff AddOrGetHandoff(GameObject roomRoot)
        {
            LocalViewportHandoff handoff = roomRoot.GetComponent<LocalViewportHandoff>();
            if (handoff == null)
            {
                handoff = roomRoot.AddComponent<LocalViewportHandoff>();
            }

            return handoff;
        }

        private static void WireHandoff(
            LocalViewportHandoff handoff,
            Transform initialShadowAnchor,
            GameObject egoProxy,
            GameObject shadowProxy)
        {
            if (handoff == null)
            {
                return;
            }

            AudioSource fractureAudioSource = handoff.gameObject.GetComponent<AudioSource>();
            if (fractureAudioSource == null)
            {
                fractureAudioSource = handoff.gameObject.AddComponent<AudioSource>();
            }

            ConfigureFractureAudioSource(fractureAudioSource);

            InputActionAsset controls = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ControlsAssetPath);
            AudioClip fractureCue = ShadowAudioClipFactory.CreateDestabilizedGuidanceCue();

            SerializedObject handoffSo = new(handoff);
            handoffSo.FindProperty("_controls").objectReferenceValue = controls;
            handoffSo.FindProperty("_egoProxyRoot").objectReferenceValue = egoProxy;
            handoffSo.FindProperty("_shadowProxyRoot").objectReferenceValue = shadowProxy;
            handoffSo.FindProperty("_initialShadowAnchor").objectReferenceValue = initialShadowAnchor;
            handoffSo.FindProperty("_fractureAudioSource").objectReferenceValue = fractureAudioSource;
            handoffSo.FindProperty("_fractureCue").objectReferenceValue = fractureCue;
            handoffSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject BuildPlayer(Scene scene)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null)
            {
                Debug.LogError($"[Wave014A Setup] Missing player prefab at '{PlayerPrefabPath}'.");
                return null;
            }

            GameObject playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab, scene) as GameObject;
            if (playerInstance == null)
            {
                Debug.LogError("[Wave014A Setup] Failed to instantiate player prefab.");
                return null;
            }

            playerInstance.name = "Player";
            playerInstance.transform.position = new Vector3(0f, 0.2f, -2.6f);
            playerInstance.transform.rotation = Quaternion.identity;
            return playerInstance;
        }

        private static void DisableShadowPerceptionController(GameObject player)
        {
            if (player == null)
            {
                return;
            }

            ShadowPerceptionController shadowPerception = player.GetComponent<ShadowPerceptionController>();
            if (shadowPerception != null)
            {
                shadowPerception.enabled = false;
            }
        }

        private static GameObject CreateProxy(Transform parent, string name, Color color, Vector3 worldPosition)
        {
            GameObject proxy = CreatePrimitive(parent, name, PrimitiveType.Capsule, Vector3.zero, new Vector3(0.55f, 0.9f, 0.55f));
            proxy.transform.position = worldPosition;
            SetRendererColor(proxy, color);
            proxy.GetComponent<Collider>().enabled = false;
            return proxy;
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

        private static void SetRendererColor(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            Material material = new Material(renderer.sharedMaterial);
            material.color = color;
            renderer.sharedMaterial = material;
        }

        private static void ConfigureFractureAudioSource(AudioSource audioSource)
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
    }
}
