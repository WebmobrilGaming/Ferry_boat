
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundEffects : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] private Button thisButton;
    [SerializeField] private int sfxKeyValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        thisButton=GetComponent<Button>();
    }

    void OnEnable()
    {
        sfxKeyValue = PlayerPrefs.GetInt(GamePrefs.isSFXOn, 0);
        thisButton.onClick.AddListener(OnButtonClicked);
    }
    void OnDisable()
    {
        thisButton.onClick.RemoveListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        if(sfxKeyValue==0)return;
        AudioManager.Instance.PlaySFX(AudioState.Button_Highlight);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sfxKeyValue == 0) return;
        AudioManager.Instance.PlaySFX(AudioState.Button_Highlight);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfxKeyValue == 0) return;
        AudioManager.Instance.PlaySFX(AudioState.Button_Click);
    }
}
