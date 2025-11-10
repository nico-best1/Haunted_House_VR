using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuManager : MonoBehaviour
{
    public GameObject menuCanvas;
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;
    public XRRayInteractor rightHandRayInteractor;
    public XRRayInteractor leftHandRayInteractor;
    public Button PlayButton;
    public Button OptionsButton;
    public Button QuitButton;
    public Button BackToMenuButton;
    public AudioMixer audioMixer;
    public GameObject controlsText;
    public GameObject title1;
    public GameObject title2;
    public GameObject Instructions;

    private void Start()
    {
        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;
        menuCanvas.SetActive(true);

        if (rightHandRayInteractor != null) rightHandRayInteractor.enabled = true;
        if (leftHandRayInteractor != null) leftHandRayInteractor.enabled = true;
    }

    public void OnPlayPressed()
    {
        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;
        menuCanvas.SetActive(false);
        Instructions.SetActive(true);

        if (rightHandRayInteractor != null) rightHandRayInteractor.enabled = false;
        if (leftHandRayInteractor != null) leftHandRayInteractor.enabled = false;
    }

    public void OnOptionsPressed()
    {
        if (PlayButton != null) PlayButton.gameObject.SetActive(false);
        if (OptionsButton != null) OptionsButton.gameObject.SetActive(false);
        if (QuitButton != null) QuitButton.gameObject.SetActive(false);
        if (title1 != null) title1.gameObject.SetActive(false);
        if (title2 != null) title2.gameObject.SetActive(false);
        if (BackToMenuButton != null) BackToMenuButton.gameObject.SetActive(true);
        if (controlsText != null) controlsText.gameObject.SetActive(true);
    }

    public void OnBackToMenuPressed()
    {
        if (PlayButton != null) PlayButton.gameObject.SetActive(true);
        if (OptionsButton != null) OptionsButton.gameObject.SetActive(true);
        if (QuitButton != null) QuitButton.gameObject.SetActive(true);
        if (title1 != null) title1.gameObject.SetActive(true);
        if (title2 != null) title2.gameObject.SetActive(true);
        if (BackToMenuButton != null) BackToMenuButton.gameObject.SetActive(false);
        if (controlsText != null) controlsText.gameObject.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }
}