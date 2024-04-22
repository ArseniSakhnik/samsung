namespace Mlsat.Services;

public class FileService
{
    private static string SourceDirectory = @"/mlsat/";
    private static string ProjectsDirectory = SourceDirectory + @"projects/";
    private static string TrashSource = @"/mlsat/trash";

    public IReadOnlyCollection<string> GetModelGraphics(
        string modelPath)
    {
        var directories = Directory.GetDirectories(modelPath);
        var graphicsUrl = new List<string>();

        foreach (var directory in directories)
        {
            var urls = Directory.GetFiles(directory)
                .Where(f => f.Substring(f.Length - 3, 3) == "jpg")
                .Select(f => f
                    .Replace(SourceDirectory, @"/content/")
                    .Replace(@"\", @"/"));

            graphicsUrl.AddRange(urls);
        }

        return graphicsUrl;
    }

    public static string GetSourceDirectory()
    {
        CreateFolderIfNotExist(SourceDirectory);
        return SourceDirectory;
    }

    public string CreateAndGetProjectDirectory(string projectTitle)
    {
        var path = ProjectsDirectory + projectTitle + @"/";
        CreateFolderIfNotExist(path);
        return path;
    }

    public string CreateAndGetProjectDatasourceDirectory(string projectTitle)
    {
        var path = CreateAndGetProjectDirectory(projectTitle) + @"datasource/";
        CreateFolderIfNotExist(path);
        return path;
    }

    public string CreateAndGetModelDirectory(string projectTitle, string model, int version)
    {
        var path = CreateAndGetProjectDirectory(projectTitle) + @"model" + @"/" + model + @"/" + version + @"/";
        CreateFolderIfNotExist(path);
        return path;
    }

    public static void CreateFolderIfNotExist(string path)
    {
        Directory.CreateDirectory(path);
    }

    public async Task<string> LoadDataSource(string projectTitle, string dataSourceTitle, IFormFile file)
    {
        var dataSourceDirectory = CreateAndGetProjectDirectory(projectTitle);
        var path = dataSourceDirectory + @"datasource/";
        CreateFolderIfNotExist(path);
        path = path + dataSourceTitle + ".csv";

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return path;
    }

    public async Task<string> LoadDataSource(IFormFile file)
    {
        var fileName = Path.GetFileName(file.Name);
        var directoryPath = @$"{TrashSource}/datasets";
        CreateFolderIfNotExist(directoryPath);
        var path = @$"{directoryPath}/{fileName}.csv";
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return path;
    }
}