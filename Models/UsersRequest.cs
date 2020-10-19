using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OKexTime.Models
{
    public class UsersRequest
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        [MaxLength(20)]
        [JsonProperty(Required = Required.Always)]
        public string Phone { get; set; }
        [JsonProperty(Required = Required.Always)]
        [MaxLength(42)]
        public string EthereumAddress { get; set; }
        [JsonProperty(Required = Required.Always)]
        public decimal ExpectedAmount { get; set; }
        [JsonIgnore]
        public bool Settled { get; set; }
        [MaxLength(100)]
        [JsonIgnore]
        public string TxId { get; set; }
        [JsonIgnore]
        [MaxLength(100)]
        public string EthereumStatus { get; set; }
        [JsonIgnore]
        [MaxLength(12)]
        public string UserId { get; set; }

    }
}
