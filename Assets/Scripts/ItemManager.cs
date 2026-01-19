//Math 클래스(수학 함수)를 사용하기 위해 필요
using System;
using System.Collections.Generic;
//CultureInfo를 사용하기 위해 필요 (소수점 저장 시 지역 설정에 영향받지 않도록)
using System.Globalization;
using UnityEngine;

//[System.Serializable] : 이 클래스를 인스펙터에서 볼 수 있게 해주는 속성
//업그레이드 아이템의 공통 부분을 정의한 기본 클래스
[System.Serializable]
public class UpgradeItem
{
    //현재 레벨
    public int level = 1;
    //레벨업 시 효과 증가 배율 (1.1 = 10% 증가)
    public float effectRiseMultiplier = 1.1f;
    //기본 비용 (인스펙터에서 설정)
    public long basicCost;
    //현재 비용 (레벨업마다 증가)
    public long cost;
    //비용 배율 (내부 계산용, double은 더 정밀한 소수점 타입)
    public double costMultiplier;
    //레벨업 시 비용 증가 배율
    public float costRiseMultiplier = 1.1f;
}

//UpgradeItem을 상속받은 클릭 업그레이드 전용 클래스
// : UpgradeItem = UpgradeItem 클래스를 상속 (부모의 모든 변수와 함수를 물려받음)
[System.Serializable]
public class ClickUpgradeItem : UpgradeItem
{
    //기본 클릭당 추가 골드 (인스펙터에서 설정)
    public long basicAddClick;
    //현재 클릭당 추가 골드
    public long addClick;
    //클릭 추가량 배율 (내부 계산용)
    public double addClickMultiplier;
}

//UpgradeItem을 상속받은 자동 클릭 업그레이드 전용 클래스
[System.Serializable]
public class AutoClickUpgradeItem : UpgradeItem
{
    //기본 초당 추가 자동 골드 (인스펙터에서 설정)
    public long basicAddAutoClick;
    //현재 초당 추가 자동 골드
    public long addAutoClick;
    //자동 클릭 추가량 배율 (내부 계산용)
    public double addAutoClickMultiplier;
}

//업그레이드 아이템들을 관리하는 매니저 클래스
public class ItemManager : MonoBehaviour
{
    //클릭 업그레이드 아이템 리스트 (인스펙터에서 설정)
    public List<ClickUpgradeItem> clickUpgrades = new();
    //자동 클릭 업그레이드 아이템 리스트
    public List<AutoClickUpgradeItem> autoClickUpgrades = new();

    //클릭 업그레이드를 다음 레벨로 올리는 함수
    public void NextClickUpgrade(int index)
    {
        var item = clickUpgrades[index];
        //효과 배율 증가 (오버플로우 방지 적용)
        item.addClickMultiplier = ClampMultiplier(item.addClickMultiplier * item.effectRiseMultiplier);
        //배율을 실제 값으로 변환
        item.addClick = ClampToMax(item.addClickMultiplier);
        //공통 업그레이드 처리 (레벨, 비용 증가)
        NextUpgrade(item);
    }

    //자동 클릭 업그레이드를 다음 레벨로 올리는 함수
    public void NextAutoClickUpgrade(int index)
    {
        var item = autoClickUpgrades[index];
        item.addAutoClickMultiplier = ClampMultiplier(item.addAutoClickMultiplier * item.effectRiseMultiplier);
        item.addAutoClick = ClampToMax(item.addAutoClickMultiplier);
        NextUpgrade(item);
    }

    //업그레이드 공통 처리 함수 (레벨 증가, 비용 증가)
    private void NextUpgrade(UpgradeItem item)
    {
        //레벨 1 증가
        item.level++;
        //비용 배율 증가
        item.costMultiplier = ClampMultiplier(item.costMultiplier * item.costRiseMultiplier);
        //배율을 실제 비용으로 변환
        item.cost = ClampToMax(item.costMultiplier);
    }

    //배율이 최대값을 넘지 않도록 제한하는 함수
    //Math.Min(a, b) : a와 b 중 작은 값 반환
    private double ClampMultiplier(double value)
    {
        return Math.Min(value, GameManager.MAX_VALUE);
    }

    //배율(double)을 실제 값(long)으로 변환하는 함수
    private long ClampToMax(double value)
    {
        //최대값 이상이면 최대값 반환
        if (value >= GameManager.MAX_VALUE)
            return GameManager.MAX_VALUE;
        //Math.Round() : 반올림
        //(long) : long 타입으로 형변환
        return (long)Math.Round(value);
    }

    //업그레이드 데이터를 저장하는 함수
    public void Save()
    {
        //모든 클릭 업그레이드 저장
        for (int i = 0; i < clickUpgrades.Count; i++)
        {
            var item = clickUpgrades[i];
            //SetInt : 정수 저장, "키" + i로 각 아이템 구분
            PlayerPrefs.SetInt("Click_Level" + i, item.level);
            PlayerPrefs.SetString("Click_Cost" + i, item.cost.ToString());
            //CultureInfo.InvariantCulture : 지역 설정에 관계없이 일관된 형식으로 저장
            //(한국은 소수점에 '.', 유럽 일부 국가는 ','를 사용하므로)
            PlayerPrefs.SetString("Click_CostMultiplier" + i, item.costMultiplier.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString("Click_AddClick" + i, item.addClick.ToString());
            PlayerPrefs.SetString("Click_AddClickMultiplier" + i, item.addClickMultiplier.ToString(CultureInfo.InvariantCulture));
        }

        //모든 자동 클릭 업그레이드 저장
        for (int i = 0; i < autoClickUpgrades.Count; i++)
        {
            var item = autoClickUpgrades[i];
            PlayerPrefs.SetInt("AutoClick_Level" + i, item.level);
            PlayerPrefs.SetString("AutoClick_Cost" + i, item.cost.ToString());
            PlayerPrefs.SetString("AutoClick_CostMultiplier" + i, item.costMultiplier.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString("AutoClick_AddAutoClick" + i, item.addAutoClick.ToString());
            PlayerPrefs.SetString("AutoClick_AddAutoClickMultiplier" + i, item.addAutoClickMultiplier.ToString(CultureInfo.InvariantCulture));
        }
    }

    //저장된 업그레이드 데이터를 불러오는 함수
    public void Load()
    {
        //모든 클릭 업그레이드 불러오기
        for (int i = 0; i < clickUpgrades.Count; i++)
        {
            var item = clickUpgrades[i];
            //GetInt(키, 기본값) : 저장된 값이 없으면 기본값 사용
            item.level = PlayerPrefs.GetInt("Click_Level" + i, 1);
            //long.Parse() : 문자열을 long으로 변환
            item.cost = long.Parse(PlayerPrefs.GetString("Click_Cost" + i, item.basicCost.ToString()));
            //double.Parse() : 문자열을 double로 변환
            item.costMultiplier = double.Parse(PlayerPrefs.GetString("Click_CostMultiplier" + i, item.basicCost.ToString()), CultureInfo.InvariantCulture);
            item.addClick = long.Parse(PlayerPrefs.GetString("Click_AddClick" + i, item.basicAddClick.ToString()));
            item.addClickMultiplier = double.Parse(PlayerPrefs.GetString("Click_AddClickMultiplier" + i, item.basicAddClick.ToString()), CultureInfo.InvariantCulture);
        }

        //모든 자동 클릭 업그레이드 불러오기
        for (int i = 0; i < autoClickUpgrades.Count; i++)
        {
            var item = autoClickUpgrades[i];
            item.level = PlayerPrefs.GetInt("AutoClick_Level" + i, 1);
            item.cost = long.Parse(PlayerPrefs.GetString("AutoClick_Cost" + i, item.basicCost.ToString()));
            item.costMultiplier = double.Parse(PlayerPrefs.GetString("AutoClick_CostMultiplier" + i, item.basicCost.ToString()), CultureInfo.InvariantCulture);
            item.addAutoClick = long.Parse(PlayerPrefs.GetString("AutoClick_AddAutoClick" + i, item.basicAddAutoClick.ToString()));
            item.addAutoClickMultiplier = double.Parse(PlayerPrefs.GetString("AutoClick_AddAutoClickMultiplier" + i, item.basicAddAutoClick.ToString()), CultureInfo.InvariantCulture);
        }
    }
}
