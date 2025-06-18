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
        private int _roleId = 0; // 0表示未登录角色
        private List<string> _userPermissions = new List<string>();

        // 存储菜单项与权限的映射
        private Dictionary<Permission, ToolStripMenuItem> _permissionMenuMap = new Dictionary<Permission, ToolStripMenuItem>();
        private ToolStripButton _loginButton;
        private ToolStripButton _permissionConfigButton;
        private ToolStripStatusLabel _statusLabel;

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

            // 创建菜单栏
            MenuStrip mainMenuStrip = new MenuStrip();
            this.Controls.Add(mainMenuStrip);
            this.MainMenuStrip = mainMenuStrip;

            // 创建工具栏
            ToolStrip toolStrip = new ToolStrip();
            this.Controls.Add(toolStrip);

            // 添加登录按钮
            _loginButton = new ToolStripButton("登录");
            _loginButton.Click += BtnLogin_Click;
            toolStrip.Items.Add(_loginButton);

            // 添加权限配置按钮
            _permissionConfigButton = new ToolStripButton("权限配置");
            _permissionConfigButton.Click += PermissionConfigButton_Click;
            _permissionConfigButton.Enabled = false; // 默认禁用
            toolStrip.Items.Add(_permissionConfigButton);

            // 添加分隔符
            toolStrip.Items.Add(new ToolStripSeparator());

            // 添加状态栏
            StatusStrip statusStrip = new StatusStrip();
            this.Controls.Add(statusStrip);

            // 添加状态标签
            _statusLabel = new ToolStripStatusLabel($"当前用户: {_username}");
            statusStrip.Items.Add(_statusLabel);

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
            // 初始化基础菜单结构
            var userMenu = new ToolStripMenuItem("用户");
            mainMenuStrip.Items.Add(userMenu);

            var loginMenuItem = new ToolStripMenuItem("登录");
            loginMenuItem.Click += LoginMenuItem_Click;
            userMenu.DropDownItems.Add(loginMenuItem);

            // 新增删除用户菜单项
            var deleteUserMenuItem = new ToolStripMenuItem("删除用户");
            deleteUserMenuItem.Click += DeleteUserMenuItem_Click;
            userMenu.DropDownItems.Add(deleteUserMenuItem);

            // 其他菜单项...

            // 建立权限与菜单项的映射
            _permissionMenuMap[Permission.Login] = loginMenuItem;
            _permissionMenuMap[Permission.DeleteUser] = deleteUserMenuItem;

            // 应用权限
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            if (_currentUserRoleId <= 0) return;

            // 获取用户权限
            var permissions = DatabaseHelper.GetUserPermissions(_currentUserRoleId);

            // 启用/禁用菜单项
            foreach (var permission in Enum.GetValues(typeof(Permission)).Cast<Permission>())
            {
                if (_permissionMenuMap.TryGetValue(permission, out var menuItem))
                {
                    menuItem.Enabled = permissions.Contains(permission);
                }
            }
        }

        // 登录按钮点击事件
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (LoginForm frm = new LoginForm())
            {
                frm.ShowDialog();
            }
        }

        private void PermissionConfigButton_Click(object sender, EventArgs e)
        {
            using (PermissionConfigForm configForm = new PermissionConfigForm())
            {
                configForm.ShowDialog();
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
    }
}