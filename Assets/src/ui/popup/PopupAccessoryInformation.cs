using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupAccessoryInformation : UIPopup<PopupAccessoryInformationArgs>
    {
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private Text itemName;
        [SerializeField]
        private Button skillIcon1;
        [SerializeField]
        private Button skillIcon2;
        [SerializeField]
        private Text skillText1;
        [SerializeField]
        private Text skillText2;
        [SerializeField]
        private Button funcButton;
        [SerializeField]
        private Text funcText;

        [SerializeField]
        private PopupSkillInformation popupSkill;

        public override void Init(PopupAccessoryInformationArgs args)
        {
            ItemInfo item = Root.GameInfo.ItemInfos[args.itemCode];

            //itemIcon.sprite = 
            itemName.text = item.Name;

            //skillIcon1.image.sprite = 
            //skillIcon2.image.sprite = 
            //skillText1.text = 
            //skillText2.text = 

            skillIcon1.onClick.AddListener(() =>
            {
                //popupSkill.gameObject.SetActive(true);
                //popupSkill.Init(new PopupSkillInformationArgs(0));
                Root.PopupManager.ShowPopup(new PopupSkillInformationArgs(0));
            });
            skillIcon2.onClick.AddListener(() =>
            {
                //popupSkill.gameObject.SetActive(true);
                //popupSkill.Init(new PopupSkillInformationArgs(0));
                Root.PopupManager.ShowPopup(new PopupSkillInformationArgs(0));
            });

            if (args.store)
            {
                funcText.text = "판  매";
                funcButton.onClick.AddListener(() =>
                {
                    // 판매로직 작동 또는 콜백 받아서 콜백을 실행
                    args.callBack();
                    this.Close();
                });
            }
            else
            {
                funcText.text = "조  합";
                funcButton.onClick.AddListener(() =>
                {
                    // 조합로직 작동 또는 콜백 받아서 콜백을 실행
                    args.callBack();
                    this.Close();
                });
            }
        }
    }

    public class PopupAccessoryInformationArgs : IPopupArgs
    {
        public string PrefabName => "AccessoryInformation";
        public long itemCode;
        public bool store;
        public System.Action callBack;

        public PopupAccessoryInformationArgs(long code, bool isStore, System.Action action)
        {
            itemCode = code;
            store = isStore;
            callBack = action;
        }
    }
}