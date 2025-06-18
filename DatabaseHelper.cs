// 数据库助手类 - 修复SQLiteCommand构造函数和LastInsertRowId问题
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System;
using System.IO;

public static class DatabaseHelper
{
    private static string connectionString = "Data Source=Permissions.db;Version=3;";

    // 初始化权限集合
    public static readonly List<PermissionDefinition> AllPermissions = InitializePermissions();

    private static List<PermissionDefinition> InitializePermissions()
    {
        var permissions = new List<PermissionDefinition>();

        // 通过反射获取所有权限枚举值及其特性
        foreach (var field in typeof(Permission).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<PermissionInfoAttribute>();
            if (attribute != null)
            {
                permissions.Add(new PermissionDefinition
                {
                    Code = field.Name,
                    Name = attribute.Name,
                    Description = attribute.Description
                });
            }
        }

        return permissions;
    }

    static DatabaseHelper()
    {
        try
        {
            CreateDatabase();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"数据库初始化失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    private static void CreateDatabase()
    {
        if (!File.Exists("Permissions.db"))
        {
            SQLiteConnection.CreateFile("Permissions.db");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        CreateTables(connection, transaction);
                        InsertInitialData(connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"数据库创建失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }
        }
    }

    private static void CreateTables(SQLiteConnection connection, SQLiteTransaction transaction)
    {
        // 创建角色表 - 使用Type列存储角色类型
        using (var command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Roles (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Type INTEGER NOT NULL,
        Name TEXT NOT NULL UNIQUE,
        Description TEXT
    )", connection, transaction))
        {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Permissions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Code TEXT NOT NULL UNIQUE,
            Name TEXT NOT NULL,
            Description TEXT
        )", connection, transaction))
        {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS RolePermissions (
            RoleId INTEGER NOT NULL,
            PermissionId INTEGER NOT NULL,
            PRIMARY KEY (RoleId, PermissionId),
            FOREIGN KEY (RoleId) REFERENCES Roles(Id),
            FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
        )", connection, transaction))
        {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            RoleId INTEGER NOT NULL,
            IsActive BOOLEAN DEFAULT 1,
            CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (RoleId) REFERENCES Roles(Id)
        )", connection, transaction))
        {
            command.ExecuteNonQuery();
        }
    }

    private static void InsertInitialData(SQLiteConnection connection, SQLiteTransaction transaction)
    {
        using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Roles", connection, transaction))
        {
            int roleCount = Convert.ToInt32(command.ExecuteScalar());

            if (roleCount == 0)
            {
                // 插入三种角色
                int adminRoleId = InsertRole(connection, transaction, RoleType.Administrator, "管理员", "系统管理员，拥有所有权限");
                int engineerRoleId = InsertRole(connection, transaction, RoleType.Engineer, "工程师", "系统工程师，拥有部分管理权限");
                int operatorRoleId = InsertRole(connection, transaction, RoleType.Operator, "操作员", "系统操作员，拥有基本操作权限");

                // 为管理员分配全部权限
                Dictionary<string, int> permissionIdMap = new Dictionary<string, int>();

                foreach (var permission in AllPermissions)
                {
                    int permissionId = InsertPermission(connection, transaction, permission.Code, permission.Name, permission.Description);
                    permissionIdMap[permission.Code] = permissionId;

                    // 为管理员分配所有权限
                    AssignPermissionToRole(connection, transaction, adminRoleId, permissionId);
                }

                // 为工程师分配部分权限
                if (permissionIdMap.ContainsKey("UserManagement"))
                {
                    AssignPermissionToRole(connection, transaction, engineerRoleId, permissionIdMap["UserManagement"]);
                }
                if (permissionIdMap.ContainsKey("FileAccess"))
                {
                    AssignPermissionToRole(connection, transaction, engineerRoleId, permissionIdMap["FileAccess"]);
                }

                // 为操作员分配基本权限
                if (permissionIdMap.ContainsKey("FileAccess"))
                {
                    AssignPermissionToRole(connection, transaction, operatorRoleId, permissionIdMap["FileAccess"]);
                }

                // 插入测试用户
                InsertUser(connection, transaction, "admin", "admin123", adminRoleId);
                InsertUser(connection, transaction, "engineer", "engineer123", engineerRoleId);
                InsertUser(connection, transaction, "operator", "operator123", operatorRoleId);
            }
        }
    }

    private static int InsertRole(SQLiteConnection connection, SQLiteTransaction transaction, RoleType type, string name, string description)
    {
        using (var command = new SQLiteCommand("INSERT INTO Roles (Type, Name, Description) VALUES (@Type, @Name, @Description)", connection, transaction))
        {
            command.Parameters.AddWithValue("@Type", (int)type);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Description", description);
            command.ExecuteNonQuery();

            return Convert.ToInt32(connection.LastInsertRowId);
        }
    }

    private static int InsertPermission(SQLiteConnection connection, SQLiteTransaction transaction, string code, string name, string description)
    {
        using (var command = new SQLiteCommand("INSERT INTO Permissions (Code, Name, Description) VALUES (@Code, @Name, @Description)", connection, transaction))
        {
            command.Parameters.AddWithValue("@Code", code);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Description", description);
            command.ExecuteNonQuery();

            // 修复：使用connection.LastInsertRowId而不是command.LastInsertRowId
            return Convert.ToInt32(connection.LastInsertRowId);
        }
    }

    private static void AssignPermissionToRole(SQLiteConnection connection, SQLiteTransaction transaction, int roleId, int permissionId)
    {
        using (var command = new SQLiteCommand("INSERT OR IGNORE INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)", connection, transaction))
        {
            command.Parameters.AddWithValue("@RoleId", roleId);
            command.Parameters.AddWithValue("@PermissionId", permissionId);
            command.ExecuteNonQuery();
        }
    }

    private static void InsertUser(SQLiteConnection connection, SQLiteTransaction transaction, string username, string password, int roleId)
    {
        using (var command = new SQLiteCommand("INSERT INTO Users (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)", connection, transaction))
        {
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@RoleId", roleId);
            command.ExecuteNonQuery();
        }
    }

    public static bool ValidateUser(string username, string password, out int roleId)
    {
        roleId = 0;
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT RoleId FROM Users WHERE Username = @Username AND Password = @Password AND IsActive = 1";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                // 调试输出：确认传入的参数
                Console.WriteLine($"Username: {username}, Password: {password}");

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        roleId = reader.GetInt32(0);
                        return true;
                    }
                    else
                    {
                        // 调试输出：确认查询无结果
                        Console.WriteLine("No matching user found.");
                    }
                }
            }
        }
        return false;
    }

    public static DataTable GetAllPermissions()
    {
        var dt = new DataTable();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Permissions ORDER BY Id";

            using (var adapter = new SQLiteDataAdapter(query, connection))
            {
                adapter.Fill(dt);
            }
        }
        return dt;
    }
    public static DataTable GetRolePermissions(int roleId)
    {
        var dt = new DataTable();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = @"SELECT p.* 
                            FROM Permissions p 
                            JOIN RolePermissions rp ON p.Id = rp.PermissionId 
                            WHERE rp.RoleId = @RoleId";

            using (var adapter = new SQLiteDataAdapter(query, connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@RoleId", roleId);
                adapter.Fill(dt);
            }
        }
        return dt;
    }

    public static void UpdateRolePermissions(int roleId, List<string> permissionCodes)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 先删除该角色所有权限
                    using (var deleteCommand = new SQLiteCommand("DELETE FROM RolePermissions WHERE RoleId = @RoleId", connection, transaction))
                    {
                        deleteCommand.Parameters.AddWithValue("@RoleId", roleId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // 再添加新分配的权限
                    foreach (var code in permissionCodes)
                    {
                        // 获取权限ID
                        using (var selectCommand = new SQLiteCommand("SELECT Id FROM Permissions WHERE Code = @Code", connection, transaction))
                        {
                            selectCommand.Parameters.AddWithValue("@Code", code);
                            var permissionId = selectCommand.ExecuteScalar();

                            if (permissionId != null)
                            {
                                using (var insertCommand = new SQLiteCommand("INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)", connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@RoleId", roleId);
                                    insertCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"权限更新失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }
    }

    public static List<string> GetUserPermissions(int roleId)
    {
        var permissions = new List<string>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = @"SELECT p.Code 
                            FROM Permissions p 
                            JOIN RolePermissions rp ON p.Id = rp.PermissionId 
                            WHERE rp.RoleId = @RoleId";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RoleId", roleId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(reader.GetString(0));
                    }
                }
            }
        }
        return permissions;
    }

    // 获取所有用户
    public static List<User> GetAllUsers()
    {
        var users = new List<User>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT u.*, r.Name AS RoleName FROM Users u JOIN Roles r ON u.RoleId = r.Id ORDER BY u.Id";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                            RoleName = reader.GetString(reader.GetOrdinal("RoleName"))
                        });
                    }
                }
            }
        }
        return users;
    }

    // 注册新用户
    public static void RegisterUser(string username, string password, int roleId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // 检查用户名是否已存在
            using (var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection))
            {
                checkCommand.Parameters.AddWithValue("@Username", username);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count > 0)
                {
                    throw new Exception("用户名已存在");
                }
            }

            // 插入新用户
            using (var command = new SQLiteCommand("INSERT INTO Users (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)", connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@RoleId", roleId);
                command.ExecuteNonQuery();
            }
        }
    }

    // 获取所有角色
    public static List<Role> GetAllRoles()
    {
        var roles = new List<Role>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Roles ORDER BY Id";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    roles.Add(new Role
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Type = (RoleType)reader.GetInt32(reader.GetOrdinal("Type")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description"))
                    });
                }
            }
        }
        return roles;
    }

    // 根据ID获取角色
    public static Role GetRoleById(int roleId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Roles WHERE Id = @Id";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", roleId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Role
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Type = (RoleType)reader.GetInt32(reader.GetOrdinal("Type")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description"))
                        };
                    }
                }
            }
        }
        return null;
    }

    // 添加角色
    public static int AddRole(RoleType type, string name, string description)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SQLiteCommand("INSERT INTO Roles (Type, Name, Description) VALUES (@Type, @Name, @Description)", connection))
            {
                command.Parameters.AddWithValue("@Type", (int)type);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);

                command.ExecuteNonQuery();

                return (int)connection.LastInsertRowId;
            }
        }
    }

    // 更新角色
    public static void UpdateRole(int roleId, RoleType type, string name, string description)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SQLiteCommand("UPDATE Roles SET Type = @Type, Name = @Name, Description = @Description WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Type", (int)type);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Id", roleId);

                command.ExecuteNonQuery();
            }
        }
    }

    // 删除角色
    public static void DeleteRole(int roleId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // 先删除角色权限关联
            using (var command1 = new SQLiteCommand("DELETE FROM RolePermissions WHERE RoleId = @RoleId", connection))
            {
                command1.Parameters.AddWithValue("@RoleId", roleId);
                command1.ExecuteNonQuery();
            }

            // 再删除角色
            using (var command2 = new SQLiteCommand("DELETE FROM Roles WHERE Id = @Id", connection))
            {
                command2.Parameters.AddWithValue("@Id", roleId);
                command2.ExecuteNonQuery();
            }
        }
    }

    // 初始化角色数据
    private static void InitializeRoles(SQLiteConnection connection, SQLiteTransaction transaction)
    {
        // 添加三种基本角色
        AddRole(RoleType.Administrator, "系统管理员", "拥有系统所有权限");
        AddRole(RoleType.Engineer, "工程师", "拥有设备管理和操作权限");
        AddRole(RoleType.Operator, "操作员", "拥有基本的操作权限");
    }
}
