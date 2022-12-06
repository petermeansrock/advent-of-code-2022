using UnityEngine;

public class Walker : MonoBehaviour {
    [SerializeField]
    private float velocity = 0.5f;

    private CharacterController controller;

    void Awake() {
        this.controller = GetComponent<CharacterController>();
    }

    void Update() {
        var movement = Vector3.right * velocity;
        controller.Move(Time.deltaTime * movement);
    }
}
