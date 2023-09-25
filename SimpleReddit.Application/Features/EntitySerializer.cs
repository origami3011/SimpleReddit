using SimpleReddit.Application.Contracts;
using System.Text.Json;

namespace SimpleReddit.Application.Features
{
    public class EntitySerializer : IEntitySerializer
    {
        /// <inheritdoc/>
        public async Task<T> DeserializeEntityAsync<T>(byte[] entity, CancellationToken cancellationToken = default)
        {
            //ArgumentValidation.ThrowIfNull(entity);

            using MemoryStream memoryStream = new MemoryStream(entity);
            var value = await JsonSerializer.DeserializeAsync<T>(memoryStream, cancellationToken: cancellationToken);
            return value;
        }

        /// <inheritdoc/>
        public async Task<byte[]> SerializeEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
        {
            using MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync<T>(memoryStream, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.ToArray();
        }
    }
}
