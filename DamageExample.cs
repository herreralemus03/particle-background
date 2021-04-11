using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Health _health;
    private TextMeshProUGUI _healthText;
    private Canvas _canvas;
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        gameObject.tag = "player";
        AddComponents();
    }

    private void AddComponents()
    {
        _health = gameObject.AddComponent<Health>();
        gameObject.AddComponent<BoxCollider2D>();
        _rb2D = gameObject.AddComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _canvas = FindObjectsOfType<Canvas>().First(e => e.renderMode == RenderMode.ScreenSpaceOverlay);
        _healthText = _canvas.gameObject.AddComponent<TextMeshProUGUI>();
        _healthText.rectTransform.anchorMax = Vector2.one;
        _healthText.rectTransform.anchorMin = Vector2.one;
    }

    private void Start()
    {
        _health.amount = 2000f;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("spike"))
            DoWhatEverOnDamageHere(other.gameObject.GetComponent<DamageObject>(), true);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("spike"))
            DoWhatEverOnDamageHere(other.gameObject.GetComponent<DamageObject>());
    }

    private void DoWhatEverOnDamageHere(DamageObject damageObject, bool stay = false)
    {
        _health.amount-= damageObject.damageAmount * (stay ? 1f : Time.deltaTime);
        _healthText.text = $"{Mathf.RoundToInt(_health.amount)}HP";
        _rb2D.AddForce(Vector2.up * 100);
        StartCoroutine(AnimateOnDamage());
    }

    private IEnumerator AnimateOnDamage()
    {
        _spriteRenderer.color = new Color32(255, 100, 100, 255);
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = new Color32(0, 255, 255, 255);
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
}
public class DamageObject : MonoBehaviour
{
    public float damageAmount;

    private void Awake()
    {
        gameObject.tag = "spike";
        gameObject.AddComponent<PolygonCollider2D>();
    }
}
public class Health : MonoBehaviour
{
    public float amount;
}
