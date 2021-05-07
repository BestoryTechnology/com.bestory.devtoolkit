using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class UnityThreadSample : MonoBehaviour
{
    [SerializeField]
    string path = "hecomi.png";

    void Awake()
    {
        //Enable Callback on the main Thread
        UnityThread.initUnityThread();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReadImageAsync();
        }
    }

    void ReadImageAsync()
    {
        string filePath =  System.IO.Path.Combine(Application.streamingAssetsPath, path);


        //Use ThreadPool to avoid freezing
        ThreadPool.QueueUserWorkItem(delegate
        {
            bool success = false;

            byte[] imageBytes;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

            try
            {
                int length = (int)fileStream.Length;
                imageBytes = new byte[length];
                int count;
                int sum = 0;

                // read until Read method returns 0
                while ((count = fileStream.Read(imageBytes, sum, length - sum)) > 0)
                    sum += count;

                success = true;
            }
            finally
            {
                fileStream.Close();
            }

            //Create Texture2D from the imageBytes in the main Thread if file was read successfully
            if (success)
            {
                UnityThread.executeInUpdate(() =>
                {
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    tex.LoadImage(imageBytes);

                    var material = GetComponent<Renderer>().material;
                    material.mainTexture = tex;
                });
            }
        });
    }
}
