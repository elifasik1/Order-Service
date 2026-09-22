using MassTransit;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Services;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T message)
        where T : class
    {
        return _publishEndpoint.Publish(message);
    }
}