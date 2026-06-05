using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private VideoClip[] tutorialVideos;
    [SerializeField] private string[] tutorialTexts;

    private int currentStep = 0;

    private void Start()
    {
        ShowStep(0);
    }

    public void OnNextButtonPressed()
    {
        currentStep++;

        if (currentStep >= tutorialTexts.Length)
        {
            SceneManager.LoadScene(1);
            return;
        }

        ShowStep(currentStep);
    }

    private void ShowStep(int step)
    {
        tutorialText.text = tutorialTexts[step];

        videoPlayer.Stop();
        videoPlayer.clip = tutorialVideos[step];
        videoPlayer.Play();
    }
}