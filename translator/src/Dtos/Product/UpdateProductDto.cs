using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace translator.src.Dtos.Clients.Product
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? Category { get; set; }
        public IFormFile? Image { get; set; }
    }
}