using System.Collections.Generic;
using System.Linq;

public class AverageValues
{
    public static List<float> CreateSmallerDataSet(List<float> dataSet, int lengthOfSmallerDataset)
    {
        var newDataSet = new List<float>();
        for (int index = 0; index < dataSet.Count / lengthOfSmallerDataset - 1; ++index)
        {
            List<float> sublist;
            if (index * lengthOfSmallerDataset + lengthOfSmallerDataset <= dataSet.Count)
                sublist = dataSet.GetRange(index * lengthOfSmallerDataset, lengthOfSmallerDataset);
            else
                sublist = dataSet.GetRange(index * lengthOfSmallerDataset, index * lengthOfSmallerDataset + lengthOfSmallerDataset - dataSet.Count);

            float averageValue = sublist.Average();
            newDataSet.Add(averageValue);
            sublist.Clear();
        }
        return newDataSet;
    }
}
