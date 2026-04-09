using MediatR;

namespace Blogs.Api.Tests.Fakes;

/// <summary>
/// Captures the dispatched command and returns a pre-configured response.
/// Configure an exception via <see cref="ThrowOnNextSend"/> to simulate failure paths.
/// </summary>
public class FakeMediator : IMediator
{
    private readonly object? _response;
    private Exception?      _nextException;

    public object? CapturedRequest { get; private set; }

    public FakeMediator( object? response = null ) => _response = response;

    public void ThrowOnNextSend( Exception ex ) => _nextException = ex;

    // Commands that return a value (e.g. CreateBlogCommand → BlogDto)
    public Task< TResponse > Send< TResponse >( IRequest< TResponse > request, CancellationToken ct = default )
    {
        CapturedRequest = request;
        MaybeThrow();
        if ( _response is TResponse typed ) return Task.FromResult( typed );
        return Task.FromResult( default( TResponse )! );
    }

    // Void commands (IRequest with no type parameter)
    public Task Send< TRequest >( TRequest request, CancellationToken ct = default ) where TRequest : IRequest
    {
        CapturedRequest = request;
        MaybeThrow();
        return Task.CompletedTask;
    }

    private void MaybeThrow()
    {
        if ( _nextException is null ) return;
        var ex = _nextException;
        _nextException = null;
        throw ex;
    }

    public Task< object? > Send( object request, CancellationToken ct = default ) => throw new NotImplementedException();
    public IAsyncEnumerable< TResponse > CreateStream< TResponse >( IStreamRequest< TResponse > request, CancellationToken ct = default ) => throw new NotImplementedException();
    public IAsyncEnumerable< object? > CreateStream( object request, CancellationToken ct = default ) => throw new NotImplementedException();
    public Task Publish( object notification, CancellationToken ct = default ) => throw new NotImplementedException();
    public Task Publish< TNotification >( TNotification notification, CancellationToken ct = default ) where TNotification : INotification => throw new NotImplementedException();
}
