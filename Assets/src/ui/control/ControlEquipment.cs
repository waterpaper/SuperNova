using UniRx;
using UnityEngine;
using System.Linq;

namespace Supernova.Unity.UI
{
    public class ControlEquipment : MonoBehaviour
    {
        [SerializeField]
        private ControlItem itemWeapon, itemArmor, itemAccL, itemAccR;

        private void Awake()
        {
            Root.State.Inventory.Weapon
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    itemWeapon.ItemInstance.Value = item;
                })
                .AddTo(this);

            Root.State.Inventory.Armor
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    itemArmor.ItemInstance.Value = item;
                })
                .AddTo(this);

            Root.State.Inventory.AccessoryLeft
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    itemAccL.ItemInstance.Value = item;
                })
                .AddTo(this);

            Root.State.Inventory.AccessoryRight
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    itemAccR.ItemInstance.Value = item;
                })
                .AddTo(this);
        }
    }
}
