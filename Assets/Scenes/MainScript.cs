using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public InputField InputField_android;
    public InputField InputField_ios;
    public InputField InputField_result;
    public Text Text_result;

    bool canClick = true;

    void Start()
    {
        InputField_android.text = PlayerPrefs.GetString("android path","");
        InputField_ios.text = PlayerPrefs.GetString("ios path", "");
    }

    public void onClickBtn()
    {
        if(!canClick)
        {
            return;
        }

        InputField_result.text = "";
        Text_result.text = "";

        int addCount = 0;
        int changeCount = 0;
        int deleteCount = 0;

        if (!Directory.Exists(InputField_android.text))
        {
            Text_result.text = "android路径不存在";
            return;
        }
        else
        {
            PlayerPrefs.SetString("android path", InputField_android.text);
        }

        if (!Directory.Exists(InputField_ios.text))
        {
            Text_result.text = "ios路径不存在";
            return;
        }
        else
        {
            PlayerPrefs.SetString("ios path", InputField_ios.text);
        }

        canClick = false;

        // 遍历安卓文件夹资源
        {
            DirectoryInfo direction = new DirectoryInfo(InputField_android.text);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].FullName.Substring(InputField_android.text.Length);
                FileInfo iosFile = findFileFromIOS(filePath);
                if(iosFile != null)
                {
                    string md5_android = GetMD5HashFromFile(files[i].FullName);
                    string md5_ios = GetMD5HashFromFile(iosFile.FullName);
                    if(md5_android != md5_ios)
                    {
                        InputField_result.text += ("<color=#F19F2D>修改----</color>" + filePath + "\n");
                        ++changeCount;
                    }
                }
                else
                {
                    InputField_result.text += ("<color=#0FD42B>增加----</color>" + filePath + "\n");
                    ++addCount;
                }
            }
        }

        // 遍历ios文件夹资源
        {
            DirectoryInfo direction = new DirectoryInfo(InputField_ios.text);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].FullName.Substring(InputField_ios.text.Length);
                FileInfo iosFile = findFileFromAndroid(filePath);
                if (iosFile == null)
                {
                    InputField_result.text += ("<color=#E53928>删除----</color>" + filePath + "\n");
                    ++deleteCount;
                }
            }
        }

        Text_result.text += ("修改:" + changeCount + "    ");
        Text_result.text += ("增加:" + addCount + "    ");
        Text_result.text += ("删除:" + deleteCount + "    ");

        canClick = true;
    }

    public FileInfo findFileFromIOS(string file)
    {
        DirectoryInfo direction = new DirectoryInfo(InputField_ios.text);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            if(files[i].FullName.Substring(InputField_ios.text.Length) == file)
            {
                return files[i];
            }
        }

        return null;
    }

    public FileInfo findFileFromAndroid(string file)
    {
        DirectoryInfo direction = new DirectoryInfo(InputField_android.text);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].FullName.Substring(InputField_android.text.Length) == file)
            {
                return files[i];
            }
        }

        return null;
    }

    string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Text_result.text = "error:" + ex.Message;
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
}
