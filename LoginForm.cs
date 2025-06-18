// 登录窗体
using PermissionManagement;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace PermissionManagement;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();

        // 在构造函数中调用 LoadUsers 方法
        LoadUsers();
    }

    // 加载所有用户到下拉框
    public void LoadUsers()
    {
        cboUsername.Items.Clear();

        // 从数据库获取所有普通用户
        var users = DatabaseHelper.GetAllUsers();

        foreach (var user in users)
        {
            cboUsername.Items.Add(user);
        }

        if (cboUsername.Items.Count > 0)
        {
            cboUsername.SelectedIndex = 0;
        }
    }

    // 登录按钮点击事件
    private void BtnLogin_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(cboUsername.Text))
        {
            MessageBox.Show("请选择用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrEmpty(txtPassword.Text))
        {
            MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int roleId;
        if (DatabaseHelper.ValidateUser(cboUsername.Text, txtPassword.Text, out roleId))
        {
            // 登录成功
            this.DialogResult = DialogResult.OK;
            this.Tag = roleId; // 存储角色ID
            this.Close();
        }
        else
        {
            MessageBox.Show("用户名或密码错误", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // 注册按钮点击事件
    private void BtnRegister_Click(object sender, EventArgs e)
    {
        using (var registerForm = new RegisterForm())
        {
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                // 注册成功，刷新用户列表
                LoadUsers();
                MessageBox.Show("注册成功，请使用新账号登录", "注册成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void cboUsername_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateRoleDisplay();
    }

    // 更新角色显示
    private void UpdateRoleDisplay()
    {
        if (cboUsername.SelectedItem is User user)
        {
            lbl_Role.Text = user.RoleName;
        }
        else
        {
            lbl_Role.Text = "";
        }
    }
}