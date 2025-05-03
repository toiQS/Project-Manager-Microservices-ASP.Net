using MediatR;
using PM.Application.Features.Project.Dtos;
using PM.Persistence;
using PM.Domain.Entities;
namespace PM.Application.Features.Project.Handlers
{
    public class CreateProjectCommand : IRequest<string>
    {
        public ProjectCreateDto Dto { get; set; }

        public CreateProjectCommand(ProjectCreateDto dto)
        {
            Dto = dto;
        }
    }

    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, string>
    {
        private readonly ApplicationDbContext _context;

        public CreateProjectHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                StatusId = request.Dto.StatusId,
                CreatedDate = DateTime.UtcNow,
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                IsDeleted = false,
                IsCompleted = false
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return project.Id;
        }
    }
}
