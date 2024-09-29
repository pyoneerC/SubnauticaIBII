using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    public Camera mainCamera;
    public float rayDistance = 2f;
    public int itemCount;
    public int goalCount = 8;
    public AudioSource audioSource;
    public AudioClip itemPickupSound;
    public AudioClip suitUpSound;
    public Image head;
    public Image body;
    public Image oxygenTank;
    public Image oxygenTankCharge;
    public Image legL;
    public Image legR;
    public Image feetR;
    public Image feetL;
    public Image breather;
    public Image knifeUp;
    public Image weightL;
    public Image weightR;
    public Image gogglesRight;
    public Image gogglesLeft;
    public Image reticle;
    public TextMeshProUGUI helptext;
    public TextMeshProUGUI equipmentCount;

    private bool _isCountingDown;
    private bool _isStaringAtItem;
    private Color _reticleOriginalColor;
    private readonly Vector2 _enlargedSize = new(100f, 100f);
    private readonly Vector2 _originalSize = new(50f, 50f);
    private const float ReticleAnimationSpeed = 5f;
    private const float AnimationSpeed = 5f;
    private const float HelpTextDelay = 0.5f;

    private void Start()
    {
        _reticleOriginalColor = reticle.color;
        helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, 0f);
    }

    private void Update()
    {
        CheckForPickupTarget();

        if (_isCountingDown)
        {
            reticle.color = Color.red;
            // Maintain the original size while counting down
            reticle.transform.localScale = Vector3.one; // or set to a specific size if needed, e.g., _originalSize
            // Optional: add a pulsing effect while maintaining size
            float pulseFactor = Mathf.PingPong(Time.time * 2, 0.1f) + 0.9f; // Example pulse effect
            reticle.transform.localScale = new Vector3(pulseFactor, pulseFactor, 1);
        }
        else if (_isStaringAtItem)
        {
            reticle.color = Color.green;
            reticle.transform.localScale = Vector2.Lerp(reticle.transform.localScale, Vector2.one, Time.deltaTime * ReticleAnimationSpeed);
        }
        else
        {
            reticle.color = _reticleOriginalColor;
            reticle.transform.localScale = Vector2.Lerp(reticle.transform.localScale, Vector2.one, Time.deltaTime * ReticleAnimationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.E) && !_isCountingDown)
        {
            StartCoroutine(TryPickUpItem());
        }

        if (_isStaringAtItem && reticle.color == Color.green)
        {
            ShowHelpText();
        }
        else
        {
            FadeOutHelpText(1f);
        }
    }

    private void CheckForPickupTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out var hit, rayDistance) && hit.collider.CompareTag("EPP"))
        {
            _isStaringAtItem = true;
            reticle.color = Color.Lerp(reticle.color, Color.green, Time.deltaTime * ReticleAnimationSpeed);
            reticle.rectTransform.sizeDelta = Vector2.Lerp(reticle.rectTransform.sizeDelta, _enlargedSize, Time.deltaTime * AnimationSpeed);
        }
        else
        {
            _isStaringAtItem = false;
            reticle.color = Color.Lerp(reticle.color, _reticleOriginalColor, Time.deltaTime * ReticleAnimationSpeed);
            reticle.rectTransform.sizeDelta = Vector2.Lerp(reticle.rectTransform.sizeDelta, _originalSize, Time.deltaTime * AnimationSpeed);
        }
    }

    private IEnumerator TryPickUpItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (!Physics.Raycast(ray, out var hit, rayDistance) || !hit.collider.CompareTag("EPP"))
            yield break;

        var halo = hit.collider.GetComponent("Halo");
        if (halo == null)
        {
            _isCountingDown = true;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 4f);
            yield return new WaitForSeconds(4f);
        }

        PickUpItem(hit.collider);
        _isCountingDown = false;
    }

    private void PickUpItem(Collider itemCollider)
    {
        Destroy(itemCollider.gameObject);
        itemCount++;
        equipmentCount.text = itemCount.ToString();
        UpdateEquipmentCountColor();

        foreach (Transform child in itemCollider.transform)
        {
            switch (child.tag)
            {
                case "oxygenTank":
                    StartCoroutine(FillSprite(oxygenTank));
                    PlaySound(itemPickupSound);
                    break;
                case "body":
                    StartCoroutine(FillSprite(body));
                    StartCoroutine(FillSprite(legL));
                    StartCoroutine(FillSprite(legR));
                    PlaySound(suitUpSound);
                    break;
                case "goggles":
                    StartCoroutine(FillSprite(gogglesLeft));
                    StartCoroutine(FillSprite(gogglesRight));
                    PlaySound(itemPickupSound);
                    break;
                case "feet":
                    StartCoroutine(FillSprite(feetL));
                    StartCoroutine(FillSprite(feetR));
                    PlaySound(itemPickupSound);
                    break;
                case "weight":
                    StartCoroutine(FillSprite(weightL));
                    StartCoroutine(FillSprite(weightR));
                    PlaySound(itemPickupSound);
                    break;
                case "oxygenTankCharge":
                    StartCoroutine(FillSprite(oxygenTankCharge));
                    PlaySound(itemPickupSound);
                    break;
                case "breather":
                    StartCoroutine(FillSprite(breather));
                    PlaySound(itemPickupSound);
                    break;
                case "knife":
                    StartCoroutine(FillSprite(knifeUp));
                    PlaySound(itemPickupSound);
                    break;
                default:
                    Debug.Log("No matching tag found.");
                    break;
            }
        }

        if (itemCount >= goalCount)
        {
            SceneManager.LoadScene("Reparation");
        }
    }

    private static IEnumerator FillSprite(Image image)
    {
        float fillDuration = 0.5f;
        float timer = 0f;
        Color originalColor = image.color;

        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            image.color = Color.Lerp(originalColor, Color.green, timer / fillDuration);
            yield return null;
        }

        image.color = Color.green;
    }

    private void UpdateEquipmentCountColor()
    {
        float progress = (float)itemCount / goalCount;

        equipmentCount.color = progress switch
        {
            <= 0.375f => Color.red,
            > 0.375f and <= 0.75f => new Color(1f, 0.65f, 0f),
            > 0.75f => Color.green,
            _ => equipmentCount.color
        };
    }

    private void ShowHelpText()
    {
        StartCoroutine(ShowHelpTextCoroutine());
    }

    private IEnumerator ShowHelpTextCoroutine()
    {
        float timer = 0f;
        float fadeDuration = 0.5f;
        helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, 0f);

        while (_isStaringAtItem && reticle.color == Color.green)
        {
            timer += Time.deltaTime;

            if (timer >= HelpTextDelay)
            {
                float alpha = Mathf.Clamp01((timer - HelpTextDelay) / fadeDuration);
                helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, alpha);
            }

            yield return null;
        }

        StartCoroutine(FadeOutHelpText(fadeDuration));
    }

    private IEnumerator FadeOutHelpText(float duration)
    {
        float elapsedTime = 0f;
        Color currentColor = helptext.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            helptext.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        helptext.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
