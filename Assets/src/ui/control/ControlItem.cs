using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class ControlItem : MonoBehaviour
    {
        public const string RES_PATH = "Assets/res/prefab/UI/ControlItem.prefab";
        public const string RES_PATH_ITEM_ICON = "Assets/res/img/skill_icon.jpg"; // TODO: filename must be modified to real path.

        [SerializeField]
        private RawImage itemIcon;
        [SerializeField]
        private Image imageBorder;

        public ReactiveProperty<ItemInstance> ItemInstance { get; private set; } = new ReactiveProperty<ItemInstance>(null);

        private void Awake()
        {
            if (imageBorder != null) { imageBorder.gameObject.SetActive(false); }
            ItemInstance
                .Subscribe(itemInstance =>
                {
                    if (itemInstance != null)
                    {
                        Root.World.StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(itemInstance.ItemID), texture =>
                        {
                            itemIcon.texture = texture;
                        }));
                        itemIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        itemIcon.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);

            Observable
                .CombineLatest(ItemInstance, Root.State.Inventory.Weapon, Root.State.Inventory.Armor, Root.State.Inventory.AccessoryLeft, Root.State.Inventory.AccessoryRight, (item, _1, _2, _3, _4) => item)
                .Where(p => imageBorder != null && p != null)
                .Subscribe(itemInstance =>
                {
                    imageBorder.gameObject.SetActive(
                        Root.State.Inventory.Weapon.Value == itemInstance.Guid ||
                        Root.State.Inventory.Armor.Value == itemInstance.Guid ||
                        Root.State.Inventory.AccessoryLeft.Value == itemInstance.Guid ||
                        Root.State.Inventory.AccessoryRight.Value == itemInstance.Guid
                        );
                })
                .AddTo(this);

            this.GetComponent<Button>()
                .OnClickAsObservable()
                .Select(_ => ItemInstance.Value)
                .Where(item => item != null)
                .Subscribe(item =>
                {
                    Root.PopupManager.ShowPopup(new PopupItemInfoArgs(item));
                })
                .AddTo(this);
        }
    }
}
