using UnityEngine;

public class HierrachyManager : MonoBehaviour
{
    public static GameObject getDirectChild(GameObject root, string childname)
    {
        if(root == null)
        {
            return null;
        }
        Transform trans = root.transform;
        if (trans == null)
        {
            return null;
        }
        Transform trans2 = trans.Find(childname);
        if (trans2 == null)
        {
            return null;
        }
        return trans2.gameObject;
    }

    public static GameObject getAParent(GameObject child, string parentname)
    {
        if (child == null)
        {
            return null;
        }
        Transform parent = child.transform.parent;
        if (parent == null)
        {
            return null;
        }
        GameObject obj = parent.gameObject;
        if (obj.name == parentname)
        {
            return obj;
        }
        return getAParent(obj, parentname);
    }

    public static GameObject FindActiveGameObjectWithName(string name)
    {
        return GameObject.Find(name);
    }

    public static GameObject FindActiveGameObjectWithTag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }

    public static GameObject[] FindActiveGameObjectsWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag);
    }
}
