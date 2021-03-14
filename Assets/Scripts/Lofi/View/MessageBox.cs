using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lofi.Game
{
    public class MessageBox : MonoBehaviour
    {

        string exitMessage = "\n\nPress Enter to exit.";
        string diedMessage = "You Died.\nEvil reigns but all is not lost.\nAnother hero will decide.";
        string loadMessage = "Legends say a hero will choose to free the land from evil. Will you make the choice?";
        string winMessage = "You have defeated the final boss!\n\nYou are a legend in your time.";

        Image messageBackground;
        Text messageText;
        bool exiting = false;
        bool died = false;
        bool won = false;

        void Start()
        {
            messageBackground = GetComponent<Image>();
            messageText = GetComponentInChildren<Text>();
            messageBackground.enabled = false;
            messageText.text = "";
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.instance == null)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!exiting)
                {
                    GameManager.instance.messageDisplayed = true;
                    messageBackground.enabled = true;
                    messageText.enabled = true;
                    messageText.text = exitMessage;
                    exiting = true;
                }
                else if (!died)
                {
                    GameManager.instance.messageDisplayed = false;
                    messageBackground.enabled = false;
                    messageText.enabled = false;
                    messageText.text = "";
                    exiting = false;
                }
                else if (died)
                {
                    //SceneManager.UnloadSceneAsync("GameScene");
                    //Resources.UnloadUnusedAssets();
                    //SceneManager.LoadScene("TitleScene");
                    //Destroy(GameManager.instance);
                    Application.Quit();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (exiting)
                {
                    //SceneManager.UnloadSceneAsync("GameScene");
                    //Resources.UnloadUnusedAssets();
                    //SceneManager.LoadScene("TitleScene");
                    //Destroy(GameManager.instance);
                    Application.Quit();
                }

                if(died)
                {
                    //SceneManager.UnloadSceneAsync("GameScene");
                    //Resources.UnloadUnusedAssets();

                    //SceneManager.LoadScene("TitleScene");
                    //Destroy(GameManager.instance);
                    Application.Quit();
                }

                if (won)
                {                    
                    SceneManager.LoadScene("WinScene");
                }
            }

            if (GameManager.instance != null)
            {
                if(GameManager.instance.GameOver)
                {
                    GameManager.instance.messageDisplayed = true;
                    died = true;
                    messageBackground.enabled = true;
                    messageText.enabled = true;
                    messageText.text = diedMessage;
                }
                else if(GameManager.instance.GameWon)
                {
                    GameManager.instance.messageDisplayed = true;
                    won = true;
                    messageBackground.enabled = true;
                    messageText.enabled = true;
                    messageText.text = winMessage;
                }
            }
        }
    }
}