using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{
    public delegate void ConfirmEventHandler(object sender, UserPopUpEventArgs e);

    public partial class UserPopUp : System.Web.UI.UserControl
    {
        public event ConfirmEventHandler Confirm;

        protected virtual void OnConfirm(UserPopUpEventArgs e)
        {
            if (Confirm != null)
                Confirm(this, e);
        }

        public string ucTitle {
            get
            {
                return lblUpTitle.Text;
            }
            set
            {
                lblUpTitle.Text = value;
            }
        }

        public string ucInnerHTML {
            get
            {
                return ltUpText.Text;
            }
            set
            {
                string html = value;

                if (ucCenterText)
                    html = "<p class='upCenterTitle'>" + value + "</p>";

                ltUpText.Text = html;
            }
        }

        public bool ucCenterText { get; set; }

        public bool ucConfirm { get; set; }

        public int ucWidth { get; set; }

        protected string upID {
            get
            {
                return ClientID;
            }
        }

        public void OpenUserPopUp( bool usingCallback = false, string callback = "" )
        {
            if (ucWidth == 0)
                ucWidth = 300;

            Utils.AppendScript("openP('up"+ upID +"'"+ ucWidth.ToString() +");");

            //Utils.AppendScript("openP('up"+ upID +"',"+ ucWidth.ToString() + "," + ( usingCallback ? "true" : "false" ) + ", " + ( callback.Equals( string.Empty ) ? null : callback ) + ")");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ucConfirm)
                    pnlConfirm.Visible = true;
            }

            btnOk.DataBind();
            btnCancel.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            OnConfirm(new UserPopUpEventArgs(false));
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            OnConfirm(new UserPopUpEventArgs(true));
        }
    }
    
    public class UserPopUpEventArgs : EventArgs
    {
        public UserPopUpEventArgs(bool s)
        {
            _confirm = s;
        }
        private bool _confirm;

        public bool Confirm
        {
            get { return _confirm; }
            set { _confirm = value; }
        }
    }

}