using UnityEngine;
using UnityEngine.UI;

public class VersatilePanel : MonoBehaviour
{
    [SerializeField]
    private Button character;
    [SerializeField]
    private Button smith;
    [SerializeField]
    private Button inventory;
    [SerializeField]
    private Button workshop;

    [SerializeField]
    private RectTransform characterPanel;
    [SerializeField]
    private RectTransform smithPanel;
    [SerializeField]
    private RectTransform inventoryPanel;
    [SerializeField]
    private RectTransform workshopPanel;

    [SerializeField]
    private Text gold;
    [SerializeField]
    private Text soul;
    [SerializeField]
    private Text ruby;

    [SerializeField]
    private Image weapon;
    [SerializeField]
    private Image armor;
    [SerializeField]
    private Image accessory1;
    [SerializeField]
    private Image accessory2;
    private string imagePrefix = "/InGame/Equipments";

    //[SerializeField]
    //private Image skillSlot1;
    //[SerializeField]
    //private Image skillSlot2;

    [SerializeField]
    private Slider bossHPBar;

    private RectTransform currentPanel;
    void Awake()
    {
        //currentPanel = characterPanel;

        //character.onClick.AddListener(() =>
        //{
        //    ChangePanel("character");
        //});
        //smith.onClick.AddListener(() =>
        //{
        //    ChangePanel("smith");
        //});
        //inventory.onClick.AddListener(() =>
        //{
        //    ChangePanel("inventory");
        //});
        //workshop.onClick.AddListener(() =>
        //{
        //    ChangePanel("workshop");
        //});

        //Root.User.Currency.Subscribe((data) =>
        //{
        //    gold.text = data.Gold;
        //    soul.text = data.Soul.ToString();
        //    ruby.text = data.Ruby.ToString();
        //});

        //Root.User.Equipments.Subscribe((data) =>
        //{
        //    // 각 장비 스테이터스 별 이미지 로딩 및 적용

        //    weapon.sprite = Resources.Load<Sprite>(string.Format("{0}/{1}/{2}", imagePrefix, "Weapon", data.GetEnumerator().Current.Index);
        //    armor.sprite = Resources.Load<Sprite>();
        //    accessory1.sprite = Resources.Load<Sprite>();
        //    accessory2.sprite = Resources.Load<Sprite>();
        //});

        //Root.User.Skill.Subscribe((data) =>
        //{

        //});
    }

    //private void ChangePanel(string panelName)
    //{
    //    currentPanel.gameObject.SetActive(false);
    //    switch (panelName)
    //    {
    //        case "character":
    //            characterPanel.gameObject.SetActive(true);
    //            currentPanel = characterPanel;
    //        break;
    //        case "smith":
    //            smithPanel.gameObject.SetActive(true);
    //        currentPanel = smithPanel;
    //        break;
    //        case "inventory":
    //            inventoryPanel.gameObject.SetActive(true);
    //            currentPanel = inventoryPanel;
    //        break;
    //        case "workshop":
    //            workshopPanel.gameObject.SetActive(true);
    //            currentPanel = workshopPanel;
    //        break;
    //    }
    //}

    //public void AppearBoss(보스 입력 받자)
    //{
    //    bossHPBar.gameObject.SetActive(true);

    //    // HP에 맞게 보스 체력 설정
    //    // float ratio = 보스 현재 체력 / 보스 최대 체력
    //}

    public void DisappearBoss()
    {
        bossHPBar.gameObject.SetActive(false);
    }
}
