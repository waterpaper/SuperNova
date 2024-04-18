using UnityEngine;

namespace Supernova.Unity
{
    public abstract class UIPopup<TPopupArgs> : MonoBehaviour
        where TPopupArgs : IPopupArgs
    {
        public bool IsClosed { get; private set; } = false;

        protected void Close()
        {
            this.IsClosed = true;
            this.gameObject.SetActive(false);
        }

        public abstract void Init(TPopupArgs args);
    }

    public interface IPopupArgs
    {
        string PrefabName { get; }
    }
}