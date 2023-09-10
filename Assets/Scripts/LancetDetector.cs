using UnityEngine;

public class LancetDetector : MonoBehaviour
{
    public QuizManager quizManager; // Assign this in the inspector

private void OnTriggerEnter(Collider other)
{
    if (QuizManager.ActiveQuiz == null) 
        return;

    SphereIdentifier sphere = other.GetComponent<SphereIdentifier>();
    if (sphere)
    {
        QuizManager.ActiveQuiz.AnswerQuestion(sphere.sphereID);
    }
}

}
