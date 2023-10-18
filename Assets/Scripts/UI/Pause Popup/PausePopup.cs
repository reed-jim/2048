using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private Button muteButton;
    [SerializeField] private Button removeAdButton;

    private void Start()
    {
        quitButton.onClick.AddListener(QuitToHome);
        replayButton.onClick.AddListener(Replay);
        continueButton.onClick.AddListener(Continue);

        muteButton.onClick.AddListener(Mute);
    }

    private void QuitToHome()
    {
        Addressables.LoadSceneAsync("Menu");
    }

    private void Replay()
    {
        Addressables.LoadSceneAsync("Gameplay");
    }

    private void Continue()
    {
        gameObject.SetActive(false);
    }

    private void Mute()
    {
        AudioListener.pause = true;
    }
}