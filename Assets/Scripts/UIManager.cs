using UnityEngine;
//Image 클래스를 사용하기 위해 필요
using UnityEngine.UI;
//List<T> 컬렉션을 사용하기 위해 필요
using System.Collections.Generic;
//TextMeshPro UI 텍스트를 사용하기 위해 필요
using TMPro;

//UI 관련 기능을 담당하는 매니저 클래스
public class UIManager : MonoBehaviour
{
    [Header("References")]
    public ItemManager itemManager;

    [Header("Game Start Panel")]
    //게임 시작 시 오프라인 보상을 표시하는 패널
    public GameObject gameStartPanel;
    //복귀 시간을 표시하는 텍스트
    public TMP_Text comebackText;
    //오프라인 보상 골드를 표시하는 텍스트
    public TMP_Text addGoldText;

    [Header("Upgrade Buttons")]
    public List<Image> clickHiddenImages = new();
    public List<Image> autoClickHiddenImages = new();
    //List<T> : 여러 개의 같은 타입 데이터를 저장하는 동적 배열
    //new() : 빈 리스트로 초기화
    //클릭 업그레이드 버튼들의 텍스트 리스트
    public List<TMP_Text> clickUpgradeButtonTexts = new();
    //자동 클릭 업그레이드 버튼들의 텍스트 리스트
    public List<TMP_Text> autoClickUpgradeButtonTexts = new();

    [Header("Status Texts")]
    //현재 골드를 표시하는 텍스트
    public TMP_Text currentGoldText;
    //현재 클릭당 골드를 표시하는 텍스트
    public TMP_Text currentClickText;
    //초당 자동 골드를 표시하는 텍스트
    public TMP_Text autoClickText;

    //버튼 활성화 색상
    private readonly Color activeColor = new Color(1, 1, 1, 1);
    //버튼 비활성화 색상
    private readonly Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1);

    //Start : 첫 번째 프레임 전에 1회 호출
    public void Start()
    {
        //오프라인 보상 계산 (out으로 여러 값을 받아옴)
        GameManager.Instance.CalculateOfflineReward(out int hours, out int minutes, out int seconds, out long reward);

        //보상이 있으면 패널 표시
        if (reward > 0)
        {
            //SetActive(true) : 게임 오브젝트 활성화
            gameStartPanel.SetActive(true);
            //$ : 문자열 보간 (변수를 {}안에 넣어서 문자열에 포함)
            comebackText.text = $"{hours}시간 {minutes}분 {seconds}초 만에 다시 오셨네요.";
            //NumberFormatter.Format() : 큰 숫자를 한글 단위(만, 억, 조, 경)로 변환 및 1000을 1,000으로 표기, 너무 작은 단위는 생략, 그래서 예를 들어 34545671004일 경우, 345억 4,567만으로 표기
            addGoldText.text = $"Add Gold : {NumberFormatter.Format(reward)}";
        }
    }

    void Update()
    {
        //상태 텍스트 업데이트
        UpdateStatusTexts();
        //업그레이드 버튼 UI 업데이트
        UpdateUpgradeButtonUIs();
    }

    //오프라인 보상 패널의 취소 버튼 클릭 시 호출
    public void CancelButtonClick()
    {
        //SetActive(false) : 게임 오브젝트 비활성화
        gameStartPanel.SetActive(false);
    }

    //현재 골드, 클릭, 자동 클릭 텍스트를 업데이트하는 함수
    private void UpdateStatusTexts()
    {
        currentGoldText.text = $"Current Gold : {NumberFormatter.Format(GameManager.Instance.GetGold())}";
        currentClickText.text = $"Current Click : {NumberFormatter.Format(GameManager.Instance.GetClick())}";
        autoClickText.text = $"Auto Click : {NumberFormatter.Format(GameManager.Instance.GetAutoClick())}";
    }

    //업그레이드 버튼들의 UI를 업데이트하는 함수
    private void UpdateUpgradeButtonUIs()
    {
        //for문 : 반복문, i가 0부터 Count-1까지 1씩 증가하며 반복
        for (int i = 0; i < clickUpgradeButtonTexts.Count; i++)
        {
            //var : 컴파일러가 자동으로 타입 추론
            //축약하기 위해 공통적인 부분을 item으로 묶음
            var item = itemManager.clickUpgrades[i];
            //\n : 줄바꿈, 업그레이드 버튼의 텍스트를 업데이트
            clickUpgradeButtonTexts[i].text = $"Level : {item.level} \nAdd Click : {NumberFormatter.Format(item.addClick)} \nCost : {NumberFormatter.Format(item.cost)}";
            //구매할 돈이 있으면, 업그레이드 버튼이 흰색, 아니면 회색으로 표시
            clickHiddenImages[i].color = GameManager.Instance.GetGold() >= item.cost ? activeColor : inactiveColor;
        }

        for (int i = 0; i < autoClickUpgradeButtonTexts.Count; i++)
        {
            var item = itemManager.autoClickUpgrades[i];
            autoClickUpgradeButtonTexts[i].text = $"Level : {item.level} \nAdd Auto Click : {NumberFormatter.Format(item.addAutoClick)} \nCost : {NumberFormatter.Format(item.cost)}";
            
            autoClickHiddenImages[i].color = GameManager.Instance.GetGold() >= item.cost ? activeColor : inactiveColor;
        }
    }

    //메인 버튼(골드 획득 버튼) 클릭 시 호출
    public void MainButtonClick()
    {
        //클릭당 골드만큼 골드 추가
        GameManager.Instance.AddGold(GameManager.Instance.GetClick());
    }

    //클릭 업그레이드 버튼 클릭 시 호출
    //index : 몇 번째 업그레이드인지 (0부터 시작)
    public void ClickUpgradeButtonClick(int index)
    {
        var item = itemManager.clickUpgrades[index];
        //골드가 부족하면 구매 불가
        if (!TryPurchase(item.cost)) return;

        //클릭당 골드 증가
        GameManager.Instance.AddClick(item.addClick);
        //다음 레벨로 업그레이드
        itemManager.NextClickUpgrade(index);
    }

    //자동 클릭 업그레이드 버튼 클릭 시 호출
    public void AutoClickUpgradeButtonClick(int index)
    {
        var item = itemManager.autoClickUpgrades[index];
        //골드가 부족하면 구매 불가
        if (!TryPurchase(item.cost)) return;

        //초당 자동 골드 증가
        GameManager.Instance.AddAutoClick(item.addAutoClick);
        itemManager.NextAutoClickUpgrade(index);
    }

    private bool TryPurchase(long cost)
    {
        //골드가 부족하면 구매 불가
        if (GameManager.Instance.GetGold() >= cost)
        {
            //비용 차감
            GameManager.Instance.MinusGold(cost); 
            //구매 성공
            return true;
        }
        
        //구매 실패
        return false;
    }
}
