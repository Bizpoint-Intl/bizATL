using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;

using BizRAD.BizBase;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizTools;
using BizRAD.BizAccounts;


namespace ATL.BizModules.UserAuthorization
{
    class UserAuthorization
    {
       
         DBAccess dbAccess = null;
         private string UserName,ModuleName,GroupName = "";
         private bool AllowNew, AllowEdit,AllowDelete, AllowConfirm, AllowReOpen, AllowRefresh, AllowSave,
                      AllowRecommend,AllowApprove = false;


        public UserAuthorization()
        {
        }

        public UserAuthorization(string ModName)
        {
            this.dbAccess = new DBAccess();
            this.UserName = Common.DEFAULT_SYSTEM_USERNAME;
            this.ModuleName = ModName;
            CheckPriviledges();
        }
     
      
    

        public void CheckPriviledges()
        {
          

            string GetSysUser = "Select * from SysUserGroup where Username='" + UserName + "'";
            this.dbAccess.ReadSQL("dtSysUserGroup", GetSysUser);

            if (this.dbAccess.DataSet.Tables["dtSysUserGroup"].Rows.Count > 0)
            {
                string GetSysGroupPriviledge = "Select * from SysgroupPrivilege";
                this.dbAccess.ReadSQL("TmpGetPermission", GetSysGroupPriviledge);

                foreach (DataRow drSysUserGroup in this.dbAccess.DataSet.Tables["dtSysUserGroup"].Rows)
                {
                    if (drSysUserGroup.RowState != DataRowState.Deleted)
                    {

                        string GetPermission = "Select * from TmpGetPermission where GroupName='" + drSysUserGroup["GroupName"].ToString() + "' and  ModuleName='" + ModuleName + "'";
                        DataTable dtGetPermission = BizFunctions.ExecuteQuery(this.dbAccess.DataSet, GetPermission);

                        if (dtGetPermission.Rows.Count > 0)
                        {
                            DataRow drGetPermission = dtGetPermission.Rows[0];

                            if (BizFunctions.IsEmpty(drGetPermission["New"]))
                            {
                                AllowSave = false;
                            }
                            else
                            {
                                AllowNew = (bool)drGetPermission["New"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Edit"]))
                            {
                                AllowEdit = false;
                            }
                            else
                            {
                                AllowEdit = (bool)drGetPermission["Edit"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Delete"]))
                            {
                                AllowDelete = false;
                            }
                            else
                            {
                                AllowDelete = (bool)drGetPermission["Delete"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Confirm"]))
                            {
                                AllowConfirm = false;
                            }
                            else
                            {
                                AllowConfirm = (bool)drGetPermission["Confirm"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Reopen"]))
                            {
                                AllowReOpen = false;
                            }
                            else
                            {
                                AllowReOpen = (bool)drGetPermission["Reopen"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Refresh"]))
                            {
                                AllowRefresh = false;
                            }
                            else
                            {
                                AllowRefresh = (bool)drGetPermission["Refresh"];
                            }

                            if (BizFunctions.IsEmpty(drGetPermission["Save"]))
                            {
                                AllowSave = false;
                            }
                            else
                            {
                                AllowSave = (bool)drGetPermission["Save"];
                            }

                            string s = drSysUserGroup["GroupName"].ToString();

                            CheckRecdApprPrivilege(drSysUserGroup["GroupName"].ToString());

                            //if (AllowRecommend && AllowApprove)
                            //{
                            //    break;
                            //}
                            //else if (!AllowRecommend && AllowApprove)
                            //{
                            //    break;
                            //}
                            //else if (AllowRecommend && !AllowApprove)
                            //{
                            //    break;
                            //}

                           

                        }
                    
                    }

                }
            }
          
        }


        public void CheckRecdApprPrivilege(string GrpNm)
        {
            string GetHRPM = "select h.ModuleName,ISNULL(h1.[group],'') AS [group],ISNULL(h1.recommend,0) as recommend, ISNULL(h1.approve,0) as approve from hrpm h "+
                                "left join hrpm1 h1 on h.modulename=h1.modulename where h.ModuleName='" + ModuleName + "' and [group]='" + GrpNm + "' and h.[status]<>'V'";
            this.dbAccess.ReadSQL("dtGetHRPM", GetHRPM);
            if (this.dbAccess.DataSet.Tables["dtGetHRPM"].Rows.Count > 0)
            {
                DataRow drGetHRPM = this.dbAccess.DataSet.Tables["dtGetHRPM"].Rows[0];

                if (!AllowRecommend)
                {
                    AllowRecommend = (bool)drGetHRPM["recommend"];
                }
                //else
                //{
                //    AllowRecommend = false;
                //}
                if (!AllowApprove)
                {
                    AllowApprove = (bool)drGetHRPM["approve"];
                }
                //else
                //{
                //    AllowApprove = false;
                //}
                
                               
            }

        }


        public bool RecommendPermission
        {
            get
            {
                return AllowRecommend;
            }
            set
            {
                AllowRecommend = value;
            }
        }

        public bool ApprovePermission
        {
            get
            {
                return AllowApprove;
            }
            set
            {
                AllowApprove = value;
            }
        }


        public bool NewPermission
        {
            get
            {
                return AllowNew;
            }
            set
            {
                AllowNew = value;
            }
        }

        public bool EditPermission
        {
            get
            {
                return AllowEdit;
            }
            set
            {
                AllowEdit = value;
            }
        }
        public bool DeletePermission
        {
            get
            {
                return AllowDelete;
            }
            set
            {
                AllowDelete = value;
            }
        }
        public bool ConfirmPermission
        {
            get
            {
                return AllowConfirm;
            }
            set
            {
                AllowConfirm = value;
            }
        }
        public bool ReOpenPermission
        {
            get
            {
                return AllowReOpen;
            }
            set
            {
                AllowReOpen = value;
            }
        }
        public bool RefreshPermission
        {
            get
            {
                return AllowRefresh;
            }
            set
            {
                AllowRefresh = value;
            }
        }
        public bool SavePermission
        {
            get
            {
                return AllowSave;
            }
            set
            {
                AllowSave = value;
            }
        }
    }
}
