//-------------------------------------------------------------------------
//
//      CosmosEngine - The Lightweight Unity3D Game Develop Framework
//
//                     Copyright 漏 2011-2014
//                   MrKelly <23110388@qq.com>
//              https://github.com/mr-kelly/CosmosEngine
//
//-------------------------------------------------------------------------
/**
* @file PlayerPrefsSerializer.cs
* @brief Code snippet from UnityForum (http://forum.unity3d.com/threads/72156-C-Serialization-PlayerPrefs-mystery)
* @author mindlube+FizixMan  
* @version 1.0
* @date 2012-06-15
*/

using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

 /// <summary>
 /// 对PlayerPrefs中保存的明文数据进行序列化
 /// </summary>
public class PlayerPrefsSerializer  
{
    public static BinaryFormatter bf = new BinaryFormatter ();

    public static bool handleNonSerializable
    {
        set 
        {            
            PlayerPrefsSerializer.bf.SurrogateSelector = new SurrogateSelector();
            PlayerPrefsSerializer.bf.SurrogateSelector.ChainSelector( new NonSerialiazableTypeSurrogateSelector() );
        }
    }

    // serializableObject is any struct or class marked with [Serializable]
    public static void Save (string prefKey, object serializableObject)
    {
        // Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");  使用這句可避免IOS無法序列化問題，暫時避免使用

        MemoryStream memoryStream = new MemoryStream ();
        bf.Serialize (memoryStream, serializableObject);
        string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());
        PlayerPrefs.SetString ( prefKey, tmp);
    }
    
	/// <summary>
	/// PlayerPrefs.GetString() 已通过解密
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="prefKey"></param>
	/// <param name="defaultVal"></param>
	/// <returns></returns>
	public static T Load<T>(string prefKey, string defaultVal = null)
    {
		if (!PlayerPrefs.HasKey(prefKey) && string.IsNullOrEmpty(defaultVal))
            return default(T);
		if (!string.IsNullOrEmpty(defaultVal))
		{
			MemoryStream memoryStream = new MemoryStream();
			bf.Serialize(memoryStream, defaultVal);
			defaultVal = System.Convert.ToBase64String(memoryStream.ToArray());
		}
        string serializedData = PlayerPrefs.GetString(prefKey,defaultVal);
        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));
        T deserializedObject = (T)bf.Deserialize(dataStream);
        return deserializedObject;
    }



	/// <summary>
	/// 编码
	/// </summary>
	/// <param name="serializableObject"></param>
	/// <returns></returns>
    public static string Encode(object serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream ();
        bf.Serialize (memoryStream, serializableObject);
        string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());
        return tmp;
    }

	/// <summary>
	/// 解码
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="serialized"></param>
	/// <returns></returns>
    public static T Decode<T>(string serialized)
    {
        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serialized));
        T deserializedObject = (T)bf.Deserialize(dataStream);
        return deserializedObject;
    }
}