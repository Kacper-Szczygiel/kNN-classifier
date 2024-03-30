using System.Globalization;
using kNN_classifier;

public class Program
{
    public static void Main(string[] args)
    {
        var trainSetName = ReadTrainingFileFromUser();
        var testSetName = ReadTestFileFromUser();
        var k = ReadKFromUser();

        List<Data> trainSet = FileToList(trainSetName);
        List<Data> testSet = FileToList(testSetName);
        
        foreach (var testData in testSet)
        {
            SelectTypeForRecord(testData, trainSet, k);
        }
        
        Console.WriteLine("Vectors;CorrectType;PredictedType");
        foreach (var data in testSet)
        {
            PrintData(data);
        }
        
        Console.WriteLine("Accuracy: {0:P2}.",CalculateAccuracy(testSet));

        string answer;
        do
        {
            Console.WriteLine("Would you like to add a new record? (yes/no)");
            answer = Console.ReadLine();
            if (answer == "no") { Environment.Exit(0); }
            Console.WriteLine("Enter record");
            String record = Console.ReadLine();
            Data dataRecord = new Data(GetVectorsFromLineWithoutType(record));
            SelectTypeForRecord(dataRecord, trainSet, k);
            PrintData(dataRecord);
        } while (answer == "yes");
    }

    private static void PrintData(Data data)
    {
        foreach (var vector in data.Vectors)
        {
            Console.Write(vector + ";");
        }
        Console.WriteLine(data.CorrectType + ";" + data.PredictedType);
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
        Console.WriteLine("Enter file name for test set");
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
        string k = Console.ReadLine();
        return int.Parse(k);
    }

    private static List<Data> FileToList(string fileName)
    {
        List<Data> returnList = new List<Data>();
        var lines = File.ReadLines(fileName);
        foreach (var line in lines)
        {
            string? type = GetTypeFromLine(line);
            List<string> vectors = GetVectorsFromLine(line);
            Data data = new Data(vectors, type);
            returnList.Add(data);
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
            sum += Math.Pow(double.Parse(v1.ElementAt(i), CultureInfo.InvariantCulture) - double.Parse(v2.ElementAt(i), CultureInfo.InvariantCulture), 2);
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

        List<string?> possibleTypes = new List<string?>();
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
        string? maxType = null;
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

    private static double CalculateAccuracy(List<Data> testSet)
    {
        double allRecords = testSet.Count;
        double correctRecord = 0;
        
        foreach (var testData in testSet)
        {
            if (testData.PredictedType == testData.CorrectType)
            {
                correctRecord++;
            }
        }

        return correctRecord / allRecords;
    }
}