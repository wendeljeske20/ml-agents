using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


/*
 * Spawns the Mover Objects (Enemies) with an interval you determine.
 */
public class Spawner : MonoBehaviour
{
	public Transform[] roads;

	[SerializeField]
	private List<GameObject> spawnableObjects;

	[Tooltip("The Spawner waits a random number of seconds between these two interval each time a object was spawned.")]
	[SerializeField]
	private float minSpawnRate;

	[SerializeField]
	private float maxSpawnRate;

	private Jumper jumper;

	private List<GameObject> spawnedObjects = new List<GameObject>();

	private void Awake()
	{
		jumper = GetComponentInChildren<Jumper>();
		//Subscribes to Reset of Player
		jumper.OnReset += DestroyAllSpawnedObjects;

		StartCoroutine(nameof(Spawn));
	}

	private IEnumerator Spawn()
	{
		Transform road = roads[Random.Range(0, roads.Length - 1)];
		GameObject obj = Instantiate(GetRandomSpawnableFromList(), road.position, road.rotation, transform);
		spawnedObjects.Add(obj);

		yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
		StartCoroutine(Spawn());
	}

	private GameObject GetRandomSpawnableFromList()
	{
		int randomIndex = UnityEngine.Random.Range(0, spawnableObjects.Count);
		return spawnableObjects[randomIndex];
	}

	private void DestroyAllSpawnedObjects()
	{
		for (int i = spawnedObjects.Count - 1; i >= 0; i--)
		{
			Destroy(spawnedObjects[i]);
			spawnedObjects.RemoveAt(i);
		}
	}
}