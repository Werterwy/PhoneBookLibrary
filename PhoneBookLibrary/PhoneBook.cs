using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Serilog;

namespace PhoneBookLibrary
{
    public class PhoneBook
    {
        private List<PhoneBookContact> contacts = null;

        private readonly string path = "";

        public PhoneBook(string path)
        {
            // Проверяем, что путь к базе данных не является пустым или null
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("Путь к БД должен быть заполнен");

            this.path = path;
            // Выполняем инициализацию базы данных
            InitializeDatabase();
            contacts = new List<PhoneBookContact>();

            // Получаем все контакты из базы данных
            List<PhoneBookContact> allContacts = GetAllContacts();

            // Добавляем все контакты в список contacts
            foreach (var contact in allContacts)
            {
                contacts.Add(contact);
            }
        }

        private void InitializeDatabase()
        {
            // Используем конструкцию using для создания объекта LiteDatabase и его автоматического освобождения после использования
            using (var db = new LiteDatabase(path))
            {
                // Получаем коллекцию контактов из базы данных
                var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                // Создаем индекс для поля Id в коллекции контактов
                contacts.EnsureIndex(x => x.Id, true);
            }
        }

        /// <summary>
        /// метод для добавление контакта в базу данных
        /// </summary>
        /// <param name="contact"></param>
        public void AddContact(PhoneBookContact contact)
        {
            bool checktry = true;
            try
            {
                // Используем LiteDB для работы с базой данных
                using (var db = new LiteDatabase(path))
                {
                    // Получаем коллекцию контактов из базы данных
                    var contacts = db.GetCollection<PhoneBookContact>(typeof(PhoneBookContact).Name);
                    // Вставляем новый контакт в базу данных
                    contacts.Insert(contact);
                }
               
            }
            catch (Exception ex)
            {
                // В случае возникновения исключения выводим сообщение об ошибке
               // Console.WriteLine($"Ошибка при добавлении контакта: {ex.Message}");

                // Логирование ошибки
                Log.Error(ex, $"Ошибка при добавлении контакта: {ex.Message}");
                throw; // Переопределение исключения после логирования
                checktry = false;
            }

            if (checktry)
            {
                // Получаем следующий идентификатор и добавляем контакт в коллекцию
                contact.Id = GetNextId();
                contacts.Add(contact);
            }

        }

        /// <summary>
        /// метод для добавление контакта
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
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

        /// <summary>
        /// метод для получение всех контактов из базы данных
        /// </summary>
        /// <returns></returns>
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
                //Console.WriteLine($"Ошибка при получении всех контактов: {ex.Message}");

                // Логирование ошибки
                Log.Error(ex, $"Ошибка при получении всех контактов: {ex.Message}");
                throw; // Переопределение исключения после логирования
            }

            return allContacts;
        }

        /// <summary>
        /// метод для поиска контакта по запросу(по имени, фамилии, номеру телефона или электронной почте)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<PhoneBookContact> SearchContacts(string query)
        {
            // Удаляем лишние пробелы в начале и в конце строки запроса
            query = query.Trim();

            // Создаем список для хранения найденных контактов
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

        /// <summary>
        /// Для получение максимального Id из базы данных
        /// </summary>
        /// <returns></returns>
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
        // get
        public List<PhoneBookContact> GetContacts()
        {
            return contacts;
        }

        /// <summary>
        /// метод для вывода по Id 
        /// </summary>
        /// <param name="id"></param>
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

        /// <summary>
        /// метод для удаление контакта из базы данных
        /// </summary>
        /// <param name="id"></param>
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
                //Console.WriteLine($"Ошибка при удалении контакта: {ex.Message}");

                // Логирование ошибки
                Log.Error(ex, $"Ошибка при удалении контакта: {ex.Message}");
                throw; // Переопределение исключения после логирования
            }
        }

        /// <summary>
        /// метод для редактирование контакта 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        public void EditContact(int id, string firstName, string lastName, string phoneNumber, string email)
        {
            // Ищем контакт по ID в локальной коллекции
            var thiscontact = contacts.FirstOrDefault(c => c.Id == id);

            // Если контакт найден, обновляем его данные
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

                    // Ищем контакт в базе данных по ID
                    var contact = contactsCollection.FindById(id);

                    // Если контакт найден, обновляем его данные в базе данных
                    if (contact != null)
                    {
                        contact.Name = firstName;
                        contact.LastName = lastName;
                        contact.PhoneNumber = phoneNumber;
                        contact.Email = email;

                        // Обновляем запись в базе данных
                        contactsCollection.Update(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                // В случае возникновения исключения выводим сообщение об ошибке
                //Console.WriteLine($"Ошибка при редактировании контакта: {ex.Message}");

                // Логирование ошибки
                Log.Error(ex, $"Ошибка при редактировании контакта: {ex.Message}");
                throw; // Переопределение исключения после логирования
            }
        }
        /// <summary>
        /// возвращает контакт с заданным Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PhoneBookContact GetContactById(int id)
        {
            return contacts.FirstOrDefault(c => c.Id == id);
        }

        // метод get для получение количество котактов
        public int GetContactCount()
        {
            return contacts.Count;
        }

        /// <summary>
        /// Метод проверяет, существует ли контакт с заданным Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContactExists(int id)
        {
            return contacts.Any(c => c.Id == id);
        }

    }
}
