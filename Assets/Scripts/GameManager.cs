//시간(DateTime 클래스)을 구하기 위해서 선언
using System;
//1초마다(IEnumrator, 코루틴) 자동 클릭 및 저장하기 위해서 선언
using System.Collections;
//Unity의 핵심 기능들을 사용하기 위해 필요, ex)MonoBehaviour, PlayerPrefs
using UnityEngine;

//public : 어디서든 접근 가능, class : 클래스(변수(int, string 등)와 함수(void)를 하나로 묶은 설계도, 이 설계도로 실체 객체를 만들어 사용 ex)GameManager gameManager = new GameManager()) 선언,
//GameManager : 클래스 이름, MonoBehaviour : MonoBehaviour(Awake, Start와 같은 생명주기 사용 가능, 게임 오브젝트에 부착 가능하게 해주는 특수 클래스)를 상속받음
public class GameManager : MonoBehaviour
{
    //private : 이 클래스 내부에서만 접근 가능, static : 클래스 전체에서 딱 하나만 존재, GameManager : 이 변수에 담을 수 있는 타입, instance : 변수 이름
    private static GameManager instance;

    // 프로퍼티(변수처럼 보이지만 내부에 로직을 넣을 수 있는 특수한 형태, ex)get : 값 가져올 때 실행, set : 값 설정할 때 실행)
    public static GameManager Instance
    {
        // 값을 가져올 때 실행되는 코드
        get
        {
            // 1. instance가 비어있는지 확인
            if (instance == null)
            {
                // 2. 비어있으면 씬에서 GameManager를 찾아서 저장
                instance = FindAnyObjectByType<GameManager>();
            }
            // 3. instance 반환
            return instance;
        }
    }
    
    //[Header("")] : 유니티 Inspecter에서 변수들을 그룹화하고 라벨을 표시하는 속성, public은 보이고, private은 [SerializeField] 붙이는 경우 제외하면 안 보여서 쓸 필요 없음.
    [Header("References")]
    //아이템 관련 데이터를 관리하는 매니저 참조
    public ItemManager itemManager;

    [Header("Constants")]
    //const : 상수(값을 변경할 수 없음), long : 큰 정수 타입(약 922경까지), L : long 타입임을 명시
    //오버플로우(숫자가 너무 커서 넘치는 현상) 방지를 위한 최대값 (약 100경)
    //최댓값
    public const long MAX_VALUE = 999_999_999_999_999_999L;

    //현재 보유 골드
    private long _gold;
    //클릭당 획득 골드 (기본값 1)
    private long _click = 1;
    //초당 자동 획득 골드
    private long _autoClick;
    //마지막 저장 시간 (오프라인 보상 계산에 사용)
    private DateTime _lastSaveTime;

    //Awake : 오브젝트가 생성될 때 가장 먼저 1회 호출되는 함수
    private void Awake()
    {
        //싱글톤 패턴 : 게임 내에 GameManager가 단 하나만 존재하도록 보장
        if (instance == null)
        {
            //this : 현재 이 스크립트가 붙어있는 GameManager 객체
            instance = this;
            //DontDestroyOnLoad : 씬이 바뀌어도 이 오브젝트를 파괴하지 않음
            DontDestroyOnLoad(gameObject);
            //저장된 데이터 불러오기
            Load();
        }
        else
        {
            //이미 GameManager가 존재하면 중복이므로 자신을 파괴
            Destroy(gameObject);
        }
    }

    //Start : Awake 이후, 첫 번째 Update 전에 1회 호출되는 함수
    private void Start()
    {
        //StartCoroutine : 코루틴(일정 시간마다 반복 실행되는 함수) 시작
        StartCoroutine(AddAutoGold());
    }

    //Update : 매 프레임마다 호출되는 함수 (1초에 약 60번)
    private void Update()
    {
        //Input.GetKeyDown : 키를 누른 순간, KeyCode.Escape : ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //게임 종료 함수 호출
            QuitGame();
        }
    }

    //현재 보유 골드를 반환하는 함수
    //return 쓰는 경우 : 값을 돌려주기, 함수 즉시 종료, 여기서는 전자에 해당
    public long GetGold() { return _gold; }

    //골드를 추가하는 함수
    //매개변수 gold : 추가할 골드량
    public long AddGold(long gold)
    {
        //추가할 골드가 0 이하면 현재 골드 그대로 반환
        if (gold <= 0) return _gold;
        //오버플로우 방지 : 최대값을 넘으면 최대값으로 고정
        if (_gold >= MAX_VALUE - gold)
            _gold = MAX_VALUE;
        else
            _gold += gold;
        return _gold;
    }

    //골드를 차감하는 함수
    public long MinusGold(long gold) { _gold -= gold; return _gold; }

    //클릭당 획득 골드를 반환하는 함수
    public long GetClick() { return _click; }

    //클릭당 획득 골드를 증가시키는 함수
    public long AddClick(long click)
    {
        if (click <= 0) return _click;
        //오버플로우 방지
        if (_click >= MAX_VALUE - click)
            _click = MAX_VALUE;
        else
            _click += click;
        return _click;
    }

    //초당 자동 획득 골드를 반환하는 함수
    public long GetAutoClick() { return _autoClick; }

    //초당 자동 획득 골드를 증가시키는 함수
    public long AddAutoClick(long click)
    {
        if (click <= 0) return _autoClick;
        //오버플로우 방지
        if (_autoClick >= MAX_VALUE - click)
            _autoClick = MAX_VALUE;
        else
            _autoClick += click;
        return _autoClick;
    }

    //오프라인 보상을 계산하고 지급하는 함수
    //out : 함수 내에서 값을 설정해서 밖으로 내보냄 (여러 값을 반환할 때 사용)
    //return과 out의 차이점 : return은 반환 1개, 간단함, out은 반환 여러 개 가능, 복잡함
    public void CalculateOfflineReward(out int hours, out int minutes, out int seconds, out long rewardGold)
    {
        //초기값 설정
        hours = 0;
        minutes = 0;
        seconds = 0;
        rewardGold = 0;

        //자동 클릭이 없으면 보상 없음
        if (_autoClick <= 0)
            return;

        //TimeSpan : 두 시간 사이의 간격을 나타내는 구조체
        //DateTime.Now : 현재 시간, _lastSaveTime : 마지막 저장 시간
        TimeSpan elapsed = DateTime.Now - _lastSaveTime;
        //(long) : long 타입으로 형변환 (소수점 버림)
        long offlineSeconds = (long)elapsed.TotalSeconds;

        //오프라인 시간이 0 이하면 보상 없음
        if (offlineSeconds <= 0)
            return;

        //보상 계산 : 초당 자동 골드 × 오프라인 시간(초)
        long reward = _autoClick * offlineSeconds;
        //보상 지급
        AddGold(reward);

        //오프라인 시간 정보 설정
        //(int) : int 타입으로 형변환
        //elapsed.TotalHours는 double 타입이기에, int 타입으로 형변환 필요, elapsed.Minutes는 int 타입이라 형변환 필요 X
        hours = (int)elapsed.TotalHours;
        minutes = elapsed.Minutes;
        seconds = elapsed.Seconds;
        rewardGold = reward;
    }

    //1초마다 자동 골드를 획득하고 저장하는 코루틴
    //IEnumerator : 코루틴 함수의 반환 타입
    private IEnumerator AddAutoGold()
    {
        //while(true) : 무한 반복
        while (true)
        {
            //자동 클릭 골드 추가
            AddGold(_autoClick);
            //데이터 저장
            Save();
            //yield return : 코루틴을 일시 정지하고 기다림
            //WaitForSeconds(1f) : 1초 대기 (f는 float 타입을 의미)
            yield return new WaitForSeconds(1f);
        }
    }

    //게임 데이터를 저장하는 함수
    //PlayerPrefs : Unity에서 제공하는 간단한 데이터 저장 시스템 (키-값 형태), int랑 float, string 형태만 제공이기에, long, double 형태는 문자열로 변환하여 저장
    private void Save()
    {
        //SetString(키, 값) : 문자열 형태로 저장
        //ToString() : 숫자를 문자열로 변환
        PlayerPrefs.SetString("Gold", _gold.ToString());
        PlayerPrefs.SetString("Click", _click.ToString());
        PlayerPrefs.SetString("AutoClick", _autoClick.ToString());
        //ToBinary() : DateTime을 long 숫자로 변환 (저장하기 쉬운 형태)
        PlayerPrefs.SetString("LastSaveTime", DateTime.Now.ToBinary().ToString());

        //itemManager가 존재하면 아이템 데이터도 저장
        if(itemManager != null) itemManager.Save();
    }

    //저장된 게임 데이터를 불러오는 함수
    private void Load()
    {
        //GetString(키, 기본값) : 저장된 값을 불러옴, 없으면 기본값 사용
        //long.Parse() : 문자열을 long 숫자로 변환
        _gold = long.Parse(PlayerPrefs.GetString("Gold", "0"));
        _click = long.Parse(PlayerPrefs.GetString("Click", "1"));
        _autoClick = long.Parse(PlayerPrefs.GetString("AutoClick", "0"));

        //마지막 저장 시간 불러오기
        string savedTime = PlayerPrefs.GetString("LastSaveTime", "");
        //삼항 연산자 : 조건 ? 참일때값 : 거짓일때값
        //string.IsNullOrEmpty() : 문자열이 null이거나 비어있으면 true
        //FromBinary() : long 숫자를 DateTime으로 변환
        _lastSaveTime = string.IsNullOrEmpty(savedTime)
            ? DateTime.Now
            : DateTime.FromBinary(long.Parse(savedTime));

        //itemManager가 존재하면 아이템 데이터도 불러오기
        if (itemManager != null) itemManager.Load();
    }

    //게임을 저장하고 종료하는 함수
    private void QuitGame()
    {
        //종료 전 데이터 저장
        Save();
        //Application.Quit() : 애플리케이션 종료 (빌드된 게임에서만 작동)
        Application.Quit();
    }
}
