using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator CheckpointText;

    readonly PlatformerModel model = GetModel<PlatformerModel>();

    private void Start()
    {
        transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        model.spawnPoint.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            PlayerPrefs.SetFloat("x", transform.position.x);
            PlayerPrefs.SetFloat("y", transform.position.y);
            PlayerPrefs.SetInt("lastCheckpoint", collision.GetComponent<CheckpointID>().ID);

            PlayerPrefs.Save();

            model.spawnPoint.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));

            CheckpointText.SetTrigger("Activate");

            Destroy(collision.gameObject);
        }
    }
}
