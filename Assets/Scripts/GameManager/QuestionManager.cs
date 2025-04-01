using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : Singleton<QuestionManager>
{
    private QuestionManagerSO data;
    private QuestionManager()
    {
        if (data == null)
        {
            data = Resources.Load<QuestionManagerSO>("Data/QuestionManagerSO");
            if (data == null)
                Debug.LogError("º”‘ÿQuestionManagerSO ß∞‹£°");
        }
    }

    public QuestionSO GetRandomQuestion()
    {
        return data.questions.Random();
    }
}
