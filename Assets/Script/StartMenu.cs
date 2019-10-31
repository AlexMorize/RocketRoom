using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button Btn_Campagne;
    public string SceneCampagne;
    public Button Btn_ShareScreen;
    public string SceneShareScreen;
    public Button Btn_LAN;
    public string SceneLAN;
    public Button Btn_Online;
    public string SceneOnline;

    public MonoBehaviour RéglageQualité;

    // Start is called before the first frame update
    void Start()
    {
        Btn_Campagne.interactable = SceneCampagne != "";
        Btn_LAN.interactable = SceneLAN != "";
        Btn_Online.interactable = SceneOnline != "";
        Btn_ShareScreen.interactable = SceneShareScreen != "";

        Btn_Campagne.onClick.AddListener(() => { SceneManager.LoadScene(SceneCampagne); });
        Btn_ShareScreen.onClick.AddListener(() => { SceneManager.LoadScene(SceneShareScreen); });
        Btn_LAN.onClick.AddListener(() => { SceneManager.LoadScene(SceneLAN); });
        Btn_Online.onClick.AddListener(() => { SceneManager.LoadScene(SceneOnline); });

        if (!Btn_Campagne.interactable) Btn_Campagne.GetComponentInChildren<Text>().color = Color.gray;
        if (!Btn_ShareScreen.interactable) Btn_ShareScreen.GetComponentInChildren<Text>().color = Color.gray;
        if (!Btn_LAN.interactable) Btn_LAN.GetComponentInChildren<Text>().color = Color.gray;
        if (!Btn_Online.interactable) Btn_Online.GetComponentInChildren<Text>().color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
