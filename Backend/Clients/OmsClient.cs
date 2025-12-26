using Backend.DAL.Models;
using System.Text;
using System.Text.Json;

namespace Backend.Clients
{
    public class OmsClient(HttpClient client)
    {
        public async Task<V1AuditLogOrderResponse> LogOrder(V1AuditLogOrderRequest request, CancellationToken token)
        {
            var requestJson = JsonSerializer.Serialize(request);
            var response = await client.PostAsync(
                "api/v1/audit-log-order",
                new StringContent(requestJson, Encoding.UTF8, "application/json"),
                token
            );
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP error: {(int)response.StatusCode}");
            }
            var content = await response.Content.ReadAsStringAsync(token);
            return JsonSerializer.Deserialize<V1AuditLogOrderResponse>(content)!;
        }
    }
}