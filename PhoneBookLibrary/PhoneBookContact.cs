using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookLibrary
{
    public class PhoneBookContact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // ToString для удобного вывода информации о контакте
        public override string ToString()
        {
            return $"ID: {Id}, Имя: {Name} {LastName}, Номер: {PhoneNumber}, Email: {Email}";
        }
    }
}
