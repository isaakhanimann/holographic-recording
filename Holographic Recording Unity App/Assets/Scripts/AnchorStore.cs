using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;

public class AnchorStore : MonoBehaviour
{
    // Start is called before the first frame update
    private SerializableAnchorMap anchorToRecordingMap;
    private BinaryFormatter bf;
    private string fileName = "holographic_recording_anchorToRecordingMap.bin";
    private string filePath;

    public void Init()
    {
        bf = new BinaryFormatter();
    	filePath = Application.persistentDataPath + "/" + fileName;
        Debug.Log(filePath);
    	if (System.IO.File.Exists(filePath))
		{
		    LoadAll();
		} else 
		{
			anchorToRecordingMap = new SerializableAnchorMap();
		}
    }

    public string GetAnchorId(string recordingId) {
        foreach(var item in anchorToRecordingMap)
		{
		  if (item.Value == recordingId) {
    		return item.Key;
		  }
		}
		return null;
    
    }

    public string GetRecordingId(string anchorId) {
        if (!anchorToRecordingMap.ContainsKey(anchorId)) {
            return null;
        }
    	return anchorToRecordingMap[anchorId];
    }

    public List<string> GetAllAnchorIds() {
        LoadAll();
    	List<string> anchorIds = new List<string>();
    	foreach(var item in anchorToRecordingMap)
		{
		  anchorIds.Add(item.Key);
		}
		return anchorIds;
    }

    public void LoadAll() {
    	FileStream fs = File.Open(filePath, FileMode.Open);
    	try
        {
            anchorToRecordingMap = (SerializableAnchorMap) bf.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public void Save(string anchorId, string recordingId)
    {
    	FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);

    	try
        {
            anchorToRecordingMap[anchorId] = recordingId;
	        bf.Serialize(fs, anchorToRecordingMap);
	        fs.Close();
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    	
    }

    public void DeleteByAnchorId(string anchorId) {
    	FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
    	
        anchorToRecordingMap.Remove(anchorId);
    	try
        {
	        bf.Serialize(fs, anchorToRecordingMap);
	        fs.Close();
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    	
    }

    public void DeleteByRecordingId(string recordingId) {
    	FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
        foreach(var item in anchorToRecordingMap)
		{
		  if (item.Value == recordingId) {
    		anchorToRecordingMap.Remove(item.Key);
		  }
		}
    	try
        {
	        bf.Serialize(fs, anchorToRecordingMap);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public void DeleteAll() {
    	FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
    	try
        {
            anchorToRecordingMap.Clear();
	        bf.Serialize(fs, anchorToRecordingMap);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }
}
