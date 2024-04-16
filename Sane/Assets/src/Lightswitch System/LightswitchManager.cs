using UnityEngine;

public class LightswitchManager : MonoBehaviour, LightswitchInteractable {
    [SerializeField] [Range(0, 360)] private float interactAngle = 90;
    [SerializeField] private bool _defaultState = true;
    public Light[] lights;
    private Collider _collider;

    private bool _isLightOn;

    public void Awake() {
        _collider = GetComponent<Collider>();
        _isLightOn = _defaultState;
        SetLights();
    }

    public void Interact() {
        _isLightOn = !_isLightOn;

        SetLights();
    }

    public bool GetConeInteractableSettings(out Vector3 objPos, out Vector3 axis, out float angle) {
        objPos = _collider.bounds.center;
        axis = transform.forward;
        angle = interactAngle;
        return true;
    }

    private void SetLights() {
        foreach (Light light in lights) light.enabled = _isLightOn;
    }
}