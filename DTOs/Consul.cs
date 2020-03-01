using System;
using System.ComponentModel.DataAnnotations;

namespace consulService.DTOs
{
    public class Consul
    {
        private string _value;
        public string Value
        {
            get { 
                if (string.IsNullOrWhiteSpace(_value))
                    return "";
                    
                byte[] data = System.Convert.FromBase64String(this._value);
                return System.Text.ASCIIEncoding.ASCII.GetString(data);
            }
            set { _value = value; }
        }
        
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        public string Key { get; set; }

        public DateTime created { get; set; }
        public DateTime? validUntil { get; set; }
    }
}