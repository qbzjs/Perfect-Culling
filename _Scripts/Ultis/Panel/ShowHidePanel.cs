using UnityEngine;
using System.Collections;
public abstract class ShowHidePanel : MonoBehaviour
{
	public delegate void onEventHandler();
    public delegate IEnumerator onCoroutineEventHandler();

	public onEventHandler onShowBegin = null;
	public onEventHandler onHideBegin = null;
	public onEventHandler onShowFinished = null;
	public onEventHandler onHideFinished = null;

    public onCoroutineEventHandler onCoroutineShowBegin = null;
    public onCoroutineEventHandler onCoroutineShowFinished = null;

	public abstract void Show();
	public abstract void Hide();
}
