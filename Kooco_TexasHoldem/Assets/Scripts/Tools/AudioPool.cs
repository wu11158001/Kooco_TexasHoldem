using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioPool
{
    Transform thisTransform;        //音效池父物件
        
    Dictionary<GameObject, AudioSource> audioDic;               //音效池
    int cleanNum;                                               //清理音效池數量

    public AudioPool(Transform parent, int cleanNum = 10)
    {
        this.cleanNum = cleanNum;
        GameObject obj = new GameObject();
        obj.name = "AudioPool";
        thisTransform = GameObject.Instantiate(obj, parent).GetComponent<Transform>();

        audioDic = new Dictionary<GameObject, AudioSource>();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySound(string soundName)
    {
        AudioClip sound = AudioManager.Instance.GetSound(soundName);
        if (sound == null)
        {
            return;
        }

        foreach (var audio in audioDic)
        {
            if (!audio.Key.activeSelf)
            {
                audio.Key.SetActive(true);
                audio.Key.name = soundName;
                audio.Value.clip = sound;
                audio.Value.Play();

                //清理音效物件池
                if (audioDic.Count >= cleanNum)
                {
                    List<GameObject> cleanList = new List<GameObject>();
                    foreach (var usingObj in audioDic)
                    {
                        if (!usingObj.Key.activeSelf)
                        {
                            cleanList.Add(usingObj.Key);
                        }
                    }

                    foreach (var cleanObj in cleanList)
                    {
                        audioDic.Remove(cleanObj);
                        GameObject.Destroy(cleanObj);
                    }
                }

                AudioManager.Instance.SoundFinished(audio.Value);
                return;
            }
        }

        GameObject newAduidObj = new GameObject();
        newAduidObj.name = soundName;
        newAduidObj.transform.SetParent(thisTransform);
        AudioSource audioSource = newAduidObj.AddComponent<AudioSource>();
        audioSource.clip = AudioManager.Instance.GetSound(soundName);
        audioSource.loop = false;
        audioSource.Play();

        audioDic.Add(newAduidObj, audioSource);
        AudioManager.Instance.SoundFinished(audioSource);
    }
}
