using UnityEngine;
using UnityEngine.UI;

public class InteractHandUI : MonoBehaviour {
    public Sprite openHand;
    public Sprite closedHand;

    private Image image;

    private void Start() {
        image = GetComponent<Image>();
    }

    public void OpenHand() {
        image.sprite = openHand;
    }

    public void CloseHand() {
        image.sprite = closedHand;
    }

    public void fadeIn(float time) {
        image.CrossFadeAlpha(1, time, false);
    }

    public void fadeOut(float time) {
        image.CrossFadeAlpha(0, time, false);
    }
}