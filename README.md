# SuperNova
3d 클리커 & 방치형 팀 게임 프로젝트<br/>
보수를 통해 unity 버전 21.3까지 테스트 되었습니다.<br/>

- 영상
[SuperNova 플레이](https://www.youtube.com/watch?v=u_XDa_1LdT0)

**목표**

- 게임 프로젝트 제작
- 추가적인 라이브러리 스터디 용도

**맡은 기능**

- 인게임 개발(플레이어, 전투, 맵)
- 아웃게임 개발(아이템 등)
- 시스템 적용

- 라이브러리 사용(Addressable, UniRx, Dotween)

---
<br/>

## ⚒️ 개발 기능


### 전투

---

### 플레이어

[Character.cs](./Assets/src/battle/character/Character.cs)<br/><br/>

- 아이템, 훈련으로 지속 성장(data 부분에 별도 저장)
- 각 무기에 스킬 장착<br/>
   → 스킬 마다 애니메이션 연결

- UniRx를 이용해 UI 및 시스템 연결<br/>
     (능력치 및 인벤토리)<br/><br/>
[State.cs](./Assets/src/sys/State.cs)<br/>
```cs
  public class State
  {
      public IStateLogic Logic { get; }
      public ReactiveProperty<Stat> Stat { get; } = new ReactiveProperty<Stat>();
      public ReactiveProperty<FinalStat> FinalStat { get; } = new ReactiveProperty<FinalStat>();
      public General General { get; } = new General();
      public Currency Currency { get; } = new Currency();
      public Inventory Inventory { get; } = new Inventory();
      public Upgrade Upgrade { get; } = new Upgrade();
      ...

    public State(IStateLogic logic, MonoBehaviour mono)
{
    this.Logic = logic;

    Observable.CombineLatest(
        Upgrade.OnChangedAsObservable(),
        Inventory.Weapon, Inventory.Armor, Inventory.AccessoryLeft, Inventory.AccessoryRight,
        (upgrade, weapon, armor, accleft, accright) => (
            upgrade,
            weapon      : Inventory.Items.FirstOrDefault(p => p.Guid == weapon),
            armor       : Inventory.Items.FirstOrDefault(p => p.Guid == armor),
        ))
        .Subscribe(data =>
        {
            var upgradeStat = new Stat(
                attack              : logic.StatUpgradeCalculator.Attack(data.upgrade.Attack.Value),
                attackSpeed         : logic.StatUpgradeCalculator.AttackSpeed(data.upgrade.AttackSpeed.Value),
                attackSpeedAmp      : logic.StatUpgradeCalculator.AttackSpeedAmp(data.upgrade.AttackSpeedAmp.Value),
                cooltimeDecrease    : logic.StatUpgradeCalculator.CooltimeDecrease(data.upgrade.CooltimeDecrease.Value),
                criticalProbabilty  : logic.StatUpgradeCalculator.CriticalProbability(data.upgrade.CriticalProbability.Value),
                criticalDamage      : logic.StatUpgradeCalculator.CriticalDamage(data.upgrade.CriticalDamage.Value),
            );

            var weaponStat      = data.weapon == null   ? new Stat() : Root.GameInfo.ItemInfos[data.weapon.ItemID].Stat + Root.GameInfo.ItemInfos[data.weapon.ItemID].UpgradeStat * data.weapon.Enchant;
            var armorStat       = data.armor == null    ? new Stat() : Root.GameInfo.ItemInfos[data.armor.ItemID].Stat + Root.GameInfo.ItemInfos[data.armor.ItemID].UpgradeStat * data.armor.Enchant;
            var accleftStat     = data.accleft == null  ? new Stat() : Root.GameInfo.ItemInfos[data.accleft.ItemID].Stat + Root.GameInfo.ItemInfos[data.accleft.ItemID].UpgradeStat * data.accleft.Enchant;
            var accrightStat    = data.accright == null ? new Stat() : Root.GameInfo.ItemInfos[data.accright.ItemID].Stat + Root.GameInfo.ItemInfos[data.accright.ItemID].UpgradeStat * data.accright.Enchant;

            var stat = upgradeStat + weaponStat + armorStat + accleftStat + accrightStat;

            this.Stat.SetValueAndForceNotify(stat);
            this.FinalStat.SetValueAndForceNotify((stat).Finalize());
        })
        .AddTo(mono);

    ...
  }
```
---

### 몬스터 및 맵

- 무한 맵 방식(풀링 이용)<br/>

     → 수치 증가 방지 위해 맵 이동으로 처리

    [TileManager.cs](./Assets/src/battle/tile/TileManager.cs)<br/><br/>


- 스테이지 별 몬스터 반복 생성 및 전투 진행<br/>

     (보스, 엘리트, 일반 몬스터)
     
    [BattleManager.cs](./Assets/src/battle/BattleManager.cs)<br/><br/>


- 공격 체크 및 데미지 처리(거리 기반)<br/><br/>

### 성장 시스템

---

### 아이템

- 무기, 방어구, 악세사리로 구성
- 등급 별 능력치 및 드랍 아이템 처리<br/>

    (보스, 엘리트, 일반 몬스터 테이블 파일에 따라 처리)<br/>

```cs
    private void DropItem(Character character, Enemy enemy)
    {
        dropCoin.Setting(character, enemy, enemy.Stage * 10, enemy.Stage * 10);

        var itemIndex = Root.State.Logic.DropItem();

        Debug.LogFormat("item: {0} 을 획득합니다.", itemIndex);
        if (itemIndex != -1)
        {
            Root.State.Inventory.Items.Add(ItemInstance.Create(itemIndex));
            dropItem.UICreate(3, itemIndex);
        }
    }
```

### 연출

- UI 및 3d 아이템 획득 연출 구현
- 화면 클릭 시 추가 재화 획득 구현

    [DropCoin.cs](./Assets/src/ui/battle/DropCoin.cs)<br/><br/>
---

### ✏️ 라이브러리
<br/>

### UniRx

- UI등 변경 사항이 많은 능력치와 인벤토리 데이터 변경 부분에 사용
- 비동기 이벤트 프로그래밍 효율적으로 하기 위해 도입했으며 비동기에 대한 이해와 구조를 배움

### Addressable

- Unity에서 지원하는 Assetbundle 관리 시스템을 활용하고자 도입
- 초기 용량 절감 확인 및 로컬 환경 테스트