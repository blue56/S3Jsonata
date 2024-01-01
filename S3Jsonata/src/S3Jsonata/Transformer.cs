using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Jsonata.Net.Native;

public class Transformer
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string Run(string Template, string Input)
    {
        //JsonataQuery query = new JsonataQuery("$.a");
        JsonataQuery query = new JsonataQuery(Template);

        string result = query.Eval(Input);

        return result;
    }

    public static async void Run(string Region, string Bucketname,
        string TemplatePath, string InputPath, string ResultPath)
    {
        // Read
        string input = await ReadS3ObjectAsync(Region, Bucketname, InputPath);
        string template = await ReadS3ObjectAsync(Region, Bucketname, TemplatePath);

        // Transform
        string resultString = Run(template, input);

        // Write result
        var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(resultString));

        SaveFile(Region, Bucketname, ResultPath, stream, "application/json");
    }

    public static void SaveFile(string Region, string Bucketname,
        string Key, Stream Stream, string ContentType)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        var _client = new AmazonS3Client(region);

        var putRequest = new PutObjectRequest
        {
            BucketName = Bucketname,
            Key = Key,
            ContentType = ContentType,
            InputStream = Stream
        };

        _client.PutObjectAsync(putRequest).Wait();
    }

    static async Task<string> ReadS3ObjectAsync(string Region, string bucketName, string key)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        using (var client = new AmazonS3Client(region)) // Change the region if needed
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (GetObjectResponse response = await client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }

}