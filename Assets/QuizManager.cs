using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TMP_Text questionText; 
    [SerializeField] Button option1Button;  
    [SerializeField] Button option2Button;
    [SerializeField] Button option3Button;
    [SerializeField] Button option4Button;
    [SerializeField]
    public SerialPortManager serialPortManager;

    private string correctAnswer;
    private Color defaultButtonColor;
    private Color correctButtonColor = Color.green;
    private Color incorrectButtonColor = Color.red;

    private string lastData = ""; 
    private bool buttonClicked = false;

    private Queue<Question> questions = new Queue<Question>();

    private float answerDelay = 2.0f;

    private void Start( )
    {
        defaultButtonColor = option1Button.GetComponent<Image>().color;

        questions.Enqueue(new Question("What is the capital of france?", "berlin", "madrid", "paris", "rome", "paris"));
      
        questions.Enqueue(new Question("What is the largest planet in our solar system?", "Earth", "Mars", "Jupiter", "Saturn", "Jupiter"));

        questions.Enqueue(new Question("What is the smallest planet in our solar system?", "Mercury", "Mars", "Venus", "Earth", "Mercury"));

        questions.Enqueue(new Question("Who wrote To Kill a Mockingbird?", "Harper Lee", "J.K. Rowling", "Ernest Hemingway", "Mark Twain", "Harper Lee"));

        questions.Enqueue(new Question("What is the capital city of Australia?", "Sydney", "Melbourne", "Brisbane", "Canberra", "Canberra"));

        questions.Enqueue(new Question("What is the chemical symbol for gold?", "Au", "Ag", "Fe", "Pb", "Au"));

        questions.Enqueue(new Question("Which planet is known as the Red Planet?", "Earth", "Venus", "Mars", "Jupiter", "Mars"));

        questions.Enqueue(new Question("What is the largest ocean on Earth?", "Atlantic Ocean", "Indian Ocean", "Arctic Ocean", "Pacific Ocean", "Pacific Ocean"));

        questions.Enqueue(new Question("In what year did the Titanic sink?", "1912", "1905", "1923", "1898", "1912"));

        questions.Enqueue(new Question("Who painted the Mona Lisa?", "Leonardo da Vinci", "Vincent van Gogh", "Pablo Picasso", "Claude Monet", "Leonardo da Vinci"));

        DisplayNextQuestion();
    }

    private void Update()
    {
        if (serialPortManager != null)
        {
            string currentData = serialPortManager.linedata.Trim();

            if (IsButtonPress(currentData) && currentData != lastData)
            {
                if (!buttonClicked)
                {
                    ClickButton(GetButtonFromData(currentData));
                    buttonClicked = true; 
                }
            }
            else
            {   
                buttonClicked = false;
            }
            lastData = currentData;
        }
       
    }

    private bool IsButtonPress(string data)
    {
        return data == "A" || data == "B" || data == "C" || data == "D";
    }

    private Button GetButtonFromData(string data)
    {
        switch (data)
        {
            case "A":
                return option1Button;
            case "B":
                return option2Button;
            case "C":
                return option3Button;
            case "D":
                return option4Button;
            default:
                Debug.LogWarning($"Unknown data received: {data}");
                return null;
        }
    }

    private void ClickButton(Button button)
    {
        if(button != null)
        {
            button.onClick.Invoke();
        }
    }
    public void SetQuestion(string question, string option1, string option2, string option3, string option4, string correctOption)
    {
        questionText.text = question;

        option1Button.GetComponentInChildren<TMP_Text>().text = option1;
        option2Button.GetComponentInChildren<TMP_Text>().text = option2;
        option3Button.GetComponentInChildren<TMP_Text>().text = option3;
        option4Button.GetComponentInChildren<TMP_Text>().text = option4;

        correctAnswer = correctOption;


        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();
        option4Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => CheckAnswer(option1, option1Button));
        option2Button.onClick.AddListener(() => CheckAnswer(option2, option2Button));
        option3Button.onClick.AddListener(() => CheckAnswer(option3, option3Button));
        option4Button.onClick.AddListener(() => CheckAnswer(option4, option4Button));

        ResetButtonColor();
    }

    void CheckAnswer(string selectedOption, Button selectedButton)
    {
        ResetButtonColor();

        if(selectedOption == correctAnswer)
        {
            selectedButton.GetComponent<Image>().color = correctButtonColor;
            Debug.Log("Correct");
            StartCoroutine(ShowNextQuestionAfterDelay(answerDelay));
        }
        else
        {
            selectedButton.GetComponent<Image>().color = incorrectButtonColor;
            Debug.Log("Incorrect");
        }
    }

    private IEnumerator ShowNextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayNextQuestion();
    }

    private void DisplayNextQuestion()
    {
        if (questions.Count > 0)
        {
            var question = questions.Dequeue();
            SetQuestion(question.QuestionText, question.Option1, question.Option2, question.Option3, question.Option4, question.CorrectAnswer);
        }
        else
        {
            Debug.Log("No more questions.");
            
        }
    }

    [System.Serializable]
    public class Question
    {
        public string QuestionText;
        public string Option1;
        public string Option2;
        public string Option3;
        public string Option4;
        public string CorrectAnswer;

        public Question(string questionText, string option1, string option2, string option3, string option4, string correctAnswer)
        {
            QuestionText = questionText;
            Option1 = option1;
            Option2 = option2;
            Option3 = option3;
            Option4 = option4;
            CorrectAnswer = correctAnswer;
        }
    }

    void ResetButtonColor()
    {
        option1Button.GetComponent<Image>().color = defaultButtonColor;
        option2Button.GetComponent<Image>().color = defaultButtonColor;
        option3Button.GetComponent<Image>().color = defaultButtonColor;
        option4Button.GetComponent<Image>().color = defaultButtonColor;
    }


}
