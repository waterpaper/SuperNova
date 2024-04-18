using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class ControlMenuBar : MonoBehaviour
    {
        [SerializeField]
        private Text textTitle;
        [SerializeField]
        private Toggle buttonCharacter, buttonInventory, buttonWorkshop;
        [SerializeField]
        private GameObject tabCharacter, tabInventory, tabWorkshop;
        private ReactiveProperty<Menu> menu = new ReactiveProperty<Menu>(Menu.Character);

        public IObservable<Menu> MenuAsObservable() => menu;

        private void Awake()
        {
            buttonCharacter.OnValueChangedAsObservable().Where(p => p).Subscribe(_ => menu.Value = Menu.Character).AddTo(this);
            buttonInventory.OnValueChangedAsObservable().Where(p => p).Subscribe(_ => menu.Value = Menu.Inventory).AddTo(this);
            buttonWorkshop.OnValueChangedAsObservable().Where(p => p).Subscribe(_ => menu.Value = Menu.Workshop).AddTo(this);

            buttonCharacter.SetIsOnWithoutNotify(menu.Value == Menu.Character);
            buttonInventory.SetIsOnWithoutNotify(menu.Value == Menu.Inventory);
            buttonWorkshop.SetIsOnWithoutNotify(menu.Value == Menu.Workshop);

            menu
                .Subscribe(menu =>
                {
                    tabCharacter.SetActive(menu == Menu.Character);
                    tabInventory.SetActive(menu == Menu.Inventory);
                    tabWorkshop.SetActive(menu == Menu.Workshop);

                    switch (menu)
                    {
                    case Menu.Character:
                        textTitle.text = "캐릭터";
                        break;
                    case Menu.Inventory:
                        textTitle.text = "인벤토리";
                        break;
                    case Menu.Workshop:
                        textTitle.text = "훈련";
                        break;
                    }
                })
                .AddTo(this);
        }
    }

    public enum Menu
    {
        Character,
        Inventory,
        Workshop,
    }
}
