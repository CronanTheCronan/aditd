using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ADoorInsideTheDark.Editor
{
    /// <summary>
    /// Editor-only Wave 004 interaction UI wiring for the active scene.
    /// </summary>
    public static class Wave004TestSceneSetup
    {
        private const string MenuPath = "ADITD/Setup/Wave 004 Test UI";

        private const string ExpectedSceneName = "Test_FirstPersonController";

        private const string CanvasName = "ADITD_TestCanvas";
        private const string InteractionPromptName = "InteractionPrompt";
        private const string PromptTextName = "PromptText";
        private const string InspectionPanelName = "InspectionPanel";
        private const string PanelContentsName = "PanelContents";
        private const string TitleTextName = "TitleText";
        private const string BodyTextName = "BodyText";
        private const string InspectableCubeName = "InspectableDebugCube";

        private static readonly Color InspectablePinkTestScene = new(0.86f, 0.42f, 0.62f, 1f);

        [MenuItem(MenuPath)]
        public static void SetupWave004TestUi()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("[ADITD Wave004 UI Setup] No valid active scene.");
                return;
            }

            if (scene.name != ExpectedSceneName)
            {
                Debug.LogWarning(
                    $"[ADITD Wave004 UI Setup] Active scene is '{scene.name}', expected '{ExpectedSceneName}'. Continuing anyway.",
                    null);
            }

            Undo.IncrementCurrentGroup();

            GameObject canvasGo = EnsureCanvas(scene);
            EnsureInteractionPrompt(canvasGo.transform);
            EnsureInspectionPanel(canvasGo.transform);

            AssignPlayerInteractor(scene);

            EnsureInspectableCube(scene);

            ApplyInspectableDebugCubePinkForTestScene(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            Undo.SetCurrentGroupName("ADITD Wave 004 Test UI Setup");

            Debug.Log(
                $"[ADITD Wave004 UI Setup] Finished setup for scene '{scene.path}'. Save the scene if needed.",
                canvasGo);
        }

        private static GameObject EnsureCanvas(Scene scene)
        {
            GameObject canvasGo = FindRootGameObjectByName(scene, CanvasName);
            if (canvasGo == null)
            {
                canvasGo = new GameObject(CanvasName);
                Undo.RegisterCreatedObjectUndo(canvasGo, $"Create {CanvasName}");
                EditorSceneManager.MoveGameObjectToScene(canvasGo, scene);
                Debug.Log($"[ADITD Wave004 UI Setup] Created root '{CanvasName}'.", canvasGo);
            }
            else
            {
                Debug.Log($"[ADITD Wave004 UI Setup] Reused existing '{CanvasName}'.", canvasGo);
            }

            Undo.RecordObject(canvasGo, $"Configure {CanvasName}");

            Canvas canvas = canvasGo.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = Undo.AddComponent<Canvas>(canvasGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added Canvas to '{CanvasName}'.", canvasGo);
            }

            Undo.RecordObject(canvas, $"Configure Canvas on {CanvasName}");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = Undo.AddComponent<CanvasScaler>(canvasGo);
                Undo.RecordObject(scaler, $"Configure CanvasScaler on {CanvasName}");
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                Debug.Log($"[ADITD Wave004 UI Setup] Added CanvasScaler to '{CanvasName}'.", canvasGo);
            }

            if (canvasGo.GetComponent<GraphicRaycaster>() == null)
            {
                Undo.AddComponent<GraphicRaycaster>(canvasGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added GraphicRaycaster to '{CanvasName}'.", canvasGo);
            }

            return canvasGo;
        }

        private static void EnsureInteractionPrompt(Transform canvasTransform)
        {
            Transform promptTransform = FindDescendant(canvasTransform, InteractionPromptName);
            GameObject promptGo = promptTransform != null
                ? promptTransform.gameObject
                : CreateUiChild(canvasTransform, InteractionPromptName, $"Create {InteractionPromptName}");

            Undo.RecordObject(promptGo, $"Configure {InteractionPromptName}");

            if (!TryGetRequiredRectTransform(promptGo, InteractionPromptName, out RectTransform promptRect))
            {
                return;
            }

            Undo.RecordObject(promptRect, $"Layout {InteractionPromptName}");
            promptRect.anchorMin = new Vector2(0.5f, 0f);
            promptRect.anchorMax = new Vector2(0.5f, 0f);
            promptRect.pivot = new Vector2(0.5f, 0f);
            promptRect.anchoredPosition = new Vector2(0f, 120f);
            promptRect.sizeDelta = new Vector2(520f, 48f);

            InteractionPromptView promptView = promptGo.GetComponent<InteractionPromptView>();
            if (promptView == null)
            {
                promptView = Undo.AddComponent<InteractionPromptView>(promptGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added InteractionPromptView to '{InteractionPromptName}'.", promptGo);
            }

            GameObject textGo = FindOrCreateUiChild(promptGo.transform, PromptTextName, $"Create {PromptTextName}");
            Undo.RecordObject(textGo, $"Configure {PromptTextName}");

            if (!TryGetRequiredRectTransform(textGo, PromptTextName, out RectTransform textRect))
            {
                return;
            }

            Undo.RecordObject(textRect, $"Layout {PromptTextName}");
            StretchAnchors(textRect);

            Text promptUiText = textGo.GetComponent<Text>();
            if (promptUiText == null)
            {
                promptUiText = Undo.AddComponent<Text>(textGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added Text to '{PromptTextName}'.", textGo);
            }

            ConfigureUiText(promptUiText, GetDefaultUIFont(), 22, TextAnchor.MiddleCenter, Color.white, $"Configure {PromptTextName} Text");

            SerializedObject promptSo = new SerializedObject(promptView);
            SerializedProperty rootProp = promptSo.FindProperty("_root");
            SerializedProperty promptTextProp = promptSo.FindProperty("_promptText");
            if (rootProp != null)
            {
                rootProp.objectReferenceValue = textGo;
            }

            if (promptTextProp != null)
            {
                promptTextProp.objectReferenceValue = promptUiText;
            }

            promptSo.ApplyModifiedProperties();

            textGo.SetActive(false);
            Debug.Log($"[ADITD Wave004 UI Setup] Prompt starts hidden ({PromptTextName} inactive).", textGo);
        }

        private static void EnsureInspectionPanel(Transform canvasTransform)
        {
            Transform panelTransform = FindDescendant(canvasTransform, InspectionPanelName);
            GameObject panelGo = panelTransform != null
                ? panelTransform.gameObject
                : CreateUiChild(canvasTransform, InspectionPanelName, $"Create {InspectionPanelName}");

            Undo.RecordObject(panelGo, $"Configure {InspectionPanelName}");

            if (!TryGetRequiredRectTransform(panelGo, InspectionPanelName, out RectTransform panelOuterRect))
            {
                return;
            }

            Undo.RecordObject(panelOuterRect, $"Layout {InspectionPanelName}");
            StretchAnchors(panelOuterRect);

            InspectionPanelView panelView = panelGo.GetComponent<InspectionPanelView>();
            if (panelView == null)
            {
                panelView = Undo.AddComponent<InspectionPanelView>(panelGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added InspectionPanelView to '{InspectionPanelName}'.", panelGo);
            }

            GameObject contentsGo =
                FindOrCreateUiChild(panelGo.transform, PanelContentsName, $"Create {PanelContentsName}");
            Undo.RecordObject(contentsGo, $"Configure {PanelContentsName}");

            if (!TryGetRequiredRectTransform(contentsGo, PanelContentsName, out RectTransform contentsRect))
            {
                return;
            }

            Undo.RecordObject(contentsRect, $"Layout {PanelContentsName}");
            StretchAnchors(contentsRect);
            contentsRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentsRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentsRect.sizeDelta = new Vector2(760f, 420f);
            contentsRect.anchoredPosition = Vector2.zero;

            Image panelImage = contentsGo.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = Undo.AddComponent<Image>(contentsGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added Image to '{PanelContentsName}'.", contentsGo);
            }

            Undo.RecordObject(panelImage, $"Configure {PanelContentsName} Image");
            panelImage.color = new Color(0.08f, 0.08f, 0.1f, 0.94f);

            GameObject titleGo = FindOrCreateUiChild(contentsGo.transform, TitleTextName, $"Create {TitleTextName}");
            Undo.RecordObject(titleGo, $"Configure {TitleTextName}");

            if (!TryGetRequiredRectTransform(titleGo, TitleTextName, out RectTransform titleRect))
            {
                return;
            }

            Undo.RecordObject(titleRect, $"Layout {TitleTextName}");
            titleRect.anchorMin = new Vector2(0f, 0.78f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, 0f);
            titleRect.offsetMax = new Vector2(-24f, -16f);

            Text titleUiText = titleGo.GetComponent<Text>();
            if (titleUiText == null)
            {
                titleUiText = Undo.AddComponent<Text>(titleGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added Text to '{TitleTextName}'.", titleGo);
            }

            ConfigureUiText(titleUiText, GetDefaultUIFont(), 28, TextAnchor.UpperLeft, Color.white, $"Configure {TitleTextName} Text");
            Undo.RecordObject(titleUiText, $"Configure {TitleTextName} Text Style");
            titleUiText.fontStyle = FontStyle.Bold;

            GameObject bodyGo = FindOrCreateUiChild(contentsGo.transform, BodyTextName, $"Create {BodyTextName}");
            Undo.RecordObject(bodyGo, $"Configure {BodyTextName}");

            if (!TryGetRequiredRectTransform(bodyGo, BodyTextName, out RectTransform bodyRect))
            {
                return;
            }

            Undo.RecordObject(bodyRect, $"Layout {BodyTextName}");
            bodyRect.anchorMin = new Vector2(0f, 0f);
            bodyRect.anchorMax = new Vector2(1f, 0.78f);
            bodyRect.offsetMin = new Vector2(24f, 24f);
            bodyRect.offsetMax = new Vector2(-24f, -8f);

            Text bodyUiText = bodyGo.GetComponent<Text>();
            if (bodyUiText == null)
            {
                bodyUiText = Undo.AddComponent<Text>(bodyGo);
                Debug.Log($"[ADITD Wave004 UI Setup] Added Text to '{BodyTextName}'.", bodyGo);
            }

            ConfigureUiText(bodyUiText, GetDefaultUIFont(), 20, TextAnchor.UpperLeft, new Color(0.9f, 0.9f, 0.92f), $"Configure {BodyTextName} Text");
            Undo.RecordObject(bodyUiText, $"Configure {BodyTextName} Text Overflow");
            bodyUiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            bodyUiText.verticalOverflow = VerticalWrapMode.Truncate;

            SerializedObject panelSo = new SerializedObject(panelView);
            SerializedProperty panelRootProp = panelSo.FindProperty("_panelRoot");
            SerializedProperty titleProp = panelSo.FindProperty("_titleText");
            SerializedProperty bodyProp = panelSo.FindProperty("_bodyText");
            if (panelRootProp != null)
            {
                panelRootProp.objectReferenceValue = contentsGo;
            }

            if (titleProp != null)
            {
                titleProp.objectReferenceValue = titleUiText;
            }

            if (bodyProp != null)
            {
                bodyProp.objectReferenceValue = bodyUiText;
            }

            panelSo.ApplyModifiedProperties();

            contentsGo.SetActive(false);
            Debug.Log($"[ADITD Wave004 UI Setup] Inspection panel starts hidden ({PanelContentsName} inactive).", contentsGo);
        }

        private static void AssignPlayerInteractor(Scene scene)
        {
            InteractionPromptView promptView = FindFirstComponentInScene<InteractionPromptView>(scene);
            InspectionPanelView inspectionPanel = FindFirstComponentInScene<InspectionPanelView>(scene);

            if (promptView == null || inspectionPanel == null)
            {
                Debug.LogWarning(
                    "[ADITD Wave004 UI Setup] Could not locate InteractionPromptView or InspectionPanelView in scene for assignment.",
                    null);
                return;
            }

            PlayerInteractor[] interactors = FindComponentsInScene<PlayerInteractor>(scene);

            if (interactors.Length == 0)
            {
                Debug.LogWarning(
                    "[ADITD Wave004 UI Setup] Skipped PlayerInteractor wiring: no PlayerInteractor found in the active scene.",
                    null);
                return;
            }

            if (interactors.Length > 1)
            {
                Debug.LogWarning(
                    $"[ADITD Wave004 UI Setup] Multiple ({interactors.Length}) PlayerInteractor instances found; assigning references on all.",
                    interactors[0]);
            }

            foreach (PlayerInteractor interactor in interactors)
            {
                Undo.RecordObject(interactor, "Assign Wave004 UI references");

                SerializedObject interactorSo = new SerializedObject(interactor);
                SerializedProperty promptProp = interactorSo.FindProperty("_promptView");
                SerializedProperty inspectionProp = interactorSo.FindProperty("_inspectionPanel");

                if (promptProp != null)
                {
                    promptProp.objectReferenceValue = promptView;
                }

                if (inspectionProp != null)
                {
                    inspectionProp.objectReferenceValue = inspectionPanel;
                }

                interactorSo.ApplyModifiedProperties();

                Debug.Log(
                    $"[ADITD Wave004 UI Setup] Assigned InteractionPromptView and InspectionPanelView on '{interactor.gameObject.name}'.",
                    interactor);
            }
        }

        private static void EnsureInspectableCube(Scene scene)
        {
            if (SceneHasInspectable(scene))
            {
                Debug.Log("[ADITD Wave004 UI Setup] Skipped inspectable cube: scene already contains an IInspectable.");
                return;
            }

            GameObject cubeGo = FindRootGameObjectByName(scene, InspectableCubeName);
            if (cubeGo != null)
            {
                if (!TryEnsureInspectableDebugObject(cubeGo))
                {
                    Debug.LogWarning(
                        $"[ADITD Wave004 UI Setup] Existing '{InspectableCubeName}' could not receive InspectableDebugObject safely.",
                        cubeGo);
                }
                else
                {
                    Debug.Log($"[ADITD Wave004 UI Setup] Reused '{InspectableCubeName}' and ensured InspectableDebugObject.", cubeGo);
                }

                return;
            }

            cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeGo.name = InspectableCubeName;
            Undo.RegisterCreatedObjectUndo(cubeGo, $"Create {InspectableCubeName}");
            EditorSceneManager.MoveGameObjectToScene(cubeGo, scene);
            cubeGo.transform.position = new Vector3(0f, 0.5f, 5f);

            Undo.AddComponent<InspectableDebugObject>(cubeGo);
            Debug.Log($"[ADITD Wave004 UI Setup] Created '{InspectableCubeName}' with BoxCollider and InspectableDebugObject.", cubeGo);
        }

        private static void ApplyInspectableDebugCubePinkForTestScene(Scene scene)
        {
            if (scene.name != ExpectedSceneName)
            {
                return;
            }

            GameObject cubeGo = FindRootGameObjectByName(scene, InspectableCubeName);
            if (cubeGo == null)
            {
                return;
            }

            InspectableDebugObject inspectable = cubeGo.GetComponent<InspectableDebugObject>();
            if (inspectable == null)
            {
                return;
            }

            Undo.RecordObject(inspectable, "Apply InspectableDebugCube tint (first-person test scene)");
            SerializedObject inspectableSo = new SerializedObject(inspectable);
            SerializedProperty tintProperty = inspectableSo.FindProperty("_startupTint");
            if (tintProperty != null)
            {
                tintProperty.colorValue = InspectablePinkTestScene;
            }

            inspectableSo.ApplyModifiedProperties();
            inspectable.RefreshStartupTint();

            Debug.Log(
                $"[ADITD Wave004 UI Setup] Ensured InspectableDebugCube startup tint ({InspectablePinkTestScene}) in '{scene.name}'.",
                inspectable);
        }

        private static bool TryEnsureInspectableDebugObject(GameObject target)
        {
            InspectableDebugObject existingInspectable = target.GetComponent<InspectableDebugObject>();
            if (existingInspectable != null)
            {
                Debug.Log($"[ADITD Wave004 UI Setup] '{target.name}' already has InspectableDebugObject.", target);
                return true;
            }

            foreach (MonoBehaviour mb in target.GetComponents<MonoBehaviour>())
            {
                if (mb == null)
                {
                    continue;
                }

                if (mb is IInteractable)
                {
                    Debug.LogWarning(
                        $"[ADITD Wave004 UI Setup] Refused InspectableDebugObject on '{target.name}': already has IInteractable ({mb.GetType().Name}).",
                        target);
                    return false;
                }
            }

            Undo.AddComponent<InspectableDebugObject>(target);
            return true;
        }

        private static bool SceneHasInspectable(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (MonoBehaviour mb in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (mb is IInspectable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static GameObject FindRootGameObjectByName(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == name)
                {
                    return root;
                }
            }

            return null;
        }

        private static Transform FindDescendant(Transform parent, string childName)
        {
            if (parent.name == childName)
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Transform found = FindDescendant(child, childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static GameObject CreateUiChild(Transform parent, string objectName, string undoLabel)
        {
            GameObject go = new GameObject(objectName, typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, undoLabel);
            go.transform.SetParent(parent, false);
            Debug.Log($"[ADITD Wave004 UI Setup] Created '{objectName}' under '{parent.name}'.", go);
            return go;
        }

        private static GameObject FindOrCreateUiChild(Transform parent, string objectName, string undoLabel)
        {
            Transform existing = parent.Find(objectName);
            if (existing != null)
            {
                Debug.Log($"[ADITD Wave004 UI Setup] Reused existing '{objectName}' under '{parent.name}'.", existing.gameObject);
                return existing.gameObject;
            }

            return CreateUiChild(parent, objectName, undoLabel);
        }

        private static void StretchAnchors(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }

        private static bool TryGetRequiredRectTransform(GameObject target, string objectName, out RectTransform rectTransform)
        {
            rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return true;
            }

            Debug.LogWarning(
                $"[ADITD Wave004 UI Setup] Expected '{objectName}' to have a RectTransform; skipping layout and wiring for this object.",
                target);
            return false;
        }

        private static void ConfigureUiText(
            Text text,
            Font font,
            int size,
            TextAnchor alignment,
            Color color,
            string undoLabel)
        {
            Undo.RecordObject(text, undoLabel);

            if (font == null)
            {
                Debug.LogWarning(
                    "[ADITD Wave004 UI Setup] No built-in LegacyRuntime/Arial font resolved via Resources; assign a font manually if labels are invisible.",
                    text);
            }

            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.supportRichText = false;
        }

        private static Font GetDefaultUIFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private static T FindFirstComponentInScene<T>(Scene scene)
            where T : UnityEngine.Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static T[] FindComponentsInScene<T>(Scene scene)
            where T : UnityEngine.Component
        {
            var list = new System.Collections.Generic.List<T>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                list.AddRange(root.GetComponentsInChildren<T>(true));
            }

            return list.ToArray();
        }
    }
}
