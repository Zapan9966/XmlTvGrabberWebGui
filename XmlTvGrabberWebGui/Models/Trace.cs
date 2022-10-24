using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace XmlTvGrabberWebGui.Models
{
    public class Trace
    {
        [Key]
        public int TraceId { get; set; }

        public string Category { get; set; }

        [Required]
        public LogLevel LogLevel { get; set; }

        public int EventId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Message { get; set; }

        public string Filename { get; set; }

        public int? FileProcessingId { get; set; }
    }

}
