using DG.Tweening;
using System;
using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;

    public float clockSpeed = 1.0f;
    private DateTime currentTime;
    private DateTime previousTime;

    private float timeSinceLastSync = 0f;
    private const float syncInterval = 3600f;

    public TextMeshProUGUI timeDisplay;

    // Добавляем ссылки на текущие твины для каждой стрелки
    private Tween secondsTween;
    private Tween minutesTween;
    private Tween hoursTween;

    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TMP_InputField secondInput;

    public TextMeshProUGUI errorMessage; // Поле для вывода ошибок

    void Start()
    {
        StartCoroutine(GetTimeFromServer());
    }

    IEnumerator GetTimeFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://yandex.com/time/sync.json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response from server: " + jsonResponse);

            long serverTimeMilliseconds = ParseJsonForTime(jsonResponse);

            if (serverTimeMilliseconds != 0)
            {
                currentTime = DateTimeOffset.FromUnixTimeMilliseconds(serverTimeMilliseconds).UtcDateTime;
                currentTime = new DateTime(1, 1, 1, currentTime.Hour, currentTime.Minute, currentTime.Second);
                Debug.Log("Время с Яндекса: " + currentTime.ToString("HH:mm:ss"));

                setPointers(currentTime);


            }
            else
            {
                Debug.LogError("Parsed time is 0. Check server response.");
            }
        }
        else
        {
            Debug.LogError("Error fetching time from server");
        }
    }

    private void setPointers(DateTime requiredTime)
    {

        // Рассчитываем целевые углы для стрелок
        float rotationSeconds = (360.0f / 60.0f) * requiredTime.Second;
        float rotationMinutes = (360.0f / 60.0f) * requiredTime.Minute;
        float rotationHours = (360.0f / 12.0f) * (requiredTime.Hour % 12) + ((360.0f / (60.0f * 12.0f)) * requiredTime.Minute);

        pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rotationSeconds);
        pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rotationMinutes);
        pointerHours.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rotationHours);
    }

    private long ParseJsonForTime(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("JSON response is null or empty");
            return 0;
        }

        try
        {
            var jsonObj = JsonUtility.FromJson<TimeResponse>(json);
            if (jsonObj.time > 0)
            {
                return jsonObj.time;
            }
            else
            {
                Debug.LogError("JSON field 'time' is invalid");
                return 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
            return 0;
        }
    }

    [Serializable]
    public class TimeResponse
    {
        public long time;
    }

    void Update()
    {
        if (currentTime != default(DateTime))
        {
            UpdateTime();
            UpdateClockHands();
        }

        
        //CheckForTimeSync();
    }

    void UpdateTime()
    {
        previousTime = currentTime;
        currentTime = currentTime.AddSeconds(Time.deltaTime * clockSpeed);
    }

    void UpdateClockHands()
    {
       
        DateTime serverTime = currentTime;  // Используем переменную currentTime

        float timeForSeconds = 1f; // Каждая секунда вращается за 1 секунду
        float timeForMinutes = 60f; // Каждая минута вращается за 60 секунд
        float timeForHours = 3600f; // Часовая стрелка совершает полный оборот за 1 час (3600 секунд)


        if (previousTime.Second != serverTime.Second) {
            float rotationSeconds = (360.0f / 60.0f) * (serverTime.Second + 1);
            pointerSeconds.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -rotationSeconds), timeForSeconds).SetEase(Ease.Linear);    
        }

        if (previousTime.Minute != serverTime.Minute)
        {
            float rotationMinutes = (360.0f / 60.0f) * (serverTime.Minute + 1);
            pointerMinutes.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -rotationMinutes), timeForMinutes).SetEase(Ease.Linear);
        }

        if (previousTime.Hour != serverTime.Hour) {
            float rotationHours = (360.0f / 12.0f) * (serverTime.Hour % 12) + ((360.0f / (60.0f * 12.0f)) * serverTime.Minute);
            pointerHours.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -rotationHours), timeForHours).SetEase(Ease.Linear);

        }

        timeDisplay.text = serverTime.ToString("HH:mm:ss");
    

    }

    void ClearPlaceholderText(TMP_InputField inputField)
    {
        inputField.text = ""; // Очищаем текст
        errorMessage.text = ""; // Очищаем текст ошибок
    }


    public void SetTimeManuallyFromButton()
    {
        // Считываем значения из InputField
        int hour = int.Parse(hourInput.text);
        int minute = int.Parse(minuteInput.text);
        int second = int.Parse(secondInput.text);

        // Считываем значения из InputField и проверяем валидность
        if (!int.TryParse(hourInput.text, out hour) || hour < 0 || hour > 23)
        {
            errorMessage.text = "Ошибка: Часы должны быть от 0 до 23.";
            return;
        }

        if (!int.TryParse(minuteInput.text, out minute) || minute < 0 || minute > 59)
        {
            errorMessage.text = "Ошибка: Минуты должны быть от 0 до 59.";
            return;
        }

        if (!int.TryParse(secondInput.text, out second) || second < 0 || second > 59)
        {
            errorMessage.text = "Ошибка: Секунды должны быть от 0 до 59.";
            return;
        }

        SetTimeManually(hour, minute, second);
        Debug.Log($"Время установлено вручную: {hour}:{minute}:{second}");

        // Очищаем текст в полях ввода
        hourInput.SetTextWithoutNotify("");
        minuteInput.SetTextWithoutNotify("");
        secondInput.SetTextWithoutNotify("");
    }


    public void SetTimeManually(int hour, int minute, int second)
    {
        // Устанавливаем новое время
        currentTime = new DateTime(1, 1, 1, hour % 24, minute % 60, second % 60);

        // Обновляем углы для стрелок
        setPointers(currentTime);

        Debug.Log("Установленное время: " + currentTime.ToString("HH:mm:ss"));


    }


    void CheckForTimeSync()
    {
        timeSinceLastSync += Time.deltaTime;
        if (timeSinceLastSync >= syncInterval)
        {
            timeSinceLastSync = 0f;
            StartCoroutine(GetTimeFromServer());
        }
    }
}
