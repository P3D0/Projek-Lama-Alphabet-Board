using UnityEngine;

public class Button : MonoBehaviour
{
	public enum Type
	{
		NORMAL,
		TOGGLE,
	}

	public enum ToggleStatus
	{
		ON,
		OFF,
	}

    [HideInInspector]
	public bool isBegan;
	public Type type;
	public ToggleStatus toggleStatus;
	public Sprite normalIcon;
	public Sprite hoverIcon;
	public AudioClip clickReleaseSFx;
	public string message;
	public bool resetIconOnRelease;
	public SpriteRenderer spriteRendererComp;
	public UnityEngine.Object messageObject;

    private void Awake()
    {
        tag = "UIButton";
    }

    private void Start()
    {
        if (messageObject == null)
        {
            messageObject = gameObject;
        }
        spriteRendererComp = GetComponent<SpriteRenderer>();
    }
}
