using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


/*
 * Spawns the Mover Objects (Enemies) with an interval you determine.
 */
public class Spawner : Singleton<Spawner>
{
	public Transform[] roads;

	[SerializeField]
	private List<GameObject> spawnableObjects;

	[Tooltip("The Spawner waits a random number of seconds between these two interval each time a object was spawned.")]
	[SerializeField]
	private float minSpawnRate;

	[SerializeField]
	private float maxSpawnRate;

	[SerializeField]
	private Jumper jumper;

	private List<GameObject> spawnedObjects = new List<GameObject>();

	int index;

	protected override void Awake()
	{
		base.Awake();

		jumper.roads = roads;
		jumper.OnReset += DestroyAllSpawnedObjects;
		jumper.enabled = true;
		StartCoroutine(nameof(Spawn));
	}

	private IEnumerator Spawn()
	{
		index = (index + 1) % roads.Length;
		//Transform road = roads[Random.Range(0, roads.Length)];
		Transform road = roads[index];
		GameObject obj = Instantiate(GetRandomSpawnableFromList(), road.position, road.rotation, transform);
		spawnedObjects.Add(obj);

		yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
		StartCoroutine(Spawn());
	}

	private GameObject GetRandomSpawnableFromList()
	{
		int randomIndex = Random.Range(0, spawnableObjects.Count);
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