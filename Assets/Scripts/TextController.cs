using TMPro;
using System.Collections;
using UnityEngine;

public class TextController : Singleton<TextController>
{
    [SerializeField] private float textDelay = .1f;
    [SerializeField] private bool isTextSoundEnabled = true;
    private TextMeshProUGUI textBox;
    private AudioSource audioSource;
    private string currentText = "";

    protected override void Awake()
    {
        base.Awake();

        textBox = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator DisplayText()
    {
        if (textDelay > 0)
        {
            for (int i = 0; i < currentText.Length; ++i)
            {
                textBox.text = currentText.Substring(0, i);

                if (isTextSoundEnabled)
                {
                    audioSource.PlayOneShot(audioSource.clip);
                }

                yield return new WaitForSeconds(textDelay);
            }
        }

        if (textBox)
        {
            textBox.text = currentText;
        }
    }

    public void SetText(string text)
    {
        StopAllCoroutines();
        currentText = text;
        StartCoroutine(DisplayText());
    }
}
