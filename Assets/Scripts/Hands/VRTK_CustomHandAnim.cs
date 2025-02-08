using UnityEngine;
using Zinnia.Action;

public class VRTK_CustomHandAnim : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private BooleanAction ThumbTouch/*, TriggerTouch*/;
    [SerializeField]
    private FloatAction GripPress, TriggerPress;

    private int ThumbLayer = 1, TriggerLayer = 2, GripLayer=3;
    private bool ThumbTouched = false/*, TriggerTouched = false*/;
    void Update()
    {
        if (!ThumbTouched && ThumbTouch.Value)
        {
            anim.SetLayerWeight(ThumbLayer, 1);
            ThumbTouched = true;
        }
        if(ThumbTouched && !ThumbTouch.Value)
        {
            anim.SetLayerWeight(ThumbLayer, 0);
            ThumbTouched = false;
        }
        if (GripPress.Value > 0)
        {
            anim.SetLayerWeight(GripLayer, GripPress.Value);
        }
        /*if (!TriggerTouched && TriggerTouch.Value)
        {
            anim.SetLayerWeight(TriggerLayer, 0.5f);
            TriggerTouched = true;
        }
        if (TriggerTouched && !TriggerTouch.Value)
        {
            anim.SetLayerWeight(TriggerLayer, 0);
            TriggerTouched = false;
        }*/
        if (TriggerPress.Value > 0)
        {
            anim.SetLayerWeight(TriggerLayer, TriggerPress.Value);
        }
    }
}
