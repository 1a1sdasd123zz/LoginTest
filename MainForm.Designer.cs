using System.Windows.Forms;

namespace PermissionManagement
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tsm_Login = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_File = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_UserManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_DeleteUser = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_RoleManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_PermissionManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_SystemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCurrentUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_Login,
            this.tsm_File,
            this.tsm_UserManagement,
            this.tsm_RoleManagement,
            this.tsm_PermissionManagement,
            this.tsm_SystemSettings});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(800, 32);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // tsm_Login
            // 
            this.tsm_Login.Name = "tsm_Login";
            this.tsm_Login.Size = new System.Drawing.Size(62, 28);
            this.tsm_Login.Text = "登录";
            this.tsm_Login.Click += new System.EventHandler(this.tsm_Login_Click);
            // 
            // tsm_File
            // 
            this.tsm_File.Name = "tsm_File";
            this.tsm_File.Size = new System.Drawing.Size(62, 28);
            this.tsm_File.Text = "文件";
            // 
            // tsm_UserManagement
            // 
            this.tsm_UserManagement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_DeleteUser});
            this.tsm_UserManagement.Name = "tsm_UserManagement";
            this.tsm_UserManagement.Size = new System.Drawing.Size(98, 28);
            this.tsm_UserManagement.Text = "用户管理";
            // 
            // tsm_DeleteUser
            // 
            this.tsm_DeleteUser.Name = "tsm_DeleteUser";
            this.tsm_DeleteUser.Size = new System.Drawing.Size(270, 34);
            this.tsm_DeleteUser.Text = "删除用户";
            this.tsm_DeleteUser.Click += new System.EventHandler(this.tsm_DeleteUser_Click);
            // 
            // tsm_RoleManagement
            // 
            this.tsm_RoleManagement.Name = "tsm_RoleManagement";
            this.tsm_RoleManagement.Size = new System.Drawing.Size(98, 28);
            this.tsm_RoleManagement.Text = "角色管理";
            // 
            // tsm_PermissionManagement
            // 
            this.tsm_PermissionManagement.Name = "tsm_PermissionManagement";
            this.tsm_PermissionManagement.Size = new System.Drawing.Size(98, 28);
            this.tsm_PermissionManagement.Text = "权限管理";
            this.tsm_PermissionManagement.Click += new System.EventHandler(this.permissionManagementToolStripMenuItem_Click);
            // 
            // tsm_SystemSettings
            // 
            this.tsm_SystemSettings.Name = "tsm_SystemSettings";
            this.tsm_SystemSettings.Size = new System.Drawing.Size(98, 28);
            this.tsm_SystemSettings.Text = "系统设置";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblCurrentUser});
            this.statusStrip1.Location = new System.Drawing.Point(0, 419);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 31);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(86, 24);
            this.toolStripStatusLabel1.Text = "当前用户:";
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(195, 24);
            this.lblCurrentUser.Text = "toolStripStatusLabel2";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "权限管理系统";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsm_File;
        private System.Windows.Forms.ToolStripMenuItem tsm_UserManagement;
        private System.Windows.Forms.ToolStripMenuItem tsm_RoleManagement;
        private System.Windows.Forms.ToolStripMenuItem tsm_PermissionManagement;
        private System.Windows.Forms.ToolStripMenuItem tsm_SystemSettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblCurrentUser;
        private ToolStripMenuItem tsm_Login;
        private ToolStripMenuItem tsm_DeleteUser;
    }
}

