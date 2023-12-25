using PhoneBookLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.txt");
            PhoneBook phoneBook = new PhoneBook(projectPath);
            while (true)
            {
                Console.WriteLine("1. Показать все контакты");
                Console.WriteLine("2. найти контакт");
                Console.WriteLine("3. Добавить контакт");
                Console.WriteLine("4. Редактирование контакта");
                Console.WriteLine("5. Удаление контакта");
                Console.WriteLine("0. Выход");

                int showChoice;
                if (!int.TryParse(Console.ReadLine(), out showChoice))
                {
                    Console.WriteLine("Неверный ввод, введите число.");
                    continue;
                }

                switch (showChoice)
                {
                    case 1:
                        // Вывод всех контактов
                        List<PhoneBookContact> allContacts = phoneBook.GetAllContacts();

                        foreach (var contact in allContacts)
                        {
                            Console.WriteLine(contact.ToString());
                        }
                        Console.WriteLine("");
                        break;
                    case 2:
                        Console.WriteLine("Введите запрос для поиска(по имени, фамилии, номеру телефона или электронной почте): ");
                        string searchQuery = Console.ReadLine();
                        phoneBook.SearchContacts(searchQuery);
                        Console.WriteLine("");
                        break;
                    case 3:
                        // Добавление контакта
                        Console.WriteLine("Введите имя контакта:");
                        string firstName = Console.ReadLine();
                        Console.WriteLine("Введите фамилию контакта:");
                        string lastName = Console.ReadLine();
                        Console.WriteLine("Введите номер телефона:");
                        string phoneNumber = Console.ReadLine();
                        Console.WriteLine("Введите электронную почту:");
                        string email = Console.ReadLine();
                        phoneBook.AddContact(firstName, lastName, phoneNumber, email);
                        Console.WriteLine("Контакт добавлен.");
                        Console.WriteLine("");
                        break;
                    case 4:
                        // Редактирование контакта
                        Console.WriteLine("Введите ID контакта для редактирования:");
                        int idToEdit;
                        if (!int.TryParse(Console.ReadLine(), out idToEdit))
                        {
                            Console.WriteLine("Неверный ввод. Пожалуйста, введите число.");
                            continue;
                        }

                        if (phoneBook.ContactExists(idToEdit))
                        {
                            phoneBook.GetShowbyId(idToEdit);
                            var buf = phoneBook.GetContactById(idToEdit);
                            string newFirstName = buf.Name;
                            string newLastName = buf.LastName;
                            string newPhoneNumber = buf.PhoneNumber;
                            string newEmail = buf.Email;
                            bool b = true;
                            while (b)
                            {
                                Console.WriteLine("Что вы хотите отредактировать:");
                                Console.WriteLine("1. Имя");
                                Console.WriteLine("2. Фамилия");
                                Console.WriteLine("2. Номер телефона");
                                Console.WriteLine("4. Электронную почту");
                                Console.WriteLine("0. Отредактировать");

                                int show;
                                if (!int.TryParse(Console.ReadLine(), out show))
                                {
                                    Console.WriteLine("Неверный ввод, введите число.");
                                    continue;
                                }
                               

                                switch (show)
                                {
                                    case 1:
                                        Console.WriteLine("Введите новое имя:");
                                        newFirstName = Console.ReadLine();
                                        break;
                                    case 2:
                                        Console.WriteLine("Введите новую фамилию:");
                                        newLastName = Console.ReadLine();
                                        break;
                                    case 3:
                                        Console.WriteLine("Введите новый номер телефона:");
                                        newPhoneNumber = Console.ReadLine();
                                        break;
                                    case 4:
                                        Console.WriteLine("Введите новую электронную почту:");
                                        newEmail = Console.ReadLine();
                                        break;
                                    case 0:
                                        phoneBook.EditContact(idToEdit, newFirstName, newLastName, newPhoneNumber, newEmail);
                                        Console.WriteLine("Контакт отредактирован.");
                                        Console.WriteLine("");
                                        b = false;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Контакт с таким ID не найден.");
                        }
                        break;
                    case 5:
                        // Удаление контакта
                        Console.WriteLine("Введите ID контакта для удаления:");
                        int idToDelete;
                        if (!int.TryParse(Console.ReadLine(), out idToDelete))
                        {
                            Console.WriteLine("Неверный ввод. Пожалуйста, введите число.");
                            continue;
                        }

                        if (phoneBook.ContactExists(idToDelete))
                        {
                            phoneBook.DeleteContact(idToDelete);
                            Console.WriteLine("Контакт удален.");
                            Console.WriteLine("");
                        }
                        else
                        {
                            Console.WriteLine("Контакт с таким ID не найден.");
                        }
                        break;
                    case 0:
                        Console.WriteLine("Путь к базе данных: " + projectPath);
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
