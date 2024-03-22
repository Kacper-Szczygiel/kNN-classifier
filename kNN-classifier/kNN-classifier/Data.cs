namespace kNN_classifier;

public class Data : IComparable<Data>
{
    public List<string> Vectors { get; set; }
    public  string CorrectType { get; set; }
    public double Distance { get; set; }
    public string PredictedType { get; set; }

    public Data(List<string> vectors, string correctType)
    {
        Vectors = vectors;
        CorrectType = correctType;
    }

    public Data(List<string> vectors)
    {
        Vectors = vectors;
    }
    
    public int CompareTo(Data other)
    {
        return Distance.CompareTo(other.Distance);
    }
}