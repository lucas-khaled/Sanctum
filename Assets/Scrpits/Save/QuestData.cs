using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public Quest[] questsAceitas;
    public Quest[] questsFinalizadas;

    public string[] giversNames;
    public int[] giversQntAceitas;

    public QuestData(QuestLog questLog)
    {
        questsAceitas = questLog.getQuestAceitas().ToArray();
        questsFinalizadas = questLog.getQuestFinalizadas().ToArray();

        QuestGiver[] questsGivers = GameObject.FindObjectsOfType<QuestGiver>();
        RelacionaGivers(questsGivers);
    }

    void RelacionaGivers(QuestGiver[] questGiver)
    {
        giversNames = new string[questGiver.Length];
        giversQntAceitas = new int[questGiver.Length];

        for(int i = 0; i<questGiver.Length; i++)
        {
            giversNames[i] = questGiver[i].name;
            giversQntAceitas[i] = questGiver[i].GetQuestAceitas();
        }
    }
}
