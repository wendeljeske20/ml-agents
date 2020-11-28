using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instanceObject;

	protected virtual void Awake()
	{
		if (!instanceObject)
		{
			instanceObject = GetComponent<T>();
		}
	}

	public static T main
	{
		get
		{
			if (instanceObject == null)
			{
				string[] split = typeof(T).Name.Split('.');
				string name = split[split.Length - 1].ToString();

				Debug.Log($"{name} instance is null");
			}

			return instanceObject;
		}
	}
}
