using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PermissionManagement;

    public partial class DeleteUserForm : Form
    {
        private List<User> _allUsers;
        private ListView lvUsers; // 定义 lvUsers 变量

        public DeleteUserForm()
        {
            InitializeComponent();
            this.Text = "删除用户";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            InitializeUI();
        }

        private void InitializeUI()
        {
            // 创建用户列表视图
            lvUsers = new ListView(); // 初始化 lvUsers 变量
            lvUsers.Location = new System.Drawing.Point(20, 20);
            lvUsers.Size = new System.Drawing.Size(340, 200);
            lvUsers.View = View.Details;
            lvUsers.FullRowSelect = true;

            // 添加列
            lvUsers.Columns.Add("用户名", 150);
            lvUsers.Columns.Add("角色", 150);

            this.Controls.Add(lvUsers);

            // 创建删除按钮
            Button btnDelete = new Button();
            btnDelete.Text = "删除选中用户";
            btnDelete.Location = new System.Drawing.Point(20, 230);
            btnDelete.Size = new System.Drawing.Size(150, 30);
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // 创建取消按钮
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new System.Drawing.Point(210, 230);
            btnCancel.Size = new System.Drawing.Size(150, 30);
            btnCancel.Click += (sender, e) => this.Close();
            this.Controls.Add(btnCancel);

            // 加载所有用户
            LoadUsers();
        }

        private void LoadUsers()
        {
            _allUsers = DatabaseHelper.GetAllUsers();

            lvUsers.Items.Clear();
            foreach (var user in _allUsers)
            {
                ListViewItem item = new ListViewItem(user.Username);
                item.SubItems.Add(user.RoleName);
                lvUsers.Items.Add(item);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lvUsers.SelectedItems.Count > 0)
            {
                var selectedItem = lvUsers.SelectedItems[0];
                var username = selectedItem.Text;
                var userToDelete = _allUsers.Find(u => u.Username == username);

                if (userToDelete != null)
                {
                    try
                    {
                        // 删除用户
                        DatabaseHelper.DeleteUser(userToDelete.Id);
                        MessageBox.Show("用户删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"用户删除失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的用户", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }