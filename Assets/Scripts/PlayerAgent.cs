using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
	[SerializeField]
	private Player player;

	[SerializeField]
	private float moveTime;

	[SerializeField]
	private float jumpForce;

	[SerializeField]
	private KeyCode jumpKey;

	[NonSerialized]
	public Transform[] roads;

	private int currentRoadIndex;


	[NonSerialized]
	public int score;

	public event Action OnReset;

	public override void Initialize()
	{
		player.Init();
		player.jumper = this;
		roads = Spawner.main.roads;
		currentRoadIndex = roads.Length / 2;

		Vector3 position = player.transform.position;
		position.x = roads[currentRoadIndex].position.x;
		player.transform.position = position;
		player.startingPosition = position;
	}

	private void FixedUpdate()
	{
		//if (!player.jumping && !player.moving)
		{
			RequestDecision();
		}

		Vector3 newPosition = player.transform.position;
		newPosition.x = roads[currentRoadIndex].position.x;
		player.transform.position = Vector3.MoveTowards(player.transform.position, newPosition, moveTime);
		player.moving = player.transform.position != newPosition;
	}

	private void MoveLeft()
	{
		if (player.moving || currentRoadIndex == 0)
		{
			return;
		}
		currentRoadIndex--;
	}

	private void MoveRight()
	{
		if (player.moving || currentRoadIndex == roads.Length - 1)
		{
			return;
		}

		currentRoadIndex++;
	}

	private void Jump()
	{
		if (!player.jumping)
		{
			player.rb.velocity = Vector3.zero;
			player.rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
			player.jumping = true;
		}
	}

	private void Reset()
	{
		score = 0;
		player.jumping = false;
		player.moving = false;
		StopAllCoroutines();
		//Reset Movement and Position
		player.transform.position = player.startingPosition;
		currentRoadIndex = roads.Length / 2;
		player.rb.velocity = Vector3.zero;

		OnReset?.Invoke();
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		//if (player.moving || player.jumping)
		{
			//return;
		}

		//Debug.Log(Mathf.FloorToInt(vectorAction[0]));

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

		if (Mathf.FloorToInt(vectorAction[1]) == 1)
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
		actionsOut[1] = 0;

		if (Input.GetKey(KeyCode.A))
		{
			actionsOut[0] = 1;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			actionsOut[0] = 2;
		}

		if (Input.GetKey(jumpKey))
		{
			actionsOut[1] = 1;
		}
	}
}