using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Outline messageOutline;

    public void SetMessage(string message, bool isError)
    {
        messageText.text = message;
        if (isError)
        {
            messageOutline.effectColor = Color.red;
        }
        else
        {
            messageOutline.effectColor = Color.green;
        }
        gameObject.SetActive(true);
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
