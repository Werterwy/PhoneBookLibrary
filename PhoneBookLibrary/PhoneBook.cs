using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace PhoneBookLibrary
{
    public class PhoneBook
    {
        // private List<PhoneBookContact> contacts = new List<PhoneBookContact>();
        private List<PhoneBookContact> contacts = null;

        private readonly string path = "";

        public PhoneBook(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("Путь к БД должен быть заполнен");

            this.path = path;
            InitializeDatabase();
            contacts = new List<PhoneBookContact>();

            List<PhoneBookContact> allContacts = GetAllContacts();

            foreach (var con in allContacts)
            {
                contacts.Add(con);
            }
        }

        private void InitializeDatabase()
        {
            using (var db = new LiteDatabase(path))
            {
                var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                contacts.EnsureIndex(x => x.Id, true);
            }
        }

        public void AddContact(PhoneBookContact contact)
        {
            try
            {
                using (var db = new LiteDatabase(path))
                {
                    var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                    contacts.Insert(contact);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении контакта: {ex.Message}");
            }
            contact.Id = GetNextId();
            contacts.Add(contact);

        }

        public void AddContact(string firstName, string lastName, string phoneNumber, string email)
        {
            var newContact = new PhoneBookContact
            {
                Name = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email
            };

            AddContact(newContact);
        }

        public List<PhoneBookContact> GetAllContacts()
        {
            List<PhoneBookContact> allContacts = new List<PhoneBookContact>();

            try
            {
                using (var db = new LiteDatabase(path))
                {
                    var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                    allContacts = contacts.FindAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении всех контактов: {ex.Message}");
            }

            return allContacts;
        }

        public List<PhoneBookContact> SearchContacts(string query)
        {
            query = query.Trim();

            List<PhoneBookContact> foundContacts = contacts
                .Where(c => c.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            c.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            c.PhoneNumber.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            c.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (foundContacts.Count > 0)
            {
                foreach (var contact in foundContacts)
                {
                    Console.WriteLine($"ID: {contact.Id}, Имя: {contact.Name} {contact.LastName}, Номер: {contact.PhoneNumber}, Email: {contact.Email}");
                }
            }
            else
            {
                Console.WriteLine("Контактов по запросу не найдено.");
            }

            return foundContacts;
        }

        public int GetNextId()
        {
            using (var db = new LiteDatabase(path))
            {
                var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                if (contacts.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    // Найти максимальный Id
                    int maxId = contacts.Max(c => c.Id);
                    return maxId;
                }
            }
        }

        public List<PhoneBookContact> GetContacts()
        {
            return contacts;
        }

        public void GetShowbyId(int id)
        {
            foreach (var contact in contacts)
            {
                if (contact.Id == id)
                {
                    Console.WriteLine(contact.ToString());
                    break;
                }
            }
        }

        public void DeleteContact(int id)
        {
            try
            {
                using (var db = new LiteDatabase(path))
                {
                    var contactsCollection = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                    contactsCollection.Delete(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении контакта: {ex.Message}");
            }
        }

        public void EditContact(int id, string firstName, string lastName, string phoneNumber, string email)
        {
            var thiscontact = contacts.FirstOrDefault(c => c.Id == id);
            if (thiscontact != null)
            {
                thiscontact.Name = firstName;
                thiscontact.LastName = lastName;
                thiscontact.PhoneNumber = phoneNumber;
                thiscontact.Email = email;
            }
            try
            {
                using (var db = new LiteDatabase(path))
                {
                    var contactsCollection = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                    var contact = contactsCollection.FindById(id);

                    if (contact != null)
                    {
                        contact.Name = firstName;
                        contact.LastName = lastName;
                        contact.PhoneNumber = phoneNumber;
                        contact.Email = email;

                        contactsCollection.Update(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при редактировании контакта: {ex.Message}");
            }
        }

        public PhoneBookContact GetContactById(int id)
        {
            return contacts.FirstOrDefault(c => c.Id == id);
        }

        public int GetContactCount()
        {
            return contacts.Count;
        }

        public bool ContactExists(int id)
        {
            return contacts.Any(c => c.Id == id);
        }

    }
}
