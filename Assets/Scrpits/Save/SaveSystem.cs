using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Data
{
    public PlayerData playerData;
    public InventarioData inventarioData;
    public QuestData questData;
    public HabilidadesData habilidadesData;
}

public static class SaveSystem
{

    public static Data data;

    static void SaveQuestData()
    {
        data.questData = new QuestData(QuestLog.questLog);
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameQuestSave.SAVE";
        FileStream stream = File.Create(path);

        var json = JsonUtility.ToJson(data.questData);

        try
        {
            formatter.Serialize(stream, Criptografa(json));
        }
        catch (SerializationException e)
        {
            Debug.LogError("Could not Serialize: " + e.Message);
        }
        finally
        {
            stream.Close();
        }
    } 

    static void LoadQuestData()
    {
        string path = Application.persistentDataPath + "/gameQuestSave.SAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);

            string json = (string)formatter.Deserialize(stream);
            json = Descriptografa(json);
            JsonUtility.FromJsonOverwrite(json, data.questData);
            stream.Close();
        }
        else
        {
            Debug.LogError("Quest File not found in " + path);
        }
    }

    public static void Save()
    {
        data = new Data();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameSave.SAVE";
        FileStream stream = File.Create(path);

        data.inventarioData = new InventarioData(Inventario.inventario);
        data.playerData = new PlayerData(Player.player.status);
        data.habilidadesData = new HabilidadesData();

        var json = JsonUtility.ToJson(data);

        try
        {
            formatter.Serialize(stream, Criptografa(json));
        }
        catch (SerializationException e)
        {
            Debug.LogError("Could not Serialize: "+ e.Message);
        }
        finally {
            stream.Close();
        }
        SaveQuestData();
    }


    public static Data Load()
    {
        string path = Application.persistentDataPath + "/gameSave.SAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);

            string json = (string)formatter.Deserialize(stream);
            json = Descriptografa(json);
            data = JsonUtility.FromJson<Data>(json);
            stream.Close();
            LoadQuestData();
            return data;
        }
        else
        {
            Debug.LogError("File not found in "+ path);
            return null;
        }        
    }

    private static string Criptografa(string json)
    {
        int aux = 0;
        char[] chars = json.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            aux = (int)chars[i];
            aux = aux + 23;
            chars[i] = (char)aux;
        }
        json = new string(chars);
        return json;
    }
    private static string Descriptografa(string json)
    {
        int aux = 0;
        char[] chars = json.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            aux = (int)chars[i];
            aux = aux - 23;
            chars[i] = (char)aux;
        }
        json = new string(chars);
        return json;
    }
}
