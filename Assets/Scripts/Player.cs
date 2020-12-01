using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[NonSerialized]
	public PlayerAgent jumper;

	[NonSerialized]
	public Rigidbody rb;

	public bool jumping;
	public bool moving;
	public Vector3 startingPosition;

	public void Init()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collidedObj)
	{
		if (collidedObj.gameObject.CompareTag("Street"))
		{
			jumping = false;
		}
		else if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover"))
		{
			jumper.AddReward(-jumper.GetCumulativeReward() * 0.2f);
			jumper.EndEpisode();
		}
	}

	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.gameObject.CompareTag("score"))
		{
			jumper.AddReward(0.1f);
			jumper.score++;
			ScoreCollector.Instance.AddScore(jumper.score);
		}
	}
}
