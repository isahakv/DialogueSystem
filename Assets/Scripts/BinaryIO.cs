using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class BinaryIO
{
	public static T ReadFromBinaryFile<T>(string path) where T : class
	{
		T obj = null;
		if (File.Exists(path))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);
			obj = (T)bf.Deserialize(file);

			file.Close();
		}
		return obj;
	}

	public static void WriteToBinaryFile<T>(string path, T obj) where T : class
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(path);
		bf.Serialize(file, obj);

		file.Close();
	}
}
