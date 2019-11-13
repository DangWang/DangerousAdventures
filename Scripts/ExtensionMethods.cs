using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Transform ReturnRoot(this Transform trans)
    {
        if(trans.parent != null)
            return trans.parent.ReturnRoot();
        else
            return trans;
    }
}