using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupWeaponInformation : UIPopup<PopupWeaponInformationArgs>
    {
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private Text itemName;
        [SerializeField]
        private Text itemStats;
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

        //[SerializeField]
        //private PopupSkillInformation popupSkill;

        public override void Init(PopupWeaponInformationArgs args)
        {
            ItemInfo item = Root.GameInfo.ItemInfos[args.itemCode];

            //itemIcon.sprite = 
            itemName.text = item.Name;
            var stat = item.Stat;
            StringBuilder sb = new StringBuilder();
            foreach (var field in typeof(Stat).GetFields(System.Reflection.BindingFlags.Instance |
                                                 System.Reflection.BindingFlags.NonPublic |
                                                 System.Reflection.BindingFlags.Public))
            {
                sb.AppendFormat(string.Format("{0} = {1}\n", field.Name, field.GetValue(stat)));
            }

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

    public class PopupWeaponInformationArgs : IPopupArgs
    {
        public string PrefabName => "WeaponInformation";
        public long itemCode;
        public bool store;
        public System.Action callBack;

        public PopupWeaponInformationArgs(long code, bool isStore, System.Action action)
        {
            itemCode = code;
            store = isStore;
            callBack = action;
        }
    }
}