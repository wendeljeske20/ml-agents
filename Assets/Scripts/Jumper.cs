using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Jumper : Agent
{
	[SerializeField]
	private float moveTime;

	[SerializeField]
	private float jumpForce;

	[SerializeField]
	private KeyCode jumpKey;

	[NonSerialized]
	public Transform[] roads;

	private int currentRoadIndex;

	private bool jumping;
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
		if (!jumping && !moving)
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

	//private IEnumerator MoveLeft()
	//{
	//	if (moving || currentRoadIndex == 0)
	//	{
	//		yield break;
	//	}
	//	currentRoadIndex--;
	//	Vector3 newPosition = transform.position;
	//	newPosition.x = roads[currentRoadIndex].position.x;
	//	moving = true;

	//	while (transform.position != newPosition)
	//	{
	//		transform.position = Vector3.MoveTowards(transform.position, newPosition, moveTime);
	//		yield return null;
	//	}

	//	moving = false;
	//}

	//private IEnumerator MoveRight()
	//{
	//	if (moving || currentRoadIndex == roads.Length - 1)
	//	{
	//		yield break;
	//	}

	//	currentRoadIndex++;
	//	Vector3 newPosition = transform.position;
	//	newPosition.x = roads[currentRoadIndex].position.x;
	//	moving = true;

	//	while (transform.position != newPosition)
	//	{
	//		transform.position = Vector3.MoveTowards(transform.position, newPosition, moveTime);
	//		yield return null;
	//	}

	//	moving = false;
	//}

	private void Jump()
	{
		if (!moving && !jumping)
		{
			rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
			jumping = true;
		}
	}

	private void Reset()
	{
		score = 0;
		jumping = false;
		moving = false;
		StopAllCoroutines();
		//Reset Movement and Position
		transform.position = startingPosition;
		currentRoadIndex = roads.Length / 2;
		rb.velocity = Vector3.zero;

		OnReset?.Invoke();
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		Debug.Log(Mathf.FloorToInt(vectorAction[0]));

		//if (moving || jumping)
		{
		//	return;
		}

		if (Mathf.FloorToInt(vectorAction[0]) == 1)
		{
			//StartCoroutine(MoveLeft());
			MoveLeft();
		}
		else if (Mathf.FloorToInt(vectorAction[0]) == 2)
		{
			//StartCoroutine(MoveRight());
			MoveRight();
		}
		else if (Mathf.FloorToInt(vectorAction[0]) == 3)
		{
			Jump();
		}
	}

	public override void OnEpisodeBegin()
	{
		Reset();
	}

	public override void Heuristic(float[] actionsOut)
	{
		actionsOut[0] = 0;

		if (Input.GetKey(KeyCode.A))
		{
			actionsOut[0] = 1;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			actionsOut[0] = 2;
		}
		else if (Input.GetKey(jumpKey))
		{
			actionsOut[0] = 3;
		}
	}

	private void OnCollisionEnter(Collision collidedObj)
	{
		if (collidedObj.gameObject.CompareTag("Street"))
		{
			jumping = false;
		}
		else if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover"))
		{
			AddReward(-1f);
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
}