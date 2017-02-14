using UnityEngine;

[System.Serializable]
public class RecognizedObject
{
    public string name;
    public string score;

    public static RecognizedObject CreateFromJSON(string str)
    {
        return JsonUtility.FromJson<RecognizedObject>(str);
    }
}