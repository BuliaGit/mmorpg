using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoSingleton<SceneManager>
{
    public UnityAction<float> onProgress = null;
    public UnityAction onSceneLoadDone = null;

    public void LoadScene(string name)
    {
        StartCoroutine(LoadLevel(name));
        Show();
    }

    private AsyncOperation async;

    IEnumerator LoadLevel(string name)
    {
        Debug.LogFormat("LoadLevel: {0}", name);
        yield return new WaitForSeconds(0.2f);

        int m_CurrProgress = 0;
        async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (m_CurrProgress <= 100)
            {
                m_CurrProgress += 10;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                async.allowSceneActivation = true;
            }

            if (onProgress != null)
                onProgress(m_CurrProgress);

            yield return null;
        }
    }

    private GameObject objUILoading = null;

    public void Show()
    {
        if (objUILoading == null)
        {
            GameObject obj = Resloader.Load<GameObject>("UI/UILoading");
            objUILoading = GameObject.Instantiate(obj);
        }
        //if (isIn)
        //    UILoading.animator.Play("LoadAnimFadeIn");
        //else

        //    UILoading.animator.Play("LoadAnimFadeOut");
    }
}

