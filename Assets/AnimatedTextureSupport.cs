using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTextureSupport : MonoBehaviour
{
    public Material mat;
    public string baseMapReference;

    public Texture2D[] textureArray;
    int currentTextureIndex;

    [Range(0f, .1f)]
    public float deltatime;

    float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime > deltatime)
        {
            mat.SetTexture(baseMapReference, textureArray[currentTextureIndex]);
            elapsedTime = 0;
            currentTextureIndex++;

            if (currentTextureIndex >= textureArray.Length)
                currentTextureIndex = 0;
        }
    }
}
