using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip finishSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem finishParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        //  Can only rotate and RespondToThrustInput when alive
        if(state==State.Alive){
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void RespondToThrustInput() {
        // GetKey applies all the time and will report the status of the named key.
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void RespondToRotateInput() {
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
        if (state!= State.Alive){
            return;
        }

        switch (collision.gameObject.tag){
            case "Friendly":
                print("Friendly");
                break;

            case "Finish":
                StartTransitionSequence();
                break;
            
            default:
                StartDyingSequence();
                break;
        }
    }

    private void StartTransitionSequence(){
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finishSound);
        finishParticles.Play();
        Invoke("LoadNextScene", 1f);  
    }

    private void StartDyingSequence(){
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("LoadCurrentScene", 1f); 
    }

    private void LoadNextScene(){
        // TODO: allow for more than 2 levels
        SceneManager.LoadScene(1);
    }

    private void LoadCurrentScene (){
        SceneManager.LoadScene(0);
    }

    private void ApplyThrust(){
        // Adding force in the direction the ship is facing
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        // so audio doesnt layer
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
