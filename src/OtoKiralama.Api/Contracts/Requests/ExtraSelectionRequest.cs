using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtoKiralama.Api.Contracts.Requests
{
    /// <summary>
    /// Rezervasyon için seçilen ekstra kalemi temsil eder.
    /// </summary>
    /// <remarks>
    /// Code için geçerli deðerler: BabySeat, ExtraDriver, GPS.
    /// </remarks>
    public sealed record ExtraSelectionRequest
    {
        /// <summary>
        /// Ekstra kodu. Geçerli deðerler: BabySeat, ExtraDriver, GPS.
        /// </summary>
        [Required, MinLength(1)]
        [JsonPropertyName("code")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Miktar (en az 1).
        /// </summary>
        [Range(1, int.MaxValue)]
        [JsonPropertyName("quantity")]
        public int Quantity { get; init; } = 1;
    }
}