using UnityEngine;

public class SetResolution : MonoBehaviour {
    // Start is called before the first frame update
    private void Start() {
        Resolution[] resolutions = Screen.resolutions;

        Screen.SetResolution(resolutions[1].width, resolutions[1].height, true);
    }

    // Update is called once per frame
    private void Update() {
    }
}