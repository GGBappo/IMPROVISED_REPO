using UnityEngine;
using System.Collections;

public class SidePanel : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool isOpen = false;
    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnValidate()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void TogglePanel()
    {
        if (!isOpen)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnRequestShowShop += ShowShopComplete;
        GameEvents.OnRequestHideShop += HideShopComplete;
    }

    private void OnDisable()
    {
        GameEvents.OnRequestShowShop -= ShowShopComplete;
        GameEvents.OnRequestHideShop -= HideShopComplete;
    }

    public void OpenPanel()
    {
        if (isOpen) return;
        anim.SetTrigger("Show");
        isOpen = true;
    }

    public void ClosePanel()
    {
        if (!isOpen) return;
        anim.SetTrigger("Hide");
        isOpen = false;
    }

    private void HideShopComplete()
    {
        if (isOpen)
        {
            ClosePanel();
        }
        StartCoroutine(FadeOut());
    }

    private void ShowShopComplete()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 2f;
            cg.alpha = t;
            yield return null;
        }
        cg.alpha = 0;
    }

    private IEnumerator FadeIn()
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            cg.alpha = t;
            yield return null;
        }
        cg.alpha = 1;
    }
}