using System;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity.Core.Metadata.Edm;

using System.Collections;
using System.Runtime.Remoting.Contexts;
using System.Collections.Generic;
using System.Reflection;

namespace EFWinFormsApp
{
    public partial class Form1 : Form
    {
        private readonly AdventureWorks2019Entities db = new AdventureWorks2019Entities();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var metadata = ((IObjectContextAdapter)db).ObjectContext.MetadataWorkspace;

            var tables = metadata.GetItemCollection(DataSpace.SSpace)
                        .GetItems<EntityContainer>()
                        .Single()
                        .BaseEntitySets
                        .OfType<EntitySet>()
                        .Where(s => !s.MetadataProperties.Contains("Type")
                        || s.MetadataProperties["Type"].ToString() == "Tables");

            foreach (var table in tables)
            {
                var tableName = table.MetadataProperties.Contains("Table")
                                && table.MetadataProperties["Table"].Value != null
                                ? table.MetadataProperties["Table"].Value.ToString()
                                : table.Name;

                cbTable.Items.Add(table.Name);

            }
        }

        private void cbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            var tableName = cb.SelectedItem.ToString();
            var test = GetAll(tableName);

            dataGrid.DataSource = test;

            /*var table = (IEnumerable)db.GetType().GetProperty(tableName).GetValue(db, null);


            List<object> results = new List<object>();

            foreach (var line in table)
            {
                //var value = line.GetType().GetProperty().GetValue(line, null);

                //if (value == tableName)
                //{
                    results.Add(line);
                //}

            }

            dataGrid.DataSource = results;*/


            /*var table1 = db.GetType()
            .GetProperty(tableName)
                           .GetValue(db, null);*/

            //var allResult = db.Employee.Select(x => x).ToList();
            //dataGrid.DataSource = allResult;
        }

        public dynamic GetAll(string Table)
        {
            var curEntityPI = db.GetType().GetProperty(Table);
            var curEntityType = curEntityPI.PropertyType.GetGenericArguments().First();
            // Getting Set<T> method
            var method = db.GetType().GetMember("Set").Cast<MethodInfo>().Where(x => x.IsGenericMethodDefinition).FirstOrDefault();
            // Making Set<SomeRealCrmObject>() method
            var genericMethod = method.MakeGenericMethod(curEntityType);
            // invoking Setmethod into invokeSet 
            dynamic invokeSet = genericMethod.Invoke(db, null);
            // invoking ToList method from Set<> invokeSet 
            return Enumerable.ToList(invokeSet);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            dataGrid.Width = Convert.ToInt32(form.Width * 0.9);
            dataGrid.Height = Convert.ToInt32(form.Height * 0.75);
        }
    }
}
