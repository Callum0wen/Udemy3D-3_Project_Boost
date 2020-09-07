using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;

	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip death;
	[SerializeField] AudioClip success;

	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem deathParticels;
	[SerializeField] ParticleSystem successParticles;

	Rigidbody rigidBody;
	AudioSource audioSource;

	enum State { Alive, Dying, Transcending}
	State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		if (state == State.Alive)
		{
			RespondToRotateInput();
			RespondToThrustInput();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (state != State.Alive) {  return; } // ignore collisions when dead

		switch (collision.gameObject.tag)
		{
			case "Friendly":
				break;
			case "Finish":
				StartSuccessSequence();
				break;
			default:
				StartDeathSequence();
				break;
		}
	}

	private void StartSuccessSequence()
	{
		state = State.Transcending;
		audioSource.Stop();
		audioSource.PlayOneShot(success);
		successParticles.Play();
		Invoke("LoadNextLevel", 1f);    // parameterise time
	}

	private void StartDeathSequence()
	{
		state = State.Dying;
		audioSource.Stop();
		audioSource.PlayOneShot(death);
		deathParticels.Play();
		Invoke("LoadFirstLevel", 1f);   // paramerterise time
	}

	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	private void LoadNextLevel()
	{
		SceneManager.LoadScene(1);  //todo allow for more than 2 levels
	}

	private void RespondToRotateInput()
	{
		rigidBody.freezeRotation = true;    //take manual control of rotation

		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.forward * rotationThisFrame);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		rigidBody.freezeRotation = false; //resume physics control of rotation
	}

	private void RespondToThrustInput()
	{
		float thrustThisFrame = mainThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.Space))    //can thrust while rotating
		{
			ApplyThrust(thrustThisFrame);
		}
		else
		{
			audioSource.Stop();
			mainEngineParticles.Stop();
		}
	}

	private void ApplyThrust(float thrustThisFrame)
	{
		rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
		if (!audioSource.isPlaying)     //so it doesn't layer
		{
			audioSource.PlayOneShot(mainEngine);
			mainEngineParticles.Play();
		}
	}
}
