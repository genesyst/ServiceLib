using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceLib
{
    public class ServiceWControl
    {
        public static int DataGridFieldIndex(DataGridView grid,string findFieldName)
        {
            int Res = -1;
            for(int i=0;i < grid.ColumnCount;i++)
            {
                string fieldName = grid.Columns[i].DataPropertyName.Trim().ToUpper();
                if(fieldName == findFieldName.Trim().ToUpper())
                {
                    Res = i;
                    break;
                }
            }

            return Res;
        }
    }
}
