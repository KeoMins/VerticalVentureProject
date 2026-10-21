using UnityEngine;

// Keeps the floor art at its normal pixel size however the Floor is scaled: a grass strip on top, dirt below it
[ExecuteAlways]
public class FloorTiler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer top;  // Grass tile, repeated along the top edge
    [SerializeField] private SpriteRenderer body; // Dirt tile, repeated to fill the rest
    [SerializeField] private float tileSize = 0.16f; // World size of one 16px tile at 100 pixels per unit

    void OnEnable() { Refresh(); }
    void OnValidate() { Refresh(); }

    void Update()
    {
        if (!Application.isPlaying) Refresh(); // Follow scale changes made in the editor
    }

    private void Refresh()
    {
        if (top == null || body == null) return;

        Vector3 scale = transform.lossyScale;
        float width = Mathf.Abs(scale.x);
        float height = Mathf.Abs(scale.y);
        if (width < 0.0001f || height < 0.0001f) return;

        float topHeight = Mathf.Min(tileSize, height);
        float bodyHeight = height - topHeight;

        // Children counter-scale so tiles stay square in world space; sprite sizes are in world units
        Vector3 inverse = new Vector3(1f / scale.x, 1f / scale.y, 1f);

        Place(top, width, topHeight, height / 2f - topHeight / 2f, inverse);
        body.enabled = bodyHeight > 0f;
        Place(body, width, bodyHeight, -topHeight / 2f, inverse);
    }

    // centerY is the strip's centre in world units relative to the floor's centre
    private void Place(SpriteRenderer sr, float width, float height, float centerY, Vector3 inverse)
    {
        sr.size = new Vector2(width, height);
        sr.transform.localScale = inverse;
        sr.transform.localPosition = new Vector3(0f, centerY * inverse.y, 0f);
    }
}
