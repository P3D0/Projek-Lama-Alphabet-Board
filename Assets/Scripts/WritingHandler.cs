using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StartApp;

public class WritingHandler : MonoBehaviour
{
	public GameObject[] letters;
	public Sprite[] lSprites;
	public Animator winDialog;
	public SpriteRenderer winDialogLetterSprite;
	public GameObject menu;
    public static int currentLetterIndex;
    bool clickBeganOrMovedOutOfLetterArea;
    int previousTracingPointIndex;
    ArrayList currentTracingPoints;
    Vector3 previousPosition;
    Vector3 currentPosition = Vector3.zero;
    public GameObject lineRendererPrefab;
	public GameObject circlePointPrefab;
    GameObject currentLineRender;
    public Material drawingMaterial;
    private bool letterDone;
    private bool setRandomColor = true;
    private bool clickStarted;
    public Transform hand;
	public static bool showCursor;
	public AudioClip cheeringSound;
	public AudioClip positiveSound;
	public AudioClip wrongSound;
    int completeCounter;

    IEnumerator Start()
    {
        completeCounter = 0;
        //StartAppWrapper.removeBanner(StartAppWrapper.BannerPosition.BOTTOM);
        //StartAppWrapper.loadAd();
       
        //Screen.showCursor = showCursor;// gantinya jdai Cusor.visible 
        Cursor.visible = showCursor;
        currentTracingPoints = new ArrayList();
        
        LoadLetter();
        yield return 0;
        yield break;
    }

    private void Update()
    {
        if (letterDone)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
        RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //Debug.Log(raycastHit2D.point);
        if (raycastHit2D.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchLetterHandle(raycastHit2D.collider.gameObject, true, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                clickStarted = true;
            }
            else if (clickStarted)
            {

                TouchLetterHandle(raycastHit2D.collider.gameObject, false, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        if (Input.GetMouseButtonUp(0) && clickStarted)
        {
            EndTouchLetterHandle();
            clickStarted = false;
            clickBeganOrMovedOutOfLetterArea = false;
        }
        if (hand != null)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = -6f;
            hand.position = position;
        }
    }

    void TouchLetterHandle(GameObject ob, bool isTouchBegain, Vector3 touchPos)
    {
        string tag = ob.tag;
        bool flag = (tag == "Letter" || tag == "TracingPoint" || tag == "Background") && currentLineRender != null;
        bool flag2 = (tag == "TracingPoint");
        if (flag && !isTouchBegain)
        {
            
            if (tag == "TracingPoint")
            {
                TracingPoint component = ob.GetComponent<TracingPoint>();
                int index = component.index;
                if (component.single_touch)
                {
                    return;
                }
                if (index != previousTracingPointIndex)
                {
                    currentTracingPoints.Add(index);
                    previousTracingPointIndex = index;
                }
            }
            else if (tag == "Background")
            {
                clickBeganOrMovedOutOfLetterArea = true;
                EndTouchLetterHandle();
                clickStarted = false;
                return;
            }
            currentPosition = touchPos;
            currentPosition.z = -5f;
            float num = Mathf.Abs(Vector3.Distance(currentPosition, new Vector3(previousPosition.x, previousPosition.y, currentPosition.z)));
            if (num <= 0.1f)
            {
                return;
            }
            previousPosition = currentPosition;
            InstaitaeCirclePoint(currentPosition, currentLineRender.transform);
            LineRenderer component2 = currentLineRender.GetComponent<LineRenderer>();
            LineRendererAttributes component3 = currentLineRender.GetComponent<LineRendererAttributes>();
            int num2 = component3.NumberOfPoints;
            num2++;
            component3.Points.Add(currentPosition);
            component3.NumberOfPoints = num2;
            component2.SetVertexCount(num2);
            component2.SetPosition(num2 - 1, currentPosition);
        }
        else if (flag2 && isTouchBegain)
        {
            TracingPoint component4 = ob.GetComponent<TracingPoint>();
            int index2 = component4.index;
            if (index2 != previousTracingPointIndex)
            {
                currentTracingPoints.Add(index2);
                previousTracingPointIndex = index2;
                if (currentLineRender == null)
                {
                    currentLineRender = Instantiate(lineRendererPrefab);
                    Debug.Log(currentLineRender);

                    if (setRandomColor)
                    {
                        currentLineRender.GetComponent<LineRendererAttributes>().SetRandomColor();
                        setRandomColor = false;
                    }
                }
                Vector3 vector = touchPos;
                vector.z = -5f;
                previousPosition = vector;
                if (component4.single_touch)
                {
                    InstaitaeCirclePoint(vector, currentLineRender.transform);
                }
                else
                {
                    InstaitaeCirclePoint(vector, currentLineRender.transform);
                    LineRenderer component5 = currentLineRender.GetComponent<LineRenderer>();
                    LineRendererAttributes component6 = currentLineRender.GetComponent<LineRendererAttributes>();
                    int num3 = component6.NumberOfPoints;
                    num3++;
                    if (component6.Points == null)
                    {
                        component6.Points = new List<Vector3>();
                    }
                    component6.Points.Add(vector);
                    component6.NumberOfPoints = num3;
                    component5.SetVertexCount(num3);
                    component5.SetPosition(num3 - 1, vector);
                }
            }
        }
    }

    void EndTouchLetterHandle()
    {
        if (currentLineRender == null || currentTracingPoints.Count == 0)
        {
            return;
        }
        TracingPart[] components = letters[currentLetterIndex].GetComponents<TracingPart>();
        bool flag = false;
        if (!clickBeganOrMovedOutOfLetterArea)
        {
            foreach (TracingPart tracingPart in components)
            {
                if (currentTracingPoints.Count == tracingPart.order.Length && !tracingPart.succeded && PreviousLettersPartsSucceeded(tracingPart, components))
                {
                    flag = true;
                    for (int j = 0; j < currentTracingPoints.Count; j++)
                    {
                        int num = (int)currentTracingPoints[j];
                        if (num != tracingPart.order[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    tracingPart.succeded = true;
                    break;
                }
            }
        }
        if (flag)
        {
            if (currentTracingPoints.Count != 1)
            {
                StartCoroutine("SmoothCurrentLine");
            }
            else
            {
                currentLineRender = null;
            }
            PlayPositiveSound();
        }
        else
        {
            PlayWrongSound();
            Destroy(currentLineRender);
            currentLineRender = null;
        }
        previousPosition = Vector2.zero;
        currentTracingPoints.Clear();
        previousTracingPointIndex = 0;
        CheckLetterDone();
        if (letterDone)
        {
            if (cheeringSound != null)
            {
                AudioSource.PlayClipAtPoint(cheeringSound, Vector3.zero, 0.8f);
            }
            hand.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void CheckLetterDone()
    {
        bool flag = true;
        TracingPart[] components = letters[currentLetterIndex].GetComponents<TracingPart>();
        foreach (TracingPart tracingPart in components)
        {
            if (!tracingPart.succeded)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            letterDone = true;
            completeCounter++;
            if (completeCounter == 2)
            {
                completeCounter = 0;
                StartAppWrapper.showAd();
                StartAppWrapper.loadAd();
            }
            menu.SetActive(false);
            GameObject[] array2 = GameObject.FindGameObjectsWithTag("LineRenderer");
            foreach (GameObject gameObject1 in array2)
            {
                gameObject1.GetComponent<LineRenderer>().enabled = false;
            }
            GameObject[] array4 = GameObject.FindGameObjectsWithTag("CirclePoint");
            foreach (GameObject gameObject2 in array4)
            {
                gameObject2.GetComponent<MeshRenderer>().enabled = false;
            }
            letters[currentLetterIndex].SetActive(false);
            winDialogLetterSprite.sprite = lSprites[currentLetterIndex];
            winDialog.SetBool("isFadingIn", true);
        }
    }

    void BackToMenu()
    {
        Application.LoadLevel("AlphabetMenu");
    }

    public void RefreshProcess()
    {
        RefreshLines();
        TracingPart[] components = letters[currentLetterIndex].GetComponents<TracingPart>();
        foreach (TracingPart tracingPart in components)
        {
            tracingPart.succeded = false;
        }
        if (hand != null)
        {
            hand.GetComponent<SpriteRenderer>().enabled = true;
        }
        letterDone = false;
    }

    void RefreshLines()
    {
        StopCoroutine("SmoothCurrentLine");
        GameObject[] array = HierrachyManager.FindActiveGameObjectsWithTag("LineRenderer");
        if (array == null)
        {
            return;
        }
        foreach (GameObject obj in array)
        {
            Destroy(obj);
        }
    }

    IEnumerator SmoothCurrentLine()
    {
        LineRendererAttributes line_attributes = currentLineRender.GetComponent<LineRendererAttributes>();
        LineRenderer ln = currentLineRender.GetComponent<LineRenderer>();
        Vector3[] vectors = SmoothCurve.MakeSmoothCurve(line_attributes.Points.ToArray(), 10f);
        int childscount = currentLineRender.transform.childCount;
        for (int i = 0; i < childscount; i++)
        {
            Destroy(currentLineRender.transform.GetChild(i).gameObject);
        }
        line_attributes.Points.Clear();
        for (int j = 0; j < vectors.Length; j++)
        {
            if (j == 0 || j == vectors.Length - 1)
            {
                InstaitaeCirclePoint(vectors[j], currentLineRender.transform);
            }
            line_attributes.NumberOfPoints = j + 1;
            line_attributes.Points.Add(vectors[j]);
            ln.SetVertexCount(j + 1);
            ln.SetPosition(j, vectors[j]);
        }
        currentLineRender = null;
        yield return new WaitForSeconds(0f);
        yield break;
    }

    public static bool PreviousLettersPartsSucceeded(TracingPart currentpart, TracingPart[] lparts)
    {
        int priority = currentpart.priority;
        if (priority == 1)
        {
            return true;
        }
        bool result = true;
        foreach (TracingPart tracingPart in lparts)
        {
            if (tracingPart.priority < priority && !tracingPart.succeded && tracingPart.order.Length != 1)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    void PlayPositiveSound()
    {
        if (positiveSound != null)
        {
            AudioSource.PlayClipAtPoint(positiveSound, Vector3.zero, 0.8f);
        }
    }

    void PlayWrongSound()
    {
        if (wrongSound != null)
        {
            AudioSource.PlayClipAtPoint(wrongSound, Vector3.zero, 0.8f);
        }
    }


    public void LoadNextLetter()
    {
        if (currentLetterIndex == letters.Length - 1)
        {
            currentLetterIndex = 0;
            Application.LoadLevel("AlphabetMenu");
        }
        else if (currentLetterIndex >= 0 && currentLetterIndex < letters.Length - 1)
        {
            letters[currentLetterIndex].SetActive(true);
            menu.SetActive(true);
            winDialog.SetBool("isFadingIn", false);
            currentLetterIndex++;
            LoadLetter();
        }
    }

    public void LoadPreviousLetter()
    {
        if (currentLetterIndex > 0 && currentLetterIndex < letters.Length)
        {
            letters[currentLetterIndex].SetActive(true);
            menu.SetActive(true);
            winDialog.SetBool("isFadingIn", false);
            currentLetterIndex--;
            LoadLetter();
        }
    }

    void LoadLetter()
    {
        if (letters == null)
        {
            return;
        }
        if (currentLetterIndex < 0 || currentLetterIndex >= letters.Length)
        {
            return;
        }
        if (letters[currentLetterIndex] == null)
        {
            return;
        }
        letterDone = false;
        RefreshProcess();
        HideLetters();
        letters[currentLetterIndex].SetActive(true);
        setRandomColor = true;
    }

    void HideLetters()
    {
        if (letters == null)
        {
            return;
        }
        foreach (GameObject obj in letters)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void InstaitaeCirclePoint(Vector3 position, Transform parent)
    {
        GameObject obj = Instantiate(circlePointPrefab);
        obj.transform.parent = parent;
        obj.GetComponent<Renderer>().material = currentLineRender.GetComponent<LineRendererAttributes>().material;
        obj.transform.position = position;
    }
}
