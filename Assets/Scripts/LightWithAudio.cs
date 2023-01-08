using UnityEngine;
using UnityEngine.UI;

public class LightWithAudio : MonoBehaviour
{
    public static float volume;

    public Image light;

    private AudioClip micRecord;
    string device;

    void Start()
    {
        device = Microphone.devices[0]; //获取设备麦克风
        micRecord = Microphone.Start(device, true, 999, 44100); //44100音频采样率   固定格式
    }

    void Update()
    {
        volume = GetMaxVolume();

        //要加粒子特效  产生拖尾
        var col = light.color;
        col.a = volume * 10;
        light.color = col;
    }

    //每一振处理那一帧接收的音频文件
    float GetMaxVolume()
    {
        float maxVolume = 0f;
        //剪切音频
        float[] volumeData = new float[128];
        int offset = Microphone.GetPosition(device) - 128 + 1;
        if (offset < 0)
        {
            return 0;
        }

        micRecord.GetData(volumeData, offset);

        for (int i = 0; i < 128; i++)
        {
            float tempMax = volumeData[i]; //修改音量的敏感值
            if (maxVolume < tempMax)
            {
                maxVolume = tempMax;
            }
        }

        return maxVolume;
    }
}