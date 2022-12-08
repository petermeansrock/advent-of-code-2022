using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contestant : MonoBehaviour {
    [SerializeField]
    private GameObject rock;
    [SerializeField]
    private GameObject paper;
    [SerializeField]
    private GameObject scissors;

    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }
}
