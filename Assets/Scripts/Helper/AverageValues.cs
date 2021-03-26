using System.Collections.Generic;
using System.Linq;

public class AverageValues
{
    public static List<float> CreateSmallerDataSet(List<float> dataSet, int lengthOfSmallerDataset)
    {
        var newDataSet = new List<float>();
        for (var index = 0; index < dataSet.Count / lengthOfSmallerDataset - 1; ++index)
        {
            var sublist = index * lengthOfSmallerDataset + lengthOfSmallerDataset <= dataSet.Count ? 
                dataSet.GetRange(index * lengthOfSmallerDataset, lengthOfSmallerDataset) :
                dataSet.GetRange(index * lengthOfSmallerDataset, index * lengthOfSmallerDataset + lengthOfSmallerDataset - dataSet.Count);

            var averageValue = sublist.Average();
            newDataSet.Add(averageValue);
            sublist.Clear();
        }
        return newDataSet;
    }
}
