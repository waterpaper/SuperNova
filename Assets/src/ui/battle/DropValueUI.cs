using UnityEngine;
using DG.Tweening;

public class DropValueUI : MonoBehaviour
{
    private void OnEnable()
    {
        this.Setting();
    }

    public void Setting()
    {
        Vector3 position = transform.position + new Vector3(0, 15, 0);

        transform.DOMove(position, 1.5f)
           .SetEase(Ease.Linear)
           .OnComplete(() =>
           {
               gameObject.SetActive(false);
           });
    }
}
