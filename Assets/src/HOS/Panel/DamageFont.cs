using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Supernova.Unity;

public class DamageFont : MonoBehaviour
{
    public RectTransform rectTransform;

    public TextMeshProUGUI textMain;

    public TextMeshProUGUI textAdd;

    public TMP_ColorGradient normalColorGradient;
    public TMP_ColorGradient criticalColorGradient;
    //private Color32 normalColorTop = new Color32(255, 93, 93, 255);
    //private Color32 normalColorBottom = new Color32(255, 255, 255, 255);

    //private Color32 criticalColorTop = new Color32(91, 0, 143, 255);
    //private Color32 criticalColorBottom = new Color32(202, 0, 46, 255);

    public void SetDamage(double damage, bool isCritical, double additionalDamage)
    {
        textMain.SetText(damage.ToString());

        if (additionalDamage != double.NaN && additionalDamage > 0)
        {
            textAdd.SetText(additionalDamage.ToString());
        }
        else
        {
            textAdd.SetText("");
        }

        if (isCritical)
        {
            textMain.colorGradientPreset = normalColorGradient;
            // new TMP_ColorGradient(criticalColorTop, criticalColorTop, criticalColorBottom, criticalColorBottom);
        }
        else
        {
            textMain.colorGradientPreset = criticalColorGradient;
                // new TMP_ColorGradient(normalColorTop, normalColorTop, normalColorBottom, normalColorBottom);
        }
    }

    public void Animate(Vector3 position)
    {
        Vector3 fontWorldPosition = position +
                    new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.0f, 1.0f), Random.Range(-0.5f, 0.5f));

        rectTransform.position = Root.World.WorldCamera.WorldToScreenPoint(fontWorldPosition);
        rectTransform.localScale = new Vector3(3.0f, 3.0f, 1);

        rectTransform.DOMoveY(rectTransform.position.y + 20, 1.0f);
        rectTransform.DOScale(Vector3.one * 2, 0.05f);
        DOTween.ToAlpha(() => textMain.color, p => textMain.color = p, 0.0f, 1.0f);
        DOTween.ToAlpha(() => textAdd.color, p => textAdd.color = p, 0.0f, 1.0f);
    }
}
