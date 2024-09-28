using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages item pickup and UI updates in the game using raycasting.
/// </summary>
public class Raycast : MonoBehaviour
{
    /// <summary>
    /// The main camera used for raycasting.
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// The maximum distance for item pickup raycasting.
    /// </summary>
    public float rayDistance = 2f;

    /// <summary>
    /// The current count of items collected.
    /// </summary>
    public int itemCount;

    /// <summary>
    /// The goal count of items to collect.
    /// </summary>
    public int goalCount = 8;

    /// <summary>
    /// Audio source for playing sound effects.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Sound effect played when an item is picked up.
    /// </summary>
    public AudioClip itemPickupSound;

    /// <summary>
    /// Sound effect played when the suit is equipped.
    /// </summary>
    public AudioClip suitUpSound;

    /// <summary>
    /// UI elements representing equipment.
    /// </summary>
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

    /// <summary>
    /// Text element showing the current count of collected items.
    /// </summary>
    public TextMeshProUGUI equipmentCount;

    private bool _isCountingDown;
    private bool _isStaringAtItem;

    private Color _reticleOriginalColor;

    /// <summary>
    /// Target size for the reticle when looking at an item.
    /// </summary>
    private readonly Vector2 _enlargedSize = new(100f, 100f);

    /// <summary>
    /// Original size of the reticle.
    /// </summary>
    private readonly Vector2 _originalSize = new(50f, 50f);

    private const float ReticleAnimationSpeed = 5f;
    private const float AnimationSpeed = 5f;
    private const float HelpTextDelay = 0.5f; // Time to stare at the item before showing help text

    private void Start()
    {
        // Store the original color of the reticle
        _reticleOriginalColor = reticle.color;
        helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, 0f); // Initially hidden
    }

    private void Update()
    {
        CheckForPickupTarget();

        // Check for interaction input to pick up items
        if (Input.GetKeyDown(KeyCode.E) && !_isCountingDown)
        {
            StartCoroutine(TryPickUpItem());
        }

        // Handle fading of help text based on reticle color
        if (_isStaringAtItem && reticle.color == Color.green)
        {
            ShowHelpText();
        }
        else
        {
            FadeOutHelpText(1f);
        }
    }

    /// <summary>
    /// Checks if the player is aiming at an item that can be picked up and updates the reticle color accordingly.
    /// </summary>
    private void CheckForPickupTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Cast a ray and check if it hits an object tagged as "EPP"
        if (Physics.Raycast(ray, out var hit, rayDistance) && hit.collider.CompareTag("EPP"))
        {
            // Animate the reticle to grow in size and change color to green
            _isStaringAtItem = true; // Set staring flag
            reticle.color = Color.Lerp(reticle.color, Color.green, Time.deltaTime * ReticleAnimationSpeed);
            reticle.rectTransform.sizeDelta = Vector2.Lerp(reticle.rectTransform.sizeDelta, _enlargedSize, Time.deltaTime * AnimationSpeed);
        }
        else
        {
            // Animate the reticle to shrink back to its original size and reset the color
            _isStaringAtItem = false; // Reset staring flag
            reticle.color = Color.Lerp(reticle.color, _reticleOriginalColor, Time.deltaTime * ReticleAnimationSpeed);
            reticle.rectTransform.sizeDelta = Vector2.Lerp(reticle.rectTransform.sizeDelta, _originalSize, Time.deltaTime * AnimationSpeed);
        }
    }

    /// <summary>
    /// Attempts to pick up an item if it is in range.
    /// </summary>
    private IEnumerator TryPickUpItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Check if the ray hits an object
        if (!Physics.Raycast(ray, out var hit, rayDistance) || !hit.collider.CompareTag("EPP"))
            yield break;

        // Optional halo check can be implemented as needed
        var halo = hit.collider.GetComponent("Halo");
        if (halo == null)
        {
            _isCountingDown = true;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 4f);
            Debug.Log("Countdown started! Picking up item in 4 seconds...");

            yield return new WaitForSeconds(4f);
        }

        PickUpItem(hit.collider);
        _isCountingDown = false;
    }

    /// <summary>
    /// Handles the logic for picking up an item and updating the equipment UI.
    /// </summary>
    /// <param name="itemCollider">The collider of the item to pick up.</param>
    private void PickUpItem(Collider itemCollider)
    {
        Destroy(itemCollider.gameObject);

        itemCount++;
        equipmentCount.text = itemCount.ToString();
        UpdateEquipmentCountColor();  // Update color based on item count

        // Update the equipment UI based on the item picked up
        foreach (Transform child in itemCollider.transform)
        {
            switch (child.tag)
            {
                case "oxygenTank":
                    oxygenTank.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "body":
                    body.color = Color.green;
                    legL.color = Color.green;
                    legR.color = Color.green;
                    PlaySound(suitUpSound);
                    break;
                case "goggles":
                    gogglesLeft.color = Color.green;
                    gogglesRight.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "feet":
                    feetL.color = Color.green;
                    feetR.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "weight":
                    weightL.color = Color.green;
                    weightR.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "oxygenTankCharge":
                    oxygenTankCharge.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "breather":
                    breather.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "knife":
                    knifeUp.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                default:
                    Debug.Log("No matching tag found.");
                    break;
            }
        }

        // Check if the goal has been reached to load the next scene
        if (itemCount >= goalCount)
        {
            SceneManager.LoadScene("Reparation");
        }
    }

    /// <summary>
    /// Updates the color of the equipment count text based on the current item count.
    /// </summary>
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

    /// <summary>
    /// Shows the help text if the player has been staring at an item for longer than the specified delay.
    /// </summary>
    private void ShowHelpText()
    {
        StartCoroutine(ShowHelpTextCoroutine());
    }

    private IEnumerator ShowHelpTextCoroutine()
    {
        float timer = 0f;
        float fadeDuration = 0.5f; // Duration for the fade in/out effect

        // Start with the help text invisible
        helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, 0f);

        while (_isStaringAtItem && reticle.color == Color.green)
        {
            timer += Time.deltaTime;

            if (timer >= HelpTextDelay)
            {
                // Fade in the help text
                float alpha = Mathf.Clamp01((timer - HelpTextDelay) / fadeDuration);
                helptext.color = new Color(helptext.color.r, helptext.color.g, helptext.color.b, alpha);
            }

            yield return null;
        }

        // Fade out if not staring at the item
        StartCoroutine(FadeOutHelpText(fadeDuration));
    }

    /// <summary>
    /// Fades out the help text if the player is not staring at the item.
    /// </summary>
    /// <param name="duration">Duration for the fade out effect.</param>
    private IEnumerator FadeOutHelpText(float duration)
    {
        float elapsedTime = 0f;
        Color currentColor = helptext.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration)); // Fade out
            helptext.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        // Ensure the help text is completely transparent at the end
        helptext.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
    }


    /// <summary>
    /// Plays the specified sound effect.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
