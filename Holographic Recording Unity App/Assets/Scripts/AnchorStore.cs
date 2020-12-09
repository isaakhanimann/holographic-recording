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
    private SerializableAnchorMap recordingToAnchorMap;
    private BinaryFormatter bf;
    private string fileName = "holographic_recording_recordingToAnchorMap.bin";
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
			recordingToAnchorMap = new SerializableAnchorMap();
		}
    }

    public string GetAnchorId(string recordingId) {
        if (!recordingToAnchorMap.ContainsKey(recordingId)) {
            return null;
        }
    	return recordingToAnchorMap[recordingId];
    }

    public string GetRecordingId(string anchorId) {
    	foreach(var item in recordingToAnchorMap)
		{
		  if (item.Value == anchorId) {
    		return item.Key;
		  }
		}
		return null;
    }

    public List<string> GetAllAnchorIds() {
        LoadAll();
    	List<string> anchorIds = new List<string>();
    	foreach(var item in recordingToAnchorMap)
		{
		  anchorIds.Add(item.Value);
		}
		return anchorIds;
    }

    public void LoadAll() {
    	FileStream fs = File.Open(filePath, FileMode.Open);
    	try
        {
            recordingToAnchorMap = (SerializableAnchorMap) bf.Deserialize(fs);
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

    public void Save(string recordingId, string anchorId)
    {
    	FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);

    	try
        {
            recordingToAnchorMap[recordingId] = anchorId;
	        bf.Serialize(fs, recordingToAnchorMap);
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
    	foreach(var item in recordingToAnchorMap)
		{
		  if (item.Value == anchorId) {
    		recordingToAnchorMap.Remove(item.Key);
		  }
		}

    	try
        {
	        bf.Serialize(fs, recordingToAnchorMap);
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
    	try
        {
            recordingToAnchorMap.Remove(recordingId);
	        bf.Serialize(fs, recordingToAnchorMap);
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
            recordingToAnchorMap.Clear();
	        bf.Serialize(fs, recordingToAnchorMap);
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
