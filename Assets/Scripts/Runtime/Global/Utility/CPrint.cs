using System;
using System.Collections.Generic;
using UnityEngine;
// ▷▶※●·
/*  ▶ 유틸리티 : CPrint
 *    - 출력에 관한 것을 구조화시키고싶다.
 *    - 프린트는 콘솔 프로젝트에서 사용한 것 처럼 출력 규칙을 유니티스럽게 바꾼 버전
 *  
 *  · 유니티에서 뭔가를 만들려고 할 때...
 *    - C# : Main
 *    - 유니티 : 유니티 생성주기에 올리고 대신 호출을 하게 한다. (이벤트 기반 구조)
 *    - 유니티에서는 global using을 권장하지 않는다.
 *      ㄴ 자동화가 완벽하게 안됨 (C# 9.0 기반이라서)
 *  
 *  · 유니티에서 "전역" 스럽게 쓰고싶다
 *    1. 글로벌 네임스페이스 -> static
 *      ㄴ 별다른 using 없어도 잘 들어감
 *    2. 네임스페이스는 유지하되 풀네임 호출로 통일한다.
 *      ㄴ 어디 소속인지 명확하다.
 *    3. using static
 *      ㄴ 추적이 어려워서 권장하지 않음
 *      
 *  ------------------------------------------------------------
 *  ▶ CPrint 업그레이드
 *    - 로그 -> 구조 / 종류
 *    - CPrint는 런타임에서도 쓸 수 있기 때문에 기본은 Editor 처리를 하지 않는다.
 */

public static class CPrint
{
    // 스위치
    public static bool Enable = true;
    // 서식 태그
    public static bool EnableRichText = true;

    // 들여쓰기
    // -> 가독성을 위해 -> 출력 앞에 공백을 붙여 구조적으로 분리하기가 좋다.
    // ㄴ 묶음 / 같은 덩어리 -> 트리구조
    private static int _indentLevel = 0;
    private const int INDENT_SPACES = 2;

    // HashSet : 중복을 허용하지 않고 고유한 요소만 저장한다. (자료구조)
    // 일반적으로 O(1) 수준을 보인다.
    // readonly : 
    private static readonly HashSet<string> _onceSet = new HashSet<string>();

    /*  ▶ readonly : 런타임에 결정됨
     *    - 한번 정해진 이후에는 다시 대입하지 못하게 막는다.
     *      ㄴ 초기화 -> 선언부에서 하거나 / 생성자에서 하거나
     *    - MonoBehaviour 떄문에 생성자를 직접적으로 사용하는 경우는 C# 대비 많지 않다.
     *    
     *  ▶ 해시셋
     *    - 컬렉션 클래스 -> 해시 테이블 기반 -> 데이터 구조
     *      ㄴ 중복되지 않은 요소들의 모임을 관리 / 이럴 경우 최적화 되어 있고 탐색이 빠름 -> 추가 / 삭제도 가능
     *  
     *  해시 테이블
     *    - 키 / 값이 쌍으로 데이터를 저장하는 자료구조
     *    - 키를 이용해 (해시 함수) 특정 인덱스로 접근 (혹은 변환) -> 데이터 저장
     *  
     *  ※ 이번주 기술 노션 확인 예정
     *  
     *  · 내부 동작
     *  1. 해시함수
     *    - 값을 해시코드 (정수) 로 바꾼다.
     *      ㄴ 같은 값이면 같은 해시코드가 나오는게 이 자료구조의 목표
     *  2. 버킷
     *    - 해시코드를 기준으로 저장 위치(버킷)를 고른다
     *      ㄴ 해시코드 % 버킷개수로 인덱스 결정
     *  3. 충돌
     *    - 서로 다른 값인데 해시코드가 겹칠 수 있다.
     *      ㄴ 버킷 안에서 추가 비교등을 수행해서 진짜 같은 값인지 확인
     *    - HashSet -> 해시 + 실제 비교 같이 사용
     *  4. 재해싱
     *    - 안에 요소가 많아지면 버킷이 뻑뻑해진다 -> 성능이 떨어질 수 있음
     *    - 더 큰 테이블을 만들고 다시 배치함
     *  
     *  ※ 내부는 해시로 위치를 찾고 -> 충돌은 비교로 해결하고 -> 많아지면 재해싱
     *  ※ 중복없이 저장 -> 빠른 컨테이너
     */

    private static string Indent
    {
        get
        {
            // 로그쪽으로
            return new string(' ', _indentLevel * INDENT_SPACES);
        }
    }
    public static void IndentPush() => _indentLevel++;
    public static void IndentPop() {
        _indentLevel--;
        if (_indentLevel < 0) _indentLevel = 0;
    }
    public static void IndentReset() => _indentLevel = 0;
    private enum ELogKind
    {
        Log,
        Warn,
        Error,
        Success
    }
    // 출력 포맷 관리를 위해
    // 들여쓰기 / 접두사 / 리치텍스트 -> kind 분류
    private static void Emit(ELogKind kind, string msg, string tag = null, string colorHex = null)
    {
        // 지금까지 만든 문자열을 콘솔로 내보내는 출력 코어
        // 색상값에 헥스를 쓰는 이유 -> 가장 범용적인 방식 (문자열로 색을 표현하기에 가장 무난)
        // ㄴ 1. 표준이고 무난함
        // ㄴ 2. 문자열 -> 로그 포맷을 만들 때 바로 끼워넣기 좋음
        // ㄴ 3. 16진수 -> 압축이 잘됨 (RGB)

        if (!Enable) return;

        // 접두사 만들기 -> tag가 있으면 해당되는 프리픽스를 만든다.
        // 단, tag가 null / 빈 문자열이면 접두사 없이 msg만 출력
        string prefix = string.Empty;
        if (!string.IsNullOrEmpty(tag))
        {
            if (EnableRichText && !string.IsNullOrEmpty(colorHex))
            {
                prefix = $"<color={colorHex}>[{tag}] </color>";
            }
            else
            {
                prefix = $"[{tag}] ";
            }
        }

        string final = $"{Indent}{prefix}{msg}";
        switch (kind)
        {
            case ELogKind.Log:
                Debug.Log(final);
                break;
            case ELogKind.Warn:
                Debug.LogWarning(final);
                break;
            case ELogKind.Error:
                Debug.LogError(final);
                break;
            case ELogKind.Success:
                Debug.Log(final);
                break;
        }
    }
    // Title / Section
    public static void Title(string title, char lineCh = '=')
    {
        Line(lineCh);
        Emit(ELogKind.Log, title);
        Line(lineCh);
    }
    public static void Section(string section, char lineCh = '-')
    {
        Emit(ELogKind.Log, section);
        Line(lineCh);
    }
    // Line / Blank
    // 구분선을 상황에 맞게 바꿀 수 있도록 문자 / 길이를 옵션으로 준 것
    // ㄴ 고정 형식이 아니기 때문에 상황에 맞게 사용하면 좋다
    public static void Line(char ch = '=', int count = 10)
    {
        Emit(ELogKind.Log, new string(ch, count));
    }
    public static void Blank(int lines = 1)
    {
        if (!Enable) return;
        if (lines <= 0) return;
        Debug.Log(new string('\n', lines));
    }
    public static void Log(string msg)
    {
        Emit(ELogKind.Log, msg);
    }
    public static void Warn(string msg)
    {
        Emit(ELogKind.Warn, msg, "WARN", "#FF9100");
    }
    public static void Error(string msg)
    {
        Emit(ELogKind.Error, msg, "ERROR", "#FF1744");
    }
    // ㄴ 경고와 달리 빨간색 -> 남발 금지 -> 진짜 필요한 것에만 사용
    public static void Success(string msg)
    {
        Emit(ELogKind.Success, msg, "OK", "#00C853");
    }
    public static void Assert(bool condition, string msg)
    {
        if (condition) return;
        Error($"[ASSERT] {msg}");
    }
    public static void CheckNull(object obj, string msg)
    {
        if (obj != null) return;
        Warn($"[NULL] {msg}");
    }
    // 참조 체크
    public static T Ref<T>(T obj, string msg) where T : class
    {
        // ★★★★
        // Q. 제네릭이 뭔가? -> C# 고급문법
        // 시간을 들여 사용해봐야 하는 것들
        // 잘 쓰면 디자인 패턴이 편해짐

        /*  T Ref<T>
         *  
         *  T Ref<T>(T obj, string msg)
         *    ㄴ obj가 null이면 경고 찍고, obj 그대로 반환
         *    ㄴ 검사 + 반환을 한방에 하고 싶다.
         *  왜 T냐?
         *    ㄴ 한줄로 끝내려고 -> 이후 어떤 타입이 들어올 지 예측이 안되지만,
         *       클래스 타입임을 명시해서 동작시키기 위해서.
         *  · 제네릭 -> 가볍게
         *    - 제네릭은 타입을 나중에 정하는 설계
         *      ㄴ 호툴할 떄 타입이 결정됨
         *    - 템플릿 / 제네릭 -> 비슷한 이야기
         *      ㄴ 클래스나 함수를 정의할 때 타입을 지정하지 않고 구현할 수 있는 매커니즘
         *      
         *  · T
         *    - 타입 자리 (타입 변수)
         *    - 제네릭은 <T>와 같은 제네릭 타입을 명시함으로서 정의가능.
         *  
         *  객체지향 특징 + 원칙
         *   ㄴ 추상화
         *  
         *  - 컴포넌트 기반 프로그래밍 (객체 / 구조 / 절차)
         *  - 제네릭 프로그래밍으로 전환이 되면 설계가 더 까다로워진다. (일반화 프로그래밍)
         *  
         *  · where T : class
         *    - C#은 사용하는데 있어 조금 괜찮은 편 -> 기본적으로 모든 데이터 타입에 도작하도록 설계가 되어야 한다.
         *    - 제네릭 클래스 또는 함수에 어떤 데이터 타입이 지정되어도 내부 로직에 변화가 발생하면 안된다.
         *    - 특정 데이터 타입에 동작하도록 데이터 타입을 제한하는 것이 가능하다.
         *    - T는 클래스만 받겠다는 제한 (참조형)
         *      ㄴ 결국 우리가 만든 함수는 null 체크가 핵심인데 int float 들어오면 피곤해진다
         *    - where T : class 로 null이 될 수 있는 타입만 허용하겠다 -> 실수 방지
         *    
         *  · 제네릭의 데이터 타입 제한
         *  class CSomeClass<T> where T : class
         *   ㄴ 타입을 참조 형식으로 제한
         *  class CSomeClass<T> where T : struct
         *   ㄴ 타입을 값 형식으로 제한
         *  class CSomeClass<T> where T : SomeClass
         *   ㄴ 클래스 씰 != 타입을 SomeClass를 직/간접적으로 상속하는 형식으로 제한
         *  class CSomeClass<T> where T : SomeInterface
         *   ㄴ 타입을 SomeInterface를 직/간접적으로 상속하는 형식으로 제한
         *  class CSomeClass<T, U> where T : U
         *   ㄴ 타입을 U (클래스 또는 인터페이스)로 직/간접적으로 상속하는 형식으로 제한
         */

        if (obj == null) Warn($"[NULL] {msg}");
        return obj;
    }
    // Vector3
    public static void V3(string label, Vector3 v, int digits = 2)
    {
        float x = (float) System.Math.Round(v.x, digits);
        float y = (float)System.Math.Round(v.y, digits);
        float z = (float)System.Math.Round(v.z, digits);

        Emit(ELogKind.Log, $"{label} : ({x}, {y}, {z})");
    }

    public static void KV(string key, object value)
    {
        // KV = key = value 형태로 값을 찍는 표준 포맷 헬퍼
        // 가장 자주 찍는 형태 -> 포맷 통일
        Log($"{key} = {value}");
    }
    public static void Group(string title, Action body, char lineCh = '=', int LineCount = 20)
    {
        if (!Enable)
        {
            return;
        }
        /*  델리게이트 (간단버전)
         *    - C# 고급 문법
         *    - 델리게이트는 함수를 변수처럼 다룰 수 있게 해주는 타입
         *      ㄴ 함수 풀링 방식 -> 열심히 조사해오기
         *      ㄴ 특정 함수를 대신 호출해주는 대리자
         *    - 프로그래밍에서 델리게이트는 콜백 함수를 의미한다.
         *      ㄴ 델리게이트를 이용하면 특정 이벤트가 발생하는 시점에 해당 이벤트를 처리하는 것이 가능하다.
         *    - 대리자는 자기가 가르키고 있는 함수를 호출하는 역할을 한다.
         *      ㄴ 함수에 대한 참조를 갖고 있다.
         *      
         *    [요약]
         *    1. 실행을 위임한다.
         *    2. 호출 주체와 실행 주체가 같지 않다.
         *    3. 콜백의 시작점
         *  
         *  ▶ Action
         *    - Action은 델리게이트의 미리 만들어진 형태
         *      ㄴ 3총사 : Action<T> / Func<T, TResult> / Predicate<T>
         *    - 델리게이트는 함수를 담을 수 있는 타입
         *    - Action -> 매개변수가 없고 반환값 없는 형태 -> C# 기본 제공
         *      ㄴ 실행할 코드 덩어리를 -> 변수처럼 전달한다.
         *      
         *      CPrint.Group("프리셋", () =>
         *      {
         *          CPrint.Log("색상 교체");
         *          ~~
         *      });
         */

        Title(title, lineCh);
        IndentPush();
        // 실행할 코드 블록을 호출(Action)
        // ?.Invoke() : body가 null이면 실행하지 않겠다. (예외 방지)
        body?.Invoke();
        IndentPop();
        Line(lineCh, LineCount);
    }
    public static void Once(string key, string msg)
    {
        if (!Enable) return;
        // 이미 키가 있는 경우 -> 재출력 금지
        if (_onceSet.Contains(key)) return;
        _onceSet.Add(key);
        Warn($"[ONCE] {msg}");
    }
    public static void OnceClear()
    {
        _onceSet.Clear();
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
    {
        if (!Enable) return;
        Debug.DrawLine(origin, direction, color, duration);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Line3D(Vector3 a, Vector3 b, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }
        Debug.DrawLine(a, b, color, duration);
    }

}