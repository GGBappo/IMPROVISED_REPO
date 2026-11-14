using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class ClosetDisplay : MonoBehaviour
{
    [Serializable]
    private class ClosetHook
    {
        public Transform hookpoint;
        [NonSerialized] public SpriteRenderer renderer;
    }

    [SerializeField] private ClothingManager clothingManager;
    [SerializeField] private TMP_Text clothingNameText;
    [SerializeField] private TMP_Text clothingInfoText;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonLabel;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private ClosetHook[] hooks = new ClosetHook[4];
    [SerializeField, Min(0.01f)] private float spriteSize = 0.29f;
    [SerializeField, Range(0f, 20f)] private float swingDegrees = 6f;
    [SerializeField, Min(0f)] private float swingsPerSecond = 0.8f;

    private int currentHookIndex;
    private GameObject spriteRoot;

    private void OnEnable()
    {
        if (clothingManager != null)
            clothingManager.OnOutfitChanged += OnOutfitChanged;

        // Restore the current label if the closet is shown again.
        UpdateUI();
    }

    private void OnDisable()
    {
        if (clothingManager != null)
            clothingManager.OnOutfitChanged -= OnOutfitChanged;

        if (spriteRoot != null)
            spriteRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (spriteRoot != null)
            Destroy(spriteRoot);
    }

    public void RefreshAllHooks()
    {
        for (int index = 0; index < hooks.Length; index++)
            UpdateClothingDisplay(index);

        UpdateUI();
    }

    public void UpdateClothingDisplay(int hookIndex)
    {
        if (clothingManager == null || hookIndex < 0 || hookIndex >= hooks.Length)
            return;

        ClosetHook hook = hooks[hookIndex];
        if (hook == null || hook.hookpoint == null)
            return;

        Clothing_SO clothing = clothingManager.GetClothing(hookIndex);
        Sprite sprite = null;
        if (clothing != null)
        {
            // Both states use the same renderer. Nothing from a clothing prefab is spawned.
            sprite = clothing.ClothingState ? lockedSprite : clothing.closetSprite;
            if (sprite == null)
                Debug.LogWarning($"No closet sprite for {clothing.name} at hook {hookIndex + 1}.", this);
        }

        // The renderer is created on demand so the closet can be shown before the first clothing is added.
        SpriteRenderer renderer = GetOrCreateRenderer(hook);
        renderer.sprite = sprite;
        renderer.enabled = sprite != null;
        if (sprite != null)
        {
            // Give differently sized source sprites a consistent maximum size in the scene.
            Vector3 size = sprite.bounds.size;
            renderer.transform.localScale = Vector3.one * (spriteSize / Mathf.Max(size.x, size.y));
        }
    }

    private SpriteRenderer GetOrCreateRenderer(ClosetHook hook)
    {
        if (hook.renderer != null)
            return hook.renderer;

        if (spriteRoot == null)
        {
            // The closet pivot and rods are stretched on different axes. A separate
            // root keeps that scale from distorting a sprite as it faces the camera.
            spriteRoot = new GameObject("Closet Sprites");
            SceneManager.MoveGameObjectToScene(spriteRoot, gameObject.scene);
            spriteRoot.SetActive(false);
        }

        GameObject artwork = new GameObject($"{hook.hookpoint.name} Sprite");
        artwork.layer = hook.hookpoint.gameObject.layer;
        artwork.transform.SetParent(spriteRoot.transform, false);
        hook.renderer = artwork.AddComponent<SpriteRenderer>();
        return hook.renderer;
    }

    public void SetCurrentHookIndex(int index)
    {
        currentHookIndex = index;
        UpdateUI();
    }

    private void OnOutfitChanged(Clothing_SO clothing)
    {
        // Equipping changes the button state even though the selected hook stays put.
        UpdateEquipButton(clothing);
    }

    public void UpdateUI()
    {
        // The selected hook may not have any clothing, so the name and info labels are cleared.
        Clothing_SO clothing = clothingManager != null
            ? clothingManager.GetClothing(currentHookIndex)
            : null;
        bool locked = clothing != null && clothing.ClothingState;

        if (clothingNameText != null)
            clothingNameText.text = clothing != null && !locked ? clothing.clothingName : string.Empty;
        if (clothingInfoText != null)
            clothingInfoText.text = locked ? "LOCKED" : clothing?.DescriptionText ?? string.Empty;

        UpdateEquipButton(clothing);
    }

    private void UpdateEquipButton(Clothing_SO clothing)
    {
        bool locked = clothing != null && clothing.ClothingState;
        bool equipped = clothing != null && GameSessionData.equippedClothing == clothing;

        if (equipButton != null)
            equipButton.interactable = clothing != null && !locked && !equipped;
        if (equipButtonLabel != null)
            equipButtonLabel.text = locked ? "Locked" : equipped ? "Equipped" : "Equip";
    }

    // The artwork is positioned in LateUpdate so it can follow the camera even if the closet is hidden.
    private void LateUpdate()
    {
        if (spriteRoot == null)
            return;

        // The bootstrapper owns the tagged camera. Resolve it each frame so the
        // closet also works when the camera starts late or is replaced.
        Camera viewCamera = Camera.main;
        if (viewCamera == null)
        {
            spriteRoot.SetActive(false);
            return;
        }

        for (int index = 0; index < hooks.Length; index++)
        {
            ClosetHook hook = hooks[index];
            if (hook?.renderer == null || hook.hookpoint == null || !hook.renderer.enabled)
                continue;

            // A slight phase offset keeps the outfits from swinging in unison.
            float phase = Time.unscaledTime * swingsPerSecond * 2f * Mathf.PI + index * 0.8f;
            float swing = swingsPerSecond > 0f ? Mathf.Sin(phase) * swingDegrees : 0f;
            PositionArtwork(hook, viewCamera, swing);
        }

        if (!spriteRoot.activeSelf)
            spriteRoot.SetActive(true);
    }

    private static void PositionArtwork(ClosetHook hook, Camera viewCamera, float swing)
    {
        Transform artwork = hook.renderer.transform;
        // The hook is a rod mesh centered on its transform. Its lower tip is
        // at local Y = -0.5, and TransformPoint follows that tip through each turn.
        Vector3 anchor = hook.hookpoint.TransformPoint(Vector3.down * 0.5f);
        Bounds bounds = hook.renderer.sprite.bounds;
        Vector3 topCenter = new Vector3(bounds.center.x, bounds.max.y, 0f) * artwork.localScale.x;
        Vector3 center = anchor - viewCamera.transform.up * topCenter.y;
        Vector3 forward = viewCamera.orthographic
            ? viewCamera.transform.forward
            : center - viewCamera.transform.position;
        if (forward.sqrMagnitude < 0.000001f)
            forward = viewCamera.transform.forward;

        Vector3 up = viewCamera.transform.up;
        if (Vector3.Cross(forward.normalized, up).sqrMagnitude < 0.000001f)
            up = viewCamera.transform.right;

        // Roll around the billboard normal so the swing never turns the sprite edge-on.
        Quaternion rotation = Quaternion.LookRotation(forward, up) *
            Quaternion.AngleAxis(swing, Vector3.forward);
        artwork.SetPositionAndRotation(anchor - rotation * topCenter, rotation);
    }
}
