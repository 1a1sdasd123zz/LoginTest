using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PermissionManagement;

// 角色权限配置窗体
public partial class PermissionConfigForm : Form
{
    public PermissionConfigForm()
    {
        InitializeComponent();
        this.Text = "角色权限配置";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new System.Drawing.Size(600, 400);

        InitializeUI();
    }

    private void InitializeUI()
    {
        // 创建角色选择下拉框
        Label lblRole = new Label();
        lblRole.Text = "选择角色:";
        lblRole.Location = new System.Drawing.Point(20, 20);
        lblRole.Size = new System.Drawing.Size(70, 20);
        this.Controls.Add(lblRole);

        // 创建权限列表视图
        lvPermissions = new ListView();
        // 设置ListView属性...

        // 添加列
        lvPermissions.Columns.Add("权限名称", 200);
        lvPermissions.Columns.Add("描述", 300);


        cboRole = new ComboBox();
        cboRole.Location = new System.Drawing.Point(100, 20);
        cboRole.Size = new System.Drawing.Size(200, 25);
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRole.SelectedIndexChanged += CboRole_SelectedIndexChanged;
        this.Controls.Add(cboRole);

        // 加载角色数据
        LoadRoles();

        // 创建权限列表视图
        lvPermissions = new ListView();
        lvPermissions.Location = new System.Drawing.Point(20, 60);
        lvPermissions.Size = new System.Drawing.Size(540, 260);
        lvPermissions.View = View.Details;
        lvPermissions.CheckBoxes = true;
        lvPermissions.FullRowSelect = true;

        // 添加列
        lvPermissions.Columns.Add("权限名称", 200);
        lvPermissions.Columns.Add("描述", 300);

        this.Controls.Add(lvPermissions);

        // 创建保存按钮
        btnSave = new Button();
        btnSave.Text = "保存配置";
        btnSave.Location = new System.Drawing.Point(380, 330);
        btnSave.Size = new System.Drawing.Size(100, 30);
        btnSave.Click += btnSave_Click;
        this.Controls.Add(btnSave);

        // 创建取消按钮
        btnCancel = new Button();
        btnCancel.Text = "取消";
        btnCancel.Location = new System.Drawing.Point(490, 330);
        btnCancel.Size = new System.Drawing.Size(70, 30);
        btnCancel.Click += (sender, e) => this.Close();
        this.Controls.Add(btnCancel);
    }

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

    private void CboRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadPermissions();
    }


    // 加载权限 - 修复DataTable使用Any方法的问题
    private void LoadPermissions()
    {
        if (cboRole.SelectedValue == null)
            return;

        int roleId = Convert.ToInt32(((DataRowView)cboRole.SelectedItem)["Id"]);
        var rolePermissions = DatabaseHelper.GetRolePermissions(roleId);

        lvPermissions.Items.Clear();

        // 根据权限枚举自动生成权限列表项
        foreach (var permission in Enum.GetValues(typeof(Permission)).Cast<Permission>())
        {
            var attribute = GetPermissionInfo(permission);
            var item = new ListViewItem(attribute.Name);
            item.SubItems.Add(attribute.Description);
            item.Tag = permission;

            // 修复：将DataTable转换为DataRow集合后使用Any方法
            bool isAssigned = rolePermissions.AsEnumerable()
                .Any(p => p["Code"].ToString() == permission.ToString());
            item.Checked = isAssigned;

            lvPermissions.Items.Add(item);
        }
    }

    // 保存权限配置 - 重构部分
    private void btnSave_Click(object sender, EventArgs e)
    {
        if (cboRole.SelectedItem == null)
            return;

        int roleId = Convert.ToInt32(((DataRowView)cboRole.SelectedItem)["Id"]);

        // 获取选中的权限
        var selectedPermissions = lvPermissions.CheckedItems.Cast<ListViewItem>()
            .Select(item => (Permission)item.Tag)
            .Select(p => p.ToString())
            .ToList();

        // 更新角色权限
        DatabaseHelper.UpdateRolePermissions(roleId, selectedPermissions);

        MessageBox.Show("权限配置已保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
    }
    // 获取权限信息
    private PermissionInfoAttribute GetPermissionInfo(Permission permission)
    {
        var field = typeof(Permission).GetField(permission.ToString());
        return field.GetCustomAttribute<PermissionInfoAttribute>();
    }

}