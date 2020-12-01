using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Player : Agent
{
	[SerializeField]
	private float moveTime;


	[NonSerialized]
	public Transform[] roads;

	private int currentRoadIndex;

	private bool moving;
	private Rigidbody rb;
	private Vector3 startingPosition;

	private int score = 0;
	public event Action OnReset;

	public override void Initialize()
	{
		rb = GetComponent<Rigidbody>();
		roads = Spawner.main.roads;
		currentRoadIndex = roads.Length / 2;

		Vector3 position = transform.position;
		position.x = roads[currentRoadIndex].position.x;
		transform.position = position;
		startingPosition = transform.position;
	}

	private void FixedUpdate()
	{
		if (!moving)
		{
			RequestDecision();
		}

		Vector3 newPosition = transform.position;
		newPosition.x = roads[currentRoadIndex].position.x;
		transform.position = Vector3.MoveTowards(transform.position, newPosition, moveTime);
		moving = transform.position != newPosition;
	}

	private void MoveLeft()
	{
		if (moving || currentRoadIndex == 0)
		{
			return;
		}
		currentRoadIndex--;
	}

	private void MoveRight()
	{
		if (moving || currentRoadIndex == roads.Length - 1)
		{
			return;
		}

		currentRoadIndex++;
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		if (moving)
		{
			return;
		}

		if (Mathf.FloorToInt(vectorAction[0]) == 1)
		{
			MoveLeft();
		}
		else if (Mathf.FloorToInt(vectorAction[0]) == 2)
		{
			MoveRight();
		}
	}

	public override void OnEpisodeBegin()
	{
		score = 0;
		moving = false;
		StopAllCoroutines();

		transform.position = startingPosition;
		currentRoadIndex = roads.Length / 2;
		rb.velocity = Vector3.zero;

		OnReset?.Invoke();
	}

	private void OnCollisionEnter(Collision collidedObj)
	{
		if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover"))
		{
			SetReward(0f);
			EndEpisode();
		}
	}

	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.gameObject.CompareTag("score"))
		{
			AddReward(0.1f);
			score++;
			ScoreCollector.Instance.AddScore(score);
		}
	}
	//mlagents-learn trainer_config.yaml --run-id="JumperAI_2" --resume
}