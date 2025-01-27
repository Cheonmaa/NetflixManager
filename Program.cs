using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace UseBDD {

    //Interface that's used in order to not repeat code and duplicate things. It's also used for Inheritance and as a contract
    interface IUserSettingBdd {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Role { get; set; }
        void DisplayMenu();

    }

    // Abstract class using the Interface providing a base implementation for user properties and enforces subclasses to implement the DisplayMenu method
    public abstract class BaseUser : IUserSettingBdd {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public BaseUser(string name, string email, string password, string role) {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
        public abstract void DisplayMenu();
    }

    // Static class that is used for Global functions. It's in order to not repeat and duplicate functions in my classes (User, Modo, Admin).
    public static class Global {
        //Delete a user with a "userId" provided.
        public static string DeleteUser(int userId) {
            using var _context = new ProjectContext();
            //Expression lambda used to check if there is userId in the table Users, if yes continue, if no Default which is null.
            var userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userEntity != null) {
                if (userEntity.Role != "Admin") {
                    _context.Users.Remove(userEntity);
                    _context.SaveChanges();
                    return $"{userEntity.Name} deleted successfully.";
                } else {
                    return "You cannot delete this user since he's an Admin.";
                }
            } else {
                return "User not found / Unknown.";
            }
        }

        //Modify a user with "userId" provided. 
        public static string ModifyUser(int userId) {
            using var _context = new ProjectContext();
            //Expression lambda used to check if there is userId in the table Users, if yes continue, if no Default which is null.
            var userEntity = _context.Users.FirstOrDefault(u => u.Id == userId); 
            string newName;
            if (userEntity == null)
                Console.WriteLine("Erreur, user null");
            do {
                Console.WriteLine("Enter new Name : ");
                newName = Console.ReadLine();
            } while (newName == null || !Regex.IsMatch(newName, "^[a-zA-Z]+$"));
            userEntity.Name = newName;
            _context.SaveChanges();
            return userEntity.Name;
        }

        //Modify user role with a provided userId
        public static string ModifyRoleUser(int userId) {
            using var _context = new ProjectContext();
            //Expression lambda used to check if there is userId in the table Users, if yes continue, if no Default which is null.
            var userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            string newRole;
            if (userEntity == null)
                Console.WriteLine("Erreur, user null");
            do {
                Console.WriteLine("Enter new Role name (Modo or Admin) : ");
                newRole = Console.ReadLine();
            } while (newRole == null && newRole != "Modo" && newRole != "Admin");
            userEntity.Role = newRole;
            _context.SaveChanges();
            return userEntity.Role;
        }

        //Method that list the users
        public static void ListUsers() {
            Console.WriteLine("User list : ");
            using var _context = new ProjectContext();
            var users = _context.Users;
            foreach (var u in users.ToList()) {
                Console.WriteLine($"Id : {u.Id} | Name : {u.Name} | Role : {u.Role}");
            }
        }
        
        //Method that list the Medias (Movie / Series)
        public static void ListMedias() {
            Console.WriteLine("Media list : ");
            using var _context = new ProjectContext();
            var medias = _context.Media;
            foreach (var m in medias.ToList()) {
                Console.WriteLine($"Id : {m.Id} | Title : {m.Title} | Type : {m.Type} | Overview : {m.Overview} | Rating : {m.Rating}");
            }
        }

        //Delete Medias with a "movieId" provided.
        public static string DeleteMovieOrSerie(int movieId) {
            using var _context = new ProjectContext();
            //Expression lambda used to check if there is userId in the table Users, if yes continue, if no Default which is null.
            var mediaEntity = _context.Media.FirstOrDefault(m => m.Id == movieId);
            if (mediaEntity != null) {
                _context.Media.Remove(mediaEntity);
                _context.SaveChanges();
                return $"{mediaEntity.Title} deleted successfully.";
            } else {
                return "You cannot delete this user since he's an Admin.";
            }
        }

        public static void AddMovie() {
            var import = new ImportCsvInBdd();
            import.AddMediaToCsv();
        }

        public static void AddCsvFile() {
            var import = new ImportCsvInBdd();
            string path = Console.ReadLine();
            import.ImportUserCSV(path);
        }
    }

    //Class getting abstract class as Parent to get the base and not repeat the attributes. It implements the functions as well but it defers since it's abastraction.
    public class Admin : BaseUser {
        public Admin(string name, string email, string password, string role) : base(name, email, password, role) { }
        //Function that display a menu with input from users. Depending on your role you got more or less.
        public override void DisplayMenu() {
            while (true) {
                Console.WriteLine("\nAdmin menu :");
                Console.WriteLine("1 - List | 2 - Add a Media | 3 - Delete | 4 - Modify a User | 5 - Add Csv File | 6 - Log off ");
                Console.Write("Choose an option : ");
                var choice = Console.ReadLine();
                switch (choice) {
                    case "1":
                        Console.WriteLine("1 - Users | 2 - Media");
                        var toList = Console.ReadLine();
                        switch (toList) {
                            case "1":
                                Global.ListUsers();
                                break;
                            case "2":
                                Global.ListMedias();
                                break;
                        }
                        break;
                    case "2":
                        Global.AddMovie();
                        break;
                    case "3":
                        Console.WriteLine("1 - Users | 2 - Media");
                        var toDelete = Console.ReadLine();
                        switch (toDelete) {
                            case "1":
                                Console.Write($"User Id to delete : ");
                                int userIdToDelete = Convert.ToInt32(Console.ReadLine());
                                Global.DeleteUser(userIdToDelete);
                                break;
                            case "2":
                                Console.Write($"Media Id to delete : ");
                                int mediaIdToDelete = Convert.ToInt32(Console.ReadLine());
                                Global.DeleteUser(mediaIdToDelete);
                                break;
                        }
                        break;
                    case "4":
                        Console.WriteLine("1 - Name | 2 - Role");
                        var modifying = Console.ReadLine();
                        switch (modifying) {
                            case "1":
                                Console.Write($"User Id to modify : ");
                                int userToModify = Convert.ToInt32(Console.ReadLine());
                                Global.ModifyUser(userToModify);
                                break;
                            case "2":
                                Console.Write($"User Id to modify : ");
                                int userRoleModify = Convert.ToInt32(Console.ReadLine());
                                Global.ModifyRoleUser(userRoleModify);
                                break;
                        }
                        break;
                    case "5":
                        Console.WriteLine("If Imports fail. Check the README.");
                        Console.WriteLine("Path of your file : (Example : C:\\TEMP\\C#better)");
                        Global.AddCsvFile();
                        break;
                    case "6":
                        Console.WriteLine("Logging off...");
                        return;
                    default:
                        Console.WriteLine("Invalid. Retry");
                        break;
                }
            }
        }
    }

    //Class getting abstract class as Parent to get the base and not repeat the attributes. It implements the functions as well but it defers since it's abastraction.
    public class Modo : BaseUser {
        public Modo(string name, string email, string password, string role) : base(name, email, password, role) { }
        //Function that display a menu with input from users. Depending on your role you got more or less.
        public override void DisplayMenu() {
            while (true) {
                Console.WriteLine("\nMenu Modo:");
                Console.WriteLine("1 - List | 2 - Add a Media | 3 - Delete a Media | 4 - Modify a User | 5 - Add Csv File | 6 - Log off ");
                Console.Write("Choose an option : ");
                var choice = Console.ReadLine();
                switch (choice) {
                    case "1":
                        Console.WriteLine("1 - Users | 2 - Media");
                        var toList = Console.ReadLine();
                        switch (toList) {
                            case "1":
                                Global.ListUsers();
                                break;
                            case "2":
                                Global.ListMedias();
                                break;
                        }
                        break;
                    case "2":
                        Global.AddMovie();
                        break;
                    case "3":
                        Console.WriteLine("Media Id to delete : ");
                        int movieId = Convert.ToInt32(Console.ReadLine());
                        Global.DeleteMovieOrSerie(movieId);
                        break;
                    case "4":
                        Console.Write($"User Id to modify : ");
                        int userToModify = Convert.ToInt32(Console.ReadLine());
                        Global.ModifyUser(userToModify);
                        break;
                    case "5":
                        Console.WriteLine("If Imports fail. Check the README.");
                        Console.WriteLine("Path of your file : (Example : C:\\TEMP\\C#better)");
                        Global.AddCsvFile();
                        break;
                    case "6":
                        Console.WriteLine("Logging off...");
                        return;
                    default:
                        Console.WriteLine("Invalid. Retry.");
                        break;
                }
            }
        }
    }

    //Class getting abstract class as Parent to get the base and not repeat the attributes. It implements the functions as well but it defers since it's abastraction.
    public class User : BaseUser {
        public User(string name, string email, string password, string role) : base(name, email, password, role) { }
        //Function that display a menu with input from users. Depending on your role you got more or less.
        public override void DisplayMenu() {
            while (true) {
                Console.WriteLine("\nMenu User:");
                Console.WriteLine("1 - List | 2 - Add a Media | 3 - Delete a Media | 4 - Add Csv File | 5 - Log off ");
                Console.Write("Choose an option : ");
                var choice = Console.ReadLine();
                switch (choice) {
                    case "1":
                        Console.WriteLine("1 - Users | 2 - Media");
                        var toList = Console.ReadLine();
                        switch (toList) {
                            case "1":
                                Global.ListUsers();
                                break;
                            case "2":
                                Global.ListMedias();
                                break;
                        }
                        break;
                    case "2":
                        Global.AddMovie();
                        break;
                    case "3":
                        Console.WriteLine("Media Id to delete : ");
                        int movieId = Convert.ToInt32(Console.ReadLine());
                        Global.DeleteMovieOrSerie(movieId);
                        break;
                    case "4":
                        Console.WriteLine("If Imports fail. Check the README.");
                        Console.WriteLine("Path of your file : (Example : C:\\TEMP\\C#better)");
                        Global.AddCsvFile();
                        break;
                    case "5":
                        Console.WriteLine("Logging off...");
                        return;
                    default:
                        Console.WriteLine("Invalid. Retry.");
                        break;
                }
            }
        }
    }
    
    //Class that is used for Login and Create functions
    public class LogOrCreate() {
        //Create an account based on the users input.
        public void CreateAccount() {
            Console.WriteLine("Wanna create an account ? (O/N)");
            string answer = Console.ReadLine();
            if (answer == "O") {
                string username;
                string email;
                string password;
                string role;
                do {
                    Console.Write("Username (Example -> CsharpBetter): ");
                    username = Console.ReadLine();
                } while (string.IsNullOrEmpty(username) || !Regex.IsMatch(username, "^[a-zA-Z]+$"));
                do {
                    Console.Write("Email (example1@gmail.com): ");
                    email = Console.ReadLine();
                } while (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"));
                do {
                    Console.Write("Password (5 char minimum): ");
                    password = Console.ReadLine();
                } while (string.IsNullOrEmpty(password) || password.Length < 5);
                role = (password == "jesuispuissant") ? "Admin" : "User";
                using var context = new ProjectContext();
                var newUser = new User(username, email, password, role);
                context.Users.Add(newUser);
                context.SaveChanges();
                Console.WriteLine("Account created. You may log");
            } else if (answer == "N") {
                Console.WriteLine("Fail");
            } else {
                Console.WriteLine("Invalid. Answer 'O' or 'N'.");
            }
        }

        //Check the database and log if users input is correct.
        public User Login() {
            var _context = new ProjectContext();
            int count = 0;
            User user = null;
            while (count < 5) {
                Console.Write("Username : ");
                string username = Console.ReadLine();
                user = _context.Users.FirstOrDefault(u => u.Name == username);
                if (user == null) {
                    Console.WriteLine("User not found. Retry.");
                    count++;
                    continue;
                }
                Console.Write("Password : ");
                string password = Console.ReadLine();
                if (user.Password != password) {
                    Console.WriteLine("Password incorrect. Retry.");
                    count++;
                } else {
                    Console.WriteLine("Login success.");
                    return user;
                }
            }
            return null;
        }

    }

    //Class for DB Table
    public class Media {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Overview { get; set; }
        public double Rating { get; set; }

    }

    //Class that import Csv, read it, and grant the right to add media to a csv
    public class ImportCsvInBdd {
        private readonly string filePathToCsv;

        //Get the directory of the Repo (Here it's NetflixManager)
        public ImportCsvInBdd() {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            filePathToCsv = Path.Combine(projectDirectory, "FilmSerie.csv");
        }

        //Function that read and import from a csv file to DB
        public void ReadAndImport() {
            if (!File.Exists(filePathToCsv)) Console.WriteLine($"File not found : {filePathToCsv}");
            try {
                var mediaList = File.ReadAllLines(filePathToCsv).Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => {
                    var columns = line.Split(',');
                    if (columns.Length < 3) Console.WriteLine($"Line incorrect : {line}");
                    return new Media {
                        Id = int.Parse(columns[0]),
                        Title = columns[1],
                        Type = columns[2],
                        Overview = columns.Length > 3 ? columns[3] : string.Empty,
                        Rating = columns.Length > 4 && double.TryParse(columns[4], out double rating) ? rating : 0.0
                    };
                }).Where(media => media != null).ToList();
                using var context = new ProjectContext();
                context.Database.EnsureCreated();
                foreach (var media in mediaList) {
                    var existingMedia = context.Media.FirstOrDefault(m => m.Title == media.Title && m.Type == media.Type);
                    if (existingMedia == null) context.Media.Add(media);
                }
                context.SaveChanges();
                Console.WriteLine("Importation success");
            } catch (Exception ex) { Console.WriteLine($"Error during importation : {ex.Message}"); }
        }

        //Function that read and import a Csv file from the user path (path of his own pc).
        public void ImportUserCSV(string path) {
            if (!File.Exists(path)) Console.WriteLine($"File not found: {path}");
            try {
                using var context = new ProjectContext();
                var mediaList = new List<Media>();
                var lines = File.ReadAllLines(path);
                foreach (var line in lines) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var columns = line.Split(',');
                    if (columns.Length < 2) { Console.WriteLine($"Invalid line: {line}"); continue; }
                    var title = columns[0].Trim();
                    if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine($"Invalid title in line: {line}"); continue; }
                    var type = columns[1].Trim();
                    if (string.IsNullOrWhiteSpace(type) || !(type == "Movie" || type == "Serie")) { Console.WriteLine($"Invalid type in line: {line}"); continue; }
                    mediaList.Add(new Media {
                        Title = title,
                        Type = type,
                        Overview = columns.Length > 2 ? columns[2].Trim() : string.Empty,
                        Rating = columns.Length > 3 && double.TryParse(columns[3], out double rating) ? rating : 0.0
                    });
                }
                context.Database.EnsureCreated();
                int addedCount = 0;
                foreach (var media in mediaList) {
                    var existingMedia = context.Media.FirstOrDefault(m => m.Title == media.Title && m.Type == media.Type);
                    if (existingMedia == null) {
                        Console.WriteLine($"Adding media: {media.Title} ({media.Type})");
                        context.Media.Add(media);
                        addedCount++;
                    }
                }
                if (addedCount > 0) {
                    context.SaveChanges();
                    Console.WriteLine("Importation successful!");
                } else {
                    Console.WriteLine("No new media was added to the database.");
                }
            } catch (Exception ex) { Console.WriteLine($"Error during importation: {ex.Message}"); }
        }

        //Function that will add a media in the CSV of the project named FilmSerie
        public void AddMediaToCsv() {
            if (!File.Exists(filePathToCsv)) {
                Console.WriteLine($"File : {filePathToCsv} can't be find.");
                return;
            }
            try {
                Console.WriteLine("Adding new Media to CSV file.");
                string title;
                do {
                    Console.Write("Title : ");
                    title = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(title));
                string type;
                do {
                    Console.Write("Type (Movie/Series) : ");
                    type = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(type) || !(type.Equals("Movie", StringComparison.OrdinalIgnoreCase) || type.Equals("Series", StringComparison.OrdinalIgnoreCase)));
                string overview;
                do {
                    Console.Write("Overview : ");
                    overview = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(overview));
                double rating;
                do {
                    Console.Write("Rating (0.0 to 10.0) : ");
                } while (!double.TryParse(Console.ReadLine(), out rating) || rating < 0.0 || rating > 10.0);
                var _context = new ProjectContext();
                int newMediaId = _context.Media.Any() ? _context.Media.Max(m => m.Id) + 1 : 1;
                var newMediaLine = $"{newMediaId},{title},{type},{overview},{rating}";
                File.AppendAllText(filePathToCsv, $"{Environment.NewLine}{newMediaLine}");
                Console.WriteLine($"The media '{title}' has been add to the CSV file.");
            } catch (Exception ex) {
                Console.WriteLine($"Error while adding the file : {ex.Message}");
            }
            ReadAndImport();
        }
    }

    //Class that will create the path of a folder that'll create the Database and get access on it
    public class ProjectContext : DbContext {
        private readonly string filePathToDB;
        public ProjectContext() {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string tempFolder = Path.Combine(projectDirectory, "TEMP");
            if (!Directory.Exists(tempFolder)) {
                Directory.CreateDirectory(tempFolder);
            }
            filePathToDB = Path.Combine(tempFolder, "NetflixManager.db");
        }
        public DbSet<User> Users { get; set; } // Table "Users"
        public DbSet<Media> Media { get; set; } // Table "Media"
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source={filePathToDB}");
        }
    }

    //Class that'll start the program with the login / create
    public class Starting {
        public User StartProgram() {
            var accessToBdd = new LogOrCreate();
            User user = null;
            while (true) {
                Console.WriteLine("Welcome to NetflixManager. 1 - Logging | 2 - Create account | 3 - Quit ");
                string answer = Console.ReadLine();
                if (answer == "1") {
                    user = accessToBdd.Login();
                    if (user == null) {
                        Console.WriteLine("Too many failed attempts. Returning to start program...");
                        continue;
                    }
                    return user;
                } else if (answer == "2") {
                    accessToBdd.CreateAccount();
                    continue;
                } else if (answer == "3") {
                    break;
                } else {
                    Console.WriteLine("Please, answer correctly 1 or 2.");
                }
                continue;
            }
            return null;
        }
    }

    //Class that'll be used right after the login. Displaying menu for the user.Role (for example User role will get display Menu of it's class ect)
    public class During {
        public void WhileLogged() {
            var start = new Starting();
            var user = start.StartProgram();
            if (user == null) {
                Console.WriteLine("No user logged in. Exiting program...");
                return;
            }
            while (true) {
                switch (user.Role) {
                    case "Admin":
                        var admin = new Admin(user.Name, user.Email, user.Password, user.Role);
                        admin.DisplayMenu();
                        break;
                    case "Modo":
                        var modo = new Modo(user.Name, user.Email, user.Password, user.Role);
                        modo.DisplayMenu();
                        break;
                    case "User":
                        var regularUser = new User(user.Name, user.Email, user.Password, user.Role);
                        regularUser.DisplayMenu();
                        break;
                    default:
                        Console.WriteLine("Role unknown. Cannot display the menu.");
                        break;
                }
                Console.WriteLine("\nWould you like to continue ? (O/N)");
                var response = Console.ReadLine();
                if (response?.ToUpper() == "N") {
                    Console.WriteLine("Thanks for using NetflixManager. Cya !");
                    break;
                } else {
                    WhileLogged();
                }
            }
        }
    }

    //Main class that is used as the .exe
    public class Program {
        static void Main(string[] args) {
            using var context = new ProjectContext();
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully.");
            var import = new ImportCsvInBdd();
            import.ReadAndImport();
            var during = new During();
            during.WhileLogged();
        }
    }
}
