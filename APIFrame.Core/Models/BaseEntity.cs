using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIFrame.Core.Models
{
    public class BaseEntity
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [JsonIgnore]
        public bool IsActive { get; set; } = true;

        [Required]
        [JsonIgnore]
        public DateTime CreationDate { get; set; } = DateTime.Now.ToLocalTime();

        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; }
    }
}
