using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : Singleton<TableManager>
{
    private Dictionary<int, Pair<bool, Table>> tables = new Dictionary<int, Pair<bool, Table>>();
    private List<int> freeTables = new List<int>();

    public int AddTable(Table table)//��ʼʱTable��TableManagerע��
    {
        int tableIndex = tables.Count;
        tables.Add(tableIndex, new Pair<bool, Table>(false, table));
        if (!freeTables.Contains(tableIndex))
            freeTables.Add(tableIndex);
        return tableIndex;
    }
    public Table OccupyTable() // GuestManager�����������룬��TableManager�Ȳ鿴����������������ٿ��������ͣ��������һ�����ӣ�Ȼ�󷵻�����
    {
        if (freeTables.Count <= 0)
            return null;
        int tableIndex = freeTables[UnityEngine.Random.Range(0, freeTables.Count)];
        freeTables.Remove(tableIndex);
        tables[tableIndex].first = true;
        return tables[tableIndex].second;
    }
    public void EmptyTable(int tableIndex)
    {
        if (tables.ContainsKey(tableIndex))
            tables[tableIndex].first = false;
        if (!freeTables.Contains(tableIndex))
            freeTables.Add(tableIndex);
    }
    public void ServeDish(int tableIndex, int dishCount, int dishIndex, string dishName)
    {
        tables[tableIndex].second.ServeDish(dishCount, dishIndex, dishName);
    }
    public void ClearTable(int tableIndex, int dishCount)
    {
        tables[tableIndex].second.ClearTable(dishCount);
    }
}
