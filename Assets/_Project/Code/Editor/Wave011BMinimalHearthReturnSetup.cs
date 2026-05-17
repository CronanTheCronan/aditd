using ADoorInsideTheDark.Hearth;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ADoorInsideTheDark.Editor
{
    public static class Wave011BMinimalHearthReturnSetup
    {
        private const string MenuPath = "ADITD/Setup/Recreate Wave 011B Minimal Hearth Return";
        private const string ScenePath = "Assets/_Project/Scenes/Wave011B_HearthMinimalReturn.unity";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";
        private static readonly Vector3 PickupColliderCenter = new(0f, 0.31f, 0.75f);
        private static readonly Vector3 PickupColliderSize = new(5.94f, 2.12f, 5.94f);
        private static readonly Vector3 PlacementColliderCenter = new(0f, 0.67f, 0.63f);
        private static readonly Vector3 PlacementColliderSize = new(2.1f, 3.34f, 2.82f);
        private static readonly Vector3 ExitColliderCenter = new(0f, 0f, 0.24f);
        private static readonly Vector3 ExitColliderSize = new(1.82f, 1.0f, 1.04f);

        [MenuItem(MenuPath)]
        public static void CreateOrRecreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("[Wave011B Setup] Scene recreation canceled by user before replacing the active scene.");
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject roomRoot = new("Wave011B_HearthMinimalReturn");
            EditorSceneManager.MoveGameObjectToScene(roomRoot, scene);

            ConfigureSceneLighting(scene);
            BuildPlayer(scene);
            BuildRoom(roomRoot.transform, out HearthMinimalReturnController controller);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log(
                $"[Wave011B Setup] Created or recreated '{ScenePath}'. Graybox for room.hearth.minimal_return.",
                roomRoot);
        }

        private static void ConfigureSceneLighting(Scene scene)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.14f, 0.13f, 0.12f, 1f);

            GameObject directionalLightGo = new("Directional Light");
            EditorSceneManager.MoveGameObjectToScene(directionalLightGo, scene);

            Light directionalLight = directionalLightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.intensity = 0.12f;
            directionalLight.color = new Color(0.72f, 0.70f, 0.67f, 1f);
            directionalLight.transform.rotation = Quaternion.Euler(42f, -18f, 0f);
        }

        private static void BuildRoom(Transform parent, out HearthMinimalReturnController controller)
        {
            GameObject floor = CreatePrimitive(parent, "Floor", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(8f, 0.25f, 8f));
            GameObject ceiling = CreatePrimitive(parent, "Ceiling", PrimitiveType.Cube, new Vector3(0f, 3.2f, 0f), new Vector3(8f, 0.25f, 8f));
            GameObject backWall = CreatePrimitive(parent, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.6f, -4f), new Vector3(8f, 3.2f, 0.25f));
            GameObject frontWall = CreatePrimitive(parent, "FrontWall", PrimitiveType.Cube, new Vector3(0f, 1.6f, 4f), new Vector3(8f, 3.2f, 0.25f));
            GameObject leftWall = CreatePrimitive(parent, "LeftWall", PrimitiveType.Cube, new Vector3(-4f, 1.6f, 0f), new Vector3(0.25f, 3.2f, 8f));
            GameObject rightWall = CreatePrimitive(parent, "RightWall", PrimitiveType.Cube, new Vector3(4f, 1.6f, 0f), new Vector3(0.25f, 3.2f, 8f));

            ApplyColor(floor, new Color(0.26f, 0.24f, 0.22f, 1f));
            ApplyColor(ceiling, new Color(0.18f, 0.17f, 0.16f, 1f));
            ApplyColor(backWall, new Color(0.20f, 0.18f, 0.17f, 1f));
            ApplyColor(frontWall, new Color(0.20f, 0.18f, 0.17f, 1f));
            ApplyColor(leftWall, new Color(0.22f, 0.20f, 0.19f, 1f));
            ApplyColor(rightWall, new Color(0.22f, 0.20f, 0.19f, 1f));

            GameObject rug = CreatePrimitive(parent, "Rug", PrimitiveType.Cube, new Vector3(0f, 0.03f, 0.65f), new Vector3(3.4f, 0.02f, 4.2f));
            ApplyColor(rug, new Color(0.31f, 0.28f, 0.25f, 1f));
            rug.GetComponent<Collider>().enabled = false;

            GameObject pickupTable = CreatePrimitive(parent, "PickupTable", PrimitiveType.Cube, new Vector3(-2.15f, 0.48f, -1.55f), new Vector3(0.75f, 0.46f, 0.6f));
            ApplyColor(pickupTable, new Color(0.40f, 0.34f, 0.29f, 1f));

            GameObject fireplaceBase = CreatePrimitive(parent, "FireplaceBase", PrimitiveType.Cube, new Vector3(0f, 0.7f, -3.35f), new Vector3(2.4f, 1.2f, 0.6f));
            GameObject mantle = CreatePrimitive(parent, "Mantle", PrimitiveType.Cube, new Vector3(0f, 1.75f, -3.18f), new Vector3(3.0f, 0.16f, 0.5f));
            GameObject shelf = CreatePrimitive(parent, "AnchorShelf", PrimitiveType.Cube, new Vector3(1.05f, 1.12f, -3.05f), new Vector3(1.4f, 0.12f, 0.44f));
            ApplyColor(fireplaceBase, new Color(0.33f, 0.29f, 0.27f, 1f));
            ApplyColor(mantle, new Color(0.42f, 0.36f, 0.31f, 1f));
            ApplyColor(shelf, new Color(0.44f, 0.38f, 0.34f, 1f));

            GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fire.name = "HearthFire";
            fire.transform.SetParent(parent, false);
            fire.transform.localPosition = new Vector3(0f, 0.82f, -3.16f);
            fire.transform.localScale = new Vector3(0.48f, 0.28f, 0.42f);
            fire.GetComponent<Collider>().enabled = false;
            ApplyColor(fire, new Color(0.92f, 0.47f, 0.18f, 1f));

            GameObject fireLightGo = new("HearthFireLight");
            fireLightGo.transform.SetParent(parent, false);
            fireLightGo.transform.localPosition = new Vector3(0f, 1.05f, -2.95f);
            Light fireLight = fireLightGo.AddComponent<Light>();
            fireLight.type = LightType.Point;
            fireLight.range = 7f;
            fireLight.intensity = 1.1f;
            fireLight.color = new Color(0.92f, 0.47f, 0.18f, 1f);
            fireLight.shadows = LightShadows.Soft;

            GameObject pickupThermos = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pickupThermos.name = "GreenThermosPickup";
            pickupThermos.transform.SetParent(parent, false);
            pickupThermos.transform.localPosition = new Vector3(-2.15f, 0.98f, -1.55f);
            pickupThermos.transform.localScale = new Vector3(0.16f, 0.26f, 0.16f);
            pickupThermos.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            ApplyColor(pickupThermos, new Color(0.22f, 0.56f, 0.38f, 1f));
            ConfigureInteractionCollider(pickupThermos, PickupColliderCenter, PickupColliderSize);
            GreenThermosReturnInteractable pickupInteractable = pickupThermos.AddComponent<GreenThermosReturnInteractable>();

            GameObject placedThermos = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            placedThermos.name = "GreenThermosPlaced";
            placedThermos.transform.SetParent(parent, false);
            placedThermos.transform.localPosition = new Vector3(1.05f, 1.3f, -3.02f);
            placedThermos.transform.localScale = new Vector3(0.15f, 0.25f, 0.15f);
            placedThermos.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
            placedThermos.GetComponent<Collider>().enabled = false;
            ApplyColor(placedThermos, new Color(0.22f, 0.56f, 0.38f, 1f));

            GameObject slotRoot = CreatePrimitive(parent, "AnchorPlacementSlot", PrimitiveType.Cube, new Vector3(1.05f, 1.22f, -2.9f), new Vector3(0.55f, 0.18f, 0.32f));
            ApplyColor(slotRoot, new Color(0.49f, 0.42f, 0.36f, 1f));
            ConfigureInteractionCollider(slotRoot, PlacementColliderCenter, PlacementColliderSize);

            GameObject slotHighlight = CreatePrimitive(slotRoot.transform, "AnchorPlacementHighlight", PrimitiveType.Cube, Vector3.zero, new Vector3(1.1f, 0.45f, 1.1f));
            ApplyColor(slotHighlight, new Color(0.76f, 0.65f, 0.34f, 1f));
            slotHighlight.GetComponent<Collider>().enabled = false;

            GameObject exitFrameLeft = CreatePrimitive(parent, "ExitFrameLeft", PrimitiveType.Cube, new Vector3(-1.0f, 1.1f, 3.72f), new Vector3(0.26f, 2.2f, 0.28f));
            GameObject exitFrameRight = CreatePrimitive(parent, "ExitFrameRight", PrimitiveType.Cube, new Vector3(1.0f, 1.1f, 3.72f), new Vector3(0.26f, 2.2f, 0.28f));
            GameObject exitLintel = CreatePrimitive(parent, "ExitLintel", PrimitiveType.Cube, new Vector3(0f, 2.32f, 3.72f), new Vector3(2.26f, 0.24f, 0.28f));
            ApplyColor(exitFrameLeft, new Color(0.33f, 0.29f, 0.27f, 1f));
            ApplyColor(exitFrameRight, new Color(0.33f, 0.29f, 0.27f, 1f));
            ApplyColor(exitLintel, new Color(0.33f, 0.29f, 0.27f, 1f));

            GameObject exitDoorRoot = CreatePrimitive(parent, "NextDoorAffordance", PrimitiveType.Cube, new Vector3(0f, 1.08f, 3.78f), new Vector3(1.4f, 2.1f, 0.14f));
            ApplyColor(exitDoorRoot, new Color(0.18f, 0.21f, 0.25f, 1f));
            ConfigureInteractionCollider(exitDoorRoot, ExitColliderCenter, ExitColliderSize);
            HearthExitStubInteractable exitInteractable = exitDoorRoot.AddComponent<HearthExitStubInteractable>();

            GameObject exitCue = CreatePrimitive(parent, "NextDoorCue", PrimitiveType.Cube, new Vector3(0f, 2.72f, 3.55f), new Vector3(0.32f, 0.32f, 0.22f));
            exitCue.GetComponent<Collider>().enabled = false;
            ApplyColor(exitCue, new Color(0.89f, 0.78f, 0.46f, 1f));

            GameObject exitCueLightGo = new("NextDoorCueLight");
            exitCueLightGo.transform.SetParent(parent, false);
            exitCueLightGo.transform.localPosition = new Vector3(0f, 2.56f, 3.18f);
            Light exitCueLight = exitCueLightGo.AddComponent<Light>();
            exitCueLight.type = LightType.Point;
            exitCueLight.range = 3.2f;
            exitCueLight.intensity = 0f;
            exitCueLight.color = new Color(0.89f, 0.78f, 0.46f, 1f);
            exitCueLight.shadows = LightShadows.None;

            controller = AddOrGetController(parent.gameObject);
            HearthAnchorPlacementSlot placementSlot = slotRoot.AddComponent<HearthAnchorPlacementSlot>();

            SerializedObject controllerSo = new(controller);
            controllerSo.FindProperty("_weightOfDoorCompleted").boolValue = true;
            controllerSo.FindProperty("_pickupThermosRoot").objectReferenceValue = pickupThermos;
            controllerSo.FindProperty("_placedThermosRoot").objectReferenceValue = placedThermos;
            controllerSo.FindProperty("_hearthFireLight").objectReferenceValue = fireLight;
            controllerSo.FindProperty("_hearthFireRenderer").objectReferenceValue = fire.GetComponent<Renderer>();
            controllerSo.FindProperty("_nextDoorCueRoot").objectReferenceValue = exitCue;
            controllerSo.FindProperty("_nextDoorVisual").objectReferenceValue = exitDoorRoot.transform;
            controllerSo.FindProperty("_nextDoorCueLight").objectReferenceValue = exitCueLight;
            controllerSo.FindProperty("_showDebugOverlay").boolValue = false;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject pickupInteractableSo = new(pickupInteractable);
            pickupInteractableSo.FindProperty("_controller").objectReferenceValue = controller;
            pickupInteractableSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject placementSlotSo = new(placementSlot);
            placementSlotSo.FindProperty("_controller").objectReferenceValue = controller;
            placementSlotSo.FindProperty("_highlightRoot").objectReferenceValue = slotHighlight;
            SetObjectArray(placementSlotSo.FindProperty("_highlightRenderers"), slotHighlight.GetComponent<Renderer>());
            placementSlotSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject exitInteractableSo = new(exitInteractable);
            exitInteractableSo.FindProperty("_controller").objectReferenceValue = controller;
            exitInteractableSo.ApplyModifiedPropertiesWithoutUndo();

            placedThermos.SetActive(false);
            slotHighlight.SetActive(false);
            exitCue.SetActive(false);
        }

        private static HearthMinimalReturnController AddOrGetController(GameObject roomRoot)
        {
            HearthMinimalReturnController controller = roomRoot.GetComponent<HearthMinimalReturnController>();
            if (controller == null)
            {
                controller = roomRoot.AddComponent<HearthMinimalReturnController>();
            }

            return controller;
        }

        private static GameObject BuildPlayer(Scene scene)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null)
            {
                Debug.LogError($"[Wave011B Setup] Missing player prefab at '{PlayerPrefabPath}'.");
                return null;
            }

            GameObject playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab, scene) as GameObject;
            if (playerInstance == null)
            {
                Debug.LogError("[Wave011B Setup] Failed to instantiate player prefab.");
                return null;
            }

            playerInstance.name = "Player";
            playerInstance.transform.position = new Vector3(0f, 0.2f, 2.45f);
            playerInstance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            return playerInstance;
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

        private static void ConfigureInteractionCollider(
            GameObject go,
            Vector3 center,
            Vector3 size)
        {
            Collider existingCollider = go.GetComponent<Collider>();
            if (existingCollider != null)
            {
                Object.DestroyImmediate(existingCollider);
            }

            BoxCollider boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.center = center;
            boxCollider.size = size;
        }

        private static void ApplyColor(GameObject go, Color color)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
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
