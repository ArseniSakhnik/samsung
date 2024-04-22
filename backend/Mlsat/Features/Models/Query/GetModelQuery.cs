using MediatR;
using Microsoft.EntityFrameworkCore;
using Mlsat.Models.Enums;
using Mlsat.Services;

namespace Mlsat.Features.Models.Query;

public class GetModelQuery : IRequest<object>
{
    public int ProjectId { get; set; }
}

public class GetModelQueryHandler : IRequestHandler<GetModelQuery, object>
{
    private readonly AppDbContext _context;
    private readonly FileService _fileService;

    public GetModelQueryHandler(AppDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<object> Handle(GetModelQuery request, CancellationToken cancellationToken)
    {
        var models = await _context.Models
            .Where(m => m.ProjectId == request.ProjectId)
            .Select(k => new GetModelQueryDto.ModelDto
            {
                Id = k.Id,
                ModelType = k.ModelType,
                ProjectId = k.ProjectId,
                DataSourceId = k.DataSourceId,
                Version = k.Version,
                Path = k.Path,
                ModelColumns = k.ModelColumns.Select(m => m.Title).ToList(),
                SpaceWeatherColumns = k.SpaceWeatherColumns.Select(m => m.Title).ToList(),
                Graphics = _fileService.GetModelGraphics(k.Path)
            })
            .ToListAsync(cancellationToken);

        return new GetModelQueryDto
        {
            Knn = models.Where(x => x.ModelType == ModelTypeEnum.Knn)
                .OrderBy(x => x.Version).ToList(),
            Lof = models.Where(x => x.ModelType == ModelTypeEnum.Lof)
                .OrderBy(x => x.Version).ToList(),
            IsolationForest = models.Where(x => x.ModelType == ModelTypeEnum.IsolationForest)
                .OrderBy(x => x.Version).ToList(),
            Gmm = models.Where(x => x.ModelType == ModelTypeEnum.Gmm)
                .OrderBy(x => x.Version).ToList(),
            Kde = models.Where(x => x.ModelType == ModelTypeEnum.Kde)
                .OrderBy(x => x.Version).ToList(),
            Ocsvm = models.Where(x => x.ModelType == ModelTypeEnum.Ocsvm)
                .OrderBy(x => x.Version).ToList(),
            Dbscan = models.Where(x => x.ModelType == ModelTypeEnum.Dbscan)
                .OrderBy(x => x.Version).ToList(),
            Optics = models.Where(x => x.ModelType == ModelTypeEnum.Optics)
                .OrderBy(x => x.Version).ToList(),
            Gad = models.Where(x => x.ModelType == ModelTypeEnum.Gad)
                .OrderBy(x => x.Version).ToList(),
            Kmad = models.Where(x => x.ModelType == ModelTypeEnum.Kmad)
                .OrderBy(x => x.Version).ToList(),
            Had = models.Where(x => x.ModelType == ModelTypeEnum.Had)
                .OrderBy(x => x.Version).ToList(),
            Sad = models.Where(x => x.ModelType == ModelTypeEnum.Sad)
                .OrderBy(x => x.Version).ToList(),
            Aad = models.Where(x => x.ModelType == ModelTypeEnum.Aad)
                .OrderBy(x => x.Version).ToList(),
            Pcad = models.Where(x => x.ModelType == ModelTypeEnum.Pcad)
                .OrderBy(x => x.Version).ToList(),
            Svd = models.Where(x => x.ModelType == ModelTypeEnum.Svd)
                .OrderBy(x => x.Version).ToList(),
            Chad = models.Where(x => x.ModelType == ModelTypeEnum.Chad)
                .OrderBy(x => x.Version).ToList(),
            Mvad = models.Where(x => x.ModelType == ModelTypeEnum.Mvad)
                .OrderBy(x => x.Version).ToList(),
            Hsad = models.Where(x => x.ModelType == ModelTypeEnum.Hsad)
                .OrderBy(x => x.Version).ToList(),
            FeatureBagging = models.Where(x => x.ModelType == ModelTypeEnum.FeatureBagging)
                .OrderBy(x => x.Version).ToList(),
            Ife = models.Where(x => x.ModelType == ModelTypeEnum.Ife)
                .OrderBy(x => x.Version).ToList(),
            Admc = models.Where(x => x.ModelType == ModelTypeEnum.Admc)
                .OrderBy(x => x.Version).ToList(),
            Autoencoder = models.Where(x => x.ModelType == ModelTypeEnum.Autoencoder)
                .OrderBy(x => x.Version).ToList(),
            Ffnn = models.Where(x => x.ModelType == ModelTypeEnum.Ffnn)
                .OrderBy(x => x.Version).ToList(),
            Gan = models.Where(x => x.ModelType == ModelTypeEnum.Gan)
                .OrderBy(x => x.Version).ToList(),
            Rnn = models.Where(x => x.ModelType == ModelTypeEnum.Rnn)
                .OrderBy(x => x.Version).ToList(),
            Cnn = models.Where(x => x.ModelType == ModelTypeEnum.Cnn)
                .OrderBy(x => x.Version).ToList(),
        };
    }
}