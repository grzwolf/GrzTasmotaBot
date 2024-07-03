using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;              // change column width of a propertygrid

namespace GrzTasmotaBot
{
    public partial class Settings: Form
    {
        public AppSettings Setting;

        public Settings(AppSettings settings)
        {
            InitializeComponent();
            this.propertyGrid.SelectedObject = settings;
        }

        // end settings dialog with ok
        private void buttonOk_Click( object sender, EventArgs e )
        {
            Setting = (AppSettings)this.propertyGrid.SelectedObject;
        }

        // change column width of a propertygrid: https://stackoverflow.com/questions/12447156/how-can-i-set-the-column-width-of-a-property-grid 2nd answer
        public static void SetLabelColumnWidth( PropertyGrid grid, int width )
        {
            if ( grid == null )
                return;
            FieldInfo fi = grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            if ( fi == null )
                return;
            Control view = fi.GetValue(grid) as Control;
            if ( view == null )
                return;
            MethodInfo mi = view.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
            if ( mi == null )
                return;
            mi.Invoke(view, new object[] { width });
        }
        private void Settings_Load( object sender, EventArgs e )
        {
            SetLabelColumnWidth(this.propertyGrid, 180);
            ResizeDescriptionArea(ref this.propertyGrid, 6);
        }

        // if description area needs more lines: https://www.codeproject.com/Articles/28193/Change-the-height-of-a-PropertyGrid-s-description
        bool ResizeDescriptionArea(ref PropertyGrid grid, int nNumLines) {
            try {
                System.Reflection.PropertyInfo pi = grid.GetType().GetProperty("Controls");
                Control.ControlCollection cc = grid.Controls;
                foreach ( Control c in cc ) {
                    Type ct = c.GetType();
                    string sName = ct.Name;
                    if ( sName == "DocComment" ) {
                        pi = ct.GetProperty("Lines");
                        pi.SetValue(c, nNumLines, null);
                        System.Reflection.FieldInfo fi = ct.BaseType.GetField(
                            "userSized",
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
                            );

                        fi.SetValue(c, true);
                        break;
                    }
                }
                return true;
            } catch ( Exception ) {
                return false;
            }
        }

    }
}
