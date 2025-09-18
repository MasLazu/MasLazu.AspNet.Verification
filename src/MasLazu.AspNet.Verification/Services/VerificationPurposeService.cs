using FluentValidation;
using MasLazu.AspNet.Verification.Abstraction.Interfaces;
using MasLazu.AspNet.Verification.Abstraction.Models;
using MasLazu.AspNet.Verification.Domain.Entities;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Framework.Application.Services;
using Mapster;

namespace MasLazu.AspNet.Verification.Services;

public class VerificationPurposeService : CrudService<VerificationPurpose, VerificationPurposeDto, CreateVerificationPurposeRequest, UpdateVerificationPurposeRequest>, IVerificationPurposeService
{
    public VerificationPurposeService(
        IRepository<VerificationPurpose> repository,
        IReadRepository<VerificationPurpose> readRepository,
        IUnitOfWork unitOfWork,
        IEntityPropertyMap<VerificationPurpose> propertyMap,
        IPaginationValidator<VerificationPurpose> paginationValidator,
        ICursorPaginationValidator<VerificationPurpose> cursorPaginationValidator,
        IValidator<CreateVerificationPurposeRequest>? createValidator = null,
        IValidator<UpdateVerificationPurposeRequest>? updateValidator = null)
        : base(repository, readRepository, unitOfWork, propertyMap, paginationValidator, cursorPaginationValidator, createValidator, updateValidator)
    {
    }

    public async Task<VerificationPurposeDto> CreateIfNotExistsAsync(Guid id, CreateVerificationPurposeRequest createRequest, CancellationToken ct = default)
    {
        await ValidateAsync(createRequest, CreateValidator, ct);

        VerificationPurpose? existingEntity = await ReadRepository.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existingEntity != null)
        {
            return existingEntity.Adapt<VerificationPurposeDto>();
        }

        VerificationPurpose entity = createRequest.Adapt<VerificationPurpose>();
        entity.Id = id;
        VerificationPurpose createdEntity = await Repository.AddAsync(entity, ct);
        await UnitOfWork.SaveChangesAsync(ct);

        return createdEntity.Adapt<VerificationPurposeDto>();
    }
}
