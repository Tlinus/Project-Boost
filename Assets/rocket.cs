using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{
    [SerializeField] float RcsThrust = 100f;
    [SerializeField] float thrust = 100f;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle;

    [SerializeField] AudioClip MainEngine;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip StartLevelSound;

    enum State { Alive, Transcending};
    State state = State.Alive;

    Rigidbody rigidbody;
    AudioSource rocketSound;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rocketSound = GetComponent<AudioSource>();

        rocketSound.PlayOneShot(StartLevelSound);
    }

    // Update is called once per frame
    void Update()
    {
        respondToCommand();
    }

    private void respondToCommand()
    {
        Thrust();
        Rotate();
    }

    private void Rotate()
    {
        rigidbody.freezeRotation = true;

        if(state == State.Transcending)
        {
            return;
        }

        float multiplicatorRotation = RcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(-Vector3.forward * multiplicatorRotation);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * multiplicatorRotation);
        }

        rigidbody.freezeRotation = false;
    }

    private void Thrust()
    {
        if(state == State.Transcending)
        {
            return;
        }
        float multiplicatorThrust = thrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up * multiplicatorThrust);
            mainEngineParticle.Play();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rocketSound.PlayOneShot(MainEngine);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rocketSound.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (state != State.Transcending)
        {
            if (collision.gameObject.tag != "Friendly" && collision.gameObject.tag != "Finnish")
            {

                rigidbody.AddExplosionForce(1000f, collision.contacts[0].point, 1000f);

                Lose();
            }
            if (collision.gameObject.tag == "Finnish")
            {
                Win();
            }

        }
    }

    private void Win()
    {
        rocketSound.Stop();
        rocketSound.PlayOneShot(winSound);
        state = State.Transcending;
        successParticle.Play();
        Invoke("LoadNextScene", 2f);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex +1;

        Console.WriteLine(currentSceneIndex);
        Console.WriteLine(nextScene);


        Debug.Log(SceneManager.sceneCountInBuildSettings);

        if (nextScene > SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
        Debug.Log(currentSceneIndex);
        Debug.Log(nextScene);


        SceneManager.LoadScene(nextScene);
    }

    private void Lose()
    {
        rocketSound.Stop();
        rocketSound.PlayOneShot(DeathSound);
        state = State.Transcending;
        deathParticle.Play();
        rigidbody.freezeRotation = false;
        Invoke("RestartLevel", 2f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
    