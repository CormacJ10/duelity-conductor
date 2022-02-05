using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitSheet", menuName = "HitSheet", order = 50)]
public class HitSheet : ScriptableObject
{
    public List<Hit> hitList1; // to work around unity not serialising jagged or multi-dimensional arrays
    public List<Hit> hitList2;
    public List<Hit> hitList3;
    public List<Hit> hitList4;
    public List<Hit> hitList5;
    public List<Hit> hitList6;
    public List<Hit> hitList7;
    public List<Hit> hitList8;

    public static implicit operator List<List<Hit>>(HitSheet sheet)
    {
        List<List<Hit>> hitList = new List<List<Hit>>();
        hitList.Add(sheet.hitList1);
        hitList.Add(sheet.hitList2);
        hitList.Add(sheet.hitList3);
        hitList.Add(sheet.hitList4);
        hitList.Add(sheet.hitList5);
        hitList.Add(sheet.hitList6);
        hitList.Add(sheet.hitList7);
        hitList.Add(sheet.hitList8);
        
        return hitList;
    }

    public static implicit operator HitSheet(List<List<Hit>> hitList)
    {
        HitSheet sheet =  ScriptableObject.CreateInstance<HitSheet>();
        switch (hitList.Count) {
            case 8: sheet.hitList8 = hitList[7]; goto case 7; // a rare fallthrough use case
            case 7: sheet.hitList7 = hitList[6]; goto case 6;
            case 6: sheet.hitList6 = hitList[5]; goto case 5;
            case 5: sheet.hitList5 = hitList[4]; goto case 4;
            case 4: sheet.hitList4 = hitList[3]; goto case 3;
            case 3: sheet.hitList3 = hitList[2]; goto case 2;
            case 2: sheet.hitList2 = hitList[1]; goto case 1;
            case 1: sheet.hitList1 = hitList[0]; break;
            default:
                Debug.Log("hitList had unexpected length!");
                break;
        }

        return sheet;
    }
}
