using UnityEngine;

public class FindScriptsInScene : MonoBehaviour
{
    private void Start()
    {
        FindScripts();
    }

    public static void FindScripts()
    {
        MonoBehaviour[] scripts = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            var scriptType = script.GetType();
            var scope = scriptType.Namespace;
            if(scope == null || !scope.StartsWith("Unity"))
                Debug.Log(script.GetType().FullName + " is used within " + script.gameObject.name);
        }
    }

    private void GameManager_OnshowScripts()
    {
        FindScripts();
    }
}

