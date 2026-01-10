using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string playerName;
    public enum Scene
    {
        Lobby,
        Game
    }

    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public static void LoadSceneByNetwork(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
}
