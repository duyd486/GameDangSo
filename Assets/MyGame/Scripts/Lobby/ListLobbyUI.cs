using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ListLobbyUI : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private GameObject lobbyInfoTemplate;
    [SerializeField] private GameObject container;

    private void Start()
    {
        MenuUI.Instance.OnListLobbyClick += MenuUI_OnListLobbyClick;


        Hide();
        backBtn.onClick.AddListener(() =>
        {
            Hide();
        });
        reloadBtn.onClick.AddListener(() =>
        {
            Debug.Log("Reload");
            LobbyManager.Instance.ListLobbies(UpdateListLobby);
        });
    }

    private void MenuUI_OnListLobbyClick(object sender, System.EventArgs e)
    {
        Show();
        UpdateListLobby();
    }

    public void UpdateListLobby()
    {
        List<Lobby> lobbies = LobbyManager.Instance.GetCurrentLobbies();

        foreach (Transform chil in container.transform)
        {
            chil.gameObject.SetActive(false);
        }

        for (int i = 0; i < lobbies.Count; i++)
        {
            GameObject lobbyInfoOb = Instantiate(lobbyInfoTemplate, container.transform);
            lobbyInfoOb.SetActive(true);
            lobbyInfoOb.GetComponent<LobbySingleUI>().UpdateLobby(lobbies[i]);
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
