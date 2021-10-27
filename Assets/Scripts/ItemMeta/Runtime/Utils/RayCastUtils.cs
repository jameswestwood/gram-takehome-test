
using System.Collections.Generic;
using UnityEngine;

public static class RaycastUtils
{
	public static void RaycastTargets<T>(Vector3 origin, int distance, int mask, out List<T> found)
	{
		found = new List<T>();
		Ray ray = Camera.main.ScreenPointToRay(origin);
		RaycastHit[] hits = Physics.RaycastAll(ray, distance, mask);
		foreach (RaycastHit hit in hits)
		{
			T target = hit.collider.GetComponent<T>();
			if (target != null)
			{
				found.Add(target);
			}
		}
	}

	public static void RaycastTarget<T>(Vector3 origin, int distance, int mask, RaycastHit[] hits, out T found)
	{
		found = default(T);
		if (Camera.main == null) return;
		Ray ray = Camera.main.ScreenPointToRay(origin);
		if (Physics.RaycastNonAlloc(ray, hits, distance, mask) > 0)
		{
			foreach (RaycastHit hit in hits)
			{
				T target;
				if (typeof(T) == typeof(GameObject))
				{
					Debug.LogError("Cannot return gameobject;");
				}

				target = hit.collider.GetComponent<T>();

				if (target != null)
				{
					found = target;
					return;
				}
			}
		}
	}

	public static void RaycastTarget(Vector3 origin, int distance, int mask, out GameObject found)
	{
		found = null;
		if (Camera.main == null) return;
		Ray ray = Camera.main.ScreenPointToRay(origin);
		RaycastHit[] hits = Physics.RaycastAll(ray, distance, mask);
		foreach (RaycastHit hit in hits)
		{
			GameObject target;
			target = hit.collider.gameObject;
			found = target;
			return;

		}
	}

	public static bool RaycastAllTargets<T>(Vector2 origin, Vector2 direction, int mask, out List<T> targets, out List<RaycastHit2D> hits)
	{
		targets = new List<T>();
		hits = new List<RaycastHit2D>();
		int RaycastDistance = 10;
		List<RaycastHit2D> hitList = new List<RaycastHit2D>(Physics2D.RaycastAll(origin, direction, RaycastDistance, mask));

		for (int i = 0; i < hitList.Count; i++)
		{
			var currentHit = hitList[i];

			if (currentHit.collider != null)
			{
				T target = currentHit.collider.GetComponent<T>();
				if (target != null)
				{
					targets.Add(target);
					hits.Add(currentHit);
				}
			}
		}

		bool isOverTargets = targets.Count > 0;
		return isOverTargets;
	}

}