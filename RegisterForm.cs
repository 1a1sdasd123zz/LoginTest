// 注册窗体
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Data;
using System.Linq;

namespace PermissionManagement;
public partial class RegisterForm : Form
{
    private TextBox txtUsername;
    private TextBox txtPassword;
    private TextBox txtConfirmPassword;
    private ComboBox cboRole;
    private Button btnRegister;
    private Button btnCancel;

    public RegisterForm()
    {
        InitializeComponent();
        this.Text = "用户注册";
        this.Size = new Size(350, 250);
        this.StartPosition = FormStartPosition.CenterScreen;

        InitializeUI();
    }

    private void InitializeUI()
    {
        // 创建控件
        var lblUsername = new Label { Text = "用户名:", Location = new Point(20, 20) };
        txtUsername = new TextBox { Location = new Point(120, 20), Width = 180 };

        var lblPassword = new Label { Text = "密码:", Location = new Point(20, 50) };
        txtPassword = new TextBox { Location = new Point(120, 50), Width = 180, PasswordChar = '*' };

        var lblConfirmPassword = new Label { Text = "确认密码:", Location = new Point(20, 80) };
        txtConfirmPassword = new TextBox { Location = new Point(120, 80), Width = 180, PasswordChar = '*' };

        var lblRole = new Label { Text = "用户角色:", Location = new Point(20, 110) };
        cboRole = new ComboBox { Location = new Point(120, 110), Width = 180 };

        btnRegister = new Button { Text = "注册", Location = new Point(80, 160), Width = 80 };
        btnCancel = new Button { Text = "取消", Location = new Point(180, 160), Width = 80 };

        // 添加控件到窗体
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(lblConfirmPassword);
        this.Controls.Add(txtConfirmPassword);
        this.Controls.Add(lblRole);
        this.Controls.Add(cboRole);
        this.Controls.Add(btnRegister);
        this.Controls.Add(btnCancel);

        // 加载角色列表
        LoadRoles();

        // 添加事件处理
        btnRegister.Click += BtnRegister_Click;
        btnCancel.Click += BtnCancel_Click;
    }

    // 加载角色列表
    // 加载角色下拉框
    private void LoadRoles()
    {
        cboRole.Items.Clear();

        // 从数据库获取所有角色
        var roles = DatabaseHelper.GetAllRoles();

        foreach (var role in roles)
        {
            cboRole.Items.Add(role);
        }

        if (cboRole.Items.Count > 0)
            cboRole.SelectedIndex = 0;
    }




    // 注册按钮点击事件
    private void BtnRegister_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtUsername.Text))
        {
            MessageBox.Show("请输入用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrEmpty(txtPassword.Text))
        {
            MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            MessageBox.Show("两次输入的密码不一致", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboRole.SelectedItem == null)
        {
            MessageBox.Show("请选择用户角色", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedRole = (Role)cboRole.SelectedItem;

        try
        {
            // 注册新用户
            DatabaseHelper.RegisterUser(txtUsername.Text, txtPassword.Text, selectedRole.Id);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"注册失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // 取消按钮点击事件
    private void BtnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}