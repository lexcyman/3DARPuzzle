using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Routes : MonoBehaviour
{
    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToSelectionMenu() {
        SceneManager.LoadScene("SelectionMenu");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void GoToPuzzle1()
    {
        SceneManager.LoadScene("Puzzle1");
    }

    public void GoToPuzzle2()
    {
        SceneManager.LoadScene("Puzzle2");
    }

    public void GoToPuzzle3()
    {
        SceneManager.LoadScene("Puzzle3");
    }

    public void GoToPuzzle4()
    {
        SceneManager.LoadScene("Puzzle4");
    }

    public void GoToPuzzle5()
    {
        SceneManager.LoadScene("Puzzle5");
    }

    public void GoToSampleOcclusion()
    {
        SceneManager.LoadScene("SampleOcclusion");
    }
}
