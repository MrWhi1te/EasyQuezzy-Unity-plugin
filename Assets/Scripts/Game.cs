using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour
{
    [Header("Темы и вопросы")]
    [Tooltip("Добавляйте необходимое количество тем, вопросов и ответы к ним")]
    public List<ThemeList> ThemeList;

    [Space]

    [Tooltip("Объект Text для вывода вопроса")] 
    public Text QText; // Текст вопроса (для вывода)
    [Tooltip("Объект Image для вывода вопроса")]
    public Image Qsprite; // Изображение вопроса
    [Tooltip("Объект Text для вывода интересного факта или пояснение ответа")]
    public Text FactText; // Текст для фактов о вопросе (для вывода)

    [Space]

    [Tooltip("Объекты Text для вывода ответов на кнопках")]
    public Text[] AnswerText; // Текст ответов
    [Tooltip("Объекты Button для вывода кнопок")]
    public Button[] AnswersBttns = new Button[4]; // Кнопки для ответов

    [Space]

    [Tooltip("Объект Image для вывода иконки правильного/неправильного ответа")]
    public Image TFIcon; // Объект Image для вывода иконки правильного/неправильного ответа
    [Tooltip("Объекты Sprite для вывода иконок правильного/неправильного ответа")]
    public Sprite[] TFIcons = new Sprite[3]; // Иконки правильного/неправильного ответа
    [Tooltip("Объект Text для вывода правильного/неправильного ответа")]
    public Text TFtext; // Текст правильного/неправильного ответа

    [Space]

    [Tooltip("Объект Text для вывода количества пройденных вопросов")]
    public Text CountQText; // Текст очков
    [Tooltip("Объект Text для вывода количества ошибок")]
    public Text CountFalseAnswText; // Текст ошибок

    [Space]

    [Tooltip("Объект Text для вывода таймера")]
    public Text TimerText; // Текст для таймера

    [Space]

    [Tooltip("Объекты Sprite для Фона вопросов (Выводятся в рандомном порядке)")]
    public List<Sprite> ImageFon = new List<Sprite>();
    [Tooltip("Основная панель с вопросом")]
    public GameObject QuestionPan; // Основная панель игры
    [Tooltip("Окно с фактом о вопросе")]
    public GameObject Fact; // Объект для показа фактов о вопросе
    [Tooltip("Кнопка следующий вопрос")]
    public GameObject NextQuestion; // Кнопка для следующего вопроса
    [Tooltip("Кнопка меню")]
    public GameObject MenuBttn; // Кнопка для следующего вопроса
    [Tooltip("Панель с итогом викторины")]
    public GameObject EndPanel; // Панель с итогами игры
    public Text EndText; // Текст с поздравлением об окончании игры
    public Text EndStatText; // Текст с результатами игры

    List<object> qList; // Лист вопросов
    QuestionList crntQ; //

    [Tooltip("Активирует функцию сохранения")]
    public bool Saver; // Активна ли функция сохранения

    [Space]
    
    private int IndexTheme; // Номер выбранной темы
    [Tooltip("Объект Text названия игры")]
    public Text NameGameText; //
    [Tooltip("Объект GameObject родительского объекта для префаба")]
    public GameObject Content; // Родительский объект сцены для создания префабов 
    [Tooltip("Основная панель с вопросом")]
    public GameObject PrefTheme; // Префаб темы
    [Tooltip("Основная панель с вопросом")]
    public GameObject Menu; // Панель меню с темами


    #region MONO
    private void Awake() //
    {
        if (PlayerPrefs.HasKey("sv") & Saver) // Проверка есть ли сохранение. Если есть загрузка из сохранения.
        {
            for (int i = 0; i < ThemeList.Count; i++)
            {
                ThemeList[i].randQ = PlayerPrefs.GetInt("Randq" + i); //
                ThemeList[i].CountQ = PlayerPrefs.GetInt("Countq" + i);
                ThemeList[i].RightAnsw = PlayerPrefs.GetInt("Right" + i); //
                ThemeList[i].FalseAnsw = PlayerPrefs.GetInt("False" + i);
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < ThemeList.Count; i++) // Цикл для назначения функций кнопок темам в меню
        {
            int temp = i; 
            ThemeList[i].ObjTheme.GetComponent<Button>().onClick.AddListener(() => OnClickPlay(temp)); // Назначение кнопкам функции слушателя
        }
        CompletedTest();
    }
    //void Update() // Обновление каждую сек.
    //{
    //}
    #endregion


    public void OnClickPlay(int index) // Нажатие на кнопку Play
    {
        //PlayerPrefs.DeleteAll();
        IndexTheme = index;
        qList = new List<object>(ThemeList[IndexTheme].QuestionList); // Создание листа категорий вопросов
        CountQText.text = ThemeList[IndexTheme].CountQ.ToString() + " вопрос из " + ThemeList[index].QuestionList.Count;
        CountFalseAnswText.text = "Ошибок: " + ThemeList[IndexTheme].FalseAnsw.ToString();
        EndPanel.SetActive(false); // Закрываем панель итога игры
        QuestionGenerate(); // Запуск генератора вопросов
        Menu.SetActive(false); // Закрываем панель меню игры
    }

    public void Resume() // Кнопка продолжить
    {
        Qsprite.gameObject.SetActive(false);
        if (TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(false);
        QuestionGenerate();
        NextQuestion.SetActive(false); // Убираем кнопку продолжить
    }

    public void StartNewGame()
    {
        ThemeList[IndexTheme].randQ = ThemeList[IndexTheme].CountQ = ThemeList[IndexTheme].RightAnsw = ThemeList[IndexTheme].FalseAnsw = 0;
        OnClickPlay(IndexTheme);
    }

    public void ExitMenu() // Кнопка выход в меню
    {
        NextQuestion.SetActive(false);
        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].gameObject.SetActive(false); //
        if (ThemeList[IndexTheme].TimerActive) { StopCoroutine("TimerQuestions"); TimerText.text = ""; }
        if (TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(false);
        EndPanel.SetActive(false);
        Menu.SetActive(true);
        CompletedTest();
    }

    void CompletedTest() // Просчет прохождения тестов
    {
        for(int i = 0; i < ThemeList.Count; i++){ThemeList[i].CompletedTextTheme.text = ThemeList[i].randQ + " из " + ThemeList[i].QuestionList.Count;}
    }
    
    void QuestionGenerate() // Генератор вопросов
    {
        if (ThemeList[IndexTheme].randQ < ThemeList[IndexTheme].QuestionList.Count) // проверка кол-ва вопросов
        {
            Fact.SetActive(false); //
            CountQText.text = ThemeList[IndexTheme].CountQ+1 + " вопрос из " + ThemeList[IndexTheme].QuestionList.Count;
            CountFalseAnswText.text = "Ошибок: " + ThemeList[IndexTheme].FalseAnsw.ToString();
            crntQ = qList[ThemeList[IndexTheme].randQ] as QuestionList; // Рандомизатор вопросов из листа

            if(crntQ.FonIcon != null) { QuestionPan.GetComponent<Image>().sprite = crntQ.FonIcon; } // Проверяем есть ли отдельный фон у вопроса
            else { int r = Random.Range(0, ImageFon.Count); QuestionPan.GetComponent<Image>().sprite = ImageFon[r]; } // Если нет отдельного фона, то выбираем рандомный фон. 

            QText.text = crntQ.Question; // Текст вопроса
            QText.gameObject.SetActive(true); // Активируем текст вопроса

            if(crntQ.Icon != null) { Qsprite.sprite = crntQ.Icon; Qsprite.gameObject.SetActive(true); } // Если есть фото для вопроса, то выбираем картинку и активируем

            FactText.text = crntQ.Fact; // Выбираем текст для факта после вопроса
            List<string> answer = new List<string>(crntQ.Answer); // Создание листа для размещения ответов

            for (int i = 0; i < crntQ.Answer.Count; i++) // Цикл рандомных ответов (Перемешивание кнопок)
            {
                int rand = Random.Range(0, answer.Count); // Рандомные ответы
                AnswerText[i].text = answer[rand]; // Текст
                answer.RemoveAt(rand); // Удаление ответа из листа
            }

            StartCoroutine(AnimBttns()); // Включение корутины анимации и вывода кнопок
        }
        else // Если 0 вопросов то конец
        {
            EndPanel.SetActive(true); // Активируем панель окончания игры
            int r = Random.Range(0, ImageFon.Count);
            EndPanel.GetComponent<Image>().sprite = ImageFon[r];
            EndText.text = "Поздравляю! Вы закончили тему " + ThemeList[IndexTheme].NameTheme; //
            EndStatText.text = "У вас " + ThemeList[IndexTheme].RightAnsw + " правильных ответов и " + ThemeList[IndexTheme].FalseAnsw + " неправильных"; //
        }
    }

    IEnumerator AnimBttns() // Анимация кнопок (Появление и активность)
    {
        MenuBttn.SetActive(false);
        yield return new WaitForSeconds(1); // Ожидание секунда
        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].interactable = false; // Отключение кликабельности кнопок
        int a = 0;

        while (a < crntQ.Answer.Count)
        {
            if (!AnswersBttns[a].gameObject.activeSelf) AnswersBttns[a].gameObject.SetActive(true); // Если кнопка не активна, то активируем
            a++;
            yield return new WaitForSeconds(1);
        }

        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].interactable = true; // Включение кликабельности кнопок

        if (ThemeList[IndexTheme].TimerActive) { StartCoroutine("TimerQuestions"); } // Если активен таймер, то запускаем корутины таймера

        MenuBttn.SetActive(true);

        yield break;
    }

    IEnumerator TrueOrFalse(bool check) // Проверка на правильность или неправильно выбраный ответ
    {
        if (ThemeList[IndexTheme].TimerActive) { StopCoroutine("TimerQuestions"); TimerText.text = ""; }

        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].interactable = false; // Отключает кликабельность кнопки
        yield return new WaitForSeconds(0.5f); // Ожидание секунда

        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].gameObject.SetActive(false); //
        Qsprite.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f); // 

        if (!TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(true); // Проверка на включение иконки

        if (FactText.text != "") { Fact.SetActive(true); } // Если Факт не пустой, то выводим

        if (check)
        {
            ThemeList[IndexTheme].RightAnsw++; // Прибавляем правильные ответы
            TFIcon.sprite = TFIcons[0]; // Иконка если правильный ответ
            TFtext.text = "Правильный ответ";
            yield return new WaitForSeconds(1.5f); // Ожидание секунда
        }
        else
        {
            ThemeList[IndexTheme].FalseAnsw++; // Прибавляем неправильные ответы
            TFIcon.sprite = TFIcons[1]; // Иконка если неправильный ответ
            TFtext.text = "Неправильный ответ";
            yield return new WaitForSeconds(1.5f); // Ожидание секунда
        }

        TFIcon.gameObject.SetActive(false); // Отключение икноки
        NextQuestion.SetActive(true);
        ThemeList[IndexTheme].randQ++; // увеличиваем счетчик вопросов
        ThemeList[IndexTheme].CountQ++; // 
        SaveGame();
        yield break; // Закрытие корутины
    }

    public void AnswerBttns(int index) // Кнопка ответов
    {
        if (AnswerText[index].text.ToString() == crntQ.Answer[0]) StartCoroutine(TrueOrFalse(true)); // Проверка правильности ответа (Кнопка ответа индекс "0")
        else StartCoroutine(TrueOrFalse(false)); // Если не правильно ответ
    }

    IEnumerator TimerQuestions() // Корутина Таймера!
    {
        int Time = ThemeList[IndexTheme].TimerForQuestions;  // Присваиваем переменной наше время на вопрос
        TimerText.text = Time.ToString(); // Текст таймера

        while (true)
        {
            if(Time > 0)   // Выполнение пока время таймера больше 0
            {
                Time--;
                TimerText.text = Time.ToString();
                yield return new WaitForSeconds(1f);
            }
            else    // 
            {
                TimerText.text = "";
                OutTime();
                yield break;
            }
        }
    }

    void OutTime() // Выполнение если кончилось время
    {
        for (int i = 0; i < AnswersBttns.Length; i++) AnswersBttns[i].gameObject.SetActive(false);
        Qsprite.gameObject.SetActive(false);

        if (!TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(true); // Проверка на включение иконки

        ThemeList[IndexTheme].FalseAnsw++; // Прибавляем неправильные ответы
        TFIcon.sprite = TFIcons[2];
        TFtext.text = "Время вышло!";
        ThemeList[IndexTheme].randQ++; // увеличиваем счетчик вопросов
        ThemeList[IndexTheme].CountQ++; // 
        NextQuestion.SetActive(true);
        SaveGame();
    }

    void SaveGame() // Сохранение процесса игры
    {
        for(int i = 0; i < ThemeList.Count; i++)
        {
            PlayerPrefs.SetInt("Randq" + i, ThemeList[i].randQ);
            PlayerPrefs.SetInt("Countq" + i, ThemeList[i].CountQ);
            PlayerPrefs.SetInt("Right" + i, ThemeList[i].RightAnsw);
            PlayerPrefs.SetInt("False" + i, ThemeList[i].FalseAnsw);
        }
        PlayerPrefs.SetString("sv", ThemeList[0].NameTheme);
    }

    public void DeleteSave() // Удаление сохранения
    {
        PlayerPrefs.DeleteAll();
    }
}

[System.Serializable]
public class QuestionList //
{
    [Tooltip("Текст вопроса")]
    public string Question; // Вопросы 
    [Tooltip("Картинка вопроса (Если есть)")]
    public Sprite Icon; // Картинка вопроса
    [Tooltip("Картинка фона для вопроса (Если есть)")]
    public Sprite FonIcon; // Фон. картинка вопроса
    [Tooltip("Текст ответов (ВАЖНО! Первый пишите правильный ответ!)")]
    public List<string> Answer = new List<string>();
    [Tooltip("Интересный факт или пояснение к ответу (оставьте пустым, если не используете)")]
    [TextArea(1,5)]
    public string Fact;
}

[System.Serializable]
public class ThemeList //
{
    [Tooltip("Название темы")]
    public string NameTheme; // Название темы
    [Tooltip("Иконка(фон) темы")]
    public Sprite IconTheme; // Иконка темы
    [Tooltip("Объект GameObject сцены в меню")]
    public GameObject ObjTheme; // Объект Темы в меню
    [Tooltip("Объект Text сцены названия темы")]
    public Text TextNameTheme; // Текст название темы
    [Tooltip("Объект Text сцены процента")]
    public Text CompletedTextTheme; // Текст на сколько завершено
    [HideInInspector]
    public bool OpenQuestions; // Переменная открытия вопросов в инспекторе
    //[HideInInspector]
    public int randQ; // Значение рандома вопросов
    [HideInInspector]
    public int CountQ; // Подсчет очков
    [HideInInspector]
    public int RightAnsw; // Подсчет правильных ответов
    [HideInInspector]
    public int FalseAnsw; // Подсчет неправильных ответов
    [Tooltip("При включении активирует таймер на каждый вопрос")]
    public bool TimerActive; // Активен ли таймер в игре
    [Tooltip("Укажите сколько времени дается на вопрос")]
    public int TimerForQuestions; // Сколько времени на вопрос
    public List<QuestionList> QuestionList = new List<QuestionList>(); //
}