using FluentValidation;
using FluentValidation.Results;
using MasLazu.AspNet.EmailSender.Abstraction.Interfaces;
using MasLazu.AspNet.EmailSender.Abstraction.Models;
using MasLazu.AspNet.Framework.Application.Interfaces;
using MasLazu.AspNet.Framework.Domain.Entities;
using MassTransit;
using Moq;

namespace MasLazu.AspNet.Verification.Tests.Fixtures;

/// <summary>
/// Factory for creating mock objects used in tests
/// </summary>
public static class MockFactory
{
    public static Mock<IRepository<T>> CreateRepositoryMock<T>() where T : BaseEntity
    {
        var mock = new Mock<IRepository<T>>();

        mock.Setup(r => r.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T entity, CancellationToken ct) => entity);

        mock.Setup(r => r.UpdateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    public static Mock<IReadRepository<T>> CreateReadRepositoryMock<T>() where T : BaseEntity
    {
        return new Mock<IReadRepository<T>>();
    }

    public static Mock<IUnitOfWork> CreateUnitOfWorkMock()
    {
        var mock = new Mock<IUnitOfWork>();

        mock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return mock;
    }

    public static Mock<IEmailSender> CreateEmailSenderMock()
    {
        var mock = new Mock<IEmailSender>();

        mock.Setup(e => e.SendEmailAsync(
            It.IsAny<EmailMessage>(),
            It.IsAny<IHtmlRenderer>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    public static Mock<IHtmlRenderer> CreateHtmlRendererMock()
    {
        var mock = new Mock<IHtmlRenderer>();

        // IHtmlRenderer doesn't have RenderAsync, it's used by EmailSender
        // We don't need to mock it as it's passed to SendEmailAsync

        return mock;
    }

    public static Mock<IPublishEndpoint> CreatePublishEndpointMock()
    {
        var mock = new Mock<IPublishEndpoint>();

        mock.Setup(p => p.Publish(
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    public static Mock<IEntityPropertyMap<T>> CreatePropertyMapMock<T>() where T : BaseEntity
    {
        return new Mock<IEntityPropertyMap<T>>();
    }

    public static Mock<IPaginationValidator<T>> CreatePaginationValidatorMock<T>() where T : BaseEntity
    {
        return new Mock<IPaginationValidator<T>>();
    }

    public static Mock<ICursorPaginationValidator<T>> CreateCursorPaginationValidatorMock<T>() where T : BaseEntity
    {
        return new Mock<ICursorPaginationValidator<T>>();
    }

    public static Mock<IValidator<T>> CreateValidatorMock<T>()
    {
        var mock = new Mock<IValidator<T>>();

        mock.Setup(v => v.ValidateAsync(
            It.IsAny<T>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        return mock;
    }
}
