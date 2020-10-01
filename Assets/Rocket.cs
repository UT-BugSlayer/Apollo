using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    Rigidbody rigidBody;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        Thrust();
        Rotate();
    }

    private void Rotate() {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        // freeze rotation before we take manual control of rotation
        rigidBody.freezeRotation = true;
        // Rotate left or right
        if (Input.GetKey(KeyCode.A)) {
            // rotate left about the Z-axis
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            // rotate right about the Z-axis
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        // resume physics control
        rigidBody.freezeRotation = false;
    }

    void OnCollisionEnter(Collision collision){
        switch (collision.gameObject.tag){
            case "Friendly":
                print("Friendly");
                break;
            
            default:
                print("Deadly");
                break;
        }
    }

    private void Thrust() {
        // GetKey applies all the time and will report the status of the named key.
        if (Input.GetKey(KeyCode.Space)) {
            // Adding force in the direction the ship is facing
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            // so audio doesnt layer
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        }
        else {
            audioSource.Stop();
        }
    }
}