using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoBox : MonoBehaviour
{
    enum InfoType{Normal, Rocket, UltraBeam};

    [SerializeField] float durationSpecial = 60f;

    TextMeshProUGUI info;
    InfoType infoType = InfoType.Normal;
    bool hasShownRocketInfo = false, hasShownUltraBeamInfo = false;
    Coroutine isShowing;

    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<TextMeshProUGUI>();
        info.enabled = false;
    }

    public void ShowInfo(string text, float duration)
    {
        if (infoType == InfoType.Normal)
        {
            if (isShowing != null)
                StopInfo();
            isShowing = StartCoroutine(PrintInfo(text, duration));
        }
    }

    private void ShowInfoSpecial(string text, float duration, InfoType type)
    {
        if (isShowing != null)
            StopInfo();
        infoType = type;
        isShowing = StartCoroutine(PrintInfo(text, duration));
    }

    private IEnumerator PrintInfo(string text, float duration)
    {
        info.text = text;
        info.enabled = true;
        yield return new WaitForSeconds(duration);
        info.enabled = false;
        infoType = InfoType.Normal;
        isShowing = null;
    }

    public void ShowUltraBeamInfo()
    {
        if (!hasShownUltraBeamInfo)
        {
            hasShownUltraBeamInfo = true;
            ShowInfoSpecial("Press c for ULTRA BEAM", durationSpecial, InfoType.UltraBeam);
        }
    }

    public void ShowRocketInfo()
    {
        if (!hasShownRocketInfo)
        {
            hasShownRocketInfo = true;
            ShowInfoSpecial("Press x for rockets", durationSpecial, InfoType.Rocket);
        }
    }

    public void ClearRocketInfo()
    {
        if (infoType == InfoType.Rocket)
            StopInfo();
    }

    public void ClearUltraBeamInfo()
    {
        if (infoType == InfoType.UltraBeam)
            StopInfo();
    }

    private void StopInfo()
    {
        StopCoroutine(isShowing);
        isShowing = null;
        info.enabled = false;
        infoType = InfoType.Normal;
    }
}
