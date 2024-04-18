using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupSkillInformation : UIPopup<PopupSkillInformationArgs>
    {
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private Text itemName;
        [SerializeField]
        private Text description;
        [SerializeField]
        private Button closeButton;

        public override void Init(PopupSkillInformationArgs args)
        {
            //itemIcon.sprite = 
            //itemName.text = 
            //description.text = 

            closeButton.onClick.AddListener(() =>
            {
                this.Close();
            });
        }
    }

    public class PopupSkillInformationArgs : IPopupArgs
    {
        public string PrefabName => "SkillInformation";
        public long skillCode;

        public PopupSkillInformationArgs(long code)
        {
            skillCode = code;
        }
    }
}