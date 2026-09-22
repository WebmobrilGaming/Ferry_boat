
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundEffects : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] private Button thisButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        thisButton=GetComponent<Button>();
    }

    void OnEnable()
    {
        thisButton.onClick.AddListener(OnButtonClicked);
    }
    void OnDisable()
    {
        thisButton.onClick.RemoveListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        AudioManager.Instance.PlaySFX(AudioState.Button_Highlight);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(AudioState.Button_Highlight);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(AudioState.Button_Click);
    }
}
