namespace CersaiCkycApi.Services;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
}
