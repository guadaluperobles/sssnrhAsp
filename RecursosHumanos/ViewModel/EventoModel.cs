using RecursosHumanos.Models;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.ViewModel {
    public class EventoModel {
        public int id { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public string? textColor { get; set; }
        public string? backgroundColor { get; set; }
        public string? borderColor { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
    }
}
