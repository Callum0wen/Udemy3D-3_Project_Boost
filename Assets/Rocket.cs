using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;

	Rigidbody rigidBody;
	AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		Rotate();
		Thrust();
	}

	void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				break;
			case "Finish":
				print("Hit Finish");
				SceneManager.LoadScene(1);
				break;
			default:
				print("Dead");
				SceneManager.LoadScene(0);
				break;
		}
	}

	private void Rotate()
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

	private void Thrust()
	{
		float thrustThisFrame = mainThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.Space))    //can thrust while rotating
		{
			rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
			if (!audioSource.isPlaying)		//so it doesn't layer
			{
				audioSource.Play();
			}
		}
		else
		{
			audioSource.Stop();
		}
	}
}
