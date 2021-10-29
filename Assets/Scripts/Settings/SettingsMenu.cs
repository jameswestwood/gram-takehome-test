using UnityEngine;
using UnityEngine.UI;

public static class Options
{
  public static string volume = "volume";
}

public class SettingsMenu : MonoBehaviour
{
  [SerializeField] private RectTransform _rectTransform;
  [SerializeField] private Slider _volumeSlider;
  
  private void Start()
  {
    // set to default position
    _rectTransform.offsetMin = new Vector2(0, 0);
    _rectTransform.offsetMax = new Vector2(0, 0);
    
    Toggle(false);

    SetInitialOptionValues();
  }

  public void Toggle(bool open)
  {
    gameObject.SetActive(open);
  }

  private void SetInitialOptionValues()
  {
    _volumeSlider.value = PlayerPrefs.HasKey(Options.volume) ? PlayerPrefs.GetFloat(Options.volume) : 0.5f;
  }

  public void SaveVolumeSliderValue()
  {
    PlayerPrefs.SetFloat(Options.volume, _volumeSlider.value);
  }
}
