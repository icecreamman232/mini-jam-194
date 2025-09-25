using UnityEngine;
using UnityEngine.Events;

public class AnimatorEventBridge : MonoBehaviour
{
    public UnityEvent[] MethodToCall;

    public void BridgeMethodForCall(int index)
    {
        MethodToCall[index].Invoke();
    }
}
