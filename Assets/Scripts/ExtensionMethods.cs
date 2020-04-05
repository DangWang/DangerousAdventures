using UnityEngine;

public static class ExtensionMethods
{
    public static Transform ReturnRoot(this Transform trans)
    {
        if (trans.parent != null)
            return trans.parent.ReturnRoot();
        return trans;
    }
}