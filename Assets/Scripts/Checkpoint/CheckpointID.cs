using UnityEngine;

public class CheckpointID : MonoBehaviour
{
	public int ID;

	private void Awake()
	{
		if (ID <= PlayerPrefs.GetInt("lastCheckpoint"))
		{
			Destroy(gameObject);
		}
	}
}
