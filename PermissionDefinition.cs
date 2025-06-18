using PermissionManagement;
using System;
using System.Collections.Generic;

// 权限定义类
public class PermissionDefinition
{
    public string Code { get; set; }      // 权限代码（枚举名称）
    public string Name { get; set; }      // 权限显示名称
    public string Description { get; set; } // 权限描述
}

// 权限定义枚举
public enum Permission
{
    [PermissionInfo("用户管理", "管理用户账户")]
    UserManagement,

    [PermissionInfo("角色管理", "管理角色和权限")]
    RoleManagement,

    [PermissionInfo("权限配置", "配置系统权限")]
    PermissionManagement,

    [PermissionInfo("系统设置", "访问系统设置")]
    SystemSettings,

    [PermissionInfo("文件管理", "访问和管理文件")]
    FileAccess,
    [PermissionInfo("删除用户", "删除系统用户")]
    DeleteUser, // 新增权限项
}

// 权限信息特性
[AttributeUsage(AttributeTargets.Field)]
public class PermissionInfoAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public PermissionInfoAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

// 权限服务接口
public interface IPermissionService
{
    List<string> GetUserPermissions(int roleId);
}

// 权限服务实现
public class DatabasePermissionService : IPermissionService
{
    public List<string> GetUserPermissions(int roleId)
    {
        return DatabaseHelper.GetUserPermissions(roleId);
    }
}

// 用户类
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }

    public override string ToString()
    {
        return $"{Username}";
    }
}

// 角色类型枚举
public enum RoleType
{
    Administrator,  // 管理员
    Engineer,     // 工程师
    Operator      // 操作员
}

// 角色类
public class Role
{
    public int Id { get; set; }
    public RoleType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
