using System;
using UnityObject = UnityEngine.Object;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static Boolean IsNotNull(this Object systemObject)
        {
            return (systemObject as UnityObject) ?? (systemObject != null);
        }

        public static Boolean IsNull(this Object systemObject)
        {
            return (systemObject is UnityObject unityObject) ? (!unityObject) : (systemObject == null);
        }
    }
}
