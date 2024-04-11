using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public List<Transform> pages;
    public Transform curPage;
    private Vector3 curPagePosition;
    private Stack<Transform> screenHistory = new Stack<Transform>();
    public bool isPageOpen = false;
    public UnityEngine.Events.UnityEvent onScreenOpen;
    public UnityEngine.Events.UnityEvent onCloseAll;
    private CanvasGroup canvasGroup;
    private void Start()
    {
        pages.RemoveAll(x => x == null);
        curPage = pages.Find(x => x.gameObject.activeSelf);
        curPagePosition = transform.position;
    }

    [ContextMenu("PopulateFromChildren()")]
    public void PopulateFromChildren()
    {

        pages = new List<Transform>();
        foreach (Transform t in transform)
        {
            if (t != transform)
            {
                pages.Add(t);
            }
        }
    }
    public void ToggleHide(bool isOn)
    {
        if (!canvasGroup)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if (isOn)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }

    }
    public void CloseAll()
    {
        // print("CloseAll");
        foreach (Transform page in pages)
        {
            if (page != null)
                page.gameObject.SetActive(false);
        }
        isPageOpen = false;
        onCloseAll.Invoke();
    }
    public void OpenPrevious()
    {
        if (screenHistory.Count > 0)
        {
            Transform previousScreen = screenHistory.Pop();
            print("OpenPrevious() : " + previousScreen.name);
            EnableSingleScreen(previousScreen.name);
        }
    }
    [ContextMenu("Open Next")]
    public void OpenNext()
    {
        int curIndex = pages.IndexOf(curPage);
        if (curIndex + 1 < pages.Count)
        {
            OpenPage(pages[curIndex + 1]);
        }
    }
    public void OpenPage(Transform page) { OpenPage(page.name); }
    public void OpenPage(string pageName)
    {
        if (curPage)
            screenHistory.Push(curPage);
        EnableSingleScreen(pageName);
        onScreenOpen.Invoke();
        // ToggleHide(false);
    }
    public void EnableSingleScreen(string pageName)
    {
        foreach (Transform page in pages)
        {
            if (page.name == pageName)
            {
                page.gameObject.SetActive(true);
                curPage = page;
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
        isPageOpen = true;
    }
    public void OpenPageAdditive(string pageName)
    {
        pages.Find(page => page.name == pageName).gameObject.SetActive(true);
    }
    public bool Contains(Transform t) { return pages.Contains(t); }

}
