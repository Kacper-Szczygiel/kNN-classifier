namespace kNN_classifier;

public class CustomException : Exception
{
    public CustomException()
    {
    }

    public CustomException(string message) : base(message)
    {
    }
}