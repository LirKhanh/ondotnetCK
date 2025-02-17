using ontaptx2.Models;
using System.Diagnostics.Tracing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ontaptx2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QlbanHangContext db;
        public MainWindow()
        {
            InitializeComponent();
            db = new QlbanHangContext();
            loadTable();
        }
        private void loadTable()
        {
            var query = from sp in db.SanPhams
                        select new
                        {
                            sp.MaSp,
                            sp.TenSp,
                            sp.MaLoaiNavigation.TenLoai,
                            sp.DonGia,
                            sp.SoLuong,
                            total = sp.DonGia * sp.SoLuong,
                        };
            dgProduct.ItemsSource = query.ToList();
        }


        private void add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string id = txtId.Text;
                string name = txtName.Text;
                int quantity = int.Parse(txtQuantity.Text);
                int price = int.Parse(txtPrice.Text);
                string kind = txtKind.Text;

                var existingProduct = db.SanPhams.FirstOrDefault(sp => sp.MaSp == id);
                if (existingProduct != null)
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập mã khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                SanPham sp = new SanPham
                {
                    MaSp = id,
                    TenSp = name,
                    SoLuong = quantity,
                    DonGia = price,
                    MaLoai = kind
                };

                db.SanPhams.Add(sp);
                db.SaveChanges();
                loadTable();

                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng dữ liệu cho số lượng và giá.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtName.Text) ||
                    string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtQuantity.Text) ||
                    string.IsNullOrEmpty(txtKind.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var spSua = db.SanPhams.FirstOrDefault(sp => sp.MaSp.Equals(txtId.Text));
                if (spSua == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với mã đã nhập.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                spSua.TenSp = txtName.Text;
                spSua.DonGia = int.Parse(txtPrice.Text);
                spSua.SoLuong = int.Parse(txtQuantity.Text);
                spSua.MaLoai = txtKind.Text;

                db.SaveChanges();
                loadTable();

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException e1)
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng dữ liệu (Số cho giá và số lượng).", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var spSua = db.SanPhams.FirstOrDefault(sp => sp.MaSp.Equals(txtId.Text));
                db.SanPhams.Remove(spSua);
                db.SaveChanges();
                loadTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            var query = from sp in db.SanPhams
                        join lsp in db.LoaiSanPhams
                        on sp.MaLoai equals lsp.MaLoai
                        group sp by lsp.TenLoai into nhom
                        select new
                        {
                            TenLoai = nhom.Key,
                            TongSoLuong = nhom.Sum(sp => sp.SoLuong),
                            TongTien = nhom.Sum(sp => sp.SoLuong * sp.DonGia)
                        };
            Window1 window1 = new Window1();
            window1.newDtg.ItemsSource = query.ToArray();
            window1.Show();
        }

        private void dgProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProduct.SelectedItem != null)
            {
                var dongChon = (dynamic)dgProduct.SelectedItem;

                txtId.Text = dongChon.MaSp;
                txtName.Text = dongChon.TenSp;
                txtPrice.Text = dongChon.DonGia.ToString();
                txtQuantity.Text = dongChon.SoLuong.ToString();
                txtKind.Text = dongChon.TenLoai.ToString();
            }
        }
    }
}