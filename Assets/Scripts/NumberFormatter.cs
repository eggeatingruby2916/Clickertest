//static class : 인스턴스를 생성하지 않고 클래스명으로 바로 접근 가능한 클래스
//ex) NumberFormatter.Format(1000) 처럼 바로 사용
//큰 숫자를 한글 단위(만, 억, 조, 경)로 변환해주는 유틸리티 클래스
public static class NumberFormatter
{
    //readonly : 선언 시 또는 생성자에서만 값을 할당할 수 있음 (이후 변경 불가)
    //string[] : 문자열 배열
    //한글 단위 배열 (경, 조, 억, 만, 빈문자열)
    private static readonly string[] Units = { "경", "조", "억", "만", "" };

    //long[] : long 타입 배열
    //각 단위에 해당하는 나눗셈 값
    //10000000000000000 = 1경, 1000000000000 = 1조, 100000000 = 1억, 10000 = 1만, 1 = 1
    private static readonly long[] Dividers = { 10000000000000000, 1000000000000, 100000000, 10000, 1 };

    //숫자를 한글 단위로 포맷팅하는 함수
    //number : 변환할 숫자
    //maxUnits = 2 : 기본값이 2, 최대 표시할 단위 개수 (예: 2이면 "1조 2345억"까지만 표시)
    public static string Format(long number, int maxUnits = 2)
    {
        //1만 미만이면 그냥 숫자로 표시
        //"N0" : 천 단위 콤마 포맷 (예: 1,234)
        if (number < 10000)
            return number.ToString("N0");

        //System.Text.StringBuilder : 문자열을 효율적으로 이어붙이기 위한 클래스
        //일반 string은 + 할 때마다 새 객체 생성, StringBuilder는 내부 버퍼 사용
        var result = new System.Text.StringBuilder();
        //표시한 단위 개수
        int unitCount = 0;

        //모든 단위를 순회 (경 → 조 → 억 → 만 → 1)
        for (int i = 0; i < Units.Length; i++)
        {
            // / : 나눗셈 (정수끼리 나누면 몫만 반환)
            //해당 단위의 값 계산 (예: 1조2345억 / 1조 = 1)
            long value = number / Dividers[i];
            // % : 나머지 연산
            //나머지를 다음 계산에 사용 (예: 1조2345억 % 1조 = 2345억)
            number %= Dividers[i];

            //값이 있으면 결과에 추가
            if (value > 0)
            {
                //string.IsNullOrEmpty() : 문자열이 null이거나 비어있으면 true
                //빈 단위("")이면 suffix 없음, 아니면 단위 + 공백
                string suffix = string.IsNullOrEmpty(Units[i]) ? "" : $"{Units[i]} ";
                //Append : StringBuilder에 문자열 추가
                result.Append($"{value.ToString("N0")}{suffix}");
                unitCount++;

                //최대 단위 개수에 도달하면 종료
                if (unitCount >= maxUnits)
                    break;
            }
        }

        //Trim() : 앞뒤 공백 제거
        //ToString() : StringBuilder를 일반 string으로 변환
        return result.ToString().Trim();
    }
}
