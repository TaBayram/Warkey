using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StorageHandler
{

    /// https://forum.unity.com/threads/simple-local-data-storage.468936/
    /// <summary>
    /// Serialize an object to the devices File System.
    /// </summary>
    /// <param name="objectToSave">The Object that will be Serialized.</param>
    /// <param name="fileName">Name of the file to be Serialized.</param>
    public void SaveData(object objectToSave, string fileName) {

        FileStream fileStream = null;
        try {
            // Add the File Path together with the files name and extension.
            // We will use .bin to represent that this is a Binary file.
            string FullFilePath = Application.persistentDataPath + "/" + fileName + ".bin";
            // We must create a new Formattwr to Serialize with.
            BinaryFormatter Formatter = new BinaryFormatter();
            // Create a streaming path to our new file location.
            fileStream = new FileStream(FullFilePath, FileMode.Create);
            // Serialize the objedt to the File Stream
            Formatter.Serialize(fileStream, objectToSave);
        }
        catch(Exception _) {

        }
        finally {
            // FInally Close the FileStream and let the rest wrap itself up.
            if (fileStream != null)
                fileStream.Close();
        }
        
    }
    /// <summary>
    /// Deserialize an object from the FileSystem.
    /// </summary>
    /// <param name="fileName">Name of the file to deserialize.</param>
    /// <returns>Deserialized Object</returns>
    public object LoadData(string fileName) {
        string FullFilePath = Application.persistentDataPath + "/" + fileName + ".bin";
        // Check if our file exists, if it does not, just return a null object.
        if (File.Exists(FullFilePath)) {
            FileStream fileStream = null;
            try {
                BinaryFormatter Formatter = new BinaryFormatter();
                fileStream = new FileStream(FullFilePath, FileMode.Open);
                object obj = Formatter.Deserialize(fileStream);
                return obj;
            }
            catch(Exception _) {
                return null;
            }
            finally {
                if (fileStream != null)
                    fileStream.Close();
            }
        }
        else {
            return null;
        }
    }
    
}
