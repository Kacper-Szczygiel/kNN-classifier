using System.Globalization;
using System.Text.RegularExpressions;
using kNN_classifier;
using Microsoft.VisualBasic;

public class Program
{
    public static void Main(string[] args)
    {
        var trainSetName = ReadTrainingFileFromUser();
        var testSetName = ReadTestFileFromUser();
        var k = ReadKFromUser();

        List<Data> trainSet = FileToList(trainSetName);
        List<Data> testSet = FileToList(testSetName);

        double allRecords = testSet.Count;
        double correctRecord = 0;
        foreach (var testData in testSet)
        {
            SelectTypeForRecord(testData, trainSet, k);
            if (testData.PredictedType == testData.CorrectType)
            {
                correctRecord++;
            }
        }
        
        Console.WriteLine("Vectors;CorrectType;PredictedType");
        foreach (var data in testSet)
        {
            Console.WriteLine(data.Vectors + ";" + data.CorrectType + ";" + data.PredictedType);
        }
        Console.WriteLine("Accuracy: " + correctRecord/allRecords * 100);
    }

    private static string ReadTrainingFileFromUser()
    {
        Console.WriteLine("Enter file name for training set");
        var trainingFile = Console.ReadLine();
        try
        {
            if (!File.Exists(trainingFile))
            {
                throw new FileNotFoundException("File doesn't exist");
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return trainingFile;
    }

    private static string ReadTestFileFromUser()
    {
        Console.WriteLine("Enter file name for training set");
        var testFile = Console.ReadLine();
        try
        {
            if (!File.Exists(testFile))
            {
                throw new FileNotFoundException("File doesn't exist");
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return testFile;
    }

    private static int ReadKFromUser()
    {
        Console.WriteLine("Enter k");
        string? k = Console.ReadLine();
        
        try
        {
            if (k == null)
            {
                throw new Exception("k can't be null");
            }
            
            const string pattern = @"^-?[0-9]+(?:\.[0-9]+)?$"; 
            var regex = new Regex(pattern);
            
            if (!regex.IsMatch(k))
            {
                throw new Exception("k must be a number");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
        
        return int.Parse(k);
    }

    private static List<Data> FileToList(string fileName)
    {
        List<Data> returnList = new List<Data>();
        var lines = File.ReadLines(fileName);
        foreach (var line in lines)
        {
            string type = GetTypeFromLine(line);
            List<string> vectors = GetVectorsFromLine(line);
            Data data = new Data(vectors, type);
        }

        return returnList;
    }

    private static string GetTypeFromLine(string line)
    {
        string[] lineSplit = line.Split(";");
        return lineSplit[^1];
    }

    private static List<string> GetVectorsFromLine(string line)
    {
        List<string> returnList = new List<string>(line.Split(";"));
        returnList.RemoveAt(returnList.Count - 1);
        return returnList;
    }
    
    private static List<string> GetVectorsFromLineWithoutType(string line)
    {
        List<string> returnList = new List<string>(line.Split(";"));
        return returnList;
    }

    private static double GetDistance(List<string> v1, List<string> v2)
    {
        double sum = 0;
        for (int i = 0; i < v1.Count - 1; i++)
        {
            sum += Math.Pow(double.Parse(v1.ElementAt(i)) - double.Parse(v2.ElementAt(i)), 2);
        }

        return Math.Sqrt(sum);
    }

    private static void SelectTypeForRecord(Data testData, List<Data> trainSet, int k)
    {
        foreach (var trainData in trainSet)
        {
            trainData.Distance = GetDistance(testData.Vectors, trainData.Vectors);
        }
        
        trainSet.Sort();

        List<string> possibleTypes = new List<string>();
        for (int i = 0; i < k; i++)
        {
            possibleTypes.Add(trainSet.ElementAt(i).CorrectType);
        }

        Dictionary<string, int> countedTypes = new Dictionary<string, int>();
        foreach (var type in possibleTypes)
        {
            if (countedTypes.ContainsKey(type))
            {
                countedTypes[type]++;
            }
            else
            {
                countedTypes.Add(type, 1);
            }
        }

        int max = 0;
        string maxType = null;
        foreach (var key in countedTypes.Keys)
        {
            if (countedTypes[key] > max)
            {
                max = countedTypes[key];
                maxType = key;
            }
        }

        testData.PredictedType = maxType;
    }
}