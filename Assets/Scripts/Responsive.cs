using UnityEngine;

public class Responsive : MonoBehaviour
{
	public enum ScaleType
	{
		PERCENTAGE,
		ASPECT_RATIO,
	}

	public enum ResponsiveMode
	{
		AWAKE,
		UPDATE,
	}

	public ScaleType scaleType;
	public ResponsiveMode responsiveMode;
	public float xposfrac = 0.5f;
	public float yposfrac = 0.5f;
	public float xscalefrac = 0.1f;
	public float yscalefrac = 0.1f;
	public float aspectfrac = 1f;
	public bool doScale = true;
	public bool doTranslate = true;
	public bool isEnabled = true;
	public Camera cam;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (isEnabled && responsiveMode == ResponsiveMode.AWAKE)
        {
            SetPositionAndScale();
        }
    }

    private void Update()
    {
        if (isEnabled && responsiveMode == ResponsiveMode.UPDATE)
        {
            SetPositionAndScale();
        }
    }

    void SetPositionAndScale()
    {
        if (doTranslate)
        {
            PercentagePositioning();
        }
        if (doScale)
        {
            if (scaleType == ScaleType.ASPECT_RATIO)
            {
                AspectRatioScaling();
            }
            else if (scaleType == ScaleType.PERCENTAGE)
            {
                PercentageScaling();
            }
        }
    }

    void PercentagePositioning()//masih ngaco 
    {
        int width = Screen.width;
        int height = Screen.height;
        Vector3 vector = cam.ScreenToWorldPoint(new Vector2(width * xposfrac, height * yposfrac));
        transform.position = new Vector3(vector.x, vector.y, transform.position.z);
    }

    void PercentageScaling()
    {
        int width = Screen.width;
        int height = Screen.height;
        Vector3 vector = cam.ScreenToWorldPoint(new Vector2(width, height));
        transform.localScale = new Vector3(vector.x * xscalefrac, vector.y * yscalefrac, transform.localScale.z);
    }

    void AspectRatioScaling()
    {
        float aspect = cam.aspect;
        transform.localScale = new Vector3(aspect * aspectfrac, aspect * aspectfrac, aspect * aspectfrac);
    }
}
