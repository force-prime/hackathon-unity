using StacksForce;
using UnityEngine;

public class NFTSprite : MonoBehaviour
{
    Rigidbody2D body;
    NftSpriteProvider image;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.simulated = false;
        image = GetComponent<NftSpriteProvider>();
    }

    void Update()
    {
        if (image.Sprite != null)
        {
            if (!body.simulated)
            {
                body.simulated = true;
                body.AddTorque(1.5f);
            }
        }
    }
}
