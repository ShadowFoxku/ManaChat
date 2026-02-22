namespace ManaChat.API.Clients
{
    public static class ClientFetcher
    {
        public const string ClientHeaderName = "X-ManaChat-Client";
        public static ManaChatClient Get(string clientName)
        {
            return clientName.ToLower() switch
            {
                "manaweb" => new ManaChatWebClient(),
                _ => throw new ArgumentException($"Unknown client: {clientName}")
            };
        }

        public static ManaChatClient GetFromHeaders(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue(ClientHeaderName, out var clientName) || string.IsNullOrWhiteSpace(clientName))
                throw new KeyNotFoundException("Client header not found");

            return Get(clientName!);
        }
    }
}
