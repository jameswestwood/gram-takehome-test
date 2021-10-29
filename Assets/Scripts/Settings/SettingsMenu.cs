using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
  [SerializeField] private RectTransform _rectTransform;
  
  private void Start()
  {
    // set to default position
    _rectTransform.offsetMin = new Vector2(0, 0);
    _rectTransform.offsetMax = new Vector2(0, 0);
    
    Toggle(false);
  }

  public void Toggle(bool open)
  {
    gameObject.SetActive(open);
  }
}
