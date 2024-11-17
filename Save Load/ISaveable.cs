using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    DataDefination GetDataID();

    void RegiseterSaveData() => DataManager.instance.RegiseterSaveData(this);
    void UnRegiseterSaveData() => DataManager.instance.UnRegiseterSaveData(this);

    void GetSaveData(Data data);
    void LoadData(Data data);
}
