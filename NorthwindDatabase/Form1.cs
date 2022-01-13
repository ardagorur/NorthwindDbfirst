using NorthwindDatabase.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindDatabase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbDoldur();
            doldur(1);
        }
        NorthwindEntities db = new NorthwindEntities();
        Products updateProduct = new Products();
        int secID = 0;
        public void cbDoldur()
        {
            cmbKategoriler.DataSource = db.Set<Categories>().Select(x => new
            {
                Ad = x.CategoryName,
                Id = x.CategoryID
            }).ToList();
            cmbKategoriler.DisplayMember = "Ad";
            cmbKategoriler.ValueMember = "Id";

            cmbUrunEklemeKategorler.DataSource = cmbKategoriler.DataSource;
            cmbUrunEklemeKategorler.DisplayMember = "Ad";
            cmbUrunEklemeKategorler.ValueMember = "Id";
            cmbGuncellemeKategoriler.DataSource = cmbKategoriler.DataSource;
            cmbGuncellemeKategoriler.DisplayMember = "Ad";
            cmbGuncellemeKategoriler.ValueMember = "Id";
        }
        public void doldur(int q)
        {
            var list = db.Set<Products>().Select(x => new ProductDto
            {
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                CategoryName = x.Categories.CategoryName,
                CategoryID = x.CategoryID
            }).Where(x=> x.CategoryID == (int)cmbKategoriler.SelectedValue).ToList();
            Guncelle();
            foreach (var item in list)
            {
                
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = item.ProductID;
                lvi.Text = item.ProductName;
                lvi.SubItems.Add(item.UnitPrice.ToString());
                lvi.SubItems.Add(item.UnitsInStock.Value.ToString());
                lvi.SubItems.Add(item.CategoryName);
                listView1.Items.Add(lvi);
            }
        }

        private void cmbKategoriler_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int secilen = 1;
                secilen = Convert.ToInt32(cmbKategoriler.SelectedValue);
                doldur(secilen);
            }
            catch
            {

            }
        }
        public void Guncelle()
        {
            listView1.Items.Clear();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            Products p = new Products();
            p.ProductName = txtEklemeUrunAd.Text;
            p.UnitPrice = Convert.ToInt32(txtEklemeFiyat.Text);
            p.UnitsInStock = (short?)nudEklemeStok.Value;
            p.CategoryID = (int)cmbUrunEklemeKategorler.SelectedValue;
            db.Set<Products>().Add(p);
            db.SaveChanges();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            updateProduct.ProductName = txtGuncellemeUrunAd.Text;
            updateProduct.UnitPrice = Convert.ToInt32(txtGuncellemeFiyat.Text);
            updateProduct.UnitsInStock = (short)nudGuncellemeStok.Value;
            updateProduct.CategoryID = Convert.ToInt32(cmbGuncellemeKategoriler.SelectedValue);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            string id ="xxx";
            //secilenId = Convert.ToInt32(listView1.Items[0].Text);
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem cashMenuItem = new MenuItem("Güncelle");
                    cashMenuItem.Tag = listView1.FocusedItem.Tag;
                    m.MenuItems.Add(cashMenuItem);
                    cashMenuItem.Click += delegate (object sender2, EventArgs e2)
                    {
                        ActionClick(sender, e, id);
                    };

                    MenuItem delMenuItem = new MenuItem("Sil");
                    delMenuItem.Click += delegate (object sender2, EventArgs e2) {
                        DelectAction(sender, e, id);
                    };// 
                    m.MenuItems.Add(delMenuItem);

                    m.Show(listView1, new Point(e.X, e.Y));

                }
            }
        }
        private void DelectAction(object sender, MouseEventArgs e, string id)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                // you can use this idea to get the ListView header's name is 'Id' before delete
                Console.WriteLine(GetTextByHeaderAndIndex(listView1, "Id", eachItem.Index));
                listView1.Items.Remove(eachItem);
            }
        }

        private void ActionClick(object sender, MouseEventArgs e, string id)
        {
            //secID = Convert.ToInt32(listView1.SelectedItems[0].Tag as Products);
            secID = Convert.ToInt32(listView1.SelectedItems[0].Tag);
            updateProduct = db.Products.Find(secID);
            txtGuncellemeUrunAd.Text = updateProduct.ProductName;
            txtGuncellemeFiyat.Text = updateProduct.UnitPrice.ToString();
            nudGuncellemeStok.Value = (decimal)updateProduct.UnitsInStock;
            cmbGuncellemeKategoriler.SelectedValue = updateProduct.CategoryID;
            
        }
        public static string GetTextByHeaderAndIndex(ListView listViewControl, string headerName, int index)
        {


            int headerIndex = -1;
            foreach (ColumnHeader header in listViewControl.Columns)
            {
                if (header.Name == headerName)
                {
                    headerIndex = header.Index;
                    break;
                }
            }
            if (headerIndex > -1)
            {
                return listViewControl.Items[index].SubItems[headerIndex].Text;
            }
            return null;
        }

    }
}
