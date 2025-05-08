using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using Platformer.Mechanics;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator CheckpointText;
    private SpriteRenderer SpriteRenderer;
    private PlayerController PlayerController;

    readonly PlatformerModel model = GetModel<PlatformerModel>();

    private APIResponse response = new();

    [Range(0,100)]
    public int Age;

    public bool Gender;

    //private void Awake()
    //{
    //    PlayerPrefs.DeleteAll();
    //}

    private void Start()
    {
        transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        model.spawnPoint.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));

        SpriteRenderer = GetComponent<SpriteRenderer>();
        PlayerController = GetComponent<PlayerController>();
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

            StartCoroutine(GetAPIResponse());

            Destroy(collision.gameObject);
        }
    }

    private IEnumerator GetAPIResponse()
    {
        string url = "https://randomuser.me/api/";
		using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            response = JsonConvert.DeserializeObject<APIResponse>(json);

            Result result = response.results[0];
            Debug.Log($"Age: {result.dob.age}");
            Debug.Log($"Gender: {result.gender}");

            SetPlayerCharacteristics(response);
        }
        else
        {
            Debug.LogWarning("Error al consultar la API");
        }
	}

    private void SetPlayerCharacteristics(APIResponse response)
    {
        Result result = response.results[0];
        SetSize(result.dob.age);
        SetColor(result.gender, result.dob.age);
        SetSpeed(result.dob.age);
    }

    private void SetSize(int age)
    {
        float size = -Mathf.Abs(age - 50) + 50;
        float normal = Mathf.InverseLerp(0, 50, size);
        size = Mathf.Lerp(0.2f, 1f, normal); 
        transform.localScale = new Vector2(size, size);
    }

    private void SetColor(string gender, int age)
    {
        Color color;
        float less = (255f - age) / 255f;
        Debug.Log(less);

        if (gender == "male")
        {
            color = new Color(0, less, less);
        }
        else
        {
            color = new Color(less, 0, less);
        }

        SpriteRenderer.color = color;
    }

    private void SetSpeed(int age)
    {
        float speed = -Mathf.Abs(age - 50) + 50;
        float normal = Mathf.InverseLerp(0, 50, speed);
        speed = Mathf.Lerp(0.5f, 3f, normal); 
        PlayerController.maxSpeed = speed;
    }
}
