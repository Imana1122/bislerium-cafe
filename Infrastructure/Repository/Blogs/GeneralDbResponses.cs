using Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Products
{
    public class GeneralDbResponses
    {
        public static ServiceResponse ItemAlreadyExist(String itemName) => new(false, $"{itemName} already created.");
        public static ServiceResponse ItemNotFound(String itemName) => new(false, $"{itemName} not found");
        public static ServiceResponse ItemCreated(String itemName) => new(true, $"{itemName} successfully created");
        public static ServiceResponse ItemUpdate(String itemName) => new(true, $"{itemName} successfully updated");
        public static ServiceResponse ItemDelete(String itemName) => new(true, $"{itemName} successfully deleted");

    }
}
