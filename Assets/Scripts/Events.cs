using UnityEngine;

public class Events : MonoBehaviour
{
    WritingHandler writingHandler;
    public Animator winDialog;
	public GameObject menu;

    private void Start()
    {
        GameObject obj = HierrachyManager.FindActiveGameObjectWithName("Letters");
        if (obj != null)
        {
            writingHandler = obj.GetComponent<WritingHandler>();
        }
    }

    public void LoadTheNextLetter(object ob)
    {
        Debug.Log("load next letter");
        if (writingHandler == null)
        {
            return;
        }
        writingHandler.LoadNextLetter();
    }

    public void LoadThePreviousLetter(object ob)
    {
        if (writingHandler == null)
        {
            return;
        }
        writingHandler.LoadPreviousLetter();
    }

    public void LoadLetter(UnityEngine.Object ob)
    {
        Debug.Log(ob + "yang di load ada di event");
        if (ob == null)
        {
            return;
        }
        WritingHandler.currentLetterIndex = int.Parse(ob.name.Split(new char[]
         {
            '-'
         })[1]);
        Application.LoadLevel("AlphabetWriting");
    }

    public void EraseLetter(UnityEngine.Object ob)
    {
        if (writingHandler == null)
        {
            return;
        }
        writingHandler.RefreshProcess();
    }

    public void CloseWinDialog(UnityEngine.Object ob)
    {
        writingHandler.letters[WritingHandler.currentLetterIndex].SetActive(true);
        menu.SetActive(true);
        GameObject[] array = GameObject.FindGameObjectsWithTag("LineRenderer");
        foreach (GameObject gameObject in array)
        {
            gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        GameObject[] array3 = GameObject.FindGameObjectsWithTag("CirclePoint");
        foreach (GameObject gameObject2 in array3)
        {
            gameObject2.GetComponent<MeshRenderer>().enabled = true;
        }
        winDialog.SetBool("isFadingIn", false);
    }

    public void LoadAlphabetMenu(UnityEngine.Object ob)
    {
        Application.LoadLevel("AlphabetMenu");
    }
}
