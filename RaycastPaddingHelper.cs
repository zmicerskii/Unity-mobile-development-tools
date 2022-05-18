using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(Image), typeof(RectTransform))]
public class RaycastPaddingHelper: UIBehaviour
{
#if UNITY_EDITOR
    
    private static Vector2 kShadowOffset = new Vector2(1, -1);
    private static Color kShadowColor = new Color(0, 0, 0, 0.5f);
    private const float kDottedLineSize = 5f;

    private void OnDrawGizmos()
    {
        var image = GetComponent<Image>();
        var gui = GetComponent<RectTransform>();
        
        var rectInOwnSpace = gui.rect;
        
        var rectInParentSpace = rectInOwnSpace;
        var ownSpace = gui.transform;
        
        var parentSpace = ownSpace;
        RectTransform guiParent = null;
        
        if (ownSpace.parent != null)
        {
             parentSpace = ownSpace.parent;
             rectInParentSpace.x += ownSpace.localPosition.x;
             rectInParentSpace.y += ownSpace.localPosition.y;
 
             parentSpace.GetComponent<RectTransform>();
        }
        
        var paddingRect = new Rect(rectInParentSpace);
        paddingRect.xMin += image.raycastPadding.x;
        paddingRect.xMax -= image.raycastPadding.z;
        paddingRect.yMin += image.raycastPadding.y;
        paddingRect.yMax -= image.raycastPadding.w;
        
        //change the color of the handles as you wish
        Handles.color = Color.green;
        DrawRect(paddingRect, parentSpace, true);
        
    }
    
    private static void DrawRect(Rect rect, Transform space, bool dotted)
    {
         var point0 = space.TransformPoint(new Vector2(rect.x, rect.y));
         var point1 = space.TransformPoint(new Vector2(rect.x, rect.yMax));
         var point2 = space.TransformPoint(new Vector2(rect.xMax, rect.yMax));
         var point3 = space.TransformPoint(new Vector2(rect.xMax, rect.y));
        
         if (!dotted)
         {
             Handles.DrawLine(point0, point1);
             Handles.DrawLine(point1, point2);
             Handles.DrawLine(point2, point3);
             Handles.DrawLine(point3, point0);
         }
         else
         {
             DrawDottedLineWithShadow(kShadowColor, kShadowOffset, point0, point1, kDottedLineSize);
             DrawDottedLineWithShadow(kShadowColor, kShadowOffset, point1, point2, kDottedLineSize);
             DrawDottedLineWithShadow(kShadowColor, kShadowOffset, point2, point3, kDottedLineSize);
             DrawDottedLineWithShadow(kShadowColor, kShadowOffset, point3, point0, kDottedLineSize);
         }
    }
    
    private static void DrawDottedLineWithShadow(Color shadowColor, Vector2 screenOffset, 
        Vector3 point1, Vector3 point2, float screenSpaceSize)
    {
        var cam = Camera.current;

        if (!cam || Event.current.type != EventType.Repaint)
        {
            return;
        }
 
        var oldColor = Handles.color;
 
        // shadow
        shadowColor.a = shadowColor.a * oldColor.a;
        Handles.color = shadowColor;
        Handles.DrawDottedLine(
        cam.ScreenToWorldPoint(cam.WorldToScreenPoint(point1) + (Vector3)screenOffset),
        cam.ScreenToWorldPoint(cam.WorldToScreenPoint(point2) + (Vector3)screenOffset), screenSpaceSize);
 
        // line itself
        Handles.color = oldColor;
        Handles.DrawDottedLine(point1, point2, screenSpaceSize);
    }
#endif
}

