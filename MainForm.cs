using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PermissionManagement
{
    // 主窗体
    public partial class MainForm : Form
    {
        private readonly IPermissionService _permissionService;
        private string _username = "未登录";
        private int _currentUserRoleId = 0;

        // 存储菜单项与权限的映射
        private Dictionary<Permission, ToolStripMenuItem> _permissionMenuMap = new Dictionary<Permission, ToolStripMenuItem>();

        public MainForm()
        {
            _permissionService = new DatabasePermissionService();
            InitializeComponent();
            InitializeUI();
            ApplyPermissions(); // 初始化时应用权限（默认仅登录按钮可用）
        }

        private void InitializeUI()
        {
            // 设置窗体标题
            this.Text = "权限管理系统";
            this.WindowState = FormWindowState.Maximized;

            InitializeMenu();
        }

        // 初始化菜单
        // 获取权限信息
        private PermissionInfoAttribute GetPermissionInfo(Permission permission)
        {
            var field = typeof(Permission).GetField(permission.ToString());
            return field.GetCustomAttribute<PermissionInfoAttribute>();
        }

        private void InitializeMenu()
        {
            // 建立权限与菜单项的映射
            _permissionMenuMap[Permission.DeleteUser] = tsm_DeleteUser;
            _permissionMenuMap[Permission.FileAccess] = tsm_File;
            _permissionMenuMap[Permission.UserManagement] = tsm_UserManagement;
            _permissionMenuMap[Permission.RoleManagement] = tsm_RoleManagement;
            _permissionMenuMap[Permission.PermissionManagement] = tsm_PermissionManagement;
            _permissionMenuMap[Permission.SystemSettings] = tsm_SystemSettings;

            // 应用权限
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            if (_currentUserRoleId <= 0)
            {
                // 未登录，禁用除登录按钮外的所有菜单项
                foreach (var menuItem in _permissionMenuMap.Values)
                {
                    menuItem.Enabled = false;
                }
                return;
            }

            // 获取用户权限
            var permissions = DatabaseHelper.GetUserPermissions(_currentUserRoleId);

            // 启用/禁用菜单项
            foreach (var permission in Enum.GetValues(typeof(Permission)).Cast<Permission>())
            {
                if (_permissionMenuMap.TryGetValue(permission, out var menuItem))
                {
                    // 将 Permission 枚举值转换为字符串
                    var permissionString = permission.ToString();
                    menuItem.Enabled = permissions.Contains(permissionString);
                }
            }
        }

        // 菜单项点击事件 - 重构部分
        private void MenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var permission = (Permission)menuItem.Tag;

            // 根据权限执行相应操作
            switch (permission)
            {
                case Permission.UserManagement:
                    //ShowUserManagement();
                    break;
                case Permission.RoleManagement:
                    //ShowRoleManagement();
                    break;
                // 其他权限处理...
            }
        }

        private void permissionManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PermissionConfigForm configForm = new PermissionConfigForm())
            {
                configForm.ShowDialog();
            }
        }

        private void tsm_Login_Click(object sender, EventArgs e)
        {
            using (LoginForm frm = new LoginForm())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _currentUserRoleId = (int)frm.Tag; // 获取登录用户的角色ID
                    ApplyPermissions(); // 重新应用权限
                    _username = ((User)frm.cboUsername.SelectedItem).Username;
                    lblCurrentUser.Text = $"当前用户: {_username}";
                }
            }
        }

        private void tsm_DeleteUser_Click(object sender, EventArgs e)
        {
            using (DeleteUserForm form = new DeleteUserForm())
            {
                form.ShowDialog();
            }
        }
    }
}